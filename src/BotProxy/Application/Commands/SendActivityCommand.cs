using BotProxy.BotServices;
using MediatR;

namespace BotProxy.Application.Commands;

public class SendActivityCommand : IRequest<string?>
{
    public string ConversationId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class SendActivityCommandHandler : IRequestHandler<SendActivityCommand, string?>
{
    private readonly IBotExternalService _botExternalService;

    public SendActivityCommandHandler(IBotExternalService botExternalService)
    {
        _botExternalService = botExternalService;
    }
    public async Task<string?> Handle(SendActivityCommand request, CancellationToken cancellationToken)
    {
        return await _botExternalService.SendActivity(request.ConversationId, request.UserId, request.Message, cancellationToken); ;
    }
}

