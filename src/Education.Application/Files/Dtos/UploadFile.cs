namespace Education.Application.Files;

/// <summary>
/// Описывает файл, переданный клиентом для сохранения в хранилище.
/// </summary>
/// <param name="OriginalFileName">Исходное имя файла, полученное от клиента.</param>
/// <param name="ContentType">MIME-тип файла, полученный от клиента.</param>
/// <param name="Length">Размер файла в байтах.</param>
/// <param name="Content">Поток с содержимым файла.</param>
public sealed record UploadFile(string OriginalFileName, string ContentType, long Length, Stream Content);

