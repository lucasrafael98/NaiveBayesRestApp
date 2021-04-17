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
    [Route("api/training/document")]
    [ApiController]
    public class TrainingDocumentsController : ControllerBase
    {
        private readonly TrainingContext _context;

        public TrainingDocumentsController(TrainingContext context)
        {
            _context = context;
        }

        // GET: api/training/document
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrainingDocument>>> GetDocuments()
        {
            return await _context.TrainingDocuments.ToListAsync();
        }

        // GET: api/training/document/{id}
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

        // POST: api/training/document
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TrainingDocument>> PostTrainingDocument(TrainingDocument trainingDocument)
        {
            _context.TrainingDocuments.Add(trainingDocument);
            await _context.SaveChangesAsync();

            try
            {
                Classifier.Instance.Train(trainingDocument);
                return Ok();
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500);
            }

        }

        private bool TrainingDocumentExists(long id)
        {
            return _context.TrainingDocuments.Any(e => e.Id == id);
        }
    }
}
