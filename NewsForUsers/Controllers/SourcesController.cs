using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using NewsForUsers.Models;
using Microsoft.AspNet.Identity;
using System.Xml;
using NewsForUsers.Readers;
using System.ServiceModel.Syndication;

namespace NewsForUsers.Controllers
{
    public class SourcesController : ApiController
    {
        private NewsForUsersModel db = new NewsForUsersModel();

        // GET: api/Sources
        [ResponseType(typeof(List<Source>))]
        [Authorize]
        [Route("api/Sources/GetSourcesByCollectionId/{id}")]
        public IHttpActionResult GetSourcesByCollectionId(int id)
        {
            int userId = this.User.Identity.GetUserId<int>();
            Collection collection = db.Collections.Where(c => c.Id == id && c.UserId == userId).FirstOrDefault();
            if (collection == null)
            {
                return BadRequest("You don't have collection with this id");
            }

            var sources = from sc in db.SourceToCollections
                          join s in db.Sources on sc.SourceId equals s.Id
                          where sc.CollectionId == collection.Id
                          select s;
            return Ok(sources);
        }

        // GET: api/Sources/5
        [ResponseType(typeof(Source))]
        public async Task<IHttpActionResult> GetSource(int id)
        {
            int userId = this.User.Identity.GetUserId<int>();

            Source source = await db.Sources.FindAsync(id);
            if (source == null)
            {
                return NotFound();
            }

            return Ok(source);
        }

        // PUT: api/Sources/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSource(int id, Source source)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != source.Id)
            {
                return BadRequest();
            }

            db.Entry(source).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SourceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Sources
        [HttpPost]
        [ResponseType(typeof(Source))]
        [Route("api/Sources/AddSourceToCollection/{id}")]
        [Authorize]
        public async Task<IHttpActionResult> PostSource(int id, Source source)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // get userId
            int userId = this.User.Identity.GetUserId<int>();
            // check if user has collection with this id
            if (!await IsUserHasCollection(id, userId)) 
            {
                return BadRequest("You don't have collection with this id");
            }
            // check if collection has this source
            if(await IsCollectionHasSource(id, source.Link))
            {
                return BadRequest("You already have source with the same link");
            }
            
            string sourceTypeName = String.Empty;
            SyndicationFeed feed = GetSyndicationFeedData(source.Link, ref sourceTypeName);
            // check if db has this source
            var sourceInDb = await db.Sources.FirstOrDefaultAsync(s => s.Link == source.Link);
            if (sourceInDb == null)
            {
                db.Sources.Add(source);
                await db.SaveChangesAsync();
                var lastAddedSource = await db.Sources.OrderByDescending(s => s.Id).FirstOrDefaultAsync();
                db.SourceToCollections.Add(new SourceToCollection() { CollectionId = id, SourceId = lastAddedSource.Id });

                foreach (SyndicationItem item in feed.Items)
                {
                    db.Entities.Add(new Entity()
                    {
                        Title = item.Title.Text,
                        PublicationDate = item.PublishDate.ToString(),
                        Link = item.Links[0].Uri.ToString(),
                        SourceId = source.Id
                    });
                }
            }
            else
            {
                db.SourceToCollections.Add(new SourceToCollection() { CollectionId = id, SourceId = sourceInDb.Id });
            }
            
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = source.Id }, source);
        }

        private int GetSourceTypeId(string sourceTypeName)
        {
            List<SourceType> sourceTypes = db.SourceTypes.ToList();
            foreach(SourceType st in sourceTypes)
            {
                if (st.TypeName == sourceTypeName)
                    return st.Id;
            }
            return 0;
        }

        // DELETE: api/Sources/5
        [ResponseType(typeof(Source))]
        [Authorize]
        [Route("api/Sources/DeleteSourceFromCollection/{collectionId}/{sourceId}")]
        public async Task<IHttpActionResult> DeleteSourceFromCollection(int collectionId, int sourceId)
        {
            int userId = this.User.Identity.GetUserId<int>();
            if(!await IsUserHasCollection(collectionId, userId))
            {
                return BadRequest("You don't have collection with this id");
            }

            if (!await IsCollectionHasSource(collectionId, sourceId))
            {
                return BadRequest("Collection doesn't have source with this id");
            }
            SourceToCollection sourceToCollectionToDelete = db.SourceToCollections.Where(sc => sc.CollectionId == collectionId && sc.SourceId == sourceId).First();
            db.SourceToCollections.Remove(sourceToCollectionToDelete);
            await db.SaveChangesAsync();

            return Ok(sourceToCollectionToDelete);
        }

        [ResponseType(typeof(IEnumerable<Entity>))]
        [Authorize]
        [Route("api/Sources/GetNewsByCollectionId/{collectionId}")]
        public async Task<IHttpActionResult> GetEntities(int collectionId)
        {
            int userId = this.User.Identity.GetUserId<int>();

            if(!await IsUserHasCollection(collectionId, userId))
            {
                BadRequest("You don't have collection with this id");
            }

            var entities = (from c in db.Collections
                           join sc in db.SourceToCollections on c.Id equals sc.CollectionId
                           join s in db.Sources on sc.SourceId equals s.Id
                           join e in db.Entities on s.Id equals e.SourceId
                           where c.Id == collectionId
                           select e);
            if(entities.Count() == 0)
            {
                BadRequest("Collection empty");
            }
            return Ok(entities);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SourceExists(int id)
        {
            return db.Sources.Count(e => e.Id == id) > 0;
        }

        private SyndicationFeed GetSyndicationFeedData(string urlFeedLocation, ref string feedTypeName)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                CheckCharacters = true,
                CloseInput = true,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
            };
            if (String.IsNullOrEmpty(urlFeedLocation))
                return null;

            using (XmlReader reader = XmlReader.Create(urlFeedLocation))
            {
                if (reader.ReadState == ReadState.Initial)
                    reader.MoveToContent();

                Atom10FeedFormatter atom = new Atom10FeedFormatter();
                // try to read it as an atom feed
                if (atom.CanRead(reader))
                {
                    atom.ReadFrom(reader);
                    feedTypeName = "Atom";
                    return atom.Feed;
                }

                Rss20FeedFormatter rss = new Rss20FeedFormatter();
                // try reading it as an rss feed
                if (rss.CanRead(reader))
                {
                    rss.ReadFrom(reader);
                    feedTypeName = "RSS";
                    return rss.Feed;
                }
                // neither?
                return null;
            }
        }

        async Task<bool> IsUserHasCollection(int collectionId, int userId)
        {
            Collection collection = await db.Collections.Where(c => c.Id == collectionId && c.UserId == userId).FirstOrDefaultAsync();
            if (collection == null)
                return false;
            else
                return true;
        }

        async Task<bool> IsCollectionHasSource(int collectionId, string link)
        {
            var userSourceInDb = await (from sc in db.SourceToCollections
                                        join c in db.Collections on sc.CollectionId equals c.Id
                                        join s in db.Sources on sc.SourceId equals s.Id
                                        where c.Id == collectionId && s.Link == link
                                        select s).FirstOrDefaultAsync();
            if (userSourceInDb == null)
                return false;
            else
                return true;
        }

        async Task<bool> IsCollectionHasSource(int collectionId, int sourceId)
        {
            var userSourceInDb = await (from sc in db.SourceToCollections
                                        join c in db.Collections on sc.CollectionId equals c.Id
                                        join s in db.Sources on sc.SourceId equals s.Id
                                        where c.Id == collectionId && s.Id == sourceId
                                        select s).FirstOrDefaultAsync();
            if (userSourceInDb == null)
                return false;
            else
                return true;
        }

        string CheckFeed(string urlFeedLocation)
        {
            if (String.IsNullOrEmpty(urlFeedLocation))
                return null;

            var sourceTypes = db.SourceTypes.ToList();

            using (XmlReader reader = XmlReader.Create(urlFeedLocation))
            {
                if (reader.ReadState == ReadState.Initial)
                    reader.MoveToContent();

                Atom10FeedFormatter atom = new Atom10FeedFormatter();
                // try to read it as an atom feed
                if (atom.CanRead(reader))
                {
                    return "Atom";
                }

                Rss20FeedFormatter rss = new Rss20FeedFormatter();
                // try reading it as an rss feed
                if (rss.CanRead(reader))
                {
                    return "RSS";
                }
                // neither?
                return "Undefined";
            }
        }
    }
}