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
}

public record ChangeMapRequest(string Map);