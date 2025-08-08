using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
                            }
                            else if (value != null && value.contacts != null && value.messages != null) {
                            //Store the received messages along with the send messages and link with that particular user.
                            }
                        }
                    }
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

                            return Ok(commonResponse);
                        }
                    }
                }
                else { 
                    return Ok(new CommonSuccessErrorResponse { ErrorCode = -2, ErrorMessage = "Raw body is empty!" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new CommonSuccessErrorResponse { ErrorCode = -2, ErrorMessage = ex.Message });
            }
        }

    }
}
