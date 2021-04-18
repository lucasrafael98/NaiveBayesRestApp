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
        public TrainingDocumentsController(){}

        // POST: api/training/document
        [HttpPost]
        public async Task<ActionResult<TrainingDocument>> PostTrainingDocument(TrainingDocument trainingDocument)
        {
            try
            {
                Classifier.Instance.Train(trainingDocument.Text, trainingDocument.Topic);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500);
            }

        }
    }
}
