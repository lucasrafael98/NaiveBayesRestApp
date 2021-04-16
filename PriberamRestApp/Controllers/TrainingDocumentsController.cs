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
    public class TrainingDocumentsController : ControllerBase
    {
        private readonly TrainingContext _context;

        public TrainingDocumentsController(TrainingContext context)
        {
            _context = context;
        }

        // GET: api/TrainingDocuments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrainingDocument>>> GetDocuments()
        {
            return await _context.TrainingDocuments.ToListAsync();
        }

        // GET: api/TrainingDocuments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TrainingDocument>> GetTrainingDocument(long id)
        {
            var trainingDocument = await _context.TrainingDocuments.FindAsync(id);

            if (trainingDocument == null)
            {
                return NotFound();
            }

            return trainingDocument;
        }

        // PUT: api/TrainingDocuments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrainingDocument(long id, TrainingDocument trainingDocument)
        {
            if (id != trainingDocument.Id)
            {
                return BadRequest();
            }

            _context.Entry(trainingDocument).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrainingDocumentExists(id))
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

        // POST: api/TrainingDocuments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TrainingDocument>> PostTrainingDocument(TrainingDocument trainingDocument)
        {
            _context.TrainingDocuments.Add(trainingDocument);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTrainingDocument", new { id = trainingDocument.Id }, trainingDocument);
        }

        // DELETE: api/TrainingDocuments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrainingDocument(long id)
        {
            var trainingDocument = await _context.TrainingDocuments.FindAsync(id);
            if (trainingDocument == null)
            {
                return NotFound();
            }

            _context.TrainingDocuments.Remove(trainingDocument);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TrainingDocumentExists(long id)
        {
            return _context.TrainingDocuments.Any(e => e.Id == id);
        }
    }
}
