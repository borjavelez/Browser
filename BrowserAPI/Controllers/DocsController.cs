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
using BrowserAPI.Models;

namespace BrowserAPI.Controllers
{
    public class DocsController : ApiController
    {
        private BrowserAPIContext db = new BrowserAPIContext();

        // GET: api/Docs
        public IQueryable<Doc> GetDocs()
        {
            return db.Docs;
        }

        // GET: api/Docs/5
        [ResponseType(typeof(Doc))]
        public async Task<IHttpActionResult> GetDoc(int id)
        {
            Doc doc = await db.Docs.FindAsync(id);
            if (doc == null)
            {
                return NotFound();
            }

            return Ok(doc);
        }

        // PUT: api/Docs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDoc(int id, Doc doc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != doc.Id)
            {
                return BadRequest();
            }

            db.Entry(doc).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocExists(id))
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

        // POST: api/Docs
        [ResponseType(typeof(Doc))]
        public async Task<IHttpActionResult> PostDoc(Doc doc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Docs.Add(doc);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = doc.Id }, doc);
        }

        // DELETE: api/Docs/5
        [ResponseType(typeof(Doc))]
        public async Task<IHttpActionResult> DeleteDoc(int id)
        {
            Doc doc = await db.Docs.FindAsync(id);
            if (doc == null)
            {
                return NotFound();
            }

            db.Docs.Remove(doc);
            await db.SaveChangesAsync();

            return Ok(doc);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DocExists(int id)
        {
            return db.Docs.Count(e => e.Id == id) > 0;
        }
    }
}