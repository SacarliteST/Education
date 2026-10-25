namespace Education.Contracts;

public static partial class ApiRoutes
{
    /// <summary>
    /// Шаблоны маршрутов для работы с заданиями практических материалов.
    /// </summary>
    public static class Tasks
    {
        /// <summary>
        /// Шаблон маршрута задания по идентификатору.
        /// </summary>
        public const string Task = PrefixV1 + "/tasks/{taskId:guid}";

        /// <summary>
        /// Шаблон маршрута обновления текста задания.
        /// </summary>
        public const string TaskText = Task + "/text";

        /// <summary>
        /// Создаёт маршрут для конкретного задания.
        /// </summary>
        public static string ForTask(Guid taskId) => ReplaceUrlSegment(Task, "taskId:guid", taskId.ToString());

        /// <summary>
        /// Создаёт маршрут обновления текста конкретного задания.
        /// </summary>
        public static string ForTaskText(Guid taskId) => ReplaceUrlSegment(TaskText, "taskId:guid", taskId.ToString());
    }
}
