using CoreRCON;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace Cs2Manager.Api.Services;

public class Cs2RconService
{
    private readonly RCON _rcon;
    private int human = 0, bot = 0, maxis = 0;
    private bool _connected = false;
    private readonly SemaphoreSlim _rconLock = new(1, 1);
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
        await _rconLock.WaitAsync();

        try
        {
            await EnsureConnectedAsync();

            return await _rcon.SendCommandAsync("status");
        }
        finally
        {
            _rconLock.Release();
        }
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
            human = humans;
            bot = bots;
            maxis = max;

            return (humans, bots, max);
        }

        return (human, bot, maxis);
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


}