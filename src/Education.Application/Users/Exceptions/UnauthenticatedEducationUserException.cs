namespace Education.Application.Users;

/// <summary>Текущий пользователь не аутентифицирован, учебного пользователя определить нельзя.</summary>
public sealed class UnauthenticatedEducationUserException()
    : InvalidOperationException("Current education user is not authenticated.");

