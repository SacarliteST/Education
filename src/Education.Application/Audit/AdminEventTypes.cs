namespace Education.Application.Audit;

/// <summary>Типы событий журнала административных действий Education.</summary>
public static class AdminEventTypes
{
    /// <summary>Профиль связан с identity-пользователем.</summary>
    public const string ProfileLinked = "ProfileLinked";

    /// <summary>Связь профиля с identity-пользователем деактивирована.</summary>
    public const string ProfileUnlinked = "ProfileUnlinked";

    /// <summary>Зарегистрирован внешний практический модуль.</summary>
    public const string ModuleRegistered = "ModuleRegistered";

    /// <summary>Изменены параметры внешнего практического модуля.</summary>
    public const string ModuleUpdated = "ModuleUpdated";

    /// <summary>Удалён внешний практический модуль.</summary>
    public const string ModuleDeleted = "ModuleDeleted";

    /// <summary>Все известные типы событий.</summary>
    public static readonly IReadOnlyList<string> All =
    [
        ProfileLinked,
        ProfileUnlinked,
        ModuleRegistered,
        ModuleUpdated,
        ModuleDeleted,
    ];
}
