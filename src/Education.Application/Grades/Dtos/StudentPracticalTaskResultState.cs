namespace Education.Application.Grades;

/// <summary>Сводное состояние прохождения одного задания внешнего модуля.</summary>
public enum StudentPracticalTaskResultState
{
    /// <summary>Задание ещё не запускалось.</summary>
    NotStarted = 0,

    /// <summary>Есть активная сессия прохождения.</summary>
    InProgress = 1,

    /// <summary>Модуль зафиксировал итоговый результат.</summary>
    Completed = 2,

    /// <summary>Запуски были, но итоговый результат не зафиксирован.</summary>
    Expired = 3,
}
