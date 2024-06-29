using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Containers;
using PoshtoBack.Data;
using PoshtoBack.Data.Models;
using PoshtoBack.Helpers;
using PoshtoBack.Hubs;

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
        VoiceRoomContainer.DbUsers = _unitOfWork.Users.GetAll().ToList();
    }

    public UserInternal AddInternalUser(string userId, string connectionId)
    {
        var existingUser = VoiceRoomContainer.InternalUsers.SingleOrDefault(user => user.Id.ToString() == userId);
        if (existingUser != null)
        {
            return existingUser;
        }
        
        var dbUser = VoiceRoomContainer.DbUsers.SingleOrDefault(user => user.Id.ToString() == userId);
        if (dbUser == null)
        {
            throw new Exception("User not found");
        }
        
        var newInternalUser = new UserInternal
        {
            Id = dbUser.Id,
            User = dbUser,
            ConnectionId = connectionId
        };
        VoiceRoomContainer.InternalUsers.Add(newInternalUser);
            
        return newInternalUser;
    }

    public async Task RemoveInternalUser(string connectionId, IClientProxy allClients)
    {
        var internalUserToRemove = VoiceRoomContainer.InternalUsers.SingleOrDefault(user => user.ConnectionId == connectionId);
        if (internalUserToRemove == null)
        {
            return;
        }

        await DisconnectInternalUser(internalUserToRemove, allClients);

        VoiceRoomContainer.InternalUsers.Remove(internalUserToRemove);
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

    public UserInternal? GetInternalUserByConnection(string connectionId)
    {
        var user = VoiceRoomContainer.InternalUsers.SingleOrDefault(user => user.ConnectionId == connectionId);
        return user;
    }
}