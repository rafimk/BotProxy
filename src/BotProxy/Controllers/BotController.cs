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

    //[HttpPost("StartConversation")]
    //public async Task<ActionResult> StartListener(string streamUrl)
    //{

    //    await _botExternalService.StartListener(streamUrl, new CancellationToken());

    //    return Ok();
    //}

    [HttpPost("SendActivity")]
    public async Task<ActionResult<string?>> SendActivity(SendActivityCommand command)
    {

        var result = await Mediator.Send(command);

        return Ok(result);
    }
}
