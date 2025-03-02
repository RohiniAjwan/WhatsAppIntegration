using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WhatsAppIntegration.Data;
using WhatsAppIntegration.Model;

namespace WhatsAppIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendMessagesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public SendMessagesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/SendMessages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SendMessage>>> GetSendMessages()
        {
            return await _context.SendMessages.ToListAsync();
        }

        // GET: api/SendMessages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SendMessage>> GetSendMessage(string id)
        {
            var sendMessage = await _context.SendMessages.FindAsync(id);

            if (sendMessage == null)
            {
                return NotFound();
            }

            return sendMessage;
        }

        // POST: api/SendMessages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostSendTemplateMessageAsync(List<String> phoneNumbers, String templateTitle)
        {
            SendMessage sendMessage = new SendMessage();
            sendMessage.PhoneNumber = phoneNumbers;
            sendMessage.TemplateTitle = templateTitle;
            _context.SendMessages.Add(sendMessage);
                           List<MessageSendingResponse> whatsAppResponse = [];
            try
            {
                if(sendMessage.PhoneNumber != null){
                    foreach (String number in sendMessage.PhoneNumber) {
                    HttpResponseMessage response =  await SendWhatsAppMessage(number, sendMessage.TemplateTitle);
                    var jsonResponse =  response.Content.ReadAsStringAsync();

                    if(jsonResponse != null){
                            MessageSendingResponse responseModel = JsonConvert.DeserializeObject<MessageSendingResponse>(await jsonResponse);
                            whatsAppResponse.Add(responseModel!);
                        }
                    }
                                        return Ok(whatsAppResponse);
}

                WhatsAppMessage whatsAppMessage = new WhatsAppMessage();
                whatsAppMessage.MessageStatus = "failed";
                MessageSendingResponse messageSendingResponse = new MessageSendingResponse();
                messageSendingResponse.Messages.Add(whatsAppMessage);
                whatsAppResponse = [];
                whatsAppResponse.Add(messageSendingResponse);

                return Ok(whatsAppResponse);
            }
            catch (Exception e)
            {
                WhatsAppMessage whatsAppMessage = new WhatsAppMessage();
                whatsAppMessage.MessageStatus = e.ToString();
                MessageSendingResponse messageSendingResponse = new MessageSendingResponse();
                messageSendingResponse.Messages.Add(whatsAppMessage);
                whatsAppResponse = [];
                whatsAppResponse.Add(messageSendingResponse);
                return Ok(whatsAppResponse);
                throw;

            }

            //return CreatedAtAction("GetSendMessage", new { id = sendMessage.PhoneNumber }, sendMessage);
        }

        private async Task<HttpResponseMessage> SendWhatsAppMessage(String number, String title)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://graph.facebook.com/v22.0/611970238655568/messages";
                string accessToken = "EAAa6PEobbbQBO6bldzNimZCiyz4rfPNDe0ekwVIsWuP6TnvweurccebKbIo8yFVSPNZBT9kO1VsNsA242ic1L5C7EQUH5lhEpRpbIIzERCRewm37ovEchqbja5ySt8OA80q3MsfvRStzRyGqCKfn3RKshtGemvUtmFrMqh211qMOzipwQiZAI7PKUj0H9OVtfZAOTglLdaQmrX7LwZCavuPMoWRmjgfZA9ypH8Pay1DrOvMm0cZC1EZD";

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var payload = new
                {
                    messaging_product = "whatsapp",
                    to = number,
                    type = "template",
                    template = new
                    {
                        name = title,
                        language = new { code = "en" }
                    }
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                return await client.PostAsync(url, content);
            }
        }
    }
}
