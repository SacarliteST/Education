using Education.Domain.Common;
using Education.Domain.Users;

namespace Education.Domain.Practicals;

/// <summary>
/// Файл решения практического задания.
/// </summary>
public sealed class CaseFile : Entity
{
    /// <summary>
    /// Путь к сохраненному файлу.
    /// </summary>
    public string Path { get; private set; } = String.Empty;

    /// <summary>
    /// Исходное имя загруженного файла.
    /// </summary>
    public string OriginalFileName { get; private set; } = String.Empty;

    /// <summary>
    /// Идентификатор задания.
    /// </summary>
    public Guid CaseId { get; private set; }

    /// <summary>
    /// Задание, к которому относится файл.
    /// </summary>
    public Case Case { get; private set; } = null!;

    /// <summary>
    /// Идентификатор пользователя, загрузившего файл.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Пользователь, загрузивший файл.
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Признак принятого решения.
    /// </summary>
    public bool IsAccepted { get; private set; }

    /// <summary>
    /// Оценка за решение.
    /// </summary>
    public int Grade { get; private set; }

    /// <summary>
    /// Комментарии к файлу решения.
    /// </summary>
    public List<CaseFileComment> Comments { get; private set; } = [];

    private CaseFile()
    {
    }

    /// <summary>
    /// Создает файл решения.
    /// </summary>
    /// <param name="caseId">Идентификатор задания.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="path">Путь к сохраненному файлу.</param>
    public CaseFile(Guid caseId, Guid userId, string path)
        : this(caseId, userId, path, global::System.IO.Path.GetFileName(path))
    {
    }

    /// <summary>
    /// Создает файл решения с исходным именем файла.
    /// </summary>
    /// <param name="caseId">Идентификатор задания.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="path">Путь к сохраненному файлу.</param>
    /// <param name="originalFileName">Исходное имя загруженного файла.</param>
    public CaseFile(Guid caseId, Guid userId, string path, string originalFileName)
    {
        CaseId = caseId;
        UserId = userId;
        Path = path;
        OriginalFileName = originalFileName;
    }

    /// <summary>
    /// Заменяет файл решения.
    /// </summary>
    /// <param name="path">Новый путь к сохраненному файлу.</param>
    public void ReplaceFile(string path)
    {
        ReplaceFile(path, global::System.IO.Path.GetFileName(path));
    }

    /// <summary>
    /// Заменяет файл решения с обновлением исходного имени файла.
    /// </summary>
    /// <param name="path">Новый путь к сохраненному файлу.</param>
    /// <param name="originalFileName">Исходное имя загруженного файла.</param>
    public void ReplaceFile(string path, string originalFileName)
    {
        if (IsAccepted)
        {
            throw new InvalidOperationException("Accepted task file cannot be replaced.");
        }

        Path = path;
        OriginalFileName = originalFileName;
    }

    /// <summary>
    /// Принимает решение и выставляет оценку.
    /// </summary>
    /// <param name="grade">Оценка за решение.</param>
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

