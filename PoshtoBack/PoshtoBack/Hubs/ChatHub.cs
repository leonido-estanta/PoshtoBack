using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Data.Models;
using Mapster;

namespace PoshtoBack.Hubs;

public class ChatHub : Hub
{
    public async Task ResendMessage(Message message)
    {
        var messageDto = message.Adapt<MessageDto>();

        await Clients.All.SendAsync("ReceiveMessage", messageDto);
    }
}