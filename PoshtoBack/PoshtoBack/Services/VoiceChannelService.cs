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
    private readonly Dictionary<int, VoiceChannelDto> _voiceChannels = new();
    public async Task ConnectUserToChannelAsync(IHubContext<VoiceHub> hub, int channelId, int userId)
    {
        foreach (var channel in _voiceChannels.Values.Where(channel => channel.ConnectedUserIds.Contains(userId) && !(channel.Id == channelId && channel.ConnectedUserIds.Contains(userId))))
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
        var updatedChannels = channels.ToDictionary(voiceChannel => voiceChannel.Id);

        var channelsToRemove = _voiceChannels.Keys.Except(updatedChannels.Keys).ToList();
        foreach (var channelIdToRemove in channelsToRemove)
        {
            _voiceChannels.Remove(channelIdToRemove);
        }

        foreach (var updatedChannel in updatedChannels)
        {
            if (!_voiceChannels.ContainsKey(updatedChannel.Key))
            {
                _voiceChannels.Add(updatedChannel.Key, new VoiceChannelDto
                {
                    Id = updatedChannel.Key,
                    ConnectedUserIds = []
                });
            }
        }

        return _voiceChannels.TryGetValue(channelId, out var channel) ? channel.ConnectedUserIds : Enumerable.Empty<int>();
    }
}