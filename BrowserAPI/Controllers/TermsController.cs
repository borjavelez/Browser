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
    public class TermsController : ApiController
    {
        private BrowserAPIContext db = new BrowserAPIContext();

        // GET: api/Terms
        public IQueryable<Term> GetTerms()
        {
            return db.Terms;
        }

        // GET: api/Terms/5
        [ResponseType(typeof(Term))]
        public async Task<IHttpActionResult> GetTerm(int id)
        {
            Term term = await db.Terms.FindAsync(id);
            if (term == null)
            {
                return NotFound();
            }

            return Ok(term);
        }

        // PUT: api/Terms/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTerm(int id, Term term)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != term.Id)
            {
                return BadRequest();
            }

            db.Entry(term).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TermExists(id))
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

        // POST: api/Terms
        [ResponseType(typeof(Term))]
        public async Task<IHttpActionResult> PostTerm(Term term)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Terms.Add(term);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = term.Id }, term);
        }

        // DELETE: api/Terms/5
        [ResponseType(typeof(Term))]
        public async Task<IHttpActionResult> DeleteTerm(int id)
        {
            Term term = await db.Terms.FindAsync(id);
            if (term == null)
            {
                return NotFound();
            }

            db.Terms.Remove(term);
            await db.SaveChangesAsync();

            return Ok(term);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TermExists(int id)
        {
            return db.Terms.Count(e => e.Id == id) > 0;
        }
    }
}