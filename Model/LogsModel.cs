using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations;

public class LogsModel
{
    [Key]
    public string? @object { get; set; }
    public List<Entry>? entry { get; set; }
}

public class Entry
{
    [Key]
    public string? id { get; set; }
    public List<Change>? changes { get; set; }
}

public class Change
{
    public Value? value { get; set; }
    [Key]
    public string? field { get; set; }
}

public class Value
{
    [Key]
    public string? messaging_product { get; set; }
    public Metadata? metadata { get; set; }
    public List<Status>? statuses { get; set; }
    public List<Contact>? contacts { get; set; }
    public List<Message>? messages { get; set; }
}

public class Metadata
{
    [JsonProperty("display_phone_number")]
    public string? DisplayPhoneNumber { get; set; }

    [Key]
    [JsonProperty("phone_number_id")]
    public string? PhoneNumberId { get; set; }
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
public partial class Origin
{
    [Key]
    public string? type { get; set; }
}
public partial class Conversation
{
    [Key]
    public string? id { get; set; }
    public Origin? origin { get; set; }
}
public partial class Pricing
{
    public bool? billable { get; set; }
    public string? pricing_model { get; set; }
    [Key]
    public string? category { get; set; }
}
public class Contact
{
    public Profile? profile { get; set; }

    [JsonProperty("wa_id")]
      [Key]
  public string? WaId { get; set; }
}

public class Profile
{
    [Key]
    public string? name { get; set; }
}

public class Message
{
    [Key]
    public string? id { get; set; }
    public string? from { get; set; }
    public string? timestamp { get; set; }
    public string? type { get; set; }
    public Interactive? interactive { get; set; }
    public Context? context { get; set; }
}

public class Context
{
    public string? from { get; set; }
    [Key]
    public string? id { get; set; }
}

public class Interactive
{
    [Key]
    public string? type { get; set; }

    [JsonProperty("nfm_reply")]
    public NfmReply? NfmReply { get; set; }
}

public class NfmReply
{
    [JsonProperty("response_json")]
    public string? ResponseJson { get; set; }
    public string? body { get; set; }
    [Key]
    public string? name { get; set; }
}
public partial class Error
{
    [Key]
    public int? code { get; set; }
    public string? title { get; set; }
    public string? details { get; set; }
}