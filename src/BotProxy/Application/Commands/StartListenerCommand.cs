using BotProxy.BotServices;
using MediatR;

namespace BotProxy.Application.Commands;


public class StartListenerCommand : IRequest<string?>
{
    public string StreamUrl { get; set; } = string.Empty;
}

public class StartListenerCommandHandler : IRequestHandler<StartListenerCommand, string?>
{
    private readonly IBotExternalService _botExternalService;

    public StartListenerCommandHandler(IBotExternalService botExternalService)
    {
        _botExternalService = botExternalService;
    }
    public async Task<string?> Handle(StartListenerCommand request, CancellationToken cancellationToken)
    {
        await _botExternalService.StartListener(request.StreamUrl,  cancellationToken); ;

        return null;
    }
}

