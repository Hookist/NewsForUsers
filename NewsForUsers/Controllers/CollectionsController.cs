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
using System.Data.Entity.Migrations;
using System.Web.Http.Tracing;
using log4net;

namespace NewsForUsers.Controllers
{
    /// <summary>
    /// User operations with collections controller
    /// </summary>
    public class CollectionsController : ApiController
    {
        private NewsForUsersModel db = new NewsForUsersModel();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: api/Collections
        /// <summary>
        /// Gets user collections informations
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ResponseType(typeof(IQueryable<Collection>))]
        public IHttpActionResult GetCollections()
        {
            Log.Debug("Gets user collections informations");
            int? userId = this.User.Identity.GetUserId<int>();

            if(userId == null)
            {
                return BadRequest("User not found");
            }

            return Ok(db.Collections.Where(c => c.UserId == userId));
        }

        // GET: api/Collections/5
        /// <summary>
        /// Gets user collection informations
        /// </summary>
        /// <param name="id">Collection Id</param>
        /// <returns></returns>
        [ResponseType(typeof(Collection))]
        [Authorize]
        public async Task<IHttpActionResult> GetCollection(int id)
        {
            Log.Debug("Gets user collection informations");
            int userId = this.User.Identity.GetUserId<int>();
            
            Collection collection = await Task.Run(() => db.Collections.Where(c => c.UserId == userId && c.Id == id).FirstOrDefault());
            if (collection == null)
            {
                return NotFound();
            }

            return Ok(collection);
        }

        // PUT: api/Collections/5
        /// <summary>
        /// Update user collection information 
        /// </summary>
        /// <param name="id">collection id</param>
        /// <param name="collection">collection</param>
        /// <returns></returns>
        [Authorize]
        [ResponseType(typeof(void))]
        [HttpPut]
        public async Task<IHttpActionResult> PutCollection(int id, Collection collection)
        {
            Log.Debug("Update user collection information ");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int userId = this.User.Identity.GetUserId<int>();
            Collection collectionInDB = await Task.Run(() => db.Collections.Where(c => c.UserId == userId && c.Id == id).FirstOrDefault());

            if (collectionInDB == null)
            {
                return NotFound();
            }
            collection.Id = collectionInDB.Id;
            collection.UserId = userId;
            // db.Entry(collection).State = EntityState.Modified;
            db.Set<Collection>().AddOrUpdate(collection);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CollectionExists(collection.Id))
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

        // POST: api/Collections
        /// <summary>
        /// Add new collection to user
        /// </summary>
        /// <param name="collection">new collection</param>
        /// <returns></returns>
        [Authorize]
        [ResponseType(typeof(Collection))]
        [HttpPost]
        public async Task<IHttpActionResult> PostCollection(Collection collection)
        {
            Log.Debug("Add new collection to user");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int userId = this.User.Identity.GetUserId<int>();
            collection.UserId = userId;
            Collection collectionInDB = await Task.Run(() => db.Collections.Where(c => c.UserId == userId && c.Name == collection.Name).FirstOrDefault());
            if(collectionInDB != null)
            {
                return BadRequest("You already have collection with this name");
            }

            db.Collections.Add(collection);
            await db.SaveChangesAsync();

            return Ok("CoolectionId : " + collection.Id);
        }

        // DELETE: api/Collections/5
        /// <summary>
        /// Delete user collection
        /// </summary>
        /// <param name="id">collection id</param>
        /// <returns></returns>
        [ResponseType(typeof(Collection))]
        [Authorize]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteCollection(int id)
        {
            Log.Debug("Delete user collection");
            int userId = this.User.Identity.GetUserId<int>();
            Collection collection = await Task.Run(() => db.Collections.Where(c => c.UserId == userId && c.Id == id).FirstOrDefault());
            if (collection == null)
            {
                return NotFound();
            }
            db.Collections.Remove(collection);

            var sourceToCollection = db.SourceToCollections.Where(sc => sc.CollectionId == collection.Id);
            if(sourceToCollection != null)
                db.SourceToCollections.RemoveRange(sourceToCollection);

            await db.SaveChangesAsync();

            return Ok(collection);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CollectionExists(int id)
        {
            return db.Collections.Count(e => e.Id == id) > 0;
        }
    }
}