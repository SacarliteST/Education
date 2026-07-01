using Education.Domain.Practicals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class CaseConfiguration : IEntityTypeConfiguration<Case>
{
    public void Configure(EntityTypeBuilder<Case> builder)
    {
        builder.ToTable("Cases");
        builder.HasKey(task => task.Id);
        builder.Property(task => task.Id).HasColumnName("id");
        builder.Property(task => task.Name).HasColumnName("pm_name").IsRequired();
        builder.Property(task => task.Text).HasColumnName("case_text").IsRequired();
        builder.Property(task => task.PracticalMaterialId).HasColumnName("practical_material_id");
        builder.HasOne(task => task.PracticalMaterial)
            .WithMany(practical => practical.Cases)
            .HasForeignKey(task => task.PracticalMaterialId);
    }
}
