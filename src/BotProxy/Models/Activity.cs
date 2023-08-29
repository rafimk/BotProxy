using Microsoft.VisualBasic;
using System.Net.Mail;

namespace BotProxy.Models;

public class Activity
{
    public string Type { get; set; }
    public string Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string ServiceUrl { get; set; }
    public string ChannelId { get; set; }
    public From From { get; set; }
    public Conversation Conversation { get; set; }
    public string Locale { get; set; }
    public string Text { get; set; }
    public string Speak { get; set; }
    public string InputHint { get; set; }
    public List<Attachment> Attachments { get; set; }
    public string ReplyToId { get; set; }
}
