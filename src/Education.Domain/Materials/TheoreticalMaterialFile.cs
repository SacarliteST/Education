using Education.Domain.Common;

namespace Education.Domain.Materials;

public sealed class TheoreticalMaterialFile : Entity
{
    public string Description { get; private set; } = String.Empty;
    public string Path { get; private set; } = String.Empty;
    public long TheoreticalMaterialId { get; private set; }
    public TheoreticalMaterial TheoreticalMaterial { get; private set; } = null!;

    private TheoreticalMaterialFile()
    {
    }

    public TheoreticalMaterialFile(long theoreticalMaterialId, string description, string path)
    {
        TheoreticalMaterialId = theoreticalMaterialId;
        Description = description;
        Path = path;
    }
}
