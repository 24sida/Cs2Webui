using Microsoft.AspNetCore.SignalR;

namespace Cs2Manager.Api.Hubs;

public class ConsoleHub : Hub
{
    public async Task SendLine(string line)
    {
        await Clients.All.SendAsync("ReceiveLine", line);
    }
}