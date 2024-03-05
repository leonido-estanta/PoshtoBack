using System.ComponentModel.DataAnnotations;

namespace PoshtoBack.Data.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string PasswordHash { get; set; }
    public string? AvatarUrl { get; set; }
    public bool? IsOnline { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }
    public bool IsOnline { get; set; }
}