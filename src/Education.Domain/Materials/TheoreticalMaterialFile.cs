using Education.Domain.Common;

namespace Education.Domain.Materials;

/// <summary>
/// Файл, прикрепленный к теоретическому материалу.
/// </summary>
public sealed class TheoreticalMaterialFile : Entity
{
    /// <summary>
    /// Описание файла.
    /// </summary>
    public string Description { get; private set; } = String.Empty;

    /// <summary>
    /// Путь к сохраненному файлу.
    /// </summary>
    public string Path { get; private set; } = String.Empty;

    /// <summary>
    /// Исходное имя загруженного файла.
    /// </summary>
    public string OriginalFileName { get; private set; } = String.Empty;

    /// <summary>
    /// Идентификатор теоретического материала.
    /// </summary>
    public Guid TheoreticalMaterialId { get; private set; }

    /// <summary>
    /// Теоретический материал, к которому прикреплен файл.
    /// </summary>
    public TheoreticalMaterial TheoreticalMaterial { get; private set; } = null!;

    private TheoreticalMaterialFile()
    {
    }

    /// <summary>
    /// Создает файл теоретического материала.
    /// </summary>
    /// <param name="theoreticalMaterialId">Идентификатор теоретического материала.</param>
    /// <param name="description">Описание файла.</param>
    /// <param name="path">Путь к сохраненному файлу.</param>
    public TheoreticalMaterialFile(Guid theoreticalMaterialId, string description, string path)
        : this(theoreticalMaterialId, description, path, global::System.IO.Path.GetFileName(path))
    {
    }

    /// <summary>
    /// Создает файл теоретического материала с исходным именем.
    /// </summary>
    /// <param name="theoreticalMaterialId">Идентификатор теоретического материала.</param>
    /// <param name="description">Описание файла.</param>
    /// <param name="path">Путь к сохраненному файлу.</param>
    /// <param name="originalFileName">Исходное имя загруженного файла.</param>
    public TheoreticalMaterialFile(Guid theoreticalMaterialId, string description, string path, string originalFileName)
    {
        TheoreticalMaterialId = theoreticalMaterialId;
        Description = description;
        Path = path;
        OriginalFileName = originalFileName;
    }
}

