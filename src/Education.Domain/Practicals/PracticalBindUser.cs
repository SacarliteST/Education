using Education.Domain.Common;
using Education.Domain.Users;

namespace Education.Domain.Practicals;

public sealed class PracticalBindUser : Entity
{
    public long PracticalMaterialId { get; private set; }
    public PracticalMaterial PracticalMaterial { get; private set; } = null!;
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    private PracticalBindUser()
    {
    }

    public PracticalBindUser(long practicalMaterialId, long userId)
    {
        PracticalMaterialId = practicalMaterialId;
        UserId = userId;
    }
}
