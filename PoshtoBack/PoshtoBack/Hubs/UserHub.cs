using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Containers;
using PoshtoBack.Data;
using PoshtoBack.Data.Models;

namespace PoshtoBack.Hubs;

[Authorize]
public class UserHub : Hub
{
    public UserHub(PoshtoDbContext context)
    {
        IUnitOfWork unitOfWork = new UnitOfWork(context);
        GlobalContainer.Initialize(unitOfWork);
    }

    public async Task EnterServer(string userId)
    {
        await GlobalContainer.UserService.EnterServer(userId, Context.ConnectionId);

        await Clients.All.SendAsync("updateServerUsers", GlobalContainer.ServerUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var singleOrDefault = GlobalContainer.ServerUsers.SingleOrDefault(w => w.ConnectionId == Context.ConnectionId);
        singleOrDefault.OnlineStatus = OnlineStatus.Offline;
        
        await Clients.All.SendAsync("updateServerUsers", GlobalContainer.ServerUsers);

        await base.OnDisconnectedAsync(exception);
    }
}