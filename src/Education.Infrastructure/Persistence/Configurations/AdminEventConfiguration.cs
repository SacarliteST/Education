using Education.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class AdminEventConfiguration : IEntityTypeConfiguration<AdminEvent>
{
    public void Configure(EntityTypeBuilder<AdminEvent> builder)
    {
        builder.HasKey(adminEvent => adminEvent.Id);

        builder.Property(adminEvent => adminEvent.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(adminEvent => adminEvent.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(adminEvent => adminEvent.ActorName)
            .HasMaxLength(200);

        builder.HasIndex(adminEvent => adminEvent.CreatedAt);
        builder.HasIndex(adminEvent => adminEvent.EventType);
        builder.HasIndex(adminEvent => adminEvent.ActorIdentityUserId);
    }
}
