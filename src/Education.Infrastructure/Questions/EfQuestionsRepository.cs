using Education.Application.Questions;
using Education.Domain.Tests;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Questions;

internal sealed class EfQuestionsRepository(EducationDbContext context)
    : RepositoryBase<Question, Guid>(context), IQuestionsRepository
{
    public override Task<Question?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Questions.FirstOrDefaultAsync(question => question.Id == id, cancellationToken);
    }

    public Task<bool> IsQuestionOwnerAsync(Guid questionId, Guid teacherUserId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Questions.AnyAsync(
            question => question.Id == questionId && question.Module.Course.UserId == teacherUserId,
            cancellationToken);
    }

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

    public async Task DeleteQuestionAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        await DatabaseContext.Questions
            .Where(question => question.Id == questionId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}


