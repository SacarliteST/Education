using Education.Application.Audit;
using Education.Domain.Audit;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Audit;

internal sealed class EfAdminEventsRepository(EducationDbContext db) : IAdminEventsRepository
{
    public async Task<IReadOnlyList<AdminEvent>> GetRecentAsync(
        int take,
        CancellationToken cancellationToken = default)
    {
        return await db.AdminEvents
            .AsNoTracking()
            .OrderByDescending(adminEvent => adminEvent.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
