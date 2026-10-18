namespace Education.Infrastructure.Users;

/// <summary>Сборка отображаемого ФИО из частей; пустое имя — «—».</summary>
internal static class PersonNameFormatter
{
    public static string Format(string? lastName, string? firstName, string? middleName)
    {
        var name = String.Join(' ', new[] { lastName, firstName, middleName }
            .Where(part => !String.IsNullOrWhiteSpace(part)));
        return String.IsNullOrWhiteSpace(name) ? "—" : name;
    }
}
