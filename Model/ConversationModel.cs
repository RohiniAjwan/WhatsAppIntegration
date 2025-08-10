namespace WhatsAppIntegration.Model
{
    public class ConversationModel
    {
        public List<ConversationUser>? conversationUserList {  get; set; }
    }
    
    public class ConversationUser
    {
        public int? Id { get; set; }
        public string? ProfileWaId { get; set; }
        public string? ProfileName { get; set; }
        public string? MessageTimestamp { get; set; }
        public string? LastMessage { get; set; }
    }
}
