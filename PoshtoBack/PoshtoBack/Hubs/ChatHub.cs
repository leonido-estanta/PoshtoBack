using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Data.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;

namespace PoshtoBack.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public async Task ResendMessage(Message message)
    {
        var messageDto = message.Adapt<MessageDto>();

        await Clients.All.SendAsync("ReceiveMessage", messageDto);
    }
}