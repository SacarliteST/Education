using Education.Domain.Common;

namespace Education.Domain.Practicals;

public sealed class CaseFileComment : Entity
{
    public string Text { get; private set; } = String.Empty;
    public bool IsGenerated { get; private set; }
    public DateTime Created { get; private set; } = DateTime.UtcNow;
    public long CaseFileId { get; private set; }
    public CaseFile CaseFile { get; private set; } = null!;

    private CaseFileComment()
    {
    }

    public CaseFileComment(long caseFileId, string text, bool isGenerated)
    {
        CaseFileId = caseFileId;
        Text = text;
        IsGenerated = isGenerated;
    }
}
