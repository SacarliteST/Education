using Education.Domain.Practicals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class PracticalMaterialConfiguration : IEntityTypeConfiguration<PracticalMaterial>
{
    public void Configure(EntityTypeBuilder<PracticalMaterial> builder)
    {
        builder.ToTable("PracticalMaterials");
        builder.HasKey(practical => practical.Id);
        builder.Property(practical => practical.Id).HasColumnName("id");
        builder.Property(practical => practical.Name).HasColumnName("pm_name").IsRequired();
        builder.Property(practical => practical.ModuleId).HasColumnName("module_id");
        builder.Property(practical => practical.IsPublic).HasColumnName("is_public");
        builder.Property(practical => practical.TriesCount).HasColumnName("tries_count");
        builder.Property(practical => practical.PercentForFive).HasColumnName("percent_for_five");
        builder.Property(practical => practical.PercentForFour).HasColumnName("percent_for_four");
        builder.Property(practical => practical.PercentForThree).HasColumnName("percent_for_three");
        builder.HasOne(practical => practical.Module)
            .WithMany(module => module.PracticalMaterials)
            .HasForeignKey(practical => practical.ModuleId);
    }
}
