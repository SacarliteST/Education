namespace Education.Domain.Practicals;

/// <summary>Вид практики.</summary>
public enum PracticalKind
{
    /// <summary>Внутренняя: теория, тест и задания с загрузкой файлов на платформе.</summary>
    Internal = 0,

    /// <summary>Внешняя: прохождение в подключённом модуле, оценку ставит модуль.</summary>
    External = 1,
}
