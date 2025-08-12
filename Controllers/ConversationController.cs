using WhatsAppIntegration.Data;
using WhatsAppIntegration.Model;
using OrderDashboard.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;
using Microsoft.Data.SqlClient;
using System.ComponentModel.Design;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using WhatsAppIntegration.Services;
using Newtonsoft.Json.Linq;

namespace WhatsAppIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly TokenSetting _tokenSettings;
        private readonly MessageServices _messageServices;

        public ConversationController(ApplicationDBContext context, IOptions<TokenSetting> tokenOptions, MessageServices messageServices)
        {
            _context = context;
            _tokenSettings = tokenOptions.Value;
            _messageServices = messageServices;
        }

        // GET: api/Contacts
        [HttpGet]
        public IActionResult GetConversationUserList(String mode)
        {
            int p_int_prmErrCode = -1;
            string p_str_prmErrorMsg = "";

            try
            {
                _context.Database.OpenConnection();
                DbCommand cmd = _context.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "sp_GetConversations";
                cmd.CommandType = CommandType.StoredProcedure;


                DbParameter prmMode = cmd.CreateParameter();
                prmMode.ParameterName = "@prmMode";
                prmMode.Value = mode;
                prmMode.DbType = DbType.String;
                cmd.Parameters.Add(prmMode);

                // Output Parameters
                DbParameter prmErrCode = cmd.CreateParameter();
                prmErrCode.ParameterName = "@prmErrCode";
                prmErrCode.DbType = DbType.Int32;
                prmErrCode.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(prmErrCode);

                DbParameter prmErrMsg = cmd.CreateParameter();
                prmErrMsg.ParameterName = "@prmErrMsg";
                prmErrMsg.DbType = DbType.String;
                prmErrMsg.Size = 500;
                prmErrMsg.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(prmErrMsg);              

                //cmd.ExecuteNonQuery();

                ConversationModel conversationModel = new ConversationModel();
                List<ConversationUser> conversationUserList = [];

                using (var reader = cmd.ExecuteReader())
                {
                    conversationUserList = reader.MapToList<ConversationUser>();
                }

                //p_int_prmErrCode = Convert.ToInt32(cmd.Parameters["@prmErrCode"].Value);
                //p_str_prmErrorMsg = Convert.ToString(cmd.Parameters["@prmErrMsg"].Value);
                conversationModel.conversationUserList = conversationUserList;
                return Ok(conversationModel);

            }
            catch (Exception ex)
            {
                ConversationModel conversationModel = new()
                {
                    conversationUserList = []
                };
                return Ok(conversationModel);
            }
        }

        [HttpPost("SendConversationMessages")]
        public async Task<IActionResult> PostSendConversationMessagesAsync(String phoneNumber, String message)
        {
            SendMessage sendMessage = new SendMessage();
            sendMessage.PhoneNumberConv = phoneNumber;
            sendMessage.TemplateTitle = message;
            _context.SendMessages.Add(sendMessage);
            List<MessageSendingResponse> whatsAppResponse = [];
            bool updateResponse = false;
            try
            {
                if (sendMessage.PhoneNumberConv != null)
                {
                        HttpResponseMessage response = await SendWhatsAppConversationMessage(sendMessage.PhoneNumberConv, sendMessage.TemplateTitle);
                        var jsonResponse = response.Content.ReadAsStringAsync();

                        if (jsonResponse != null)
                        {
                            MessageSendingResponse responseModel = JsonConvert.DeserializeObject<MessageSendingResponse>(await jsonResponse);
                            whatsAppResponse.Add(responseModel!);
                        }
                    //UpdateLogs(whatsAppResponse, sendMessage.TemplateTitle, sendMessage.PhoneNumberConv);


                    string message1 = "";
                    string message2 = "";
                    string message3 = "";
                    string message4 = "";
                    string message5 = "";
                    string body = message;
                    int len = body.Length;

                    if (len <= 5)
                    {
                        // If length <= 5, put whole string in MessageResponseJson1
                        message1 = body;
                        message2 = string.Empty;
                        message3 = string.Empty;
                        message4 = string.Empty;
                        message5 = string.Empty;
                    }
                    else
                    {
                        // Split into 5 equal (or near equal) parts
                        int partSize = (int)Math.Ceiling(len / 5.0);

                        message1 = body.Substring(0, Math.Min(partSize, len));
                        message2 = (len > partSize) ? body.Substring(partSize, Math.Min(partSize, len - partSize)) : string.Empty;
                        message3 = (len > partSize * 2) ? body.Substring(partSize * 2, Math.Min(partSize, len - partSize * 2)) : string.Empty;
                        message4 = (len > partSize * 3) ? body.Substring(partSize * 3, Math.Min(partSize, len - partSize * 3)) : string.Empty;
                        message5 = (len > partSize * 4) ? body.Substring(partSize * 4, Math.Min(partSize, len - partSize * 4)) : string.Empty;
                    }
                    IncomingMedia incomingMedia = new IncomingMedia();
                    incomingMedia.ProfileWaId = phoneNumber;
                    incomingMedia.ToMessageId = whatsAppResponse[0].Messages[0].Id;
                    incomingMedia.MessageMainType = "text";
                    incomingMedia.MessageResponseJson1 = message1;
                    incomingMedia.MessageResponseJson2 = message2;
                    incomingMedia.MessageResponseJson3 = message3;
                    incomingMedia.MessageResponseJson4 = message4;
                    incomingMedia.MessageResponseJson5 = message5;

                    updateResponse = await _messageServices.SaveIncomingMessage(incomingMedia);

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

        private async Task<HttpResponseMessage> SendWhatsAppConversationMessage(String number, String message)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://graph.facebook.com/v22.0/{_tokenSettings.PhoneNumberId}/messages";
                string accessToken = _tokenSettings.AccessToken;

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var payload = new
                {
                    messaging_product = "whatsapp",
                    recipient_type= "individual",
                    to = number,
                    type = "text",
                    text= new {
                    preview_url= true,
                        body= message
                      }                
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                return await client.PostAsync(url, content);
            }
        }
    }
}
