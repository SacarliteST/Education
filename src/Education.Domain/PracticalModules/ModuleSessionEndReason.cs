namespace Education.Domain.PracticalModules;

/// <summary>Причина завершения попытки (для UI).</summary>
public enum ModuleSessionEndReason
{
    /// <summary>Модуль прислал оценку.</summary>
    Completed = 0,

    /// <summary>Истёк лимит времени / потолок.</summary>
    Timeout = 1,

    /// <summary>Студент нажал «Прервать попытку».</summary>
    Abandoned = 2,
}
