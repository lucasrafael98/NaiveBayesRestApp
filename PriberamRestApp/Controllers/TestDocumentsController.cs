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

        public TestDocumentsController(){}

        // POST: api/test/document
        [HttpPost]
        public async Task<ActionResult<TestDocument>> PostTestDocument(TestDocument testDocument)
        {

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
    }
}
