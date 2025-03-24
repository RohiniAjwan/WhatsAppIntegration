using System.ComponentModel.DataAnnotations;

namespace WhatsAppIntegration.Model
{
    public class CommonSuccessErrorResponse
    {
        [Key]
        public int ErrorCode { get; set; }
        public int Id { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
