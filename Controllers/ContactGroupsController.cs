using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhatsAppIntegration.Data;
using WhatsAppIntegration.Model;

namespace WhatsAppIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactGroupsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ContactGroupsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/ContactGroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactGroup>>> GetContactGroups()
        {
            return await _context.ContactGroups.ToListAsync();
        }

        // GET: api/ContactGroups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactGroup>> GetContactGroup(int? id)
        {
            var contactGroup = await _context.ContactGroups.FindAsync(id);

            if (contactGroup == null)
            {
                return NotFound();
            }

            return contactGroup;
        }

        // PUT: api/ContactGroups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactGroup(int? id, ContactGroup contactGroup)
        {
            if (id != contactGroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(contactGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactGroupExists(id))
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

        // POST: api/ContactGroups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ContactGroup>> PostContactGroup(ContactGroup contactGroup)
        {
            _context.ContactGroups.Add(contactGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContactGroup", new { id = contactGroup.Id }, contactGroup);
        }

        // DELETE: api/ContactGroups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactGroup(int? id)
        {
            var contactGroup = await _context.ContactGroups.FindAsync(id);
            if (contactGroup == null)
            {
                return NotFound();
            }

            _context.ContactGroups.Remove(contactGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactGroupExists(int? id)
        {
            return _context.ContactGroups.Any(e => e.Id == id);
        }
    }
}
