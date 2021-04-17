using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriberamRestApp.Models;
using PriberamRestApp.Classification;

namespace PriberamRestApp.Controllers
{
    [Route("api/test/document")]
    [ApiController]
    public class TestDocumentsController : ControllerBase
    {
        private readonly TestContext _context;

        public TestDocumentsController(TestContext context)
        {
            _context = context;
        }

        // GET: api/test/document
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestDocument>>> GetTestDocuments()
        {
            return await _context.TestDocuments.ToListAsync();
        }

        // GET: api/test/document/{id}
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

        // POST: api/test/document
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TestDocument>> PostTestDocument(TestDocument testDocument)
        {
            _context.TestDocuments.Add(testDocument);
            await _context.SaveChangesAsync();

            Task<Classifier.Topic> result = Classifier.Instance.ClassifyAsync(testDocument);
            Classifier.Topic resultingTopic = await result;

            if(resultingTopic == Classifier.Topic.None)
            {
                return StatusCode(500);
            }
            else
            {
                return Ok(new Topic(resultingTopic.ToString()));
            }
        }

        private bool TestDocumentExists(long id)
        {
            return _context.TestDocuments.Any(e => e.Id == id);
        }
    }
}
