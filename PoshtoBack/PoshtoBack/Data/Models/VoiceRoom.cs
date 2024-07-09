using System.ComponentModel.DataAnnotations;

namespace PoshtoBack.Data.Models;

public class VoiceRoom
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
}

public class VoiceRoomDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class VoiceRoomInternal
{
    public int Id { get; set; }
    public VoiceRoomDto VoiceRoom { get; set; }
    public List<UserInternal> ConnectedUsers { get; set; }
}