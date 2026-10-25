using Education.Application.Identity;
using Education.Domain.Users;

namespace Education.Application.Users;

/// <summary>Правила сборки учебного профиля из claims токена Identity.</summary>
public static class ProfileClaims
{
    /// <summary>Значение, которым заполняются неизвестные части ФИО (так сложилось в существующих данных).</summary>
    public const string Unknown = "-";

    /// <summary>Роль учебной системы по ролям токена: Admin, затем Teacher, иначе Student.</summary>
    public static Guid ResolveRoleId(IReadOnlySet<string> roles)
    {
        if (roles.Contains("Admin"))
        {
            return RoleIds.Admin;
        }

        return roles.Contains("Teacher") ? RoleIds.Teacher : RoleIds.Student;
    }

    /// <summary>Логин: email; без email — <c>identity-&lt;первые 8 символов id&gt;</c>.</summary>
    public static string ResolveLogin(string? email, Guid userId) =>
        String.IsNullOrWhiteSpace(email) ? "identity-" + userId.ToString("N")[..8] : email.Trim();

    /// <summary>
    /// ФИО из claims. Структурированные <c>given_name</c>/<c>family_name</c>/<c>middle_name</c> точны; если их нет
    /// (старые аккаунты), строка <c>name</c> разбирается как «Имя Фамилия [Отчество]».
    /// </summary>
    public static ProfileFromClaims FromCurrentUser(ICurrentUser user)
    {
        var login = ResolveLogin(user.Email, user.UserId);
        var given = Clean(user.GivenName);
        var family = Clean(user.FamilyName);
        var middle = Clean(user.MiddleName);
        if (given is not null || family is not null)
        {
            return new ProfileFromClaims(login, given ?? Unknown, family ?? Unknown, middle ?? Unknown, true);
        }

        var name = Clean(user.Name);
        if (name is not null && name.Contains('@'))
        {
            name = null; // Identity подставляет email, когда отображаемого имени нет.
        }

        var words = (name ?? String.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return words.Length switch
        {
            0 => new ProfileFromClaims(login, Unknown, LocalPart(user.Email) ?? Unknown, Unknown, false),
            1 => new ProfileFromClaims(login, Unknown, words[0], Unknown, false),
            2 => new ProfileFromClaims(login, words[0], words[1], Unknown, false),
            _ => new ProfileFromClaims(login, words[0], words[1], String.Join(' ', words.Skip(2)), false),
        };
    }

    private static string? Clean(string? value) => String.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? LocalPart(string? email)
    {
        var trimmed = Clean(email);
        if (trimmed is null)
        {
            return null;
        }

        var at = trimmed.IndexOf('@');
        return at > 0 ? trimmed[..at] : trimmed;
    }
}
