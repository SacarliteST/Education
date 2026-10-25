using Education.Domain.Materials;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

/// <summary>Отображение сущности <see cref="TheoreticalMaterial"/> на таблицу базы данных.</summary>
internal sealed class TheoreticalMaterialConfiguration : IEntityTypeConfiguration<TheoreticalMaterial>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TheoreticalMaterial> builder)
    {
        builder.ToTable("TheoreticalMaterials");
        builder.HasKey(theory => theory.Id);
        builder.Property(theory => theory.Id).HasColumnName("id");
        builder.Property(theory => theory.Name).HasColumnName("tm_name").IsRequired();
        builder.Property(theory => theory.Text).HasColumnName("lecture_text").IsRequired();
        builder.Property(theory => theory.ModuleId).HasColumnName("module_id");
        builder.HasOne(theory => theory.Module)
            .WithMany(module => module.TheoreticalMaterials)
            .HasForeignKey(theory => theory.ModuleId);
    }
}


