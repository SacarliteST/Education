using Education.Domain.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class QuestionTypeConfiguration : IEntityTypeConfiguration<QuestionType>
{
    public void Configure(EntityTypeBuilder<QuestionType> builder)
    {
        builder.ToTable("QuestionTypes");
        builder.HasKey(questionType => questionType.Id);
        builder.Property(questionType => questionType.Id).HasColumnName("id");
        builder.Property(questionType => questionType.Name).HasColumnName("qt_name").IsRequired();
        builder.HasData(
            new QuestionType(1, "Вопрос с одним ответом"),
            new QuestionType(2, "Вопрос с несколькими ответами"),
            new QuestionType(3, "Вопрос с соотнесением"),
            new QuestionType(4, "Вопрос с вводом ответа"));
    }
}
