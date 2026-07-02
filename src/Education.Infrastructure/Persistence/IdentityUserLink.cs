namespace Education.Infrastructure.Persistence;

public sealed class IdentityUserLink
{
    public long Id { get; set; }
    public long LegacyUserId { get; set; }
    public Guid IdentityUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
