using Education.Domain.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class TestResultConfiguration : IEntityTypeConfiguration<TestResult>
{
    public void Configure(EntityTypeBuilder<TestResult> builder)
    {
        builder.ToTable("TestResults");
        builder.HasKey(result => result.Id);
        builder.Property(result => result.Id).HasColumnName("id");
        builder.Property(result => result.StatedDate).HasColumnName("started_at");
        builder.Property(result => result.TurnedDate).HasColumnName("turned_at");
        builder.Property(result => result.TryNumber).HasColumnName("try_number");
        builder.Property(result => result.IsCompleted).HasColumnName("is_completed");
        builder.Property(result => result.Score).HasColumnName("score");
        builder.Property(result => result.MaxScore).HasColumnName("max_score");
        builder.Property(result => result.UserId).HasColumnName("user_id");
        builder.Property(result => result.PracticalMaterialId).HasColumnName("practical_material_id");
        builder.HasOne(result => result.User).WithMany().HasForeignKey(result => result.UserId);
        builder.HasOne(result => result.PracticalMaterial)
            .WithMany(practical => practical.TestResults)
            .HasForeignKey(result => result.PracticalMaterialId);
    }
}


