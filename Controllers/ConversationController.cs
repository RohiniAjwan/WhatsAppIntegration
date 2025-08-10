using WhatsAppIntegration.Data;
using WhatsAppIntegration.Model;
using OrderDashboard.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;
using Microsoft.Data.SqlClient;
using System.ComponentModel.Design;

namespace WhatsAppIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ConversationController(ApplicationDBContext context)
        {
            _context = context;
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
    }
}
