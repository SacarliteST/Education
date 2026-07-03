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
    public User(string login, string firstName, string lastName, string middleName, Guid roleId)
    {
        Login = login;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        RoleId = roleId;
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
}

