using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WhatsAppIntegration.Data;
using WhatsAppIntegration.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WhatsAppIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetTemplatesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public GetTemplatesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/GetTemplates
        [HttpGet]
        public async Task<IActionResult> GetGetTemplatesList()
        {
            GetTemplates getTemplates = new GetTemplates();            

            try
            {
                HttpResponseMessage response = await GetWhatsAppTemplates();
                var jsonResponse = response.Content.ReadAsStringAsync();

                if (jsonResponse != null)
                {
                    getTemplates = JsonConvert.DeserializeObject<GetTemplates>(await jsonResponse);
                }

                return Ok(getTemplates);



            }
            catch (Exception e)
            {
                
                return Ok(getTemplates);
                throw e;
            }
        }

// POST: api/GetTemplates
// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//[HttpPost]
//public async Task<ActionResult<GetTemplates>> PostGetTemplates(GetTemplates getTemplates)
//{
//    _context.GetTemplatesList.Add(getTemplates);
//    try
//    {
//        await _context.SaveChangesAsync();
//    }
//    catch (DbUpdateException)
//    {
//        if (GetTemplatesExists(getTemplates.Id))
//        {
//            return Conflict();
//        }
//        else
//        {
//            throw;
//        }
//    }

//    return CreatedAtAction("GetGetTemplates", new { id = getTemplates.Id }, getTemplates);
//}

private async Task<HttpResponseMessage> GetWhatsAppTemplates()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://graph.facebook.com/v22.0/594349090422639/message_templates?fields=name,status,category,language,components,created_time&limit=1000";
                string accessToken = "EAAa6PEobbbQBOzJlBIzEDpVnnAl8b5GY1bNXSASU4vzQUhQ4LQC3fuWFy5hEZCOnTJt1wXQBrPsuOZAhf4eU32ETSqXex32L45ZAzZBLSCrFx9aMKcscanZCFzPa2y7jOi6jM4CrDWCJ88mwZAt7V06PQf5v3NZCgkGG1AZC2FxuB41zsbPhiMe7VIZBZBqF0GPZCe5iAm8hCCIoVLOrRhgAF1ZAWyou9fJ2PHwLr6vSdz4AkLYT8sDdf9gZD";

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                //var jsonPayload = JsonConvert.SerializeObject(payload);
                //var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                return await client.GetAsync(url);
            }
        }
    }
}
