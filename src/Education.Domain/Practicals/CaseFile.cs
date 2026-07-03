using Education.Domain.Common;
using Education.Domain.Users;

namespace Education.Domain.Practicals;

public sealed class CaseFile : Entity
{
    public string Path { get; private set; } = String.Empty;
    public string OriginalFileName { get; private set; } = String.Empty;
    public Guid CaseId { get; private set; }
    public Case Case { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public bool IsAccepted { get; private set; }
    public int Grade { get; private set; }
    public List<CaseFileComment> Comments { get; private set; } = [];

    private CaseFile()
    {
    }

    public CaseFile(Guid caseId, Guid userId, string path)
        : this(caseId, userId, path, global::System.IO.Path.GetFileName(path))
    {
    }

    public CaseFile(Guid caseId, Guid userId, string path, string originalFileName)
    {
        CaseId = caseId;
        UserId = userId;
        Path = path;
        OriginalFileName = originalFileName;
    }

    public void ReplaceFile(string path)
    {
        ReplaceFile(path, global::System.IO.Path.GetFileName(path));
    }

    public void ReplaceFile(string path, string originalFileName)
    {
        if (IsAccepted)
        {
            throw new InvalidOperationException("Accepted task file cannot be replaced.");
        }

        Path = path;
        OriginalFileName = originalFileName;
    }

    public void Accept(int grade)
    {
        if (grade is < 2 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(grade), "Grade must be in range from 2 to 5.");
        }

        IsAccepted = true;
        Grade = grade;
    }
}

