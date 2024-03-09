using System.ComponentModel.DataAnnotations;

namespace PoshtoBack.Data.Models;

public class VoiceChannel
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
}

public class VoiceChannelConnectDto
{
    public int UserId { get; set; }
    public int ChannelId { get; set; }
}