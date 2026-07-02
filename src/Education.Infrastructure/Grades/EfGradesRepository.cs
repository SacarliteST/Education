using Education.Application.Grades;
using Education.Domain.Practicals;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Grades;

internal sealed class EfGradesRepository(EducationDbContext context) : IGradesRepository
{
    public async Task<PracticalGrade> GetPracticalGradeAsync(
        long practicalId,
        long studentUserId,
        CancellationToken cancellationToken = default)
    {
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
}
