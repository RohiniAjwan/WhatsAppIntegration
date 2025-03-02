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
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactGroup> ContactGroups { get; set; }
        public DbSet<GetTemplates> GetTemplatesList { get; set; }
        public DbSet<Component> GetComponentList { get; set; }
        public DbSet<Button> GetButtonList { get; set; }
        public DbSet<Cursors> GetCursorsList { get; set; }
        public DbSet<Paging> GetPagingList { get; set; }
        public DbSet<TemplateData> GetTemplateDataList { get; set; }

    }
}
