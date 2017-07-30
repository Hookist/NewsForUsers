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

namespace NewsForUsers.Controllers
{
    public class CollectionsController : ApiController
    {
        private NewsForUsersModel db = new NewsForUsersModel();

        // GET: api/Collections
        public IQueryable<Collection> GetCollections()
        {
            return db.Collections;
        }

        // GET: api/Collections/5
        [ResponseType(typeof(Collection))]
        public async Task<IHttpActionResult> GetCollection(int id)
        {
            Collection collection = await db.Collections.FindAsync(id);
            if (collection == null)
            {
                return NotFound();
            }

            return Ok(collection);
        }

        // PUT: api/Collections/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCollection(int id, Collection collection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != collection.Id)
            {
                return BadRequest();
            }

            db.Entry(collection).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CollectionExists(id))
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
        [ResponseType(typeof(Collection))]
        public async Task<IHttpActionResult> PostCollection(Collection collection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Collections.Add(collection);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = collection.Id }, collection);
        }

        // DELETE: api/Collections/5
        [ResponseType(typeof(Collection))]
        public async Task<IHttpActionResult> DeleteCollection(int id)
        {
            Collection collection = await db.Collections.FindAsync(id);
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