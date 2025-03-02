using System.ComponentModel.DataAnnotations;

namespace WhatsAppIntegration.Model
{
    public partial class Contact
    {
        [Key]
        public int? Id {  get; set; }
        public String? Name { get; set; }
        public String? PhoneNumber { get; set; }
        public String? NumberType { get; set; }  
    }
    
    public partial class ContactGroup
    {
        [Key]
        public int? Id {  get; set; }
        public List<Contact>? ContactList { get; set; }  
    }
}
