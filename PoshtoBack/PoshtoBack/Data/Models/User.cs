using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PoshtoBack.Data.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Email { get; set; }
    public string? Name { get; set; }
    public string PasswordHash { get; set; }
    public string? AvatarUrl { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }
    public OnlineStatus OnlineStatus { get; set; }
}

public class ServerUserDto : UserDto
{
    public string ConnectionId { get; set; }
}

public class UserInternal
{
    public int Id { get; set; }
    public UserDto User { get; set; }
    public string ConnectionId { get; set; }
    
    [JsonIgnore]
    public VoiceRoomInternal? CurrentVoiceRoom { get; set; }
}