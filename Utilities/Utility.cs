
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using WhatsAppIntegration.Model;

namespace OrderDashboard.Utilities
{
    public static class Utility
    {
        public static ContactsResponse MapToListContactsSync<T>(this DbDataReader dr) where T : new()
        {
            ContactsResponse contactResponse = new ContactsResponse();
                    List<Contact> contactList = new List<Contact>();
            try
            {
                    if (dr != null && dr.HasRows)
                    {
                        while (dr.Read())
                        {
                        Contact entity = new Contact();
                            entity.Id = dr["Id"] != null ? Convert.ToInt32(dr["Id"]) : 0;
                            entity.Name = dr["Name"] != null ? dr["Name"].ToString() : "";
                            entity.PhoneNumber1 = dr["PhoneNumber1"] != null ? dr["PhoneNumber1"].ToString() : "";
                            entity.PhoneNumber2 = dr["PhoneNumber2"] != null ? dr["PhoneNumber2"].ToString() : "";
                            entity.Nationality = dr["Nationality"] != null ? dr["Nationality"].ToString() : ""; 
                            entity.Gender = dr["Gender"] != null ? dr["Gender"].ToString() : "";
                            entity.AreaId = dr["AreaId"] != DBNull.Value ? Convert.ToInt32(dr["AreaId"]) : 0;
                            entity.AreaName = dr["AreaName"] != DBNull.Value ? dr["AreaName"].ToString() : "";
                            entity.CompanyId = dr["CompanyId"] != null ? Convert.ToInt32(dr["CompanyId"]) : 0;
                            entity.CompanyName = dr["CompanyName"] != null ? dr["CompanyName"].ToString() : "";
                            entity.CreatedDate = dr["CreatedDate"] != null ? dr["CreatedDate"].ToString() : "";
                            entity.CreatedBy = dr["CreatedBy"] != null ? Convert.ToInt32(dr["CreatedBy"]) : 0;
                            entity.UpdatedDate = dr["UpdatedDate"] != null ? dr["UpdatedDate"].ToString() : "";
                            entity.UpdatedBy = dr["UpdatedBy"] != null ? Convert.ToInt32(dr["UpdatedBy"]) : 0;

                        contactList.Add(entity);
                    }
                    
                    contactResponse.ContactList = contactList;

                        dr.Close();
                        return contactResponse;

                }
                else
                    
                contactResponse.ContactList = [];
                return contactResponse;               
            }
            catch (Exception ex)
            {
                dr.Close();
                contactResponse.ContactList = null;
                return contactResponse;
                throw ex;
            }

        }

        public static CompanyResponse MapToListCompanySync<T>(this DbDataReader dr) where T : new()
        {
            CompanyResponse companyResponse = new CompanyResponse();
            List<CompanyMaster> companyMasterList = new List<CompanyMaster>();
            try
            {
                if (dr != null && dr.HasRows)
                {
                    while (dr.Read())
                    {
                        CompanyMaster entity = new CompanyMaster();
                        entity.Id = dr["Id"] != null ? Convert.ToInt32(dr["Id"]) : 0;
                        entity.Name = dr["Name"] != null ? dr["Name"].ToString() : "";
                        entity.PhoneNumber = dr["PhoneNumber"] != null ? dr["PhoneNumber"].ToString() : "";
                        entity.Address = dr["Address"] != null ? dr["Address"].ToString() : "";
                        entity.CreatedDate = dr["CreatedDate"] != null ? dr["CreatedDate"].ToString() : "";
                        entity.CreatedBy = dr["CreatedBy"] != null ? dr["CreatedBy"].ToString() : "";
                        entity.UpdatedDate = dr["UpdatedDate"] != null ? dr["UpdatedDate"].ToString() : "";
                        entity.UpdatedBy = dr["UpdatedBy"] != null ? dr["UpdatedBy"].ToString() : "";

                        companyMasterList.Add(entity);
                    }

                    companyResponse.CompanyMasterList = companyMasterList;

                    dr.Close();
                    return companyResponse;

                }
                else

                    companyResponse.CompanyMasterList = [];
                return companyResponse;
            }
            catch (Exception ex)
            {
                dr.Close();
                companyResponse.CompanyMasterList = null;
                return companyResponse;
                throw ex;
            }

        }

        public static Login? MapToLoginRecordSync<T>(this DbDataReader dr) where T : new()
        {
            Login loginResponse = new Login();
            try
            {
                if (dr != null && dr.HasRows)
                {
                    while (dr.Read())
                    {
                        loginResponse.Id = dr["Id"] != null ? Convert.ToInt32(dr["Id"]) : 0;
                        loginResponse.CompanyId = dr["CompanyId"] != null ? Convert.ToInt32(dr["CompanyId"]) : 0;
                        loginResponse.Name = dr["Name"] != null ? dr["Name"].ToString() : "";
                        loginResponse.UserName = dr["UserName"] != null ? dr["UserName"].ToString() : "";
                        loginResponse.Message = dr["Message"] != null ? dr["Message"].ToString() : "";
                    }

                    dr.Close();
                    return loginResponse;
                }
                else
                    
                return null;
            }
            catch (Exception ex)
            {
                dr.Close();
                return null;
                throw ex;
            }

        }
        
        public static Login? ConvertStatusJsonStringToDataSet<T>(this DbDataReader dr) where T : new()
        {
            Login loginResponse = new Login();
            try
            {
                if (dr != null && dr.HasRows)
                {
                    while (dr.Read())
                    {
                        loginResponse.Id = dr["Id"] != null ? Convert.ToInt32(dr["Id"]) : 0;
                        loginResponse.CompanyId = dr["CompanyId"] != null ? Convert.ToInt32(dr["CompanyId"]) : 0;
                        loginResponse.Name = dr["Name"] != null ? dr["Name"].ToString() : "";
                        loginResponse.UserName = dr["UserName"] != null ? dr["UserName"].ToString() : "";
                        loginResponse.Message = dr["Message"] != null ? dr["Message"].ToString() : "";
                    }

                    dr.Close();
                    return loginResponse;
                }
                else
                    
                return null;
            }
            catch (Exception ex)
            {
                dr.Close();
                return null;
                throw ex;
            }

        }

        public static bool SavePayLoad(string p_Str_Payload)
        {
            try
            {
                string f_str_fileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "Load");

                if (!string.IsNullOrWhiteSpace(f_str_fileDir))
                {
                    string f_str_filename = $"{DateTime.Now:yyyyMMddHHmmssfff}.txt";
                    string fullPath = Path.Combine(f_str_fileDir, f_str_filename);

                    if (!Directory.Exists(f_str_fileDir))
                    {
                        Directory.CreateDirectory(f_str_fileDir);
                    }

                    using (var fs = new StreamWriter(fullPath, true))
                    {
                        fs.WriteLine(p_Str_Payload);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                DoHandle(ex.ToString(), "SavePayLoad");
                return false;
            }
        }

        public static void DoHandle(string exMessage, string ExeFunction)
        {
            string strPath = CreateLog();
            using (StreamWriter sw = File.AppendText(strPath))
            {
                sw.WriteLine($"DATA       : {ExeFunction}");
                sw.WriteLine($"TIME STAMP : {DateTime.Now}");
                sw.WriteLine($"MESSAGE    : {exMessage}");
                sw.WriteLine("-------------------------------------------------------------");
            }
        }

        public static string CreateLog()
        {
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "Track");
            string filename = $"Log_{DateTime.Now:ddMMMyyyy}.txt";
            string path = Path.Combine(directory, filename);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(path))
            {
                using (var sw = File.CreateText(path))
                {
                    sw.Close();
                }
            }

            return path;
        }

    }
}
