using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WhatsAppIntegration.Model
{
    public class CreateTemplateRequest
    {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("language")]
            public string Language { get; set; }

            [JsonProperty("category")]
            public string Category { get; set; }

            [JsonProperty("components")]
            public List<Components> Components { get; set; }

            public CreateTemplateRequest()
            {
                Components = new List<Components>();
            }
        }

        public class Components
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("format", NullValueHandling = NullValueHandling.Ignore)]
            public string Format { get; set; }

            [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
            public string Text { get; set; }

            [JsonProperty("buttons", NullValueHandling = NullValueHandling.Ignore)]
            public List<Buttons> Buttons { get; set; }
        }

        public class Buttons
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }

        [JsonProperty("phone_number", NullValueHandling = NullValueHandling.Ignore)]
        public string PhoneNumber { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

    }
}
