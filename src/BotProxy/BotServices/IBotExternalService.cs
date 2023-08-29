using BotProxy.Models;

namespace BotProxy.BotServices;

public interface IBotExternalService
{
    Task<ConversationsResponse?> StartConversation(CancellationToken cancellationToken);

    Task StartListener(string streamUrl, CancellationToken cancellationToken);

    Task<string?> SendActivity(string conversationId, string userId, string message, CancellationToken cancellationToken);

    Task<RootObject?> RetrieveActivities(string conversationId, CancellationToken cancellationToken);
}
