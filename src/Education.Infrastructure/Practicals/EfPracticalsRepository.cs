using Education.Application.Practicals;
using Education.Domain.Practicals;
using Education.Domain.Tests;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Practicals;

internal sealed class EfPracticalsRepository(EducationDbContext context)
    : RepositoryBase<PracticalMaterial, Guid>(context), IPracticalsRepository
{
    public override Task<PracticalMaterial?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.PracticalMaterials.FirstOrDefaultAsync(practical => practical.Id == id, cancellationToken);
    }

    public Task<bool> IsPracticalOwnerAsync(Guid practicalId, Guid teacherUserId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.PracticalMaterials.AnyAsync(
            practical => practical.Id == practicalId && practical.Module.Course.UserId == teacherUserId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<PracticalMaterial>> GetPracticalsAsync(
        Guid moduleId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.PracticalMaterials
            .AsNoTracking()
            .Where(practical => practical.ModuleId == moduleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<PracticalMaterial> CreatePracticalAsync(
        CreatePracticalCommand command,
        CancellationToken cancellationToken = default)
    {
        var practical = new PracticalMaterial(command.ModuleId, command.Name);
        await DatabaseContext.PracticalMaterials.AddAsync(practical, cancellationToken);
        await DatabaseContext.SaveChangesAsync(cancellationToken);

        return practical;
    }

    public async Task PublishPracticalAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        var practical = await DatabaseContext.PracticalMaterials.FirstOrDefaultAsync(
            item => item.Id == practicalId,
            cancellationToken);
        practical?.Publish();
        await DatabaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Case>> GetTasksAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.Cases
            .AsNoTracking()
            .Where(task => task.PracticalMaterialId == practicalId)
            .ToListAsync(cancellationToken);
    }

    public async Task<PracticalQuestionsSetup?> GetQuestionsSetupAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default)
    {
        var practical = await DatabaseContext.PracticalMaterials
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == practicalId, cancellationToken);
        if (practical is null)
        {
            return null;
        }

        var questions = await DatabaseContext.Questions
            .AsNoTracking()
            .Where(question => question.ModuleId == practical.ModuleId)
            .Select(question => new SelectableQuestion(
                question.Id,
                question.Text,
                question.QuestionTypeId,
                question.Options,
                question.PracticalMaterialBindQuestions.Any(bind => bind.PracticalMaterialId == practicalId)))
            .ToListAsync(cancellationToken);

        return new PracticalQuestionsSetup(
            questions,
            practical.IsPublic,
            practical.TriesCount,
            practical.PercentForFive,
            practical.PercentForFour,
            practical.PercentForThree);
    }

    public async Task ConfigureQuestionsAsync(
        ConfigurePracticalQuestionsCommand command,
        CancellationToken cancellationToken = default)
    {
        var practical = await DatabaseContext.PracticalMaterials.FirstOrDefaultAsync(
            item => item.Id == command.PracticalId,
            cancellationToken);
        if (practical is null)
        {
            return;
        }

        practical.ConfigureTest(
            command.TriesCount,
            command.PercentForFive,
            command.PercentForFour,
            command.PercentForThree);

        var currentBinds = await DatabaseContext.PracticalMaterialBindQuestions
            .Where(bind => bind.PracticalMaterialId == command.PracticalId)
            .ToListAsync(cancellationToken);
        var requestedIds = command.QuestionIds.ToHashSet();

        DatabaseContext.PracticalMaterialBindQuestions.RemoveRange(
            currentBinds.Where(bind => !requestedIds.Contains(bind.QuestionId)));

        var currentIds = currentBinds.Select(bind => bind.QuestionId).ToHashSet();
        var newBinds = requestedIds
            .Where(questionId => !currentIds.Contains(questionId))
            .Select(questionId => new PracticalMaterialBindQuestion(command.PracticalId, questionId));

        await DatabaseContext.PracticalMaterialBindQuestions.AddRangeAsync(newBinds, cancellationToken);
        await DatabaseContext.SaveChangesAsync(cancellationToken);
    }
}


