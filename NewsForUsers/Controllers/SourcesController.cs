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
using System.ServiceModel.Syndication;
using NewsForUsers.FeedFormaters;
using System.Web.Http.Tracing;
using log4net;

namespace NewsForUsers.Controllers
{
    /// <summary>
    /// Operations with feeds controller
    /// </summary>
    public class SourcesController : ApiController
    {
        private NewsForUsersModel db = new NewsForUsersModel();

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: api/Sources/GetSourcesByCollectionId/5
        /// <summary>
        /// Get feed link from user collection
        /// </summary>
        /// <param name="id">collection id</param>
        /// <returns></returns>
        [ResponseType(typeof(List<Source>))]
        [Authorize]
        [Route("api/Sources/GetSourcesByCollectionId/{id}")]
        public IHttpActionResult GetSourcesByCollectionId(int id)
        {
            Log.Debug("Get feed link from user collection");
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

        // POST: api/Sources/AddSourceToCollection/5
        /// <summary>
        /// Add feed link to collection
        /// </summary>
        /// <param name="id">collection id</param>
        /// <param name="source">feed source link</param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(Source))]
        [Route("api/Sources/AddSourceToCollection/{id}")]
        [Authorize]
        public async Task<IHttpActionResult> PostSource(int id, Source source)
        {
            Log.Debug("Add feed link to collection");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // get userId
            int userId = this.User.Identity.GetUserId<int>();
            // check if user has collection with this id
            if (!await db.IsUserHasCollection(id, userId))
            {
                return BadRequest("You don't have collection with this id");
            }
            // check if collection has this source
            if (await db.IsCollectionHasSource(id, source.Link))
            {
                return BadRequest("You already have source with the same link");
            }

            string sourceTypeName = String.Empty;
            List<Entity> entities = FeedHelper.GetEntitiesFromFeed(source.Link).ToList();
            if (entities == null)
            {
                return BadRequest("Wrong feed format or url");
            }

            // check if db has this source
            var sourceInDb = await db.Sources.FirstOrDefaultAsync(s => s.Link == source.Link);
            if (sourceInDb == null)
            {
                db.Sources.Add(source);
                await db.SaveChangesAsync();
                var lastAddedSource = await db.Sources.OrderByDescending(s => s.Id).FirstOrDefaultAsync();
                db.SourceToCollections.Add(new SourceToCollection() { CollectionId = id, SourceId = lastAddedSource.Id });

                foreach (var item in entities)
                {
                    db.Entities.Add(new Entity()
                    {
                        Title = item.Title,
                        PublicationDate = item.PublicationDate,
                        Link = item.Link,
                        SourceId = source.Id
                    });
                }
            }
            else
            {
                db.SourceToCollections.Add(new SourceToCollection() { CollectionId = id, SourceId = sourceInDb.Id });
            }

            await db.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Sources/DeleteSourceFromCollection/5/2
        /// <summary>
        /// Delete feed link from collection 
        /// </summary>
        /// <param name="collectionId">collection id</param>
        /// <param name="sourceId">feed source id</param>
        /// <returns></returns>
        [ResponseType(typeof(Source))]
        [Authorize]
        [Route("api/Sources/DeleteSourceFromCollection/{collectionId}/{sourceId}")]
        public async Task<IHttpActionResult> DeleteSourceFromCollection(int collectionId, int sourceId)
        {
            Log.Debug("Delete feed link from collection ");
            int userId = this.User.Identity.GetUserId<int>();
            if (!await db.IsUserHasCollection(collectionId, userId))
            {
                return BadRequest("You don't have collection with this id");
            }

            if (!await db.IsCollectionHasSource(collectionId, sourceId))
            {
                return BadRequest("Collection doesn't have source with this id");
            }
            SourceToCollection sourceToCollectionToDelete = db.SourceToCollections.Where(sc => sc.CollectionId == collectionId && sc.SourceId == sourceId).First();
            db.SourceToCollections.Remove(sourceToCollectionToDelete);
            await db.SaveChangesAsync();

            return Ok(sourceToCollectionToDelete);
        }

        // GET: api/Sources/GetNewsByCollectionId/5
        /// <summary>
        /// Get news by collectionId 
        /// </summary>
        /// <param name="collectionId"> collection id</param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<Entity>))]
        [Authorize]
        [Route("api/Sources/GetNewsByCollectionId/{collectionId}")]
        public async Task<IHttpActionResult> GetEntities(int collectionId)
        {
            Log.Debug("Get news by collectionId ");
            int userId = this.User.Identity.GetUserId<int>();

            if (!await db.IsUserHasCollection(collectionId, userId))
            {
                BadRequest("You don't have collection with this id");
            }

            var entities = (
                from c in db.Collections
                join sc in db.SourceToCollections on c.Id equals sc.CollectionId
                join s in db.Sources on sc.SourceId equals s.Id
                join e in db.Entities on s.Id equals e.SourceId
                where c.Id == collectionId
                select e
            );
            if (entities.Count() == 0)
            {
                BadRequest("Collection empty");
            }
            return Ok(entities);
        }

        // POST: api/Sources/GetNewsByCollectionIdAndPeriod/5
        /// <summary>
        /// Get news by collectionId and time period.
        /// </summary>
        /// <param name="collectionId">collection id</param>
        /// <param name="period">PeriodModel</param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<Entity>))]
        [Authorize]
        [Route("api/Sources/GetNewsByCollectionIdAndPeriod/{collectionId}")]
        [HttpPost]
        public async Task<IHttpActionResult> GetEntities(int collectionId, PeriodModel period)
        {
            Log.Debug("Get news by collectionId and time period");
            int userId = this.User.Identity.GetUserId<int>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await db.IsUserHasCollection(collectionId, userId))
            {
                BadRequest("You don't have collection with this id");
            }
            if (period.startDate == null || period.endDate == null)
            {
                BadRequest("Wrong date format");
            }
            if (period.startDate > period.endDate)
            {
                BadRequest("Strart date larger then End date");
            }

            var entities = (
                from c in db.Collections
                join sc in db.SourceToCollections on c.Id equals sc.CollectionId
                join s in db.Sources on sc.SourceId equals s.Id
                join e in db.Entities on s.Id equals e.SourceId
                where c.Id == collectionId && e.PublicationDate >= period.startDate
                && e.PublicationDate <= period.endDate
                select e
            );
            if (entities.Count() == 0)
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

    }

    public class PeriodModel
    {
        public DateTimeOffset startDate { get; set; }
        public DateTimeOffset endDate { get; set; }
    }
}