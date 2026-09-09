using Education.Domain.PracticalModules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class PracticalTaskEventConfiguration : IEntityTypeConfiguration<PracticalTaskEvent>
{
    public void Configure(EntityTypeBuilder<PracticalTaskEvent> builder)
    {
        builder.ToTable("PracticalTaskEvents");
        builder.HasKey(taskEvent => taskEvent.Id);
        builder.Property(taskEvent => taskEvent.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(taskEvent => taskEvent.SessionId).HasColumnName("session_id");
        builder.Property(taskEvent => taskEvent.Kind).HasColumnName("kind").IsRequired().HasMaxLength(100);
        builder.Property(taskEvent => taskEvent.OccurredAt).HasColumnName("occurred_at");
        builder.Property(taskEvent => taskEvent.Payload).HasColumnName("payload").HasColumnType("jsonb").IsRequired();
        builder.HasIndex(taskEvent => taskEvent.SessionId);
        builder.HasOne<PracticalModuleSession>()
            .WithMany()
            .HasForeignKey(taskEvent => taskEvent.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
