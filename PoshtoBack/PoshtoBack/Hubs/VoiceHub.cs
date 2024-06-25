using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Data;
using PoshtoBack.Data.Models;
using PoshtoBack.Helpers;
using PoshtoBack.Services;

namespace PoshtoBack.Hubs;

public class VoiceHub : Hub
{
    private readonly VoiceRoomService _voiceRoomService;
    private readonly UserService _userService;

    public VoiceHub(PoshtoDbContext context)
    {
        IUnitOfWork unitOfWork = new UnitOfWork(context);
        _voiceRoomService = new VoiceRoomService(unitOfWork);
        _userService = new UserService(unitOfWork, this);
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
        var user = _userService.AddInternalUser(userId, Context.ConnectionId);
        await _userService.DisconnectInternalUser(user);
        var room = _voiceRoomService.AddUserToRoom(user, voiceRoomId);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, voiceRoomId);

        await room.SendUserListUpdate(Clients.Caller, true);
        await room.SendUserListUpdate(Clients.Others, false);
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Disconnect();

        await base.OnDisconnectedAsync(exception);
    }

    public async Task Disconnect()
    {
        await _userService.RemoveInternalUser(Context.ConnectionId);
    }
}