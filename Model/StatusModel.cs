using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhatsAppIntegration.Model
{
    [Keyless]
    public partial class StatusModelResponse
    {
        public List<StatusModel> StatusModelList { get; set; }
        public int? ErrorCode { get; set; }

    }
    public partial class StatusModel
    {
        public string? Name { get; set; }
        public string? TemplateName { get; set; }
        [Key]
        public string? MessageId { get; set; }
        public string? Sent { get; set; }
        public string? Delivered { get; set; }
        public string? Read { get; set; }
        public string? Failed { get; set; }
    }
}
