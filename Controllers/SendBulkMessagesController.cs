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
    public class SendBulkMessagesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public SendBulkMessagesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/SendBulkMessages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SendBulkMessage>>> GetSendBulkMessages()
        {
            return await _context.SendBulkMessages.ToListAsync();
        }

        // GET: api/SendBulkMessages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SendBulkMessage>> GetSendBulkMessage(int? id)
        {
            var sendBulkMessage = await _context.SendBulkMessages.FindAsync(id);

            if (sendBulkMessage == null)
            {
                return NotFound();
            }

            return sendBulkMessage;
        }

        // PUT: api/SendBulkMessages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSendBulkMessage(int? id, SendBulkMessage sendBulkMessage)
        {
            if (id != sendBulkMessage.Id)
            {
                return BadRequest();
            }

            _context.Entry(sendBulkMessage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SendBulkMessageExists(id))
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

        // POST: api/SendBulkMessages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SendBulkMessage>> PostSendBulkMessage(SendBulkMessage sendBulkMessage)
        {
            _context.SendBulkMessages.Add(sendBulkMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSendBulkMessage", new { id = sendBulkMessage.Id }, sendBulkMessage);
        }

        // DELETE: api/SendBulkMessages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSendBulkMessage(int? id)
        {
            var sendBulkMessage = await _context.SendBulkMessages.FindAsync(id);
            if (sendBulkMessage == null)
            {
                return NotFound();
            }

            _context.SendBulkMessages.Remove(sendBulkMessage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SendBulkMessageExists(int? id)
        {
            return _context.SendBulkMessages.Any(e => e.Id == id);
        }
    }
}
