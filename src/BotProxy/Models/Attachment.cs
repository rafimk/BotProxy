namespace BotProxy.Models;

public class Attachment
{
    public string ContentType { get; set; } = string.Empty;
    public Content Content { get; set; } = new();
}
