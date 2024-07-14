using Mapster;
using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Data.Models;
using Microsoft.AspNetCore.Authorization;
using PoshtoBack.Containers;
using PoshtoBack.Data;

namespace PoshtoBack.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private IUnitOfWork _unitOfWork;
    public ChatHub(PoshtoDbContext context)
    {
        _unitOfWork = new UnitOfWork(context);
        GlobalContainer.Initialize(_unitOfWork);
    }
    
    public async Task AddMessage(string userId, string text) // TODO: remove userId sending
    {
        var newMessage = new Message()
        {
            SenderId = Convert.ToInt32(userId),
            Timestamp = DateTime.UtcNow,
            Text = text
        };
        _unitOfWork.Messages.Add(newMessage);
        _unitOfWork.Save();

        var message = newMessage.Adapt<MessageDto>();
        
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}