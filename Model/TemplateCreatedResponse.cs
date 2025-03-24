using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WhatsAppIntegration.Model
{
    public class TemplateCreatedResponse
    {
        [Key]
        public int? Id {  get; set; }
        public string? Status {  get; set; }   
        public string? Message {  get; set; }   
    }

    public class CommonErrorResponse
    {
        [JsonPropertyName("error")]
        public ErrorDetails Error { get; set; }
    }

    public class ErrorDetails
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("fbtrace_id")]
        public string FbTraceId { get; set; }
    }
}
