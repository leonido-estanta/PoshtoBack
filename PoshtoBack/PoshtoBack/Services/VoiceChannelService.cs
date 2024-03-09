using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Data.Models;
using PoshtoBack.Hubs;

namespace PoshtoBack.Services;

public class VoiceChannelDto
{
    public int Id { get; set; }
    public HashSet<int> ConnectedUserIds { get; init; } = [];
}

public class VoiceChannelService
{
    private Dictionary<int, VoiceChannelDto> _voiceChannels;
    public async Task ConnectUserToChannelAsync(IHubContext<VoiceHub> hub, int channelId, int userId)
    {
        foreach (var channel in _voiceChannels.Values.Where(channel => channel.ConnectedUserIds.Contains(userId)))
        {
            await hub.Clients.All.SendAsync("VoiceDisconnect", new VoiceChannelConnectDto
            {
                UserId = userId,
                ChannelId = channel.Id
            });
                
            channel.ConnectedUserIds.Remove(userId);
        }

        if (_voiceChannels.TryGetValue(channelId, out var channelToConnect))
        {
            channelToConnect.ConnectedUserIds.Add(userId);
        }
        
        await hub.Clients.All.SendAsync("VoiceConnect", new VoiceChannelConnectDto
        {
            UserId = userId,
            ChannelId = channelId
        });
    }

    public async Task DisconnectUserFromChannelAsync(IHubContext<VoiceHub> hub, int channelId, int userId)
    {
        if (_voiceChannels.TryGetValue(channelId, out var channel))
        {
            channel.ConnectedUserIds.Remove(userId);
        }
        
        await hub.Clients.All.SendAsync("VoiceDisconnect", new VoiceChannelConnectDto
        {
            UserId = userId,
            ChannelId = channelId
        });
    }

    public IEnumerable<int> GetConnectedUsers(int channelId, IEnumerable<VoiceChannel> channels)
    {
        _voiceChannels = channels.ToDictionary(
            voiceChannel => voiceChannel.Id,
            voiceChannel => new VoiceChannelDto
            {
                Id = voiceChannel.Id,
                ConnectedUserIds = []
            });
        
        return _voiceChannels.TryGetValue(channelId, out var channel) ? channel.ConnectedUserIds : Enumerable.Empty<int>();
    }
}