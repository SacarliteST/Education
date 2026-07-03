namespace Education.Application.AdminProfiles;

/// <summary>
/// Возникает, когда профиль или identity-пользователь уже связаны активной связью.
/// </summary>
public sealed class AdminProfileAlreadyLinkedException : InvalidOperationException;

