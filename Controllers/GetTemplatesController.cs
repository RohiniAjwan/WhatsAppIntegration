﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

 //POST: api/GetTemplates
 //To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CommonSuccessErrorResponse>> PostMarketingTemplates(CreateTemplate createTemplate)
        {
            try
            {
                CommonSuccessErrorResponse commonSuccessErrorResponse = new CommonSuccessErrorResponse();
                TemplateCreatedResponse commonResponse = new TemplateCreatedResponse();
                CommonErrorResponse commonErrorResponse = new CommonErrorResponse();
                HttpResponseMessage response = await CreateMarketingTemplate(createTemplate);
                var jsonResponse = response.Content.ReadAsStringAsync();

                if (jsonResponse != null)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        commonResponse = JsonConvert.DeserializeObject<TemplateCreatedResponse>(await jsonResponse);

                        commonSuccessErrorResponse.ErrorCode = 0;
                        if (commonResponse != null)
                        {
                            commonSuccessErrorResponse.Id = commonResponse?.Id ?? 0;
                        }
                        commonSuccessErrorResponse.ErrorMessage = commonResponse.Status;
                        return Ok(commonSuccessErrorResponse);
                    }
                    else {
                        commonErrorResponse = JsonConvert.DeserializeObject<CommonErrorResponse>(await jsonResponse);

                        commonSuccessErrorResponse.ErrorCode = 1;
                        commonSuccessErrorResponse.ErrorMessage = commonErrorResponse.Error.Message;
                        return Ok(commonSuccessErrorResponse);
                    }

                }

                commonSuccessErrorResponse.ErrorCode = 1;
                commonSuccessErrorResponse.ErrorMessage = "Something went wrong. Please try again";
                return Ok(commonResponse);
            }
            catch (Exception ex)
            {
                CommonSuccessErrorResponse commonSuccessErrorResponse = new CommonSuccessErrorResponse();
                commonSuccessErrorResponse.ErrorCode = 1;
                commonSuccessErrorResponse.ErrorMessage = ex.Message;
                return Ok(commonSuccessErrorResponse);
                throw (ex);
            }
        }

        private async Task<HttpResponseMessage> GetWhatsAppTemplates()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://graph.facebook.com/v22.0/1387600328936557/message_templates?fields=name,status,category,language,components,created_time&limit=1000";
                string accessToken = "EAAa6PEobbbQBOzHI8ffk1mtadKPKaLsSw3nFEP8HBdnAAGZAcZBeZAMk25n6hNM1W2aNUYW1dxnHF7w3U2T4r1eTGAMEqpBwY73gsNHLtd0HJONvLkhP0ZAjxssBBnMD1PUoPH1zjrlE1YrqGi7ENqnwO36VSEgMGZB1mARWWxPLkNlLZAs37De6TrEhFVLsvIC10TfhhfZAn78eFwf";

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                //var jsonPayload = JsonConvert.SerializeObject(payload);
                //var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                return await client.GetAsync(url);
            }
        }

        private async Task<HttpResponseMessage> CreateMarketingTemplate(CreateTemplate createTemplate)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://graph.facebook.com/v22.0/1387600328936557/message_templates";
                string accessToken = "EAAa6PEobbbQBOzHI8ffk1mtadKPKaLsSw3nFEP8HBdnAAGZAcZBeZAMk25n6hNM1W2aNUYW1dxnHF7w3U2T4r1eTGAMEqpBwY73gsNHLtd0HJONvLkhP0ZAjxssBBnMD1PUoPH1zjrlE1YrqGi7ENqnwO36VSEgMGZB1mARWWxPLkNlLZAs37De6TrEhFVLsvIC10TfhhfZAn78eFwf";

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var request = new CreateTemplateRequest
                {
                    Name = createTemplate.templateTitle??"",
                    Category = "MARKETING",
                    Language = "en_US",
                    Components = new List<Components>
            {
                new Components { Type = "HEADER", Format = createTemplate.title??"", Text = createTemplate.titleValue??"" },
                new Components { Type = "BODY", Text = createTemplate.body??"" },
                new Components { Type = "FOOTER", Text = createTemplate.footer??"" },
                new Components
                {
                    Type = "BUTTONS",
                    Buttons = new List<Buttons>
                    {
                        createTemplate.phoneNumberValue != null ? new Buttons
                            {
                                Type = "phone_number",
                                Text = createTemplate.phoneNumberTitle??"",
                                PhoneNumber = createTemplate.phoneNumberValue
                            } : null,
                        createTemplate.websiteLinkValue != null ? new Buttons
                            {
                                Type = "url",
                                Text = createTemplate.websiteLinkTitle??"",
                                Url = createTemplate.websiteLinkValue
                            } : null
                        //new Buttons { Type = "phone_number", Text = createTemplate.phoneNumberTitle??"", PhoneNumber = createTemplate.phoneNumberValue??"" },
                        //new Buttons { Type = "url", Text = createTemplate.websiteLinkTitle??"", Url = createTemplate.websiteLinkValue??"" }
                    }.Where(b => b != null).ToList()
                }
            }
                };

                var jsonPayload = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                return await client.PostAsync(url, content);
            }
        }
    }
}
                    //
/**/