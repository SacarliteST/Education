namespace Education.Domain.Practicals;

/// <summary>Вид практики.</summary>
public enum PracticalKind
{
    /// <summary>Внутренняя: теория, тест и задания с загрузкой файлов на платформе.</summary>
    Internal = 0,

    /// <summary>Внешняя: прохождение в подключённом модуле, оценку ставит модуль.</summary>
    External = 1,
}

/// <summary>Строковые значения <see cref="PracticalKind"/> для БД и wire-формата.</summary>
public static class PracticalKinds
{
    /// <summary>Enum → строка (<c>internal</c>/<c>external</c>).</summary>
    public static string ToWire(this PracticalKind kind) =>
        kind == PracticalKind.External ? "external" : "internal";

    /// <summary>Строка → enum. Всё, кроме <c>external</c>, трактуется как <c>internal</c>.</summary>
    public static PracticalKind Parse(string? value) =>
        value == "external" ? PracticalKind.External : PracticalKind.Internal;
}
