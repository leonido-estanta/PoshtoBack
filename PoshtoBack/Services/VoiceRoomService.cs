using Mapster;
using PoshtoBack.Containers;
using PoshtoBack.Data;
using PoshtoBack.Data.Models;

namespace PoshtoBack.Services;

public class VoiceRoomService
{
    public VoiceRoomService(IUnitOfWork unitOfWork)
    {
        GlobalContainer.DbVoiceRooms = unitOfWork.VoiceRooms.GetAll().ToList();

        InitializeRoomsFromDatabase();
    }

    private void InitializeRoomsFromDatabase()
    {
        foreach (var voiceRoom in GlobalContainer.DbVoiceRooms)
        {
            var internalRoom = new VoiceRoomInternal
            {
                Id = voiceRoom.Id,
                VoiceRoom = voiceRoom.Adapt<VoiceRoomDto>(),
                ConnectedUsers = []
            };

            GlobalContainer.InternalVoiceRooms.Add(internalRoom);
        }
    }

    public VoiceRoomInternal AddUserToRoom(UserInternal internalUser, string roomId)
    {
        var internalVoiceRoom = GlobalContainer.InternalVoiceRooms.FirstOrDefault(w => w.Id.ToString() == roomId);
        internalVoiceRoom?.ConnectedUsers.Add(internalUser);
        internalUser.CurrentVoiceRoom = internalVoiceRoom;

        return internalVoiceRoom;
    }
}