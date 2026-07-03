namespace Education.Application.Users;

public sealed class UnauthenticatedEducationUserException()
    : InvalidOperationException("Current education user is not authenticated.");

