using Education.Domain.Common;

namespace Education.Domain.Practicals;

public sealed class Case : Entity
{
    public string Name { get; private set; } = String.Empty;
    public string Text { get; private set; } = String.Empty;
    public Guid PracticalMaterialId { get; private set; }
    public PracticalMaterial PracticalMaterial { get; private set; } = null!;
    public List<CaseFile> CaseFiles { get; private set; } = [];

    private Case()
    {
    }

    public Case(Guid practicalMaterialId, string name, string text)
    {
        PracticalMaterialId = practicalMaterialId;
        Name = name;
        Text = text;
    }

    public void UpdateText(string text) => Text = text;
}

