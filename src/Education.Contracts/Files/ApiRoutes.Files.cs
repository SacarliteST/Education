namespace Education.Contracts;

public static partial class ApiRoutes
{
    /// <summary>
    /// Шаблоны маршрутов для работы с файлами.
    /// </summary>
    public static class Files
    {
        /// <summary>
        /// Шаблон маршрута скачивания файла по серверному ключу.
        /// </summary>
        public const string Download = PrefixV1 + "/files/{**storageKey}";

        /// <summary>
        /// Создаёт маршрут скачивания файла.
        /// </summary>
        public static string ForDownload(string storageKey) => PrefixV1 + "/files/" + Uri.EscapeDataString(storageKey);
    }
}
