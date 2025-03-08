using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;

namespace WhatsAppIntegration.Model
{
    public class Login
    {
        [Key]
        public int? Id { get; set; }
        public int? CompanyId { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        //public string? CreatedDate { get; set; }
        //public string? UpdatedDate { get; set; }
        public string? Message { get; set; }
        public string? Password { get; set; }
    }

}
