using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Containers;
using PoshtoBack.Data;
using PoshtoBack.Data.Models;
using PoshtoBack.Helpers;
using PoshtoBack.Services;

namespace PoshtoBack.Hubs;

public class VoiceHub : Hub
{
    public VoiceHub(PoshtoDbContext context)
    {
        IUnitOfWork unitOfWork = new UnitOfWork(context);
        if (!VoiceRoomContainer.Loaded)
        {
            VoiceRoomContainer.VoiceRoomService = new VoiceRoomService(unitOfWork);
            VoiceRoomContainer.UserService = new UserService(unitOfWork);

            VoiceRoomContainer.Loaded = true;
        }
    }

    public string GetConnectionId()
    {
        return Context.ConnectionId;
    }

    public RtcIceServer[] GetIceServers()
    {
        return new[]
        {
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
        };
    }

    public async Task JoinVoiceRoom(string userId, string voiceRoomId)
    {
        var user = VoiceRoomContainer.UserService.AddInternalUser(userId, Context.ConnectionId);
        await VoiceRoomContainer.UserService.DisconnectInternalUser(user, Clients.All);
        var room = VoiceRoomContainer.VoiceRoomService.AddUserToRoom(user, voiceRoomId);

        await Groups.AddToGroupAsync(Context.ConnectionId, voiceRoomId);

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
        await VoiceRoomContainer.UserService.RemoveInternalUser(Context.ConnectionId, allClients);
    }

    public async Task SendSignal(string signal, string targetConnectionId)
    {
        var callingUser = VoiceRoomContainer.UserService.GetInternalUserByConnection(Context.ConnectionId);
        var targetUser = VoiceRoomContainer.UserService.GetInternalUserByConnection(targetConnectionId);

        if (callingUser == null || targetUser == null)
        {
            return;
        }

        await Clients.Client(targetConnectionId).SendAsync("receiveSignal", callingUser, signal);
    }

    public async Task SendRoomsData(bool self)
    {
        await VoiceRoomContainer.VoiceRoomService.InternalVoiceRooms.SendRoomListUpdate(self ? Clients.Caller : Clients.All);
    }
}