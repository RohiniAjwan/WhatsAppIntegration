using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using WhatsAppIntegration.Data;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WhatsAppIntegration.Model
{
    [Keyless]
    public class GetTemplates
    {
        public List<TemplateData> Data { get; set; }
        [NotMapped]
        public Paging Paging { get; set; }
       
    }

    public class TemplateData
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string Language { get; set; }
        [NotMapped]
        public List<Component> Components { get; set; }
        [Key]
        public string Id { get; set; }
    }

    [Keyless]
    public class Component
    {
        public string Type { get; set; }
        public string Format { get; set; }
        public string Text { get; set; }
        [NotMapped]
        public List<Button> Buttons { get; set; }
      
    }

    [Keyless]
    public class Button
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }

        [JsonProperty("flow_id")]
        public long FlowId { get; set; }
       
    }

    [Keyless]
    public class Paging
    {
        [NotMapped]
        public Cursors Cursors { get; set; }
        
    }

    [Keyless]
    public class Cursors
    {
       
        public string Before { get; set; }
        public string After { get; set; }
    }
    [Keyless]
    public class CreateTemplate
    {
        public string? title {  get; set; }
        public string? titleValue { get; set; }
        public string? templateTitle { get; set; }
        public string? body { get; set; }
        public string? footer { get; set; }
        public string? phoneNumberTitle { get; set; }
        public string? phoneNumberValue { get; set; }
        public string? websiteLinkTitle { get; set; }
        public string? websiteLinkValue { get; set; }
        public string? metaId { get; set; }

    }
}