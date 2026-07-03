using Education.Domain.Common;

namespace Education.Domain.Users;

public sealed class Role : Entity
{
    public string Name { get; private set; } = String.Empty;

    private Role()
    {
    }

    public Role(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}

