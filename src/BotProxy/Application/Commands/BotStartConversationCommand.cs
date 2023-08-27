using BotProxy.BotServices;
using MediatR;

namespace BotProxy.Application.Commands;

public class BotStartConversationCommand : IRequest<ConversationsResponse?>
{
}

public class BotStartConversationCommandHandler : IRequestHandler<BotStartConversationCommand, ConversationsResponse?>
{
    private readonly IBotExternalService _botExternalService;

    public BotStartConversationCommandHandler(IBotExternalService botExternalService)
    {
        _botExternalService = botExternalService;
    }
    public async Task<ConversationsResponse?> Handle(BotStartConversationCommand request, CancellationToken cancellationToken)
    {
        return await _botExternalService.StartConversation(cancellationToken);
    }
}
