using System.ComponentModel.DataAnnotations;

namespace WhatsAppIntegration.Model
{
    public class LogsModel
    {
        [Key]
        public string? @object { get; set; }
        public List<Entry>? entry { get; set; }
    }
    public partial class Change
    {
        public Value? value { get; set; }
        [Key]
        public string? field { get; set; }
    }

    public partial class Conversation
    {
        [Key]
        public string? id { get; set; }
        public Origin? origin { get; set; }
    }

    public partial class Entry
    {
        [Key]
        public string? id { get; set; }
        public List<Change>? changes { get; set; }
    }

    public partial class Error
    {
        [Key]
        public int? code { get; set; }
        public string? title { get; set; }
        public string? details { get; set; }
    }

    public partial class Metadata
    {
        public string? display_phone_number { get; set; }
        [Key]
        public string? phone_number_id { get; set; }
    }

    public partial class Origin
    {
        [Key]
        public string? type { get; set; }
    }

    public partial class Pricing
    {
        public bool? billable { get; set; }
        [Key]
        public string? pricing_model { get; set; }
        public string? category { get; set; }
    }

    public partial class Status
    {
        [Key]
        public string? id { get; set; }
        public string? status { get; set; }
        public string? timestamp { get; set; }
        public string? recipient_id { get; set; }
        public Conversation? conversation { get; set; }
        public Pricing? pricing { get; set; }
        public List<Error>? errors { get; set; }
    }

    public partial class Value
    {
        [Key]
        public string? messaging_product { get; set; }
        public Metadata? metadata { get; set; }
        public List<Status>? statuses { get; set; }
    }

}
