using Cs2Manager.Api.Services;
using Cs2Manger.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cs2Manager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServerController : ControllerBase
{
    private readonly Cs2RconService _rcon;

    public ServerController(Cs2RconService rcon)
    {
        _rcon = rcon;
    }

    [HttpPost("map")]
    public async Task<IActionResult> ChangeMap([FromBody] ChangeMapRequest request)
    {
        var result = await _rcon.ChangeMap(request.Map);
        return Ok(result);

    }
    [HttpPost("gamemode")]
    public async Task<IActionResult> SetGameMode([FromBody] SetGameModeRequest request)
    {
        var result = await _rcon.SetGameMode(request.GameType, request.GameMode);
        return Ok(result);
    }

    public record SetGameModeRequest(int GameType, int GameMode);


    [HttpGet("playercount")]
    public async Task<IActionResult> GetPlayerAndbotsCount()
    {
        var (currentplayers, currentbots, max) = await _rcon.GetPlayerAndbotsCount();
        return Ok(new { currentplayers, currentbots, max });
    }

    [HttpGet("players")]
    public async Task<IActionResult> GetPlayerNames()
    {
        var names = await _rcon.GetNametag();
        return Ok(names);
    }
    [HttpPost("pause-toggle")]
    public async Task<IActionResult> TogglePause([FromBody] TogglePauseRequest request)
    {
        var result = await _rcon.TogglePause(request.IsPaused);
        return Ok(result);
    }

    public record TogglePauseRequest(bool IsPaused);
}

public record ChangeMapRequest(string Map);