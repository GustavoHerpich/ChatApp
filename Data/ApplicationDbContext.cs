using Chat.Data.Map;
using Chat.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> _options) : DbContext(_options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ChatSession> Chats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserMap());
        modelBuilder.ApplyConfiguration(new ChatMap());
        modelBuilder.ApplyConfiguration(new MessageMap());

        base.OnModelCreating(modelBuilder);
    }
}