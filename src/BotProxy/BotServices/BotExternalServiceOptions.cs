namespace BotProxy.BotServices;

public class BotExternalServiceOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string AuthorizationToken { get; set; } = string.Empty;
    public double RetryDelay { get; set; } = 1;
    public int RetryCount { get; set; } = 5;
}
