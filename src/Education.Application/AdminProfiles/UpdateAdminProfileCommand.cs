namespace Education.Application.AdminProfiles;

/// <summary>Изменение учебных данных профиля: ФИО и роль приходят из Identity, здесь правится только группа.</summary>
public sealed record UpdateAdminProfileCommand(string? GroupName = null);
