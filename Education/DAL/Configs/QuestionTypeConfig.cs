using Education.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.DAL.Configs;

internal class QuestionTypeConfig : IEntityTypeConfiguration<QuestionType>
{
    public void Configure(EntityTypeBuilder<QuestionType> builder)
    {
        builder.HasData(new List<QuestionType>()
        {
            new() { Id = 1, Name = "Вопрос с одним ответом" },
            new() { Id = 2, Name = "Вопрос с несколькими ответами" },
            new() { Id = 3, Name = "Вопрос с соотнесением" },
            new() { Id = 4, Name = "Вопрос с вводом ответа" }
        });
    }
}
