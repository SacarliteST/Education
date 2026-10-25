using Education.Domain.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

/// <summary>Отображение сущности <see cref="QuestionType"/> на таблицу базы данных.</summary>
internal sealed class QuestionTypeConfiguration : IEntityTypeConfiguration<QuestionType>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<QuestionType> builder)
    {
        builder.ToTable("QuestionTypes");
        builder.HasKey(questionType => questionType.Id);
        builder.Property(questionType => questionType.Id).HasColumnName("id");
        builder.Property(questionType => questionType.Name).HasColumnName("qt_name").IsRequired();
        builder.HasData(
            new QuestionType(QuestionTypeIds.SingleChoice, "Вопрос с одним ответом"),
            new QuestionType(QuestionTypeIds.MultipleChoice, "Вопрос с несколькими ответами"),
            new QuestionType(QuestionTypeIds.Match, "Вопрос с соотнесением"),
            new QuestionType(QuestionTypeIds.ShortAnswer, "Вопрос с вводом ответа"));
    }
}


