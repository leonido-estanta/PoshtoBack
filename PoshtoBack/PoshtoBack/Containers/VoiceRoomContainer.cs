using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Data.Models;
using PoshtoBack.Services;

namespace PoshtoBack.Containers;

public static class VoiceRoomContainer
{
    public static bool Loaded { get; set; } = false;
    public static List<User> DbUsers { get; set; } = [];
    public static List<UserInternal> InternalUsers { get; set; } = [];
    public static VoiceRoomService VoiceRoomService;
    public static UserService UserService;
}