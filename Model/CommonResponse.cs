using Microsoft.EntityFrameworkCore;

namespace WhatsAppIntegration.Model
{
    [Keyless]
    public class CommonResponse
    {
        public int? Error {  get; set; }
        public string? Message {  get; set; }   
    }
}
