using System.Text.Json.Serialization;

namespace Education.Contracts.AdminProfiles;

/// <summary>
/// Роль, назначаемая учебному профилю при связывании с identity-пользователем.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<ProfileRole>))]
public enum ProfileRole
{
    /// <summary>Студент. Только такие профили попадают в списки назначения на курс/практику.</summary>
    Student,

    /// <summary>Преподаватель.</summary>
    Teacher,

    /// <summary>Администратор.</summary>
    Admin,
}
