using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Data;
using PoshtoBack.Data.Models;
using PoshtoBack.Helpers;

namespace PoshtoBack.Services;

public class UserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly Hub _voiceHub;

    public List<User> DbUsers { get; set; } = [];
    public List<UserInternal> InternalUsers { get; set; } = [];

    public UserService(IUnitOfWork unitOfWork, Hub voiceHub)
    {
        _unitOfWork = unitOfWork;
        _voiceHub = voiceHub;
        
        InitializeUsersFromDatabase();
    }
    
    private void InitializeUsersFromDatabase()
    {
        DbUsers = _unitOfWork.Users.GetAll().ToList();
    }

    public UserInternal AddInternalUser(string userId, string connectionId)
    {
        var existingUser = InternalUsers.SingleOrDefault(user => user.Id.ToString() == userId)?.User;
        
        var user = existingUser ?? DbUsers.SingleOrDefault(user => user.Id.ToString() == userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        
        var newInternalUser = new UserInternal
        {
            Id = user.Id,
            User = user,
            ConnectionId = connectionId
        };
        
        InternalUsers.Add(newInternalUser);

        return newInternalUser;
    }

    public async Task RemoveInternalUser(string connectionId)
    {
        var internalUserToRemove = InternalUsers.SingleOrDefault(user => user.ConnectionId == connectionId);
        if (internalUserToRemove == null)
        {
            return;
        }

        await DisconnectInternalUser(internalUserToRemove);

        InternalUsers.Remove(internalUserToRemove);
    }

    public async Task DisconnectInternalUser(UserInternal userInternal)
    {
        if (userInternal.CurrentVoiceRoom != null)
        {
            userInternal.CurrentVoiceRoom.InternalUsers.Remove(userInternal);
            await userInternal.CurrentVoiceRoom.SendUserListUpdate(_voiceHub.Clients.Others, false);
        }
    }
}