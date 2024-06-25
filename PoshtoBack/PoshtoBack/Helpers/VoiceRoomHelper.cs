using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Data.Models;

namespace PoshtoBack.Helpers;

public static class VoiceRoomHelper
{
    public static async Task SendUserListUpdate(this VoiceRoomInternal room, IClientProxy to, bool callTo)
    {
        await to.SendAsync(callTo ? "callToUserList" : "updateUserList", room.Id, room.InternalUsers);
    }
}