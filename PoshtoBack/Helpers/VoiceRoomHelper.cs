using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Data.Models;

namespace PoshtoBack.Helpers;

public static class VoiceRoomHelper
{
    public static async Task SendUserListUpdate(this VoiceRoomInternal room, IClientProxy to, bool callTo)
    {
        await to.SendAsync(callTo ? "callToUserList" : "updateUserList", room.Id, room.ConnectedUsers);
    }

    public static async Task SendRoomListUpdate(this List<VoiceRoomInternal> voiceRoomInternals, IClientProxy to)
    {
        await to.SendAsync("updateRoomsData", voiceRoomInternals); 
    }
}