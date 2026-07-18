using CoreRCON;
using System.Diagnostics;
using System.Net;

namespace Cs2Manager.Api.Services;

public class Cs2RconService
{
    private readonly RCON _rcon;
    private bool _connected = false;

    public Cs2RconService(IConfiguration config)
    {
        var host = config["Rcon:Host"];
        var port = ushort.Parse(config["Rcon:Port"]!);
        var password = config["Rcon:Password"];

        _rcon = new RCON(IPAddress.Parse(host!), port, password!);
    }

    private async Task EnsureConnectedAsync()
    {
        if (!_connected)
        {
            await _rcon.ConnectAsync();
            _connected = true;
        }
    }

    public async Task<string> ChangeMap(string mapName)
    {
        await EnsureConnectedAsync();
        Debug.WriteLine($"ChangeMap command sent: changelevel {mapName}");
        return await _rcon.SendCommandAsync($"changelevel {mapName}");
       
    }

    public async Task<string> KickPlayer(string playerName)
    {
        await EnsureConnectedAsync();
        return await _rcon.SendCommandAsync($"kick \"{playerName}\"");
    }

    public async Task<string> SetCvar(string cvar, string value)
    {
        await EnsureConnectedAsync();
        return await _rcon.SendCommandAsync($"{cvar} {value}");
    }

    public async Task<string> RunCommand(string raw)
    {
        await EnsureConnectedAsync();
        return await _rcon.SendCommandAsync(raw);
    }
}