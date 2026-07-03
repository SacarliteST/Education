using Education.Domain.Common;

namespace Education.Domain.Materials;

public sealed class TheoreticalMaterialLink : Entity
{
    public string Description { get; private set; } = String.Empty;
    public string Link { get; private set; } = String.Empty;
    public Guid TheoreticalMaterialId { get; private set; }
    public TheoreticalMaterial TheoreticalMaterial { get; private set; } = null!;

    private TheoreticalMaterialLink()
    {
    }

    public TheoreticalMaterialLink(Guid theoreticalMaterialId, string description, string link)
    {
        TheoreticalMaterialId = theoreticalMaterialId;
        Description = description;
        Link = link;
    }
}

