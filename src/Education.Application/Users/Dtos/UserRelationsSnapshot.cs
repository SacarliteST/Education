namespace Education.Application.Users;

public sealed record UserRelationsSnapshot(
    Guid LegacyUserId,
    IReadOnlySet<Guid> OwnedCourseIds,
    IReadOnlySet<Guid> AssignedCourseIds,
    IReadOnlySet<Guid> AssignedPracticalIds,
    IReadOnlySet<Guid> CaseFileIds,
    IReadOnlySet<Guid> TestResultIds);

