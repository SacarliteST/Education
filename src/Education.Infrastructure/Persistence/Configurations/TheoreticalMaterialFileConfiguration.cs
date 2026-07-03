using Education.Domain.Materials;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class TheoreticalMaterialFileConfiguration : IEntityTypeConfiguration<TheoreticalMaterialFile>
{
    public void Configure(EntityTypeBuilder<TheoreticalMaterialFile> builder)
    {
        builder.ToTable("TheoreticalMaterialFiles");
        builder.HasKey(file => file.Id);
        builder.Property(file => file.Id).HasColumnName("id");
        builder.Property(file => file.Description).HasColumnName("description").IsRequired();
        builder.Property(file => file.Path).HasColumnName("path").IsRequired();
        builder.Property(file => file.OriginalFileName)
            .HasColumnName("original_file_name")
            .HasMaxLength(255)
            .IsRequired()
            .HasDefaultValue(String.Empty);
        builder.Property(file => file.TheoreticalMaterialId).HasColumnName("theoretical_material_id");
        builder.HasOne(file => file.TheoreticalMaterial)
            .WithMany(theory => theory.Files)
            .HasForeignKey(file => file.TheoreticalMaterialId);
    }
}


