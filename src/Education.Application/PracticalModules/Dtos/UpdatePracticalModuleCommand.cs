namespace Education.Application.PracticalModules;

/// <summary>Команда изменения внешнего практического модуля.</summary>
/// <param name="Name">Название модуля.</param>
/// <param name="Description">Описание модуля.</param>
/// <param name="PracticeType">Тип практики, которую обслуживает модуль.</param>
/// <param name="BasePath">Базовый путь интерфейса модуля на платформе.</param>
/// <param name="IdentityAudience">Audience токена обмена для обращения к модулю.</param>
/// <param name="Configuration">Конфигурация модуля в формате JSON.</param>
/// <param name="IsEnabled">Модуль включён и доступен для запуска практик.</param>
public sealed record UpdatePracticalModuleCommand(
    string Name,
    string Description,
    string PracticeType,
    string BasePath,
    string IdentityAudience,
    string Configuration,
    bool IsEnabled);
