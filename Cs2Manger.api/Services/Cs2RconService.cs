using CoreRCON;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

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
    public async Task<string> SetGameMode(int gameType, int gameMode)
    {
        await EnsureConnectedAsync();
        await _rcon.SendCommandAsync($"game_type {gameType}");
        return await _rcon.SendCommandAsync($"game_mode {gameMode}");
    }
    public async Task<string> GetStatus()
    {
        await EnsureConnectedAsync();
        return await _rcon.SendCommandAsync("status");
    }
   

    public async Task<string> SetCvar(string cvar, string value)
    {
        await EnsureConnectedAsync();
        return await _rcon.SendCommandAsync($"{cvar} {value}");
    }

    
    public async Task<(int currentplayers, int currentbots, int max)> GetPlayerAndbotsCount()
    {
        var status = await GetStatus();

        var match = Regex.Match(status, @"players\s*:\s*(\d+)\s*humans,\s*(\d+)\s*bots\s*\((\d+)\s*max\)");

        if (match.Success)
        {
            int humans = int.Parse(match.Groups[1].Value);
            int bots = int.Parse(match.Groups[2].Value);
            int max = int.Parse(match.Groups[3].Value);
            return (humans, bots, max);
        }

        return (0, 0,0);
    }
    public async Task<List<string>> GetNametag()
    {
        var status = await GetStatus();
        var matches = Regex.Matches(status, @"^\s*\d+\s+.*?'(.+?)'\s*$", RegexOptions.Multiline);
        var names = new List<string>();
        foreach (Match m in matches)
        {
            names.Add(m.Groups[1].Value);
        }

        return names;

    }
    public async Task<string> TogglePause(bool isPaused)
    {
        await EnsureConnectedAsync();
        var command = isPaused ? "mp_unpause_match" : "mp_pause_match";
        return await _rcon.SendCommandAsync(command);
    }


    public async Task<string> GetAll()
    {

               await EnsureConnectedAsync();
            return await _rcon.SendCommandAsync("status; mp_pause_match; mp_unpause_match; changelevel; game_type; game_mode");
    }
}