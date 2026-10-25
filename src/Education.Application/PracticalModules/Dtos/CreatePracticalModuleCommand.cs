namespace Education.Application.PracticalModules;

/// <summary>Команда регистрации внешнего практического модуля.</summary>
/// <param name="Slug">Уникальный короткий идентификатор модуля.</param>
/// <param name="Name">Название модуля.</param>
/// <param name="Description">Описание модуля.</param>
/// <param name="PracticeType">Тип практики, которую обслуживает модуль.</param>
/// <param name="BasePath">Базовый путь интерфейса модуля на платформе.</param>
/// <param name="IdentityAudience">Audience токена обмена для обращения к модулю.</param>
/// <param name="Configuration">Конфигурация модуля в формате JSON.</param>
public sealed record CreatePracticalModuleCommand(
    string Slug,
    string Name,
    string Description,
    string PracticeType,
    string BasePath,
    string IdentityAudience,
    string Configuration);
