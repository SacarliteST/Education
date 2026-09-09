namespace Education.Application.AdminProfiles;

/// <summary>
/// Среди переданных на назначение идентификаторов есть такие, которым не соответствует
/// учебный профиль с активной связью с identity-сервисом.
/// </summary>
public sealed class UnknownStudentsException(IReadOnlyCollection<Guid> userIds)
    : Exception(Build(userIds))
{
    /// <summary>Идентификаторы, которые не удалось сопоставить с привязанным профилем.</summary>
    public IReadOnlyCollection<Guid> UserIds { get; } = userIds;

    private static string Build(IReadOnlyCollection<Guid> userIds) =>
        userIds.Count == 0
            ? "Среди переданных пользователей есть не привязанные к identity-сервису."
            : $"Пользователи не найдены или не привязаны к identity-сервису: {String.Join(", ", userIds)}.";
}
