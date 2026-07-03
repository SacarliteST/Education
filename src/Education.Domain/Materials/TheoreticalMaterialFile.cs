using Education.Domain.Common;

namespace Education.Domain.Materials;

public sealed class TheoreticalMaterialFile : Entity
{
    public string Description { get; private set; } = String.Empty;
    public string Path { get; private set; } = String.Empty;
    public string OriginalFileName { get; private set; } = String.Empty;
    public Guid TheoreticalMaterialId { get; private set; }
    public TheoreticalMaterial TheoreticalMaterial { get; private set; } = null!;

    private TheoreticalMaterialFile()
    {
    }

    public TheoreticalMaterialFile(Guid theoreticalMaterialId, string description, string path)
        : this(theoreticalMaterialId, description, path, global::System.IO.Path.GetFileName(path))
    {
    }

    public TheoreticalMaterialFile(Guid theoreticalMaterialId, string description, string path, string originalFileName)
    {
        TheoreticalMaterialId = theoreticalMaterialId;
        Description = description;
        Path = path;
        OriginalFileName = originalFileName;
    }
}

