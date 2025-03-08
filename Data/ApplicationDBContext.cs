using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhatsAppIntegration.Model;


namespace WhatsAppIntegration.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext() { }
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }
        public DbSet<SendMessage> SendMessages { get; set; }
        public DbSet<SendBulkMessage> SendBulkMessages { get; set; }
        public DbSet<ContactGroup> ContactGroups { get; set; }
        public DbSet<GetTemplates> GetTemplatesList { get; set; }
        public DbSet<Component> GetComponentList { get; set; }
        public DbSet<Button> GetButtonList { get; set; }
        public DbSet<Cursors> GetCursorsList { get; set; }
        public DbSet<Paging> GetPagingList { get; set; }
        public DbSet<TemplateData> GetTemplateDataList { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        [BindProperty]
        public DbSet<ContactsResponse> ContactResponse { get; set; }
        public DbSet<CompanyMaster> CompanysMaster { get; set; }
        [BindProperty]
        public DbSet<CompanyResponse> CompanysResponse { get; set; }
        public DbSet<WhatsAppIntegration.Model.Login> Login { get; set; } = default!;
        public DbSet<WhatsAppIntegration.Model.CommonResponse> CommonResponses { get; set; } = default!;

    }
}
