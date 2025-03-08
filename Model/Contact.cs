using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WhatsAppIntegration.Model
{
    [Keyless]
    public partial class ContactsResponse
    { 
        public List<Contact>? ContactList {  get; set; }
    }
    public partial class Contact
    {
        [Key]
        public int? Id {  get; set; }
        public String? Name { get; set; }
        public String? PhoneNumber1 { get; set; }
        public String? PhoneNumber2 { get; set; }
        public String? Nationality { get; set; }  
        public String? Gender { get; set; }  
        public int? AreaId { get; set; }  
        public String? AreaName { get; set; }  
        public String? CompanyName { get; set; }  
        public int? CompanyId { get; set; }  
        public String? CreatedDate { get; set; }  
        public int? CreatedBy { get; set; }  
        public String? UpdatedDate { get; set; }  
        public int? UpdatedBy { get; set; }  
    }
    
    public partial class ContactGroup
    {
        [Key]
        public int? Id {  get; set; }
        public List<Contact>? ContactList { get; set; }  
    }
}