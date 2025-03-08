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
using WhatsAppIntegration.Data;
using WhatsAppIntegration.Model;
using OrderDashboard.Utilities;

namespace WhatsAppIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyMastersController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public CompanyMastersController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/CompanyMasters
        [HttpGet]
        public IActionResult GetCompanysMaster()
        {
            //int p_int_prmErrCode = -1;
            //string p_str_prmErrorMsg = "";

            try
            {
                _context.Database.OpenConnection();
                DbCommand cmd = _context.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "sp_GetCompanyMasterList";
                cmd.CommandType = CommandType.StoredProcedure;


                /*// Adding parameters correctly
                DbParameter prmFromDate = cmd.CreateParameter();
                prmFromDate.ParameterName = "@prmFromDate";
                prmFromDate.Value = startDate;
                prmFromDate.DbType = DbType.Date;
                cmd.Parameters.Add(prmFromDate);

                DbParameter prmCompanyId = cmd.CreateParameter();
                prmCompanyId.ParameterName = "@prmCompanyId";
                prmCompanyId.Value = companyId;
                prmCompanyId.DbType = DbType.Int32;
                cmd.Parameters.Add(prmCompanyId);

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
                cmd.Parameters.Add(prmErrMsg);*/

                cmd.ExecuteNonQuery();

                CompanyResponse companyResponse;

                using (var reader = cmd.ExecuteReader())
                {
                    companyResponse = reader.MapToListCompanySync<CompanyResponse>();
                }

                //p_int_prmErrCode = Convert.ToInt32(cmd.Parameters["@prmErrCode"].Value);
                //p_str_prmErrorMsg = Convert.ToString(cmd.Parameters["@prmErrMsg"].Value);

                return Ok(companyResponse);

            }
            catch (Exception ex)
            {
                CompanyResponse companyResponse = new CompanyResponse();
                companyResponse.CompanyMasterList = [];
                return Ok(companyResponse);
            }
        }

        // GET: api/CompanyMasters/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<CompanyMaster>> GetCompanyMaster(int? id)
        //{
        //    var companyMaster = await _context.CompanysMaster.FindAsync(id);

        //    if (companyMaster == null)
        //    {
        //        return NotFound();
        //    }

        //    return companyMaster;
        //}

        // PUT: api/CompanyMasters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutCompanyMaster(int? id, CompanyMaster companyMaster)
        //{
        //    if (id != companyMaster.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(companyMaster).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CompanyMasterExists(id))
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

        //// POST: api/CompanyMasters
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<CompanyMaster>> PostCompanyMaster(CompanyMaster companyMaster)
        //{
        //    _context.CompanysMaster.Add(companyMaster);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetCompanyMaster", new { id = companyMaster.Id }, companyMaster);
        //}

        //// DELETE: api/CompanyMasters/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCompanyMaster(int? id)
        //{
        //    var companyMaster = await _context.CompanysMaster.FindAsync(id);
        //    if (companyMaster == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.CompanysMaster.Remove(companyMaster);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool CompanyMasterExists(int? id)
        //{
        //    return _context.CompanysMaster.Any(e => e.Id == id);
        //}
    }
}
