namespace Education.Application.Users;

public sealed record UserRelationsSnapshot(
    long LegacyUserId,
    IReadOnlySet<long> OwnedCourseIds,
    IReadOnlySet<long> AssignedCourseIds,
    IReadOnlySet<long> AssignedPracticalIds,
    IReadOnlySet<long> CaseFileIds,
    IReadOnlySet<long> TestResultIds);
