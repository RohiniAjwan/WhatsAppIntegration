using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Dynamic;
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
using Microsoft.Data.SqlClient;

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
        [HttpPost("SendMessages")]
        public async Task<IActionResult> PostSendTemplateMessageAsync(List<String> phoneNumbers, String templateTitle, String languageCode)
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
                    HttpResponseMessage response =  await SendWhatsAppMessage(number, sendMessage.TemplateTitle, languageCode);
                    var jsonResponse =  response.Content.ReadAsStringAsync();

                    if(jsonResponse != null){
                            MessageSendingResponse responseModel = JsonConvert.DeserializeObject<MessageSendingResponse>(await jsonResponse);
                            whatsAppResponse.Add(responseModel!);
                        }
                    }
                         UpdateLogs(whatsAppResponse, templateTitle);
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

        // POST: api/SendMediaMessages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("SendMediaMessages")]
        public async Task<IActionResult> PostSendTemplateMediaMessageAsync(List<String> phoneNumbers, String templateTitle,
                       String typeValue, String linkValue, String languageCode)

        {
            SendMessage sendMessage = new SendMessage();
            sendMessage.PhoneNumber = phoneNumbers;
            sendMessage.TemplateTitle = templateTitle;
            _context.SendMessages.Add(sendMessage);
            List<MessageSendingResponse> whatsAppResponse = [];
            try
            {
                if (sendMessage.PhoneNumber != null)
                {
                    foreach (String number in sendMessage.PhoneNumber)
                    {
                        HttpResponseMessage response = await SendWhatsAppMediaMessages(number, sendMessage.TemplateTitle, typeValue, linkValue, languageCode);
                        var jsonResponse = response.Content.ReadAsStringAsync();

                        if (jsonResponse != null)
                        {
                            MessageSendingResponse responseModel = JsonConvert.DeserializeObject<MessageSendingResponse>(await jsonResponse);
                            whatsAppResponse.Add(responseModel!);
                        }
                    }
                    UpdateLogs(whatsAppResponse, templateTitle);
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
        // POST: api/SendDocumentMessages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("SendDocumentMessages")]
        public async Task<IActionResult> PostSendTemplateDocumentMessageAsync(List<String> phoneNumbers, String templateTitle,
                       String typeValue, String linkValue, String languageCode, String fileName)

        {
            SendMessage sendMessage = new SendMessage();
            sendMessage.PhoneNumber = phoneNumbers;
            sendMessage.TemplateTitle = templateTitle;
            _context.SendMessages.Add(sendMessage);
            List<MessageSendingResponse> whatsAppResponse = [];
            try
            {
                if (sendMessage.PhoneNumber != null)
                {
                    foreach (String number in sendMessage.PhoneNumber)
                    {
                        HttpResponseMessage response = await SendWhatsAppDocumentMessages(number, sendMessage.TemplateTitle, typeValue, linkValue, languageCode, fileName);
                        var jsonResponse = response.Content.ReadAsStringAsync();

                        if (jsonResponse != null)
                        {
                            MessageSendingResponse responseModel = JsonConvert.DeserializeObject<MessageSendingResponse>(await jsonResponse);
                            whatsAppResponse.Add(responseModel!);
                        }
                    }
                    UpdateLogs(whatsAppResponse, templateTitle);
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

        private async Task<HttpResponseMessage> SendWhatsAppMediaMessages(String number, String title, String typeValue, String linkValue, String languageCode)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://graph.facebook.com/v22.0/545009808703733/messages";
                string accessToken = "EAAa6PEobbbQBOzHI8ffk1mtadKPKaLsSw3nFEP8HBdnAAGZAcZBeZAMk25n6hNM1W2aNUYW1dxnHF7w3U2T4r1eTGAMEqpBwY73gsNHLtd0HJONvLkhP0ZAjxssBBnMD1PUoPH1zjrlE1YrqGi7ENqnwO36VSEgMGZB1mARWWxPLkNlLZAs37De6TrEhFVLsvIC10TfhhfZAn78eFwf";

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                
                dynamic parameter = new ExpandoObject();
                parameter.type = typeValue;

                dynamic linkParameter = new ExpandoObject();
                linkParameter.link = linkValue;
                //((IDictionary<string, object>)parameter)[typeValue] = linkValue;
                ((IDictionary<string, object>)parameter)[typeValue] = linkParameter;
  
                var payload = new
                {
                    messaging_product = "whatsapp",
                    to = number,
                    type = "template",
                    template = new
                    {
                        name = title,
                        language = new { code = languageCode },
                   
                    components = new[]
                    {
                        new
                        {
                            type = "header",
                            parameters = new[] { parameter }
                        }
                    }
                  }
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                return await client.PostAsync(url, content);
            }
        }
        
        private async Task<HttpResponseMessage> SendWhatsAppDocumentMessages(String number, String title, String typeValue, String linkValue,
            String languageCode, String fileName)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://graph.facebook.com/v22.0/545009808703733/messages";
                string accessToken = "EAAa6PEobbbQBOzHI8ffk1mtadKPKaLsSw3nFEP8HBdnAAGZAcZBeZAMk25n6hNM1W2aNUYW1dxnHF7w3U2T4r1eTGAMEqpBwY73gsNHLtd0HJONvLkhP0ZAjxssBBnMD1PUoPH1zjrlE1YrqGi7ENqnwO36VSEgMGZB1mARWWxPLkNlLZAs37De6TrEhFVLsvIC10TfhhfZAn78eFwf";

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                
                dynamic parameter = new ExpandoObject();
                parameter.type = typeValue;

                dynamic linkParameter = new ExpandoObject();
                linkParameter.link = linkValue;
                linkParameter.filename = fileName;
                //((IDictionary<string, object>)parameter)[typeValue] = linkValue;
                ((IDictionary<string, object>)parameter)[typeValue] = linkParameter;
  
                var payload = new
                {
                    messaging_product = "whatsapp",
                    to = number,
                    type = "template",
                    template = new
                    {
                        name = title,
                        language = new { code = languageCode },
                   
                    components = new[]
                    {
                        new
                        {
                            type = "header",
                            parameters = new[] { parameter }
                        }
                    }
                  }
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                return await client.PostAsync(url, content);
            }
        }

        private async Task<HttpResponseMessage> SendWhatsAppMessage(String number, String title, String languageCode)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://graph.facebook.com/v22.0/545009808703733/messages";
                string accessToken = "EAAa6PEobbbQBOzHI8ffk1mtadKPKaLsSw3nFEP8HBdnAAGZAcZBeZAMk25n6hNM1W2aNUYW1dxnHF7w3U2T4r1eTGAMEqpBwY73gsNHLtd0HJONvLkhP0ZAjxssBBnMD1PUoPH1zjrlE1YrqGi7ENqnwO36VSEgMGZB1mARWWxPLkNlLZAs37De6TrEhFVLsvIC10TfhhfZAn78eFwf";

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
                        language = new { code = languageCode }
                    }
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                return await client.PostAsync(url, content);
            }
        }

        private void UpdateLogs(List<MessageSendingResponse> messageSendingResponseList, string templateName) {
            try
            {
                int p_int_prmErrCode = -1;
                string p_str_prmErrorMsg = "";

                _context.Database.OpenConnection();
                using (var conn = (SqlConnection)_context.Database.GetDbConnection())  // Cast to SqlConnection
                {
                    using (var cmd = new SqlCommand("sp_InsertBulkSentLogs", conn)) // Use SqlCommand
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Create DataTable for Table-Valued Parameter
                        DataTable messageLogMaster = new DataTable();
                        messageLogMaster.Columns.Add("MessageId", typeof(string));
                        messageLogMaster.Columns.Add("PhoneNumber", typeof(string));
                        messageLogMaster.Columns.Add("TemplateName", typeof(string));
                        
                        // Populate DataTable
                        foreach (MessageSendingResponse c in messageSendingResponseList)
                        {
                            messageLogMaster.Rows.Add(c.Messages[0].Id??"", c.Contacts[0].Input??"", templateName);
                        }

                        // Add Table-Valued Parameter
                        var param = cmd.Parameters.AddWithValue("@messageSendLogs", messageLogMaster);
                        param.SqlDbType = SqlDbType.Structured;  // Now it works because we're using SqlCommand

                        // Output Parameters
                        var prmErrCode = new SqlParameter("@prmErrCode", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(prmErrCode);

                        var prmErrMsg = new SqlParameter("@prmErrMsg", SqlDbType.NVarChar, 500)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(prmErrMsg);

                        // Execute Stored Procedure
                        cmd.ExecuteNonQuery();

                        // Retrieve Output Parameters
                        //p_int_prmErrCode = (int)(prmErrCode.Value ?? -1);
                        //p_str_prmErrorMsg = prmErrMsg.Value?.ToString() ?? "Unknown error";

                        // Prepare response
                        //var commonResponse = new CommonResponse
                        //{
                        //    Error = p_int_prmErrCode == 0 ? 0 : -1,
                        //    Message = p_int_prmErrCode == 0 ? "Uploaded Successfully" : "Something went wrong"
                        //};

                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

    }
}
