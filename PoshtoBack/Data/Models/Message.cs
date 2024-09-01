using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoshtoBack.Data.Models;

public class Message
{
    [Key]
    public int Id { get; set; }
    
    public int SenderId { get; set; }
    [ForeignKey("SenderId")]
    public User Sender { get; set; }
    
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
}

public class MessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
}

public class AddMessageDto
{
    public string Text { get; set; }
}