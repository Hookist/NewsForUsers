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

namespace NewsForUsers.Controllers
{
    public class CollectionsController : ApiController
    {
        private NewsForUsersModel db = new NewsForUsersModel();

        // GET: api/Collections
        [Authorize]
        public IQueryable<Collection> GetCollections()
        {
            int userId = this.User.Identity.GetUserId<int>();
            return db.Collections.Where(c => c.UserId == userId);
        }

        // GET: api/Collections/5
        [ResponseType(typeof(Collection))]
        public async Task<IHttpActionResult> GetCollection(int id)
        {
            int userId = this.User.Identity.GetUserId<int>();
            
            Collection collection = await Task.Run(() => db.Collections.Where(c => c.UserId == userId && c.Id == id).FirstOrDefault());
            if (collection == null)
            {
                return NotFound();
            }

            return Ok(collection);
        }

        // PUT: api/Collections/5
        [Authorize]
        [ResponseType(typeof(void))]
        [HttpPut]
        public async Task<IHttpActionResult> PutCollection(int id, Collection collection)
        {
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
        [Authorize]
        [ResponseType(typeof(Collection))]
        [HttpPost]
        public async Task<IHttpActionResult> PostCollection(Collection collection)
        {
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
        [ResponseType(typeof(Collection))]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteCollection(int id)
        {
            int userId = this.User.Identity.GetUserId<int>();
            Collection collection = await Task.Run(() => db.Collections.Where(c => c.UserId == userId && c.Id == id).FirstOrDefault());
            if (collection == null)
            {
                return NotFound();
            }
            db.Collections.Remove(collection);
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