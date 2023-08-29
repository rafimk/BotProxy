namespace BotProxy.Models;

public class Content
{
    public string Text { get; set; } = string.Empty;
    public List<Button> Buttons { get; set; } = new List<Button>();
}
