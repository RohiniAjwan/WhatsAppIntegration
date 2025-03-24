using WhatsAppIntegration.Data;
using WhatsAppIntegration.Model;
using OrderDashboard.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WhatsAppIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ContactsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Contacts
        [HttpGet]
        public IActionResult GetContacts(int companyId)
        {
            int p_int_prmErrCode = -1;
            string p_str_prmErrorMsg = "";

            try
            {
                _context.Database.OpenConnection();
                DbCommand cmd = _context.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "sp_GetCustomerByCompanyId";
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

                ContactsResponse contactsResponse;

                using (var reader = cmd.ExecuteReader())
                {
                    contactsResponse = reader.MapToListContactsSync<ContactsResponse>();
                }

                //p_int_prmErrCode = Convert.ToInt32(cmd.Parameters["@prmErrCode"].Value);
                //p_str_prmErrorMsg = Convert.ToString(cmd.Parameters["@prmErrMsg"].Value);

                return Ok(contactsResponse);

            }
            catch (Exception ex)
            {
                ContactsResponse contactsResponse = new ContactsResponse();
                contactsResponse.ContactList = [];
                return Ok(contactsResponse);
            }
        }

        // GET: api/Contacts/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Contact>> GetContact(int? id)
        //{
        //    var contact = await _context.Contacts.FindAsync(id);

        //    if (contact == null)
        //    {
        //        return NotFound();
        //    }

        //    return contact;
        //}

        // PUT: api/Contacts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutContact(int? id, Contact contact)
        //{
        //    if (id != contact.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(contact).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ContactExists(id))
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

        // POST: api/Contacts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]// Make sure to use this namespace
    public IActionResult PostContact(List<Contact> contactList)
    {
        int p_int_prmErrCode = -1;
        string p_str_prmErrorMsg = "";

        try
        {
            _context.Database.OpenConnection();
            using (var conn = (SqlConnection)_context.Database.GetDbConnection())  // Cast to SqlConnection
            {
                using (var cmd = new SqlCommand("sp_InsertBulkCustomers", conn)) // Use SqlCommand
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Create DataTable for Table-Valued Parameter
                    DataTable customerTable = new DataTable();
                    customerTable.Columns.Add("Name", typeof(string));
                    customerTable.Columns.Add("PhoneNumber1", typeof(string));
                        customerTable.Columns.Add("PhoneNumber2", typeof(string));
                        customerTable.Columns.Add("Nationality", typeof(string));
                    customerTable.Columns.Add("Gender", typeof(string));
                    customerTable.Columns.Add("AreaName", typeof(string));
                    customerTable.Columns.Add("CompanyId", typeof(int));
                    customerTable.Columns.Add("CreatedBy", typeof(int));
                    customerTable.Columns.Add("UpdatedBy", typeof(int));

                    // Populate DataTable
                    foreach (Contact c in contactList)
                    {
                        customerTable.Rows.Add(c.Name, c.PhoneNumber1, c.PhoneNumber2, c.Nationality, c.Gender, c.AreaName, c.CompanyId, c.CreatedBy, c.CreatedBy);
                    }

                    // Add Table-Valued Parameter
                    var param = cmd.Parameters.AddWithValue("@CustomersList", customerTable);
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
        catch (Exception ex)
        {
            return Ok(new CommonSuccessErrorResponse { ErrorCode = -2, ErrorMessage = ex.Message });
        }
        finally
        {
            _context.Database.CloseConnection();
        }
    }

    /*        public IActionResult PostContact(List<Contact> contactList)
            {
                int p_int_prmErrCode = -1;
                string p_str_prmErrorMsg = "";
                try
                {
                    _context.Database.OpenConnection();
                    DbCommand cmd = _context.Database.GetDbConnection().CreateCommand();
                    // Create DataTable matching the table type
                    DataTable customerTable = new DataTable();
                    customerTable.Columns.Add("Name", typeof(string));
                    customerTable.Columns.Add("PhoneNumber1", typeof(string));
                    customerTable.Columns.Add("PhoneNumber2", typeof(string));
                    customerTable.Columns.Add("Nationality", typeof(string));
                    customerTable.Columns.Add("Gender", typeof(string));
                    customerTable.Columns.Add("AreaName", typeof(string));
                    customerTable.Columns.Add("CompanyId", typeof(int));
                    customerTable.Columns.Add("CreatedBy", typeof(int));
                    customerTable.Columns.Add("UpdatedBy", typeof(int));

                    // Add sample records
                    foreach (Contact c in contactList)
                    {
                        customerTable.Rows.Add(c.Name, c.PhoneNumber1, c.PhoneNumber2, c.Nationality, c.Gender, c.AreaName, c.CompanyId, c.CreatedBy, c.CreatedBy);
                    }

                    using (var conn = _context.Database.GetDbConnection())
                    {
                        conn.Open();
                        using (cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "sp_InsertBulkCustomers";
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add Table-Valued Parameter
                            var param = cmd.CreateParameter();
                            param.ParameterName = "@CustomersList";
                            param.Value = customerTable;
                            param.DbType = DbType.Object; // Table-Valued Parameter (TVP)
                            cmd.Parameters.Add(param);

                            // Output Parameters
                            var prmErrCode = cmd.CreateParameter();
                            prmErrCode.ParameterName = "@prmErrCode";
                            prmErrCode.DbType = DbType.Int32;
                            prmErrCode.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(prmErrCode);

                            var prmErrMsg = cmd.CreateParameter();
                            prmErrMsg.ParameterName = "@prmErrMsg";
                            prmErrMsg.DbType = DbType.String;
                            prmErrMsg.Size = 500;
                            prmErrMsg.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(prmErrMsg);

                            cmd.ExecuteNonQuery();

                            p_int_prmErrCode = Convert.ToInt32(cmd.Parameters["@prmErrCode"].Value);
                            p_str_prmErrorMsg = Convert.ToString(cmd.Parameters["@prmErrMsg"].Value);
                            // Prepare response
                            var commonResponse = new CommonResponse
                            {
                                Error = p_int_prmErrCode == 0 ? 0 : -1,
                                Message = p_int_prmErrCode == 0 ? "Uploaded Successfully" : "Something went wrong"
                            };
                            _context.Database.CloseConnection();
                            conn.Close();
                            return Ok(commonResponse);
                        }
                    }
                } catch (Exception ex) {
                    CommonResponse commonResponse = new CommonResponse();
                    commonResponse.Error = -2;
                    commonResponse.Message = ex.Message;
                    _context.Database.CloseConnection();
                    //conn.Close();
                    return Ok(commonResponse);
                    throw;
                }

            }*/

    //// DELETE: api/Contacts/5
    //[HttpDelete("{id}")]
    //public async Task<IActionResult> DeleteContact(int? id)
    //{
    //    var contact = await _context.Contacts.FindAsync(id);
    //    if (contact == null)
    //    {
    //        return NotFound();
    //    }

    //    _context.Contacts.Remove(contact);
    //    await _context.SaveChangesAsync();

    //    return NoContent();
    //}

    //private bool ContactExists(int? id)
    //{
    //    return _context.Contacts.Any(e => e.Id == id);
    //}
}
    }
