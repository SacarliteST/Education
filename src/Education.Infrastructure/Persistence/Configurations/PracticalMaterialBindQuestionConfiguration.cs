using Education.Domain.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class PracticalMaterialBindQuestionConfiguration : IEntityTypeConfiguration<PracticalMaterialBindQuestion>
{
    public void Configure(EntityTypeBuilder<PracticalMaterialBindQuestion> builder)
    {
        builder.ToTable("PracticalMaterialBindQuestions");
        builder.HasKey(bind => bind.Id);
        builder.Property(bind => bind.Id).HasColumnName("id");
        builder.Property(bind => bind.QuestionId).HasColumnName("question_id");
        builder.Property(bind => bind.PracticalMaterialId).HasColumnName("practical_material_id");
        builder.HasOne(bind => bind.Question)
            .WithMany(question => question.PracticalMaterialBindQuestions)
            .HasForeignKey(bind => bind.QuestionId);
        builder.HasOne(bind => bind.PracticalMaterial)
            .WithMany(practical => practical.PracticalMaterialBindQuestions)
            .HasForeignKey(bind => bind.PracticalMaterialId);
    }
}


