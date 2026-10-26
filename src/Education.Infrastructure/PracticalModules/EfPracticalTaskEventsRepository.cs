using Education.Application.PracticalModules;
using Education.Domain.PracticalModules;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.PracticalModules;

/// <summary>EF Core-реализация хранилища событий заданий практики.</summary>
internal sealed class EfPracticalTaskEventsRepository : IPracticalTaskEventsRepository
{
    private readonly EducationDbContext context;

    /// <summary>Создаёт экземпляр класса <see cref="EfPracticalTaskEventsRepository"/>.</summary>
    /// <param name="context">Контекст базы данных Education.</param>
    public EfPracticalTaskEventsRepository(EducationDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public Task<int> CountByKindAsync(Guid sessionId, string kind, CancellationToken cancellationToken = default)
    {
        return context.PracticalTaskEvents.CountAsync(
            taskEvent => taskEvent.SessionId == sessionId && taskEvent.Kind == kind, cancellationToken);
    }

    /// <inheritdoc />
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
