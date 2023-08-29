using BotProxy.BotServices;
using BotProxy.Models;
using MediatR;

namespace BotProxy.Application.Commands;


public class RetrieveActivitiesCommand : IRequest<RootObject?>
{
    public string ConversationId { get; set; } = string.Empty;
}

public class RetrieveActivitiesCommandHandler : IRequestHandler<RetrieveActivitiesCommand, RootObject?>
{
    private readonly IBotExternalService _botExternalService;

    public RetrieveActivitiesCommandHandler(IBotExternalService botExternalService)
    {
        _botExternalService = botExternalService;
    }
    public async Task<RootObject?> Handle(RetrieveActivitiesCommand request, CancellationToken cancellationToken)
    {
        return await _botExternalService.RetrieveActivities(request.ConversationId, cancellationToken); ;
    }
}
