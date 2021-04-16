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
    public class ClassifyItemsController : ControllerBase
    {
        private readonly ClassifyContext _context;

        public ClassifyItemsController(ClassifyContext context)
        {
            _context = context;
        }

        // GET: api/ClassifyItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassifyItem>>> GetItems()
        {
            return await _context.Items.ToListAsync();
        }

        // GET: api/ClassifyItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClassifyItem>> GetClassifyItem(long id)
        {
            var classifyItem = await _context.Items.FindAsync(id);

            if (classifyItem == null)
            {
                return NotFound();
            }

            return classifyItem;
        }

        // PUT: api/ClassifyItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClassifyItem(long id, ClassifyItem classifyItem)
        {
            if (id != classifyItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(classifyItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassifyItemExists(id))
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

        // POST: api/ClassifyItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ClassifyItem>> PostClassifyItem(ClassifyItem classifyItem)
        {
            _context.Items.Add(classifyItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClassifyItem), new { id = classifyItem.Id }, classifyItem);
        }

        // DELETE: api/ClassifyItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClassifyItem(long id)
        {
            var classifyItem = await _context.Items.FindAsync(id);
            if (classifyItem == null)
            {
                return NotFound();
            }

            _context.Items.Remove(classifyItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClassifyItemExists(long id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}
