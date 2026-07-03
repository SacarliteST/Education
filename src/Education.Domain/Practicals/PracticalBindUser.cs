using Education.Domain.Common;
using Education.Domain.Users;

namespace Education.Domain.Practicals;

public sealed class PracticalBindUser : Entity
{
    public Guid PracticalMaterialId { get; private set; }
    public PracticalMaterial PracticalMaterial { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    private PracticalBindUser()
    {
    }

    public PracticalBindUser(Guid practicalMaterialId, Guid userId)
    {
        PracticalMaterialId = practicalMaterialId;
        UserId = userId;
    }
}

