using BotProxy.Application.Commands;
using BotProxy.BotServices;
using Microsoft.AspNetCore.Mvc;

namespace BotProxy.Controllers;


public class BotController : ApiControllerBase
{
    [HttpPost("StartConversation")]
    public async Task<ActionResult<ConversationsResponse?>> StartConversation()
    {
        var result = await Mediator.Send(new BotStartConversationCommand());
     
        return Ok(result);
    }

    [HttpPost("StartListener")]
    public async Task<ActionResult> StartListener(StartListenerCommand command)
    {

        var result = await Mediator.Send(command);

        return Ok();
    }

    [HttpPost("SendActivity")]
    public async Task<ActionResult<string?>> SendActivity(SendActivityCommand command)
    {

        var result = await Mediator.Send(command);

        return Ok(result);
    }

    [HttpPost("RetrieveActivities")]
    public async Task<ActionResult<string?>> RetrieveActivities(RetrieveActivitiesCommand command)
    {

        var result = await Mediator.Send(command);

        return Ok(result);
    }
}
