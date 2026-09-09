using System.Security.Cryptography;
using System.Text.Json;
using Education.Application.Identity;
using Education.Application.Practicals;
using Education.Application.Users;
using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

public sealed class ModuleSessionsService(
    IEducationUserResolver userResolver,
    ICurrentUser currentUser,
    IPracticalsRepository practicalsRepository,
    IPracticalModulesRepository practicalModulesRepository,
    IPracticalModuleSessionsRepository sessionsRepository,
    IPracticalTaskEventsRepository eventsRepository,
    IModulePushClient pushClient,
    ITokenExchangeClient tokenExchangeClient,
    IModuleIntegrationConfig integrationConfig,
    TimeProvider timeProvider) : IModuleSessionsService
{
    public async Task<StartModuleSessionResult> StartAsync(
        Guid practicalId,
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        // Тело пуша адресует владельца сессии по identity-id (== sub обменянного токена),
        // а не по legacy-id Education — модуль сверяет именно его.
        var identityUserId = currentUser.UserId;
        var binding = await practicalsRepository.GetExternalTaskBindingAsync(practicalId, taskId, cancellationToken)
            ?? throw new ExternalTaskNotFoundException();
        var module = await practicalModulesRepository.GetByIdAsync(binding.PracticalModuleId, cancellationToken)
            ?? throw new ExternalTaskNotFoundException();

        var now = timeProvider.GetUtcNow();
        var latest = await sessionsRepository.GetLatestForUserTaskAsync(userId, taskId, cancellationToken);

        if (latest is { IsActive: true } && !latest.IsExpiredBy(now))
        {
            var launchUrl = await PushAndBuildLaunchUrlAsync(
                module, latest, binding.ExternalTaskRef, identityUserId, cancellationToken);
            return new StartModuleSessionResult(
                latest.Id, launchUrl, latest.ExpiresAt, latest.TryNumber, Resumed: true);
        }

        if (latest is { IsActive: true })
        {
            latest.Expire(now, ModuleSessionEndReason.Timeout);
            await sessionsRepository.SaveChangesAsync(cancellationToken);
        }

        var attemptsCount = await sessionsRepository.CountForUserTaskAsync(userId, taskId, cancellationToken);
        if (attemptsCount >= binding.TriesCount)
        {
            throw new TriesExhaustedException();
        }

        var sessionId = Guid.NewGuid();
        var expiresAt = binding.TimeLimitMinutes is { } minutes ? now.AddMinutes(minutes) : (DateTimeOffset?)null;
        var session = new PracticalModuleSession(
            sessionId,
            taskId,
            userId,
            attemptsCount + 1,
            GenerateSessionKey(),
            integrationConfig.BuildReturnUrl(binding.CourseId, binding.ModuleId, practicalId, sessionId),
            now,
            expiresAt);

        // Пуш до персиста — неудачный запуск не тратит попытку (спека E4.5).
        await pushClient.PushAsync(
            ReadSessionsEndpoint(module),
            module.Slug,
            ToPush(session, binding.ExternalTaskRef, identityUserId),
            cancellationToken);
        await sessionsRepository.AddAsync(session, cancellationToken);

        var newLaunchUrl = await BuildLaunchUrlAsync(module, session, cancellationToken);
        return new StartModuleSessionResult(
            session.Id, newLaunchUrl, session.ExpiresAt, session.TryNumber, Resumed: false);
    }

    public async Task<ModuleSessionStatus?> GetStatusAsync(
        Guid practicalId,
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        var session = await sessionsRepository.GetByIdAsync(sessionId, cancellationToken);
        if (session is null || session.UserId != userId)
        {
            return null;
        }

        await LazyExpireAsync(session, cancellationToken);
        return ToStatus(session);
    }

    public async Task<CurrentModuleSessionInfo> GetCurrentAsync(
        Guid practicalId,
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        var binding = await practicalsRepository.GetExternalTaskBindingAsync(practicalId, taskId, cancellationToken)
            ?? throw new ExternalTaskNotFoundException();

        var latest = await sessionsRepository.GetLatestForUserTaskAsync(userId, taskId, cancellationToken);
        if (latest is not null)
        {
            await LazyExpireAsync(latest, cancellationToken);
        }

        var attemptsCount = await sessionsRepository.CountForUserTaskAsync(userId, taskId, cancellationToken);
        var bestGrade = await sessionsRepository.BestGradeForUserTaskAsync(userId, taskId, cancellationToken);

        return new CurrentModuleSessionInfo(
            latest is null ? null : ToStatus(latest),
            attemptsCount,
            binding.TriesCount,
            binding.TimeLimitMinutes,
            bestGrade);
    }

    public async Task<AbandonOutcome> AbandonAsync(
        Guid practicalId,
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        var session = await sessionsRepository.GetByIdAsync(sessionId, cancellationToken);
        if (session is null || session.UserId != userId)
        {
            return AbandonOutcome.NotFound;
        }

        if (!session.IsActive)
        {
            return AbandonOutcome.AlreadyTerminal;
        }

        session.Expire(timeProvider.GetUtcNow(), ModuleSessionEndReason.Abandoned);
        await sessionsRepository.SaveChangesAsync(cancellationToken);
        return AbandonOutcome.Abandoned;
    }

    public async Task<IReadOnlyList<ModuleSessionSummary>?> ListForPracticalAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default)
    {
        var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await practicalsRepository.IsPracticalOwnerAsync(practicalId, userId, cancellationToken))
        {
            return null;
        }

        return await sessionsRepository.ListForPracticalAsync(practicalId, cancellationToken);
    }

    public async Task<IReadOnlyList<ModuleSessionEvent>?> GetEventsAsync(
        Guid practicalId,
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        var session = await sessionsRepository.GetByIdAsync(sessionId, cancellationToken);
        if (session is null)
        {
            return null;
        }

        if (!await practicalsRepository.IsTaskInPracticalAsync(session.PracticalTaskId, practicalId, cancellationToken))
        {
            return null;
        }

        var isOwner = session.UserId == userId;
        var isTeacher = await practicalsRepository.IsPracticalOwnerAsync(practicalId, userId, cancellationToken);
        if (!isOwner && !isTeacher)
        {
            return null;
        }

        var events = await eventsRepository.GetForSessionAsync(sessionId, cancellationToken);
        return events
            .Select(taskEvent => new ModuleSessionEvent(
                taskEvent.Id, taskEvent.Kind, taskEvent.OccurredAt, taskEvent.Payload))
            .ToList();
    }

    public async Task<CompleteOutcome> CompleteAsync(
        Guid sessionId,
        string sessionKey,
        int grade,
        string? completionData,
        CancellationToken cancellationToken = default)
    {
        var session = await sessionsRepository.GetByIdAsync(sessionId, cancellationToken);
        if (session is null || session.SessionKey != sessionKey)
        {
            return CompleteOutcome.Unauthorized;
        }

        var now = timeProvider.GetUtcNow();
        if (session.IsExpiredBy(now))
        {
            session.Expire(now, ModuleSessionEndReason.Timeout);
            await sessionsRepository.SaveChangesAsync(cancellationToken);
        }

        switch (session.Status)
        {
            case ModuleSessionState.Completed:
                return CompleteOutcome.AlreadyCompleted;
            case ModuleSessionState.Expired:
                return CompleteOutcome.SessionClosed;
            default:
                session.Complete(now, Math.Clamp(grade, 0, 100), completionData);
                await sessionsRepository.SaveChangesAsync(cancellationToken);
                return CompleteOutcome.Accepted;
        }
    }

    private async Task LazyExpireAsync(PracticalModuleSession session, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        if (!session.IsExpiredBy(now))
        {
            return;
        }

        session.Expire(now, ModuleSessionEndReason.Timeout);
        await sessionsRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<string> PushAndBuildLaunchUrlAsync(
        PracticalModule module,
        PracticalModuleSession session,
        string taskRef,
        Guid identityUserId,
        CancellationToken cancellationToken)
    {
        await pushClient.PushAsync(
            ReadSessionsEndpoint(module),
            module.Slug,
            ToPush(session, taskRef, identityUserId),
            cancellationToken);
        return await BuildLaunchUrlAsync(module, session, cancellationToken);
    }

    private async Task<string> BuildLaunchUrlAsync(
        PracticalModule module,
        PracticalModuleSession session,
        CancellationToken cancellationToken)
    {
        var exchanged = await tokenExchangeClient.ExchangeAsync(
            module.IdentityAudience, session.Id, session.ExpiresAt, cancellationToken);
        return integrationConfig.BuildLaunchUrl(module.BasePath, session.Id, exchanged.AccessToken);
    }

    private static ModuleSessionPush ToPush(
        PracticalModuleSession session, string taskRef, Guid identityUserId) => new(
        session.Id,
        session.SessionKey,
        identityUserId,
        taskRef,
        session.ReturnUrl,
        session.ExpiresAt);

    private static string ReadSessionsEndpoint(PracticalModule module)
    {
        try
        {
            using var document = JsonDocument.Parse(module.Configuration);
            if (document.RootElement.TryGetProperty("sessionsEndpoint", out var endpoint)
                && endpoint.ValueKind == JsonValueKind.String
                && !String.IsNullOrWhiteSpace(endpoint.GetString()))
            {
                return endpoint.GetString()!;
            }
        }
        catch (JsonException)
        {
            // ниже — общее сообщение
        }

        throw new ModulePushFailedException(
            $"У модуля '{module.Slug}' не задан configuration.sessionsEndpoint.");
    }

    private static string GenerateSessionKey()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static ModuleSessionStatus ToStatus(PracticalModuleSession session) => new(
        session.Id,
        session.Status,
        session.TryNumber,
        session.StartedAt,
        session.ExpiresAt,
        session.EndReason,
        session.Grade,
        session.EndedAt);
}
