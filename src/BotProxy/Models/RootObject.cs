namespace BotProxy.Models;

public class RootObject
{
    public List<Activity> Activities { get; set; } = new List<Activity>();
    public string Watermark { get; set; } = string.Empty;
}
