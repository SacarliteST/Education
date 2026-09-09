using Education.Domain.PracticalModules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class PracticalModuleConfiguration : IEntityTypeConfiguration<PracticalModule>
{
    public void Configure(EntityTypeBuilder<PracticalModule> builder)
    {
        builder.ToTable("PracticalModules");
        builder.HasKey(module => module.Id);
        builder.Property(module => module.Id).HasColumnName("id");
        builder.Property(module => module.Slug).HasColumnName("slug").IsRequired().HasMaxLength(100);
        builder.HasIndex(module => module.Slug).IsUnique();
        builder.Property(module => module.Name).HasColumnName("pm_name").IsRequired().HasMaxLength(200);
        builder.Property(module => module.Description).HasColumnName("description").IsRequired();
        builder.Property(module => module.PracticeType).HasColumnName("practice_type").IsRequired().HasMaxLength(100);
        builder.Property(module => module.BasePath).HasColumnName("base_path").IsRequired().HasMaxLength(200);
        builder.Property(module => module.IdentityAudience).HasColumnName("identity_audience").IsRequired().HasMaxLength(200);
        builder.Property(module => module.IsEnabled).HasColumnName("is_enabled");
        builder.Property(module => module.Configuration).HasColumnName("configuration").HasColumnType("jsonb").IsRequired();
    }
}
