using Microsoft.EntityFrameworkCore;
using PoshtoBack.Data.Models;

namespace PoshtoBack.Data;

public class PoshtoDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<VoiceChannel> VoiceChannels { get; set; }

    public PoshtoDbContext(DbContextOptions<PoshtoDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Message>().ToTable("Messages");
        modelBuilder.Entity<VoiceChannel>().ToTable("VoiceChannels");
    }
}