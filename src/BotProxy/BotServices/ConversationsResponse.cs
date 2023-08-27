namespace BotProxy.BotServices;

public class ConversationsResponse
{
    public string conversationId { get; set; } = string.Empty;
    public string token { get; set; } = string.Empty;
    public int expires_in { get; set; } = 0;
    public string streamUrl { get; set; } = string.Empty;
    public string referenceGrammarId { get; set; } = string.Empty;
}
