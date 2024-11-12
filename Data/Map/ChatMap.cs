using Chat.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Data.Map;

public class ChatMap : IEntityTypeConfiguration<ChatSession>
{
    public void Configure(EntityTypeBuilder<ChatSession> builder)
    {
        builder.ToTable("Chats");
        builder.HasKey(c => c.ChatId);
        builder.Property(c => c.ChatId).IsRequired();
        builder.Property(c => c.ChatId).ValueGeneratedOnAdd();

        builder
            .HasMany(c => c.Participants)
            .WithMany(u => u.Chats)
            .UsingEntity(j => j.ToTable("ChatParticipants")); 
    }
}
