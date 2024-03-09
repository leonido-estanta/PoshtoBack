using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Data.Models;

namespace PoshtoBack.Hubs;

public class VoiceHub : Hub
{
    public async Task ConnectUser(VoiceChannelConnectDto model)
    {
        await Clients.All.SendAsync("VoiceConnect", model);
    }
    
    public async Task DisconnectUser(VoiceChannelConnectDto model)
    {
        await Clients.All.SendAsync("VoiceDisconnect", model);
    }
}