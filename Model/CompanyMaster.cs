using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WhatsAppIntegration.Model
{
    [Keyless]
    public partial class CompanyResponse
    {
        public List<CompanyMaster>? CompanyMasterList { get; set; }
    }
    public class CompanyMaster
    {
        [Key]
        public int? Id { get; set; }
        public String? Name { get; set; }
        public String? PhoneNumber { get; set; }
        public String? Address { get; set; }
        public String? CreatedDate { get; set; }
        public String? CreatedBy { get; set; }
        public String? UpdatedDate { get; set; }
        public String? UpdatedBy { get; set; }
    }
}
