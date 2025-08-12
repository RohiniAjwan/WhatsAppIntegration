using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using WhatsAppIntegration.Data;

namespace WhatsAppIntegration.Model
{
 public partial class SendMessage
    {
        public List<string>? PhoneNumber { get; set; }
        public string? PhoneNumberConv { get; set; }
        [Key]
        public string? TemplateTitle { get; set; }
    }

    public partial class SendBulkMessage
    {
        [Key]
        public int? Id { get; set; }
        public string? Message { get; set; }
        public List<string>? PhoneNumber { get; set; }
        public string? Type { get; set; }
    }

}