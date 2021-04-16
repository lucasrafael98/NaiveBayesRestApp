using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriberamRestApp.Models;

namespace PriberamRestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDocumentsController : ControllerBase
    {
        private readonly TestContext _context;

        public TestDocumentsController(TestContext context)
        {
            _context = context;
        }

        // GET: api/TestDocuments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestDocument>>> GetTestDocuments()
        {
            return await _context.TestDocuments.ToListAsync();
        }

        // GET: api/TestDocuments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TestDocument>> GetTestDocument(long id)
        {
            var testDocument = await _context.TestDocuments.FindAsync(id);

            if (testDocument == null)
            {
                return NotFound();
            }

            return testDocument;
        }

        // PUT: api/TestDocuments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTestDocument(long id, TestDocument testDocument)
        {
            if (id != testDocument.Id)
            {
                return BadRequest();
            }

            _context.Entry(testDocument).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestDocumentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TestDocuments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TestDocument>> PostTestDocument(TestDocument testDocument)
        {
            _context.TestDocuments.Add(testDocument);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTestDocument", new { id = testDocument.Id }, testDocument);
        }

        // DELETE: api/TestDocuments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestDocument(long id)
        {
            var testDocument = await _context.TestDocuments.FindAsync(id);
            if (testDocument == null)
            {
                return NotFound();
            }

            _context.TestDocuments.Remove(testDocument);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TestDocumentExists(long id)
        {
            return _context.TestDocuments.Any(e => e.Id == id);
        }
    }
}
