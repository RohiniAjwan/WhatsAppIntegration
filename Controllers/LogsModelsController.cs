using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

        public LogsModelsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/LogsModels
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<LogsModel>>> GetlogsModels()
        //{
        //    return await _context.logsModels.ToListAsync();
        //}

        //// GET: api/LogsModels/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<LogsModel>> GetLogsModel(string id)
        //{
        //    var logsModel = await _context.logsModels.FindAsync(id);

        //    if (logsModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return logsModel;
        //}

        // PUT: api/LogsModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutLogsModel(string id, LogsModel logsModel)
        //{
        //    if (id != logsModel.object)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(logsModel).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!LogsModelExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/LogsModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> PostLogsModel([FromBody] LogsModel logsModel)
        {
            if (logsModel == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                String payload = JsonConvert.SerializeObject(logsModel);
                Utility.SavePayLoad(payload);

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
                        var commonResponse = new CommonResponse
                        {
                            Error = p_int_prmErrCode == 0 ? 0 : -1,
                            Message = p_int_prmErrCode == 0 ? "Uploaded Successfully" : "Something went wrong"
                        };

                        return Ok(commonResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new CommonResponse { Error = -2, Message = ex.Message });
            }
        }

        // DELETE: api/LogsModels/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteLogsModel(string id)
        //{
        //    var logsModel = await _context.logsModels.FindAsync(id);
        //    if (logsModel == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.logsModels.Remove(logsModel);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool LogsModelExists(string id)
        //{
        //    return _context.logsModels.Any(e => e.object == id);
        //}
    }
}
