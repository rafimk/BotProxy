namespace BotProxy.BotServices;

public class SendActivityRequest
{
    public string Locale { get; set;} = string.Empty;
    public string Type { get; set; } = string.Empty;
    public From From { get; set; } = new();
    public string Text { get; set; } = string.Empty;
}

public class From
{
    public string Id { get; set; } = string.Empty;
};
