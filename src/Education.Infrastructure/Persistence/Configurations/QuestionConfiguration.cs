using Education.Domain.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");
        builder.HasKey(question => question.Id);
        builder.Property(question => question.Id).HasColumnName("id");
        builder.Property(question => question.Text).HasColumnName("question_text").IsRequired();
        builder.Property(question => question.Options).HasColumnName("question_body").HasColumnType("jsonb").IsRequired();
        builder.Property(question => question.Answer).HasColumnName("answer").HasColumnType("jsonb").IsRequired();
        builder.Property(question => question.Weight).HasColumnName("weight");
        builder.Property(question => question.QuestionTypeId).HasColumnName("question_type_id");
        builder.Property(question => question.ModuleId).HasColumnName("module_id");
        builder.HasOne(question => question.QuestionType).WithMany().HasForeignKey(question => question.QuestionTypeId);
        builder.HasOne(question => question.Module).WithMany(module => module.Questions).HasForeignKey(question => question.ModuleId);
    }
}
