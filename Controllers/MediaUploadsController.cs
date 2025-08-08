using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using WhatsAppIntegration.Data;
using WhatsAppIntegration.Model;
using Microsoft.IdentityModel.Tokens;
using OrderDashboard.Utilities;
using Newtonsoft.Json;
using System.Net;
using Azure;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace WhatsAppIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaUploadsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly TokenSetting _tokenSettings;

        public MediaUploadsController(ApplicationDBContext context, IOptions<TokenSetting> tokenOptions)
        {
            _context = context;
            _tokenSettings = tokenOptions.Value;
        }

        // GET: api/MediaUploads
        /*                [HttpGet]
                        public async Task<ActionResult<IEnumerable<MediaUploads>>> GetMediaUploadsById(int CompanyId)
                        {
                            return await _context.MediaUploads.ToListAsync();
                        }*/

        // GET: api/MediaUploads/5
        [HttpGet("{companyId}")]
                public async Task<ActionResult<MediaUploadResponse>> GetMediaUploads(int? companyId)
                {
                    try
                    {
                        _context.Database.OpenConnection();
                        DbCommand cmd = _context.Database.GetDbConnection().CreateCommand();
                        cmd.CommandText = "sp_GetMediaByCompany";
                        cmd.CommandType = CommandType.StoredProcedure;


                        /*// Adding parameters correctly
                        DbParameter prmFromDate = cmd.CreateParameter();
                        prmFromDate.ParameterName = "@prmFromDate";
                        prmFromDate.Value = startDate;
                        prmFromDate.DbType = DbType.Date;
                        cmd.Parameters.Add(prmFromDate);*/

                        DbParameter prmCompanyId = cmd.CreateParameter();
                        prmCompanyId.ParameterName = "@CompanyId";
                        prmCompanyId.Value = companyId;
                        prmCompanyId.DbType = DbType.Int32;
                        cmd.Parameters.Add(prmCompanyId);

                        // Output Parameters
                        //DbParameter prmErrCode = cmd.CreateParameter();
                        //prmErrCode.ParameterName = "@prmErrCode";
                        //prmErrCode.DbType = DbType.Int32;
                        //prmErrCode.Direction = ParameterDirection.Output;
                        //cmd.Parameters.Add(prmErrCode);

                        //DbParameter prmErrMsg = cmd.CreateParameter();
                        //prmErrMsg.ParameterName = "@prmErrMsg";
                        //prmErrMsg.DbType = DbType.String;
                        //prmErrMsg.Size = 500;
                        //prmErrMsg.Direction = ParameterDirection.Output;
                        //cmd.Parameters.Add(prmErrMsg);

                        cmd.ExecuteNonQuery();

                MediaUploadResponse contactsResponse;

                        using (var reader = cmd.ExecuteReader())
                        {
                            contactsResponse = reader.MapToListMediaUploadsSync<MediaUploadResponse>();
                        }

                        //p_int_prmErrCode = Convert.ToInt32(cmd.Parameters["@prmErrCode"].Value);
                        //p_str_prmErrorMsg = Convert.ToString(cmd.Parameters["@prmErrMsg"].Value);

                        return Ok(contactsResponse);

                    }
                    catch (Exception ex)
                    {
                MediaUploadResponse contactsResponse = new MediaUploadResponse();
                        contactsResponse.MediaUploadDataList = [];
                        return Ok(contactsResponse);
                    }
        }

        // POST: api/MediaUploads
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostMediaUploads(IFormFile? file, [FromForm] int? companyId,  [FromForm] string? fileName = "", [FromForm] string? name = "", [FromForm] string? type = "")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                // Ensure filename is valid
                fileName = string.IsNullOrWhiteSpace(fileName) ? $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}" : fileName;

                // Set up the file path to store the uploaded file in "wwwroot/WEBPOSIMG"
                var uploadsFolderPath = "C:\\inetpub\\wwwroot\\WEBPOSIMG\\";

                // Ensure the directory exists
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                string filePath = Path.Combine(uploadsFolderPath, fileName);

                // Save the file asynchronously without early disposal
                await using (var fileStream = new FileStream(filePath, FileMode.Create)){
                    await file.CopyToAsync(fileStream);
                }

                // File URL (assuming the folder is publicly accessible)
                var fileUrl = $"http://www.ajwancid.com/WEBPOSIMG/{fileName}";

                //insert the record into DB
                CommonSuccessErrorResponse commonSuccessErrorResponse = await InsertMediaIntoDB("insert", companyId, name, fileUrl, null, null, filePath, type);

                return Ok(commonSuccessErrorResponse);
            }
            catch (Exception ex) {
                CommonSuccessErrorResponse commonSuccessErrorResponse = new CommonSuccessErrorResponse();
                commonSuccessErrorResponse.ErrorCode = 1;
                commonSuccessErrorResponse.ErrorMessage = ex.ToString();
                return Ok(commonSuccessErrorResponse);
            }
        }

        [HttpPost("MediaUploadsMeta")]
        public async Task<ActionResult> PostMediaUploadsMeta(string? fileName, int? mediaUploadId)
        {
            try
            {
                // Set up the file path to store the uploaded file in "wwwroot/WEBPOSIMG"
                var uploadsFolderPath = "C:\\inetpub\\wwwroot\\WEBPOSIMG\\";                

                return Ok(await UploadMediaFileAsync(mediaUploadId, uploadsFolderPath+fileName));
            }
            catch (Exception ex) {
                CommonSuccessErrorResponse commonSuccessErrorResponse = new CommonSuccessErrorResponse();
                commonSuccessErrorResponse.ErrorCode = 1;
                commonSuccessErrorResponse.ErrorMessage = ex.ToString();
                return Ok(commonSuccessErrorResponse);
            }
        }

        private async Task<CommonSuccessErrorResponse> InsertMediaIntoDB(string type, int? companyId, string? name, string? url, string? metaId, int? id, string? file, string? fileType) { 
            try
            {
                _context.Database.OpenConnection();
                DbCommand cmd = _context.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "sp_InsertUpdateMediaUpload";
                cmd.CommandType = CommandType.StoredProcedure;

                DbParameter prmMode = cmd.CreateParameter();
                prmMode.ParameterName = "@mode";
                prmMode.Value = type;
                prmMode.DbType = DbType.String;
                cmd.Parameters.Add(prmMode);

                DbParameter prmName = cmd.CreateParameter();
                prmName.ParameterName = "@Name";
                prmName.Value = name;
                prmName.DbType = DbType.String;
                cmd.Parameters.Add(prmName);

                DbParameter prmId = cmd.CreateParameter();
                prmId.ParameterName = "@Id";
                prmId.Value = id;
                prmId.DbType = DbType.Int32;
                cmd.Parameters.Add(prmId);

                DbParameter prmCompanyId = cmd.CreateParameter();
                prmCompanyId.ParameterName = "@CompanyId";
                prmCompanyId.Value = companyId;
                prmCompanyId.DbType = DbType.Int32;
                cmd.Parameters.Add(prmCompanyId);

                DbParameter prmUrl = cmd.CreateParameter();
                prmUrl.ParameterName = "@Url";
                prmUrl.Value = url;
                prmUrl.DbType = DbType.String;
                cmd.Parameters.Add(prmUrl);

                DbParameter prmMediaType = cmd.CreateParameter();
                prmMediaType.ParameterName = "@MediaType";
                prmMediaType.Value = fileType;
                prmMediaType.DbType = DbType.String;
                cmd.Parameters.Add(prmMediaType);

                DbParameter prmMetaId = cmd.CreateParameter();
                prmMetaId.ParameterName = "@MetaId";
                prmMetaId.Value = metaId;
                prmMetaId.DbType = DbType.String;
                cmd.Parameters.Add(prmMetaId);

                // Output Parameters
                DbParameter prmErrCode = cmd.CreateParameter();
                prmErrCode.ParameterName = "@prmErrCode";
                prmErrCode.DbType = DbType.Int32;
                prmErrCode.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(prmErrCode);

                DbParameter prmMediaUploadId = cmd.CreateParameter();
                prmMediaUploadId.ParameterName = "@MediaUploadId";
                prmMediaUploadId.DbType = DbType.Int32;
                prmMediaUploadId.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(prmMediaUploadId);

                DbParameter prmErrMsg = cmd.CreateParameter();
                prmErrMsg.ParameterName = "@prmErrMsg";
                prmErrMsg.DbType = DbType.String;
                prmErrMsg.Size = 500;
                prmErrMsg.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(prmErrMsg);

                cmd.ExecuteNonQuery();

                int p_int_prmMediaUploadId = Convert.ToInt32(cmd.Parameters["@MediaUploadId"].Value);
                int p_int_prmErrCode = Convert.ToInt32(cmd.Parameters["@prmErrCode"].Value);
                string p_str_prmErrorMsg = Convert.ToString(cmd.Parameters["@prmErrMsg"].Value);

                CommonSuccessErrorResponse commonSuccessErrorResponse = new CommonSuccessErrorResponse();
                if (p_int_prmErrCode == 0)
                {
                    //call whatsapp api
                    if(type == "insert"){
                        return await UploadMediaFileAsync(p_int_prmMediaUploadId, file ?? "");                  
                    }else if(type == "update meta")
                    {
                        commonSuccessErrorResponse.ErrorCode = 0;
                        commonSuccessErrorResponse.ErrorMessage = "Uploaded Successfully";
                        return commonSuccessErrorResponse;
                    }
                }
                    commonSuccessErrorResponse.ErrorCode = 1;
                    commonSuccessErrorResponse.ErrorMessage = "Something went wrong 1st phase. Please try again.";
                    return commonSuccessErrorResponse;
            }
            catch (Exception ex)
            {
                CommonSuccessErrorResponse commonSuccessErrorResponse = new CommonSuccessErrorResponse();
                commonSuccessErrorResponse.ErrorCode = 1;
                commonSuccessErrorResponse.ErrorMessage = ex.ToString();
                return commonSuccessErrorResponse;
            }

        }

        private async Task<CommonSuccessErrorResponse> UploadMediaFileAsync(int? mediaUploadId, string filePath)
        {
            try { 
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://graph.facebook.com/v22.0/{_tokenSettings.PhoneNumberId}/media";
                string accessToken = _tokenSettings.AccessToken;

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(new StringContent("whatsapp"), "messaging_product");

                    // Determine file MIME type dynamically
                    string mimeType = OrderDashboard.Utilities.Utility.GetMimeType(filePath);
                    string fileType = mimeType.StartsWith("video") ? "video" : (mimeType.StartsWith("image") ? "image" : "document");

                    formData.Add(new StringContent(fileType), "type");

                    // Attach the file
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        var fileContent = new StreamContent(fileStream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);
                        formData.Add(fileContent, "file", Path.GetFileName(filePath));

                        CommonSuccessErrorResponse commonSuccessErrorResponse = new CommonSuccessErrorResponse();
                        MediaUploads commonResponse = new MediaUploads();
                        CommonErrorResponse commonErrorResponse = new CommonErrorResponse();
                        HttpResponseMessage response = await client.PostAsync(url, formData);

                        var jsonResponse = response.Content.ReadAsStringAsync();

                        if (jsonResponse != null)
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                commonResponse = JsonConvert.DeserializeObject<MediaUploads>(await jsonResponse);

                                commonSuccessErrorResponse.ErrorCode = 0;
                                if (commonResponse != null)
                                {
                                    commonSuccessErrorResponse.MetaId = commonResponse?.Id ?? "";
                                    //call db function
                                    await InsertMediaIntoDB("update meta", null, null, null, commonResponse?.Id ?? "", mediaUploadId, null, null);

                                }
                                commonSuccessErrorResponse.ErrorMessage = "Uploaded Successfully";
                                return commonSuccessErrorResponse;
                            }
                            else
                            {
                                commonErrorResponse = JsonConvert.DeserializeObject<CommonErrorResponse>(await jsonResponse);

                                commonSuccessErrorResponse.ErrorCode = 1;
                                commonSuccessErrorResponse.ErrorMessage = commonErrorResponse?.Error?.Message ?? "";
                                return commonSuccessErrorResponse;
                            }

                        }
                            commonSuccessErrorResponse.ErrorCode = 1;
                            commonSuccessErrorResponse.ErrorMessage = "Something went wrong 2nd phase. Please try again.";
                            return commonSuccessErrorResponse;
                    }
                }
            }
        } catch (Exception ex)
            {
                CommonSuccessErrorResponse commonSuccessErrorResponse = new CommonSuccessErrorResponse();
                commonSuccessErrorResponse.ErrorCode = 1;
                commonSuccessErrorResponse.ErrorMessage = ex.ToString();
                return commonSuccessErrorResponse;
            }
        }           
    }
}
