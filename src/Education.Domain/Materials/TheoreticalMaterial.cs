using Education.Domain.Common;
using Education.Domain.Courses;

namespace Education.Domain.Materials;

public sealed class TheoreticalMaterial : Entity
{
    public string Name { get; private set; } = String.Empty;
    public string Text { get; private set; } = String.Empty;
    public Guid ModuleId { get; private set; }
    public Module Module { get; private set; } = null!;
    public List<TheoreticalMaterialFile> Files { get; private set; } = [];
    public List<TheoreticalMaterialLink> Links { get; private set; } = [];

    private TheoreticalMaterial()
    {
    }

    public TheoreticalMaterial(Guid moduleId, string name, string text)
    {
        ModuleId = moduleId;
        Name = name;
        Text = text;
    }

    public void Rename(string name) => Name = name;
    public void UpdateText(string text) => Text = text;
}

