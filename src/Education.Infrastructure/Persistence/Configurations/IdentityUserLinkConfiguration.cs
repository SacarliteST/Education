using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class IdentityUserLinkConfiguration : IEntityTypeConfiguration<IdentityUserLink>
{
    public void Configure(EntityTypeBuilder<IdentityUserLink> builder)
    {
        builder.ToTable("IdentityUserLinks");
        builder.HasKey(link => link.Id);
        builder.Property(link => link.Id).HasColumnName("id");
        builder.Property(link => link.LegacyUserId).HasColumnName("legacy_user_id");
        builder.Property(link => link.IdentityUserId).HasColumnName("identity_user_id");
        builder.Property(link => link.CreatedAt).HasColumnName("created_at");
        builder.Property(link => link.IsActive).HasColumnName("is_active");
        builder.HasIndex(link => link.LegacyUserId).IsUnique();
        builder.HasIndex(link => link.IdentityUserId).IsUnique();
        builder.HasOne<Education.Domain.Users.User>()
            .WithMany()
            .HasForeignKey(link => link.LegacyUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
