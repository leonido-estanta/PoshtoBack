using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Containers;
using PoshtoBack.Data;
using PoshtoBack.Data.Models;
using PoshtoBack.Helpers;

namespace PoshtoBack.Hubs;

[Authorize]
public class VoiceHub : Hub
{
    public VoiceHub(PoshtoDbContext context)
    {
        IUnitOfWork unitOfWork = new UnitOfWork(context);
        GlobalContainer.Initialize(unitOfWork);
    }

    public string GetConnectionId()
    {
        return Context.ConnectionId;
    }

    public RtcIceServer[] GetIceServers()
    {
        return
        [
            new RtcIceServer
            {
                Urls = "stun:stun.l.google.com:19302"
            },
            new RtcIceServer
            {
                Urls = "turn:your-turn-server.com:3478",
                Username = "your-username",
                Credential = "your-credential"
            }
        ];
    }

    public async Task JoinVoiceRoom(string userId, string voiceRoomId)
    {
        var user = GlobalContainer.UserService.AddInternalUser(userId, Context.ConnectionId);
        await GlobalContainer.UserService.DisconnectInternalUser(user, Clients.All);
        var room = GlobalContainer.VoiceRoomService.AddUserToRoom(user, voiceRoomId);

        await room.SendUserListUpdate(Clients.Caller, true);
        await room.SendUserListUpdate(Clients.Others, false);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Leave(Clients.All);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task Leave(IClientProxy allClients)
    {
        await GlobalContainer.UserService.RemoveInternalUser(Context.ConnectionId, allClients);
    }

    public async Task SendSignal(string signal, string targetConnectionId)
    {
        var callingUser = GlobalContainer.UserService.GetInternalUserByConnection(Context.ConnectionId);
        var targetUser = GlobalContainer.UserService.GetInternalUserByConnection(targetConnectionId);

        if (callingUser == null || targetUser == null)
        {
            return;
        }

        await Clients.Client(targetConnectionId).SendAsync("receiveSignal", callingUser, signal);
    }

    public async Task SendRoomsData(bool self)
    {
        await GlobalContainer.InternalVoiceRooms.SendRoomListUpdate(self ? Clients.Caller : Clients.All);
    }
}