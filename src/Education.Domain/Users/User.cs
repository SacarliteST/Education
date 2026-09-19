using Education.Domain.Common;
using Education.Domain.Courses;
using Education.Domain.Practicals;

namespace Education.Domain.Users;

/// <summary>
/// Профиль пользователя учебной системы.
/// </summary>
public sealed class User : Entity
{
    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public string Login { get; private set; } = String.Empty;

    /// <summary>
    /// Пароль из легаси-модели пользователя.
    /// </summary>
    public string Password { get; private set; } = String.Empty;

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string FirstName { get; private set; } = String.Empty;

    /// <summary>
    /// Фамилия пользователя.
    /// </summary>
    public string LastName { get; private set; } = String.Empty;

    /// <summary>
    /// Отчество пользователя.
    /// </summary>
    public string MiddleName { get; private set; } = String.Empty;

    /// <summary>
    /// Учебная группа студента (например, «ИС-21»). Необязательна; для преподавателей и администраторов обычно пуста.
    /// </summary>
    public string? GroupName { get; private set; }

    /// <summary>
    /// Максимальная длина названия учебной группы.
    /// </summary>
    public const int GroupNameMaxLength = 50;

    /// <summary>
    /// Идентификатор роли пользователя.
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// Роль пользователя.
    /// </summary>
    public Role Role { get; private set; } = null!;

    /// <summary>
    /// Курсы, созданные пользователем.
    /// </summary>
    public List<Course> Courses { get; private set; } = [];

    /// <summary>
    /// Файлы практических заданий, загруженные пользователем.
    /// </summary>
    public List<CaseFile> CaseFiles { get; private set; } = [];

    /// <summary>
    /// Связи пользователя с доступными курсами.
    /// </summary>
    public List<CourseBindUser> CourseBindUsers { get; private set; } = [];

    /// <summary>
    /// Связи пользователя с доступными практическими материалами.
    /// </summary>
    public List<PracticalBindUser> PracticalBindUsers { get; private set; } = [];

    private User()
    {
    }

    /// <summary>
    /// Создает профиль пользователя.
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="firstName">Имя пользователя.</param>
    /// <param name="lastName">Фамилия пользователя.</param>
    /// <param name="middleName">Отчество пользователя.</param>
    /// <param name="roleId">Идентификатор роли пользователя.</param>
    /// <param name="groupName">Учебная группа; пустая строка приравнивается к отсутствию группы.</param>
    public User(string login, string firstName, string lastName, string middleName, Guid roleId, string? groupName = null)
    {
        Login = login;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        RoleId = roleId;
        GroupName = NormalizeGroupName(groupName);
    }

    /// <summary>
    /// Создает профиль студента для совместимости с легаси-сценариями.
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="firstName">Имя пользователя.</param>
    /// <param name="lastName">Фамилия пользователя.</param>
    /// <param name="middleName">Отчество пользователя.</param>
    /// <returns>Профиль пользователя с ролью студента.</returns>
    public static User CreateLegacyProfile(string login, string firstName, string lastName, string middleName)
    {
        return new User(login, firstName, lastName, middleName, RoleIds.Student);
    }

    /// <summary>
    /// Обновляет отображаемые данные пользователя.
    /// </summary>
    /// <param name="login">Новый логин пользователя.</param>
    /// <param name="firstName">Новое имя пользователя.</param>
    /// <param name="lastName">Новая фамилия пользователя.</param>
    /// <param name="middleName">Новое отчество пользователя.</param>
    public void UpdateDisplayName(string login, string firstName, string lastName, string middleName)
    {
        Login = login;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
    }

    /// <summary>
    /// Меняет учебную группу пользователя.
    /// </summary>
    /// <param name="groupName">Новая группа; пустая строка или <see langword="null"/> снимают группу.</param>
    public void ChangeGroup(string? groupName)
    {
        GroupName = NormalizeGroupName(groupName);
    }

    /// <summary>
    /// Приводит название группы к каноническому виду: без пробелов по краям, пустое значение — <see langword="null"/>.
    /// </summary>
    /// <param name="groupName">Исходное название группы.</param>
    /// <returns>Нормализованное название или <see langword="null"/>.</returns>
    public static string? NormalizeGroupName(string? groupName)
    {
        var trimmed = groupName?.Trim();
        return String.IsNullOrEmpty(trimmed) ? null : trimmed;
    }
}

