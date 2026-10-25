using Education.Application.Questions;
using Education.Domain.Tests;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Questions;

/// <summary>EF Core-реализация хранилища вопросов.</summary>
internal sealed class EfQuestionsRepository : RepositoryBase<Question, Guid>, IQuestionsRepository
{
    /// <summary>Создаёт экземпляр класса <see cref="EfQuestionsRepository"/>.</summary>
    /// <param name="context">Контекст базы данных Education.</param>
    public EfQuestionsRepository(EducationDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public override Task<Question?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Questions.FirstOrDefaultAsync(question => question.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> IsQuestionOwnerAsync(Guid questionId, Guid teacherUserId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Questions.AnyAsync(
            question => question.Id == questionId && question.Module.Course.UserId == teacherUserId,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Question>> GetQuestionsAsync(
        Guid moduleId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.Questions
            .AsNoTracking()
            .Where(question => question.ModuleId == moduleId)
            .OrderByDescending(question => question.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Question> CreateQuestionAsync(
        CreateQuestionCommand command,
        CancellationToken cancellationToken = default)
    {
        var question = new Question(
            command.ModuleId,
            command.Type,
            command.Text,
            command.Body,
            command.Answer,
            command.Weight);
        await DatabaseContext.Questions.AddAsync(question, cancellationToken);
        await DatabaseContext.SaveChangesAsync(cancellationToken);

        return question;
    }

    /// <inheritdoc />
    public async Task UpdateQuestionAsync(
        Guid questionId,
        UpdateQuestionCommand command,
        CancellationToken cancellationToken = default)
    {
        var question = await DatabaseContext.Questions.FirstOrDefaultAsync(
            item => item.Id == questionId,
            cancellationToken);
        question?.Update(command.Type, command.Text, command.Body, command.Answer, command.Weight);
        await DatabaseContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteQuestionAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        await DatabaseContext.Questions
            .Where(question => question.Id == questionId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<double> SumWeightsAsync(
        IReadOnlyCollection<Guid> questionIds,
        CancellationToken cancellationToken = default)
    {
        if (questionIds.Count == 0)
        {
            return 0;
        }

        return await DatabaseContext.Questions
            .Where(question => questionIds.Contains(question.Id))
            .SumAsync(question => question.Weight, cancellationToken);
    }
}


