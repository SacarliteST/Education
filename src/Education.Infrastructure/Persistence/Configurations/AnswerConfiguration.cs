using Education.Domain.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("Answers");
        builder.HasKey(answer => answer.Id);
        builder.Property(answer => answer.Id).HasColumnName("id");
        builder.Property(answer => answer.Answers).HasColumnName("answer").HasColumnType("jsonb").IsRequired();
        builder.Property(answer => answer.PracticalMaterialBindQuestionId).HasColumnName("practical_material_bind_question_id");
        builder.Property(answer => answer.TestResultId).HasColumnName("test_result_id");
        builder.HasOne(answer => answer.PracticalMaterialBindQuestion)
            .WithMany(bind => bind.Answers)
            .HasForeignKey(answer => answer.PracticalMaterialBindQuestionId);
        builder.HasOne(answer => answer.TestResult)
            .WithMany(result => result.Answers)
            .HasForeignKey(answer => answer.TestResultId);
    }
}
