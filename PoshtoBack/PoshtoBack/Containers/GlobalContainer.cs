using PoshtoBack.Data;
using PoshtoBack.Data.Models;
using PoshtoBack.Services;

namespace PoshtoBack.Containers;

public static class GlobalContainer
{
    public static bool Loaded { get; set; } = false;
    
    public static List<User> DbUsers { get; set; } = [];
    public static List<UserInternal> InternalUsers { get; set; } = [];
    public static List<ServerUserDto> ServerUsers { get; set; } = [];
    public static List<VoiceRoom> DbVoiceRooms { get; set; } = [];
    public static List<VoiceRoomInternal> InternalVoiceRooms { get; set; } = [];
    
    public static VoiceRoomService VoiceRoomService;
    public static UserService UserService;

    public static void Initialize(IUnitOfWork unitOfWork)
    {
        if (Loaded) return;
        
        VoiceRoomService = new VoiceRoomService(unitOfWork);
        UserService = new UserService(unitOfWork);

        Loaded = true;
    }
}