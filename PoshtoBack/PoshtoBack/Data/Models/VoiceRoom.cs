using System.ComponentModel.DataAnnotations;

namespace PoshtoBack.Data.Models;

public class VoiceRoom
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
}

public class VoiceRoomConnectDto
{
    public string UserId { get; set; }
    public string RoomId { get; set; }
}

public class VoiceRoomInternal
{
    public int Id { get; set; }
    public VoiceRoom VoiceRoom { get; set; }
    public List<UserInternal> ConnectedUsers { get; set; }
}