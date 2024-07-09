using Mapster;
using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Containers;
using PoshtoBack.Data;
using PoshtoBack.Data.Models;
using PoshtoBack.Helpers;

namespace PoshtoBack.Services;

public class UserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        InitializeUsersFromDatabase();
    }

    private void InitializeUsersFromDatabase()
    {
        GlobalContainer.DbUsers = _unitOfWork.Users.GetAll().ToList();

        InitializeServerUsersFromDatabase();
    }
    
    private void InitializeServerUsersFromDatabase()
    {
        foreach (var user in GlobalContainer.DbUsers)
        {
            var userDto = user.Adapt<ServerUserDto>();
            GlobalContainer.ServerUsers.Add(userDto);
        }
    }

    public UserInternal AddInternalUser(string userId, string connectionId)
    {
        var existingUser = GlobalContainer.InternalUsers.SingleOrDefault(user => user.Id.ToString() == userId);
        if (existingUser != null)
        {
            return existingUser;
        }
        
        var dbUser = GlobalContainer.DbUsers.SingleOrDefault(user => user.Id.ToString() == userId);
        if (dbUser == null)
        {
            throw new Exception("User not found");
        }
        
        var newInternalUser = new UserInternal
        {
            Id = dbUser.Id,
            User = dbUser.Adapt<UserDto>(),
            ConnectionId = connectionId
        };
        GlobalContainer.InternalUsers.Add(newInternalUser);
            
        return newInternalUser;
    }

    public async Task RemoveInternalUser(string connectionId, IClientProxy allClients)
    {
        var internalUserToRemove = GlobalContainer.InternalUsers.SingleOrDefault(user => user.ConnectionId == connectionId);
        if (internalUserToRemove == null)
        {
            return;
        }

        await DisconnectInternalUser(internalUserToRemove, allClients);

        GlobalContainer.InternalUsers.Remove(internalUserToRemove);
    }

    public async Task DisconnectInternalUser(UserInternal userInternal, IClientProxy allClients)
    {
        var room = userInternal.CurrentVoiceRoom;
        if (room != null && room.ConnectedUsers.Contains(userInternal))
        {
            room.ConnectedUsers.Remove(userInternal);
            await room.SendUserListUpdate(allClients, false);
        }
    }
    
    public async Task EnterServer(string userId, string connectionId)
    {
        var existingUser = GlobalContainer.ServerUsers.SingleOrDefault(user => user.Id.ToString() == userId);
        existingUser.OnlineStatus = OnlineStatus.Online;
        existingUser.ConnectionId = connectionId;
    }

    public UserInternal? GetInternalUserByConnection(string connectionId)
    {
        var user = GlobalContainer.InternalUsers.SingleOrDefault(user => user.ConnectionId == connectionId);
        return user;
    }
}