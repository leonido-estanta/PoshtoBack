using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Data;
using PoshtoBack.Data.Models;
using PoshtoBack.Hubs;

namespace PoshtoBack.Services;

public class VoiceRoomService
{
    private readonly IUnitOfWork _unitOfWork;

    private List<VoiceRoom> DbVoiceRooms { get; set; } = [];
    private List<VoiceRoomInternal> InternalVoiceRooms { get; set; }

    public VoiceRoomService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        InitializeRoomsFromDatabase();
    }

    private void InitializeRoomsFromDatabase()
    {
        DbVoiceRooms = _unitOfWork.VoiceRooms.GetAll().ToList();

        foreach (var voiceRoom in DbVoiceRooms)
        {
            var internalRoom = new VoiceRoomInternal
            {
                Id = voiceRoom.Id,
                VoiceRoom = voiceRoom,
                InternalUsers = []
            };
            
            InternalVoiceRooms.Add(internalRoom);
        }
    }

    public VoiceRoomInternal AddUserToRoom(UserInternal internalUser, string roomId)
    {
        var internalVoiceRoom = InternalVoiceRooms.FirstOrDefault(w => w.Id.ToString() == roomId);
        internalVoiceRoom?.InternalUsers.Add(internalUser);
        internalUser.CurrentVoiceRoom = internalVoiceRoom;

        return internalVoiceRoom;
    }
}
