using AutoForwarder.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoForwarder.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserBotController : ControllerBase
{
    private readonly IUserBotService userBotService;
    private static CancellationTokenSource cancellationTokenSource;

    public UserBotController(IUserBotService userBotService)
    {
        this.userBotService = userBotService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> ForwardMessage()
    {
        if (cancellationTokenSource != null)
        {
            return BadRequest("Service is already running");
        }

        cancellationTokenSource = new CancellationTokenSource();


        await this.userBotService.ForwardMessageAsync(cancellationTokenSource.Token);
        return Ok();
    }
    [HttpPost("stop")]
    public IActionResult StopForwardMessage()
    {
        if (cancellationTokenSource == null)
        {
            return BadRequest("Service is not running");
        }
        cancellationTokenSource.Cancel();
        cancellationTokenSource = null;
        return Ok();
    }
}
