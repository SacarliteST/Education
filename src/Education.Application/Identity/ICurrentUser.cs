namespace Education.Application.Identity;

public interface ICurrentUser
{
    Guid UserId { get; }
    string? Email { get; }
    string? Name { get; }
    IReadOnlySet<string> Roles { get; }
    bool IsAuthenticated { get; }
}
