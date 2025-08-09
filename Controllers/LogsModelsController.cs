using System.Data;
using System.Data.Common;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using OrderDashboard.Utilities;
using WhatsAppIntegration.Data;
using WhatsAppIntegration.Model;

namespace WhatsAppIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsModelsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly string _validToken;
        private readonly TokenSetting _tokenSettings;

        public LogsModelsController(ApplicationDBContext context, IConfiguration configuration, IOptions<TokenSetting> tokenOptions)
        {
            _context = context;
            _validToken = configuration["Token:BearerToken"];
            _tokenSettings = tokenOptions.Value;
        }


        [HttpGet("webhook")]
        public IActionResult VerifyWebhook(
            [FromQuery(Name = "hub.mode")] string mode,
            [FromQuery(Name = "hub.challenge")] string challenge,
            [FromQuery(Name = "hub.verify_token")] string verify_token)
        {
            Console.WriteLine($"Received Webhook Verification Request: mode={mode}, challenge={challenge}, verify_token={verify_token}");

            string verifyToken = "S8vmOYkOMPrz6xHv9iu6GVKsQWCt0kvAbsP66b8GkTsIlPJUikQvmKCPb7";

            if (mode == "subscribe" && verify_token == verifyToken)
            {
                Console.WriteLine("Verification Successful. Sending challenge back.");
                return Ok(challenge);
            }

            Console.WriteLine("Verification Failed.");
            return Unauthorized("Invalid verification token");
        }


        //[Authorize]
        [HttpPost("webhook")]
        [Produces("application/json")]
        public async Task<IActionResult> PostLogsModel(/*[FromBody] LogsModel logsModel*/)
        {

            try
            {
                if (Request.Body == null)
                {
                    Utility.DoHandle("Invalid data.", "SavePayLoad");
                    return BadRequest("Invalid data.");
                    //return;
                }

                string rawBody = "";
                using (var reader = new StreamReader(Request.Body))
                {
                    rawBody = await reader.ReadToEndAsync();
                    Utility.SavePayLoad(rawBody);
                }
                /*String payload = JsonConvert.SerializeObject(logsModel);
                Utility.SavePayLoad(payload);*/

                if (!string.IsNullOrWhiteSpace(rawBody))
                {
                    var logsModel = JsonConvert.DeserializeObject<LogsModel>(rawBody);
                    List<Status> statusList = [];
                    foreach (var entry in logsModel.entry ?? new List<Entry>())
                    {
                        foreach (var change in entry.changes ?? new List<Change>())
                        {
                            var value = change.value;

                            if (value != null && value.statuses != null)
                            {
                                foreach (var status in value.statuses)
                                {
                                    Status st = new Status();

                                    st.id = status.id;
                                    st.status = status.status;
                                    st.recipient_id = status.recipient_id;
                                    st.timestamp = status.timestamp;
                                    statusList.Add(st);
                                }
                                bool response = await StoreStatuses(statusList);

                                return Ok(new CommonSuccessErrorResponse { ErrorCode = response ? 1 : -5, ErrorMessage = response ? "Success" : "Failure"});
                            }
                            else if (value.contacts != null && value.messages != null)
                            {
                                bool response = false;
                                foreach (Message message in value.messages)
                                {
                                    string from = message.from;
                                    string messageType = message.type;
                                    string timestamp = message.timestamp;
                                    switch (messageType)
                                    {
                                        case "text":
                                            var image = message.text;
                                            string message1 = "";
                                            string message2 = "";
                                            string message3 = "";
                                            string message4 = "";
                                            string message5 = "";
                                            if (message.text != null && !string.IsNullOrEmpty(message.text.body))
                                            {
                                                string body = message.text.body;
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
                                            }
                                            IncomingMedia incomingMedia = new IncomingMedia();
                                            incomingMedia.EntryId = entry.id;
                                            incomingMedia.DisplayPhoneNumber = value.metadata?.DisplayPhoneNumber;
                                            incomingMedia.DispalyPhoneNumberId = value.metadata?.PhoneNumberId;
                                            incomingMedia.ProfileName = value.contacts[0].profile?.name;
                                            incomingMedia.ProfileWaId = value.contacts[0].WaId;
                                            //incomingMedia.ToNumber = null;
                                            //incomingMedia.ToMessageId = null;
                                            incomingMedia.FromNumber = message.from;
                                            incomingMedia.FromMessageId = message.id;
                                            incomingMedia.MessageTimestamp = message.timestamp;
                                            incomingMedia.MessageMainType = message.type;
                                            //incomingMedia.MessageSubType = null;
                                            //incomingMedia.MimeType = null;
                                            //incomingMedia.SHA256 = null;
                                            //incomingMedia.MediaId = null;
                                            //incomingMedia.DocumentName = null;
                                            incomingMedia.MessageResponseJson1 = message1;
                                            incomingMedia.MessageResponseJson2 = message2;
                                            incomingMedia.MessageResponseJson3 = message3;
                                            incomingMedia.MessageResponseJson4 = message4;
                                            incomingMedia.MessageResponseJson5 = message5;
                                            //incomingMedia.MessageBody = null;
                                            //incomingMedia.MessageName = null;
                                            incomingMedia.Field = change.field;
                                            //incomingMedia.LocationLatitude = null;
                                            /*incomingMedia.LocationLongitude = null;
                                            incomingMedia.LocationName = null;
                                            incomingMedia.LocationAddress = null;
                                            incomingMedia.Phone = null;
                                            incomingMedia.PhoneType = null;
                                            incomingMedia.Email = null;
                                            incomingMedia.EmailType = null;
                                            incomingMedia.FormattedName = null;
                                            incomingMedia.FirstName = null;
                                            incomingMedia.LastName = null;*/

                                            // Example: Store in DB or forward to UI
                                                response = await SaveIncomingMessage(incomingMedia);
                                            break;

                                        /*case "image":
                                            var image = message.image;
                                            // Example: Store in DB or forward to UI
                                            SaveIncomingMedia(new IncomingMedia
                                            {
                                                From = from,
                                                Type = "image",
                                                MediaId = image.id,
                                                MimeType = image.mime_type,
                                                Caption = image.caption,
                                                Sha256 = image.sha256,
                                                Timestamp = timestamp
                                            });
                                            break;

                                        case "video":
                                            var video = message.video;
                                            SaveIncomingMedia(new IncomingMedia
                                            {
                                                From = from,
                                                Type = "video",
                                                MediaId = video.id,
                                                MimeType = video.mime_type,
                                                Caption = video.caption,
                                                Sha256 = video.sha256,
                                                Timestamp = timestamp
                                            });
                                            break;

                                        case "audio":
                                            var audio = message.audio;
                                            SaveIncomingMedia(new IncomingMedia
                                            {
                                                From = from,
                                                Type = "audio",
                                                MediaId = audio.id,
                                                MimeType = audio.mime_type,
                                                Sha256 = audio.sha256,
                                                Timestamp = timestamp
                                            });
                                            break;

                                        case "document":
                                            var audio = message.audio;
                                            SaveIncomingMedia(new IncomingMedia
                                            {
                                                From = from,
                                                Type = "audio",
                                                MediaId = audio.id,
                                                MimeType = audio.mime_type,
                                                Sha256 = audio.sha256,
                                                Timestamp = timestamp
                                            });
                                            break;*/

                                        /*case "contacts":
                                            foreach (var contact in message.contacts)
                                            {
                                                SaveIncomingContact(new IncomingContact
                                                {
                                                    From = from,
                                                    FullName = contact.name.formatted_name,
                                                    FirstName = contact.name.first_name,
                                                    LastName = contact.name.last_name,
                                                    Phones = contact.phones.Select(p => p.phone).ToList(),
                                                    Emails = contact.emails?.Select(e => e.email).ToList(),
                                                    Timestamp = timestamp
                                                });
                                            }
                                            break;

                                        case "location":
                                            var loc = message.location;
                                            SaveIncomingLocation(new IncomingLocation
                                            {
                                                From = from,
                                                Latitude = loc.latitude,
                                                Longitude = loc.longitude,
                                                Name = loc.name,
                                                Address = loc.address,
                                                Timestamp = timestamp
                                            });
                                            break;*/

                                        default:
                                            //LogUnknownMessageType(messageType, message);
                                            break;
                                    }
                                }
                                return Ok(new CommonSuccessErrorResponse { ErrorCode = response ? 1 : -5, ErrorMessage = response ? "Success" : "Failure" });
                            }
                        }
                    }
                    return Ok(new CommonSuccessErrorResponse { ErrorCode = 1, ErrorMessage = "Success"});
                }
                else {
                    return Ok(new CommonSuccessErrorResponse { ErrorCode = -2, ErrorMessage = "Raw body is empty!" });
                    //return;
                }
            }
            catch (Exception ex)
            {
                return Ok(new CommonSuccessErrorResponse { ErrorCode = -2, ErrorMessage = ex.Message });
                //return;
            }
        }

        [NonAction]
        private async Task<bool> StoreStatuses(List<Status> statusList) {
            try
            {
                _context.Database.OpenConnection();
                using (var conn = (SqlConnection)_context.Database.GetDbConnection())  // Cast to SqlConnection
                {
                    using (var cmd = new SqlCommand("sp_InsertBulkStatusList", conn)) // Use SqlCommand
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Create DataTable for Table-Valued Parameter
                        DataTable statusTable = new DataTable();
                        statusTable.Columns.Add("Id", typeof(string));
                        statusTable.Columns.Add("Status", typeof(string));
                        statusTable.Columns.Add("RecipientId", typeof(string));
                        statusTable.Columns.Add("Timestamp", typeof(string));

                        // Populate DataTable
                        foreach (Status c in statusList)
                        {
                            statusTable.Rows.Add(c.id, c.status, c.recipient_id, c.timestamp);
                        }

                        // Add Table-Valued Parameter
                        var param = cmd.Parameters.AddWithValue("@StatusTableList", statusTable);
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

                        int p_int_prmErrCode = -1;
                        string p_str_prmErrorMsg = "";

                        // Retrieve Output Parameters
                        p_int_prmErrCode = (int)(prmErrCode.Value ?? -1);
                        p_str_prmErrorMsg = prmErrMsg.Value?.ToString() ?? "Unknown error";

                        // Prepare response
                        var commonResponse = new CommonSuccessErrorResponse
                        {
                            ErrorCode = p_int_prmErrCode == 0 ? 0 : -1,
                            ErrorMessage = p_int_prmErrCode == 0 ? "Uploaded Successfully" : "Something went wrong"
                        };

                        return p_int_prmErrCode == 1;
                    }
                }
            }
            catch (Exception ex) { 
                return false;
            }

        }

        [NonAction]
        private async Task<bool> SaveIncomingMessage(IncomingMedia incomingMedia) {

            try
            {
                _context.Database.OpenConnection();
                DbCommand cmd = _context.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "sp_InsertReceiveLogMaster";
                cmd.CommandType = CommandType.StoredProcedure;


                // Adding parameters correctly
                DbParameter prmEntryId = cmd.CreateParameter();
                prmEntryId.ParameterName = "@prmEntryId";
                prmEntryId.Value = (object?)incomingMedia.EntryId ?? DBNull.Value;
                prmEntryId.DbType = DbType.String;
                prmEntryId.SourceColumnNullMapping = true;
                cmd.Parameters.Add(prmEntryId);

                DbParameter prmDisplayPhoneNumber = cmd.CreateParameter();
                prmDisplayPhoneNumber.ParameterName = "@prmDisplayPhoneNumber";
                prmDisplayPhoneNumber.Value = (object?)incomingMedia.DisplayPhoneNumber ?? DBNull.Value;
                prmDisplayPhoneNumber.DbType = DbType.String;
                cmd.Parameters.Add(prmDisplayPhoneNumber);

                DbParameter prmDispalyPhoneNumberId = cmd.CreateParameter();
                prmDispalyPhoneNumberId.ParameterName = "@prmDispalyPhoneNumberId";
                prmDispalyPhoneNumberId.Value = (object?)incomingMedia.DispalyPhoneNumberId ?? DBNull.Value;
                prmDispalyPhoneNumberId.DbType = DbType.String;
                cmd.Parameters.Add(prmDispalyPhoneNumberId);

                DbParameter prmProfileName = cmd.CreateParameter();
                prmProfileName.ParameterName = "@prmProfileName";
                prmProfileName.Value = (object?)incomingMedia.ProfileName ?? DBNull.Value;
                prmProfileName.DbType = DbType.String;
                cmd.Parameters.Add(prmProfileName);

                DbParameter prmProfileWaId = cmd.CreateParameter();
                prmProfileWaId.ParameterName = "@prmProfileWaId";
                prmProfileWaId.Value = (object?)incomingMedia.ProfileWaId ?? DBNull.Value;
                prmProfileWaId.DbType = DbType.String;
                cmd.Parameters.Add(prmProfileWaId);

                DbParameter prmToNumber = cmd.CreateParameter();
                prmToNumber.ParameterName = "@prmToNumber";
                prmToNumber.Value = (object?)incomingMedia.ToNumber ?? DBNull.Value;
                prmToNumber.DbType = DbType.String;
                cmd.Parameters.Add(prmToNumber);

                DbParameter prmToMessageId = cmd.CreateParameter();
                prmToMessageId.ParameterName = "@prmToMessageId";
                prmToMessageId.Value = (object?)incomingMedia.ToMessageId ?? DBNull.Value;
                prmToMessageId.DbType = DbType.String;
                cmd.Parameters.Add(prmToMessageId);

                DbParameter prmFromNumber = cmd.CreateParameter();
                prmFromNumber.ParameterName = "@prmFromNumber";
                prmFromNumber.Value = (object?)incomingMedia.FromNumber ?? DBNull.Value;
                prmFromNumber.DbType = DbType.String;
                cmd.Parameters.Add(prmFromNumber);

                DbParameter prmFromMessageId = cmd.CreateParameter();
                prmFromMessageId.ParameterName = "@prmFromMessageId";
                prmFromMessageId.Value = (object?)incomingMedia.FromMessageId ?? DBNull.Value;
                prmFromMessageId.DbType = DbType.String;
                cmd.Parameters.Add(prmFromMessageId);

                DbParameter prmMessageTimestamp = cmd.CreateParameter();
                prmMessageTimestamp.ParameterName = "@prmMessageTimestamp";
                prmMessageTimestamp.Value = (object?)incomingMedia.MessageTimestamp ?? DBNull.Value;
                prmMessageTimestamp.DbType = DbType.String;
                cmd.Parameters.Add(prmMessageTimestamp);

                DbParameter prmMessageMainType = cmd.CreateParameter();
                prmMessageMainType.ParameterName = "@prmMessageMainType";
                prmMessageMainType.Value = (object?)incomingMedia.MessageMainType ?? DBNull.Value;
                prmMessageMainType.DbType = DbType.String;
                cmd.Parameters.Add(prmMessageMainType);

                DbParameter prmMessageSubType = cmd.CreateParameter();
                prmMessageSubType.ParameterName = "@prmMessageSubType";
                prmMessageSubType.Value = (object?)incomingMedia.MessageSubType ?? DBNull.Value;
                prmMessageSubType.DbType = DbType.String;
                cmd.Parameters.Add(prmMessageSubType);

                DbParameter prmMimeType = cmd.CreateParameter();
                prmMimeType.ParameterName = "@prmMimeType";
                prmMimeType.Value = (object?)incomingMedia.MimeType ?? DBNull.Value;
                prmMimeType.DbType = DbType.String;
                cmd.Parameters.Add(prmMimeType);

                DbParameter prmSHA256 = cmd.CreateParameter();
                prmSHA256.ParameterName = "@prmSHA256";
                prmSHA256.Value = (object?)incomingMedia.SHA256 ?? DBNull.Value ;
                prmSHA256.DbType = DbType.String;
                cmd.Parameters.Add(prmSHA256);

                DbParameter prmMediaId = cmd.CreateParameter();
                prmMediaId.ParameterName = "@prmMediaId";
                prmMediaId.Value = (object?)incomingMedia.MediaId ?? DBNull.Value;
                prmMediaId.DbType = DbType.String;
                cmd.Parameters.Add(prmMediaId);

                DbParameter prmDocumentName = cmd.CreateParameter();
                prmDocumentName.ParameterName = "@prmDocumentName";
                prmDocumentName.Value = (object?)incomingMedia.DocumentName ?? DBNull.Value;
                prmDocumentName.DbType = DbType.String;
                cmd.Parameters.Add(prmDocumentName);

                DbParameter prmMessageResponseJson1 = cmd.CreateParameter();
                prmMessageResponseJson1.ParameterName = "@prmMessageResponseJson1";
                prmMessageResponseJson1.Value = (object?)incomingMedia.MessageResponseJson1 ?? DBNull.Value;
                prmMessageResponseJson1.DbType = DbType.String;
                cmd.Parameters.Add(prmMessageResponseJson1);

                DbParameter prmMessageResponseJson2 = cmd.CreateParameter();
                prmMessageResponseJson2.ParameterName = "@prmMessageResponseJson2";
                prmMessageResponseJson2.Value = (object?)incomingMedia.MessageResponseJson2 ?? DBNull.Value;
                prmMessageResponseJson2.DbType = DbType.String;
                cmd.Parameters.Add(prmMessageResponseJson2);

                DbParameter prmMessageResponseJson3 = cmd.CreateParameter();
                prmMessageResponseJson3.ParameterName = "@prmMessageResponseJson3";
                prmMessageResponseJson3.Value = (object?)incomingMedia.MessageResponseJson3 ?? DBNull.Value;
                prmMessageResponseJson3.DbType = DbType.String;
                cmd.Parameters.Add(prmMessageResponseJson3);

                DbParameter prmMessageResponseJson4 = cmd.CreateParameter();
                prmMessageResponseJson4.ParameterName = "@prmMessageResponseJson4";
                prmMessageResponseJson4.Value = (object?)incomingMedia.MessageResponseJson4 ?? DBNull.Value;
                prmMessageResponseJson4.DbType = DbType.String;
                cmd.Parameters.Add(prmMessageResponseJson4);

                DbParameter prmMessageResponseJson5 = cmd.CreateParameter();
                prmMessageResponseJson5.ParameterName = "@prmMessageResponseJson5";
                prmMessageResponseJson5.Value = (object?)incomingMedia.MessageResponseJson5 ?? DBNull.Value;
                prmMessageResponseJson5.DbType = DbType.String;
                cmd.Parameters.Add(prmMessageResponseJson5);

                DbParameter prmMessageBody = cmd.CreateParameter();
                prmMessageBody.ParameterName = "@prmMessageBody";
                prmMessageBody.Value = (object?)incomingMedia.MessageBody ?? DBNull.Value;
                prmMessageBody.DbType = DbType.String;
                cmd.Parameters.Add(prmMessageBody);

                DbParameter prmMessageName = cmd.CreateParameter();
                prmMessageName.ParameterName = "@prmMessageName";
                prmMessageName.Value = (object?)incomingMedia.MessageName ?? DBNull.Value;
                prmMessageName.DbType = DbType.String;
                cmd.Parameters.Add(prmMessageName);

                DbParameter prmField = cmd.CreateParameter();
                prmField.ParameterName = "@prmField";
                prmField.Value = (object?)incomingMedia.Field ?? DBNull.Value;
                prmField.DbType = DbType.String;
                cmd.Parameters.Add(prmField);

                DbParameter prmLocationLatitude = cmd.CreateParameter();
                prmLocationLatitude.ParameterName = "@prmLocationLatitude";
                prmLocationLatitude.Value = (object?)incomingMedia.LocationLatitude ?? DBNull.Value;
                prmLocationLatitude.DbType = DbType.String;
                cmd.Parameters.Add(prmLocationLatitude);

                DbParameter prmLocationLongitude = cmd.CreateParameter();
                prmLocationLongitude.ParameterName = "@prmLocationLongitude";
                prmLocationLongitude.Value = (object?)incomingMedia.LocationLongitude ?? DBNull.Value;
                prmLocationLongitude.DbType = DbType.String;
                cmd.Parameters.Add(prmLocationLongitude);

                DbParameter prmLocationName = cmd.CreateParameter();
                prmLocationName.ParameterName = "@prmLocationName";
                prmLocationName.Value = (object?)incomingMedia.LocationName ?? DBNull.Value;
                prmLocationName.DbType = DbType.String;
                cmd.Parameters.Add(prmLocationName);

                DbParameter prmLocationAddress = cmd.CreateParameter();
                prmLocationAddress.ParameterName = "@prmLocationAddress";
                prmLocationAddress.Value = (object?)incomingMedia.LocationAddress ?? DBNull.Value;
                prmLocationAddress.DbType = DbType.String;
                cmd.Parameters.Add(prmLocationAddress);

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

                cmd.ExecuteNonQuery();

                int errCode = (prmErrCode.Value != DBNull.Value) ? Convert.ToInt32(prmErrCode.Value) : 0;
                string? errMsg = (prmErrMsg.Value != DBNull.Value) ? prmErrMsg.Value.ToString() : string.Empty;

                CommonLocalErrorResponse commonSuccessErrorResponse = new CommonLocalErrorResponse();
                commonSuccessErrorResponse.ErrorCode = errCode;
                commonSuccessErrorResponse.ErrorMessage = errMsg??"";

                /* using (var reader = cmd.ExecuteReader())
                 {
                     loginResponse = reader.MapToLoginRecordSync<Login>();
                 }

                 return Ok(loginResponse);*/
                return errCode == 1;

            }
            catch (Exception ex)
            {
                return false;
                /* Login loginResponse = new Login();
                 loginResponse.Message = "Something Went Wrong";
                 return Ok(loginResponse);*/
                throw ex;
            }

        }
    }
}
