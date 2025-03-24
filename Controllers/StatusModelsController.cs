using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WhatsAppIntegration.Data;
using WhatsAppIntegration.Model;
using OrderDashboard.Utilities;

namespace WhatsAppIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusModelsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public StatusModelsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/StatusModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusModel>>> GetStatusModels(int companyId, string fromDate, string toDate)
        {
            int p_int_prmErrCode = -1;
            string p_str_prmErrorMsg = "";

            try
            {
                _context.Database.OpenConnection();
                DbCommand cmd = _context.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "sp_GetStatusAndLogs";
                cmd.CommandType = CommandType.StoredProcedure;


                /*// Adding parameters correctly
                DbParameter prmFromDate = cmd.CreateParameter();
                prmFromDate.ParameterName = "@prmFromDate";
                prmFromDate.Value = startDate;
                prmFromDate.DbType = DbType.Date;
                cmd.Parameters.Add(prmFromDate);*/

                DbParameter prmCompanyId = cmd.CreateParameter();
                prmCompanyId.ParameterName = "@prmCompanyId";
                prmCompanyId.Value = companyId;
                prmCompanyId.DbType = DbType.Int32;
                cmd.Parameters.Add(prmCompanyId);

                DbParameter prmFromDate = cmd.CreateParameter();
                prmFromDate.ParameterName = "@prmFromDate";
                prmFromDate.Value = fromDate;
                prmFromDate.DbType = DbType.String;
                cmd.Parameters.Add(prmFromDate);

                DbParameter prmToDate = cmd.CreateParameter();
                prmToDate.ParameterName = "@prmToDate";
                prmToDate.Value = toDate;
                prmToDate.DbType = DbType.String;
                cmd.Parameters.Add(prmToDate);

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

                    StatusModelResponse statusModelResponse;

                    using (var reader = cmd.ExecuteReader())
                    {
                        statusModelResponse = reader.MapToListStatusSync<StatusModelResponse>();
                    }

                    //p_str_prmErrorMsg = Convert.ToString(cmd.Parameters["@prmErrMsg"].Value);
                    return Ok(statusModelResponse);
            }
            catch (Exception ex)
            {
                StatusModelResponse statusModelResponse = new StatusModelResponse();
                statusModelResponse.ErrorCode = 0;
                statusModelResponse.StatusModelList = [];
                return Ok(statusModelResponse);
            }
        }

        // GET: api/StatusModels/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<StatusModel>> GetStatusModel(string id)
        //{
        //    var statusModel = await _context.StatusModels.FindAsync(id);

        //    if (statusModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return statusModel;
        //}

        // PUT: api/StatusModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutStatusModel(string id, StatusModel statusModel)
        //{
        //    if (id != statusModel.MessageId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(statusModel).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!StatusModelExists(id))
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

        // POST: api/StatusModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<StatusModel>> PostStatusModel(StatusModel statusModel)
        //{
        //    _context.StatusModels.Add(statusModel);
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (StatusModelExists(statusModel.MessageId))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetStatusModel", new { id = statusModel.MessageId }, statusModel);
        //}

        // DELETE: api/StatusModels/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteStatusModel(string id)
        //{
        //    var statusModel = await _context.StatusModels.FindAsync(id);
        //    if (statusModel == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.StatusModels.Remove(statusModel);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool StatusModelExists(string id)
        //{
        //    return _context.StatusModels.Any(e => e.MessageId == id);
        //}
    }
}
