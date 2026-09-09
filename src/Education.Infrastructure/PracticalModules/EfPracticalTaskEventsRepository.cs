using Education.Application.PracticalModules;
using Education.Domain.PracticalModules;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.PracticalModules;

internal sealed class EfPracticalTaskEventsRepository(EducationDbContext context) : IPracticalTaskEventsRepository
{
    public async Task InsertIfNewAsync(PracticalTaskEvent taskEvent, CancellationToken cancellationToken = default)
    {
        var exists = await context.PracticalTaskEvents
            .AnyAsync(existing => existing.Id == taskEvent.Id, cancellationToken);
        if (exists)
        {
            return;
        }

        context.PracticalTaskEvents.Add(taskEvent);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Гонка переигрывания: событие уже вставлено параллельно — это ок.
            context.Entry(taskEvent).State = EntityState.Detached;
        }
    }

    public async Task<IReadOnlyList<PracticalTaskEvent>> GetForSessionAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        return await context.PracticalTaskEvents
            .AsNoTracking()
            .Where(taskEvent => taskEvent.SessionId == sessionId)
            .OrderBy(taskEvent => taskEvent.OccurredAt)
            .ThenBy(taskEvent => taskEvent.Id)
            .ToListAsync(cancellationToken);
    }
}
