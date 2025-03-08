using System;
using System.Collections.Generic;
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
using NuGet.Protocol.Plugins;

namespace WhatsAppIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public LoginController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Login
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Login>>> GetLogin()
        //{
        //    return await _context.Login.ToListAsync();
        //}

        // GET: api/Login/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Login>> GetLogin(int? id)
        //{
        //    var login = await _context.Login.FindAsync(id);

        //    if (login == null)
        //    {
        //        return NotFound();
        //    }

        //    return login;
        //}

        // PUT: api/Login/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutLogin(int? id, Login login)
        //{
        //    if (id != login.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(login).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!LoginExists(id))
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

        // POST: api/Login
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public IActionResult PostLogin(Login login)
        {
            //int p_int_prmErrCode = -1;
            //string p_str_prmErrorMsg = "";

            try
            {
                _context.Database.OpenConnection();
                DbCommand cmd = _context.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "sp_LoginStaff";
                cmd.CommandType = CommandType.StoredProcedure;


                // Adding parameters correctly
                DbParameter prmUserName = cmd.CreateParameter();
                prmUserName.ParameterName = "@UserName";
                prmUserName.Value = login.UserName;
                prmUserName.DbType = DbType.String;
                cmd.Parameters.Add(prmUserName);

                DbParameter prmPassword = cmd.CreateParameter();
                prmPassword.ParameterName = "@Password";
                prmPassword.Value = login.Password;
                prmPassword.DbType = DbType.String;
                cmd.Parameters.Add(prmPassword);

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

                Login loginResponse;

                using (var reader = cmd.ExecuteReader())
                {
                    loginResponse = reader.MapToLoginRecordSync<Login>();
                }

                //p_int_prmErrCode = Convert.ToInt32(cmd.Parameters["@prmErrCode"].Value);
                //p_str_prmErrorMsg = Convert.ToString(cmd.Parameters["@prmErrMsg"].Value);

                return Ok(loginResponse);

            }
            catch (Exception ex)
            {
                Login loginResponse = new Login();
                loginResponse.Message = "Something Went Wrong";
                return Ok(loginResponse);
            }
        }

        // DELETE: api/Login/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteLogin(int? id)
        //{
        //    var login = await _context.Login.FindAsync(id);
        //    if (login == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Login.Remove(login);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool LoginExists(int? id)
        {
            return _context.Login.Any(e => e.Id == id);
        }
    }
}
