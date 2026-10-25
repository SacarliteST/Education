namespace Education.Contracts;

public static partial class ApiRoutes
{
    /// <summary>
    /// Шаблоны маршрутов для работы с практическими материалами.
    /// </summary>
    public static class Practicals
    {
        /// <summary>
        /// Маршрут коллекции практических материалов.
        /// </summary>
        public const string PracticalsList = PrefixV1 + "/practicals";

        /// <summary>
        /// Шаблон маршрута практического материала по идентификатору.
        /// </summary>
        public const string Practical = PracticalsList + "/{practicalId:guid}";

        /// <summary>
        /// Шаблон маршрута публикации практического материала.
        /// </summary>
        public const string Publish = Practical + "/publish";

        /// <summary>
        /// Шаблон маршрута настройки вопросов практического материала.
        /// </summary>
        public const string Questions = Practical + "/questions";

        /// <summary>
        /// Шаблон маршрута заданий практического материала.
        /// </summary>
        public const string Tasks = Practical + "/tasks";

        /// <summary>
        /// Шаблон маршрута привязки внешнего модуля к практике.
        /// </summary>
        public const string Module = Practical + "/module";

        /// <summary>
        /// Маршрут коллекции сессий внешнего модуля практики (POST — запуск/продолжение).
        /// </summary>
        public const string ModuleSessions = Practical + "/module-sessions";

        /// <summary>
        /// Маршрут гейта: последняя сессия студента по заданию.
        /// </summary>
        public const string ModuleSessionCurrent = ModuleSessions + "/current";

        /// <summary>
        /// Шаблон маршрута конкретной сессии внешнего модуля.
        /// </summary>
        public const string ModuleSession = ModuleSessions + "/{sessionId:guid}";

        /// <summary>
        /// Шаблон маршрута прерывания попытки.
        /// </summary>
        public const string ModuleSessionAbandon = ModuleSession + "/abandon";

        /// <summary>
        /// Шаблон маршрута ленты событий («цифрового следа») попытки.
        /// </summary>
        public const string ModuleSessionEvents = ModuleSession + "/events";

        /// <summary>
        /// Шаблон маршрута набора студентов, назначенных на практический материал.
        /// </summary>
        public const string Students = Practical + "/students";

        /// <summary>
        /// Шаблон маршрута точечного изменения набора студентов практического материала (добавить и убрать).
        /// </summary>
        public const string StudentChanges = Students + "/changes";

        /// <summary>
        /// Создаёт маршрут для конкретного практического материала.
        /// </summary>
        public static string ForPractical(Guid practicalId) => ReplaceUrlSegment(
            Practical,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут для настройки вопросов практического материала.
        /// </summary>
        public static string ForQuestions(Guid practicalId) => ReplaceUrlSegment(
            Questions,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут публикации практического материала.
        /// </summary>
        public static string ForPublish(Guid practicalId) => ReplaceUrlSegment(
            Publish,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут заданий практического материала.
        /// </summary>
        public static string ForTasks(Guid practicalId) => ReplaceUrlSegment(
            Tasks,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут привязки внешнего модуля к практике.
        /// </summary>
        public static string ForModule(Guid practicalId) => ReplaceUrlSegment(
            Module,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут коллекции сессий внешнего модуля практики.
        /// </summary>
        public static string ForModuleSessions(Guid practicalId) => ReplaceUrlSegment(
            ModuleSessions,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут гейта последней сессии.
        /// </summary>
        public static string ForModuleSessionCurrent(Guid practicalId) => ReplaceUrlSegment(
            ModuleSessionCurrent,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут конкретной сессии внешнего модуля.
        /// </summary>
        public static string ForModuleSession(Guid practicalId, Guid sessionId) => ReplaceUrlSegment(
                ModuleSession,
                "practicalId:guid",
                practicalId.ToString())
            .Replace("{sessionId:guid}", sessionId.ToString());

        /// <summary>
        /// Создаёт маршрут прерывания попытки.
        /// </summary>
        public static string ForModuleSessionAbandon(Guid practicalId, Guid sessionId) => ReplaceUrlSegment(
                ModuleSessionAbandon,
                "practicalId:guid",
                practicalId.ToString())
            .Replace("{sessionId:guid}", sessionId.ToString());

        /// <summary>
        /// Создаёт маршрут ленты событий попытки.
        /// </summary>
        public static string ForModuleSessionEvents(Guid practicalId, Guid sessionId) => ReplaceUrlSegment(
                ModuleSessionEvents,
                "practicalId:guid",
                practicalId.ToString())
            .Replace("{sessionId:guid}", sessionId.ToString());

        /// <summary>
        /// Создаёт маршрут набора студентов практического материала.
        /// </summary>
        public static string ForStudents(Guid practicalId) => ReplaceUrlSegment(
            Students,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут точечного изменения набора студентов практического материала.
        /// </summary>
        public static string ForStudentChanges(Guid practicalId) => ReplaceUrlSegment(
            StudentChanges,
            "practicalId:guid",
            practicalId.ToString());
    }
}
