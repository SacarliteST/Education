namespace Education.Infrastructure.Users;

/// <summary>Сборка отображаемого ФИО из частей; пустое имя — «—».</summary>
internal static class PersonNameFormatter
{
    /// <summary>Собирает ФИО в порядке «фамилия имя отчество», пропуская пустые части.</summary>
    /// <param name="lastName">Фамилия.</param>
    /// <param name="firstName">Имя.</param>
    /// <param name="middleName">Отчество.</param>
    /// <returns>Отображаемое ФИО или «—», если все части пусты.</returns>
    public static string Format(string? lastName, string? firstName, string? middleName)
    {
        var name = String.Join(' ', new[] { lastName, firstName, middleName }
            .Where(part => !String.IsNullOrWhiteSpace(part)));
        return String.IsNullOrWhiteSpace(name) ? "—" : name;
    }
}
