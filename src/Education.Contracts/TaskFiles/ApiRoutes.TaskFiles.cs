namespace Education.Contracts;

public static partial class ApiRoutes
{
    /// <summary>
    /// Шаблоны маршрутов для работы с файлами сдачи заданий.
    /// </summary>
    public static class TaskFiles
    {
        /// <summary>
        /// Маршрут файла сдачи текущего студента по заданию.
        /// </summary>
        public const string StudentTaskFile = PrefixV1 + "/tasks/{taskId:guid}/file";

        /// <summary>
        /// Маршрут файлов сдачи указанного задания для преподавателя.
        /// </summary>
        public const string TaskFilesByTask = PrefixV1 + "/tasks/{taskId:guid}/files";

        /// <summary>
        /// Маршрут файлов сдачи практического материала для преподавателя.
        /// </summary>
        public const string PracticalTaskFiles = Practicals.Practical + "/task-files";

        /// <summary>
        /// Шаблон маршрута файла сдачи по идентификатору.
        /// </summary>
        public const string TaskFile = PrefixV1 + "/task-files/{taskFileId:guid}";

        /// <summary>
        /// Шаблон маршрута комментариев файла сдачи.
        /// </summary>
        public const string Comments = TaskFile + "/comments";

        /// <summary>
        /// Шаблон маршрута принятия файла сдачи.
        /// </summary>
        public const string Accept = TaskFile + "/accept";

        /// <summary>
        /// Создаёт маршрут файла сдачи текущего студента по заданию.
        /// </summary>
        public static string ForStudentTaskFile(Guid taskId) => ReplaceUrlSegment(
            StudentTaskFile,
            "taskId:guid",
            taskId.ToString());

        /// <summary>
        /// Создаёт маршрут файлов сдачи указанного задания.
        /// </summary>
        public static string ForTaskFilesByTask(Guid taskId) => ReplaceUrlSegment(
            TaskFilesByTask,
            "taskId:guid",
            taskId.ToString());

        /// <summary>
        /// Создаёт маршрут файлов сдачи практического материала.
        /// </summary>
        public static string ForPracticalTaskFiles(Guid practicalId) => ReplaceUrlSegment(
            PracticalTaskFiles,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут комментариев файла сдачи.
        /// </summary>
        public static string ForComments(Guid taskFileId) => ReplaceUrlSegment(
            Comments,
            "taskFileId:guid",
            taskFileId.ToString());

        /// <summary>
        /// Создаёт маршрут принятия файла сдачи.
        /// </summary>
        public static string ForAccept(Guid taskFileId) => ReplaceUrlSegment(
            Accept,
            "taskFileId:guid",
            taskFileId.ToString());
    }
}
