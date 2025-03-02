using Newtonsoft.Json;

namespace WhatsAppIntegration.Model
{
    public class MessageSendingResponse
    {
        [JsonProperty("messaging_product")]
        public string MessagingProduct { get; set; }

        [JsonProperty("contacts")]
        public List<Contact> Contacts { get; set; }

        [JsonProperty("messages")]
        public List<WhatsAppMessage> Messages { get; set; }
    }

    public partial class Contact
    {
        [JsonProperty("input")]
        public string Input { get; set; }

        [JsonProperty("wa_id")]
        public string WaId { get; set; }
    }

    public partial class WhatsAppMessage
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("message_status")]
        public string MessageStatus { get; set; }
    }
}
