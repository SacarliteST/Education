using Education.Application.Audit;
using Education.Application.Identity;
using Education.Domain.Audit;
using Education.Infrastructure.Persistence;

namespace Education.Infrastructure.Audit;

internal sealed class EfAdminEventRecorder(
    EducationDbContext db,
    ICurrentUser currentUser,
    TimeProvider timeProvider) : IAdminEventRecorder
{
    public async Task RecordAsync(
        string eventType,
        string description,
        CancellationToken cancellationToken = default)
    {
        db.AdminEvents.Add(AdminEvent.Create(
            currentUser.IsAuthenticated ? currentUser.UserId : null,
            currentUser.Name,
            eventType,
            description,
            timeProvider.GetUtcNow()));

        await db.SaveChangesAsync(cancellationToken);
    }
}
