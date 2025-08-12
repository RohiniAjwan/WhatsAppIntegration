using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;
using WhatsAppIntegration.Model;
using WhatsAppIntegration.Data;
using Microsoft.Extensions.Options;

namespace WhatsAppIntegration.Services
{
    public class MessageServices
    {
        private readonly ApplicationDBContext _context;
        public MessageServices(ApplicationDBContext context)
        {
            _context = context;
            /*_validToken = configuration["Token:BearerToken"];
            _tokenSettings = tokenOptions.Value;*/
        }

        public async Task<bool> SaveIncomingMessage(IncomingMedia incomingMedia)
        {

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
                prmSHA256.Value = (object?)incomingMedia.SHA256 ?? DBNull.Value;
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
                commonSuccessErrorResponse.ErrorMessage = errMsg ?? "";

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
