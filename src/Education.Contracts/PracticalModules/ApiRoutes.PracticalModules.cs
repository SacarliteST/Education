namespace Education.Contracts;

public static partial class ApiRoutes
{
    /// <summary>
    /// Шаблоны маршрутов реестра внешних практических модулей (администратор).
    /// </summary>
    public static class PracticalModules
    {
        /// <summary>
        /// Маршрут коллекции модулей.
        /// </summary>
        public const string ModulesList = PrefixV1 + "/admin/practical-modules";

        /// <summary>
        /// Шаблон маршрута конкретного модуля.
        /// </summary>
        public const string Module = ModulesList + "/{id:guid}";

        /// <summary>
        /// Маршрут списка включённых модулей для преподавателя (пикер привязки).
        /// </summary>
        public const string EnabledModulesList = PrefixV1 + "/practical-modules";

        /// <summary>
        /// Шаблон маршрута каталога заданий модуля (ядро проксирует ручку модуля).
        /// </summary>
        public const string ModuleTasks = PrefixV1 + "/practical-modules/{practicalModuleId:guid}/tasks";

        /// <summary>
        /// Шаблон маршрута SSO-ссылки преподавателя в контур авторинга модуля.
        /// </summary>
        public const string ModuleAuthoringLink =
            PrefixV1 + "/practical-modules/{practicalModuleId:guid}/authoring-link";

        /// <summary>
        /// Маршрут списка модулей с открытым банком заданий (студент).
        /// </summary>
        public const string TrainingModulesList = PrefixV1 + "/practical-modules/training";

        /// <summary>
        /// Шаблон маршрута ссылки входа студента в банк заданий модуля.
        /// </summary>
        public const string ModuleTrainingLink =
            PrefixV1 + "/practical-modules/{practicalModuleId:guid}/training-link";

        /// <summary>
        /// Шаблон маршрута приёма оценки от бэкенда модуля (сервер-сервер, <c>X-Service-Key</c>).
        /// </summary>
        public const string SessionComplete = PrefixV1 + "/module-sessions/{sessionId:guid}/complete";

        /// <summary>
        /// Создаёт маршрут ссылки входа студента в банк заданий модуля.
        /// </summary>
        public static string ForModuleTrainingLink(Guid practicalModuleId) =>
            ReplaceUrlSegment(ModuleTrainingLink, "practicalModuleId:guid", practicalModuleId.ToString());

        /// <summary>
        /// Создаёт маршрут приёма оценки от модуля.
        /// </summary>
        public static string ForSessionComplete(Guid sessionId) =>
            ReplaceUrlSegment(SessionComplete, "sessionId:guid", sessionId.ToString());

        /// <summary>
        /// Создаёт маршрут конкретного модуля.
        /// </summary>
        public static string ForModule(Guid id) => ReplaceUrlSegment(Module, "id:guid", id.ToString());

        /// <summary>
        /// Создаёт маршрут каталога заданий модуля.
        /// </summary>
        public static string ForModuleTasks(Guid practicalModuleId) =>
            ReplaceUrlSegment(ModuleTasks, "practicalModuleId:guid", practicalModuleId.ToString());

        /// <summary>
        /// Создаёт маршрут SSO-ссылки преподавателя в контур авторинга модуля.
        /// </summary>
        public static string ForModuleAuthoringLink(Guid practicalModuleId) =>
            ReplaceUrlSegment(ModuleAuthoringLink, "practicalModuleId:guid", practicalModuleId.ToString());
    }
}
