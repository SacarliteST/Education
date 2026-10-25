using Education.Domain.Materials;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

/// <summary>Отображение сущности <see cref="TheoreticalMaterialLink"/> на таблицу базы данных.</summary>
internal sealed class TheoreticalMaterialLinkConfiguration : IEntityTypeConfiguration<TheoreticalMaterialLink>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TheoreticalMaterialLink> builder)
    {
        builder.ToTable("TheoreticalMaterialLinks");
        builder.HasKey(link => link.Id);
        builder.Property(link => link.Id).HasColumnName("id");
        builder.Property(link => link.Description).HasColumnName("description").IsRequired();
        builder.Property(link => link.Link).HasColumnName("link").IsRequired();
        builder.Property(link => link.TheoreticalMaterialId).HasColumnName("theoretical_material_id");
        builder.HasOne(link => link.TheoreticalMaterial)
            .WithMany(theory => theory.Links)
            .HasForeignKey(link => link.TheoreticalMaterialId);
    }
}


