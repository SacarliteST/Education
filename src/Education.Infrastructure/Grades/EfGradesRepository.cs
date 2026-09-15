using Education.Application.Grades;
using Education.Domain.Practicals;
using Education.Domain.PracticalModules;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Grades;

internal sealed class EfGradesRepository(EducationDbContext context) : IGradesRepository
{
    public async Task<PracticalGrade> GetPracticalGradeAsync(
        Guid practicalId,
        Guid studentUserId,
        CancellationToken cancellationToken = default)
    {
        var kind = await context.PracticalMaterials
            .Where(practical => practical.Id == practicalId)
            .Select(practical => practical.Kind)
            .FirstOrDefaultAsync(cancellationToken);
        if (kind == PracticalKind.External)
        {
            return await GetExternalPracticalGradeAsync(practicalId, studentUserId, cancellationToken);
        }

        var totalTasks = await context.Cases.CountAsync(
            task => task.PracticalMaterialId == practicalId,
            cancellationToken);
        var acceptedTasks = await context.CaseFiles
            .Include(file => file.Case)
            .CountAsync(
                file => file.Case.PracticalMaterialId == practicalId
                    && file.UserId == studentUserId
                    && file.IsAccepted,
                cancellationToken);

        var messages = new List<string>();
        if (totalTasks != acceptedTasks)
        {
            messages.Add("Выполните все задания");
        }

        var testResults = await context.TestResults
            .Include(result => result.PracticalMaterial)
            .AsNoTracking()
            .Where(result => result.PracticalMaterialId == practicalId && result.UserId == studentUserId && result.IsCompleted)
            .ToListAsync(cancellationToken);
        if (testResults.Count == 0)
        {
            messages.Add("Пройдите тестирование");
        }

        if (messages.Count != 0)
        {
            return new PracticalGrade(null, messages);
        }

        var bestTestGrade = testResults.Max(result => result.PracticalMaterial.CalculateTestGrade(result.Score, result.MaxScore));
        var taskGrades = await context.CaseFiles
            .Include(file => file.Case)
            .Where(file => file.Case.PracticalMaterialId == practicalId && file.UserId == studentUserId)
            .Select(file => file.Grade)
            .ToListAsync(cancellationToken);
        var grade = taskGrades.Count == 0
            ? bestTestGrade
            : (int)Math.Ceiling((bestTestGrade + taskGrades.Average()) / 2);

        return new PracticalGrade(grade, []);
    }

    private async Task<PracticalGrade> GetExternalPracticalGradeAsync(
        Guid practicalId,
        Guid studentUserId,
        CancellationToken cancellationToken)
    {
        var bestScore = await context.PracticalModuleSessions
            .Where(session => session.UserId == studentUserId
                && session.Status == ModuleSessionState.Completed
                && session.Grade != null
                && context.Cases.Any(task =>
                    task.Id == session.PracticalTaskId && task.PracticalMaterialId == practicalId))
            .MaxAsync(session => (int?)session.Grade, cancellationToken);

        if (bestScore is null)
        {
            return new PracticalGrade(null, ["Пройдите практику"]);
        }

        // session.Grade — составной балл модуля 0..100, а не готовая оценка по
        // 5-балльной шкале: переводим его теми же порогами, что и обычные
        // тесты/кейсы этой практики, иначе внешняя и внутренняя практика
        // показывают студенту оценку в разных, несовместимых шкалах.
        var practical = await context.PracticalMaterials
            .FirstAsync(material => material.Id == practicalId, cancellationToken);
        var grade = practical.CalculateTestGrade(bestScore, 100);

        return new PracticalGrade(grade, []);
    }
}


