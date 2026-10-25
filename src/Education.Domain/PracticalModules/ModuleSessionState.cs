namespace Education.Domain.PracticalModules;

/// <summary>Состояние попытки внешнего модуля.</summary>
public enum ModuleSessionState
{
    /// <summary>Идёт: принимает работу студента.</summary>
    Active = 0,

    /// <summary>Завершена: пришла оценка от модуля.</summary>
    Completed = 1,

    /// <summary>Истекла по времени или прервана студентом.</summary>
    Expired = 2,
}
