using Education.Application.PracticalModules;
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

    public Task<bool> IsTaskOwnerAsync(Guid taskId, Guid teacherUserId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Cases.AnyAsync(
            task => task.Id == taskId && task.PracticalMaterial.Module.Course.UserId == teacherUserId,
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

    public async Task<PracticalDetail?> GetDetailAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default)
    {
        var practical = await DatabaseContext.PracticalMaterials
            .AsNoTracking()
            .Where(item => item.Id == practicalId)
            .Select(item => new
            {
                item.Id,
                item.Name,
                item.Kind,
                item.IsPublic,
                item.TriesCount,
                item.TimeLimitMinutes,
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (practical is null)
        {
            return null;
        }

        ExternalModuleBinding? binding = null;
        if (practical.Kind == PracticalKind.External)
        {
            binding = await DatabaseContext.Cases
                .AsNoTracking()
                .Where(task => task.PracticalMaterialId == practicalId && task.PracticalModuleId != null)
                .Join(
                    DatabaseContext.PracticalModules,
                    task => task.PracticalModuleId,
                    module => module.Id,
                    (task, module) => new ExternalModuleBinding(
                        module.Id, module.Slug, module.Name, task.Id, task.ExternalTaskRef!))
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new PracticalDetail(
            practical.Id,
            practical.Name,
            practical.Kind,
            practical.IsPublic,
            practical.TriesCount,
            practical.TimeLimitMinutes,
            binding);
    }

    public Task<bool> IsTaskInPracticalAsync(
        Guid taskId,
        Guid practicalId,
        CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Cases.AnyAsync(
            task => task.Id == taskId && task.PracticalMaterialId == practicalId,
            cancellationToken);
    }

    public async Task<bool> HasStudentActivityAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default)
    {
        var hasFiles = await DatabaseContext.CaseFiles
            .AnyAsync(file => file.Case.PracticalMaterialId == practicalId, cancellationToken);
        if (hasFiles)
        {
            return true;
        }

        return await DatabaseContext.TestResults
            .AnyAsync(result => result.PracticalMaterialId == practicalId, cancellationToken);
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

    public async Task DeletePracticalAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        await DatabaseContext.PracticalMaterials
            .Where(practical => practical.Id == practicalId)
            .ExecuteDeleteAsync(cancellationToken);
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

    public Task<Case?> GetTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Cases
            .AsNoTracking()
            .FirstOrDefaultAsync(task => task.Id == taskId, cancellationToken);
    }

    public async Task<Case> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken = default)
    {
        var task = new Case(command.PracticalId, command.Name, "Текст задания");
        await DatabaseContext.Cases.AddAsync(task, cancellationToken);
        await DatabaseContext.SaveChangesAsync(cancellationToken);

        return task;
    }

    public async Task UpdateTaskTextAsync(Guid taskId, string text, CancellationToken cancellationToken = default)
    {
        var task = await DatabaseContext.Cases.FirstOrDefaultAsync(item => item.Id == taskId, cancellationToken);
        if (task is null)
        {
            return;
        }

        task.UpdateText(text);
        await DatabaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        await DatabaseContext.Cases
            .Where(task => task.Id == taskId)
            .ExecuteDeleteAsync(cancellationToken);
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

    public async Task BindModuleAsync(BindPracticalModuleCommand command, CancellationToken cancellationToken = default)
    {
        var practical = await DatabaseContext.PracticalMaterials.FirstOrDefaultAsync(
            item => item.Id == command.PracticalId,
            cancellationToken);
        if (practical is null)
        {
            return;
        }

        practical.BindExternalModule(command.TriesCount, command.TimeLimitMinutes);

        await DatabaseContext.Cases
            .Where(task => task.PracticalMaterialId == command.PracticalId)
            .ExecuteDeleteAsync(cancellationToken);

        var task = new Case(command.PracticalId, "Задание внешнего модуля", String.Empty);
        task.LinkExternalTask(command.PracticalModuleId, command.ExternalTaskRef);
        await DatabaseContext.Cases.AddAsync(task, cancellationToken);

        await DatabaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ExternalTaskBinding?> GetExternalTaskBindingAsync(
        Guid practicalId,
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.Cases
            .AsNoTracking()
            .Where(task => task.Id == taskId
                && task.PracticalMaterialId == practicalId
                && task.PracticalModuleId != null
                && task.PracticalMaterial.Kind == PracticalKind.External)
            .Select(task => new ExternalTaskBinding(
                task.PracticalModuleId!.Value,
                task.ExternalTaskRef!,
                task.PracticalMaterial.TriesCount,
                task.PracticalMaterial.TimeLimitMinutes,
                task.PracticalMaterial.ModuleId,
                task.PracticalMaterial.Module.CourseId))
            .FirstOrDefaultAsync(cancellationToken);
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


