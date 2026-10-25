namespace Education.Contracts;

public static partial class ApiRoutes
{
    /// <summary>
    /// Шаблоны маршрутов для работы с модулями.
    /// </summary>
    public static class Modules
    {
        /// <summary>
        /// Маршрут коллекции модулей.
        /// </summary>
        public const string ModulesList = PrefixV1 + "/modules";

        /// <summary>
        /// Шаблон маршрута модуля по идентификатору.
        /// </summary>
        public const string Module = ModulesList + "/{moduleId:guid}";

        /// <summary>
        /// Шаблон маршрута теоретических материалов модуля.
        /// </summary>
        public const string ModuleTheories = Module + "/theories";

        /// <summary>
        /// Шаблон маршрута практических материалов модуля.
        /// </summary>
        public const string ModulePracticals = Module + "/practicals";

        /// <summary>
        /// Шаблон маршрута вопросов модуля.
        /// </summary>
        public const string ModuleQuestions = Module + "/questions";

        /// <summary>
        /// Создаёт маршрут для конкретного модуля.
        /// </summary>
        public static string ForModule(Guid moduleId) => ReplaceUrlSegment(Module, "moduleId:guid", moduleId.ToString());

        /// <summary>
        /// Создаёт маршрут для теоретических материалов конкретного модуля.
        /// </summary>
        public static string ForModuleTheories(Guid moduleId) => ReplaceUrlSegment(
            ModuleTheories,
            "moduleId:guid",
            moduleId.ToString());

        /// <summary>
        /// Создаёт маршрут для практических материалов конкретного модуля.
        /// </summary>
        public static string ForModulePracticals(Guid moduleId) => ReplaceUrlSegment(
            ModulePracticals,
            "moduleId:guid",
            moduleId.ToString());

        /// <summary>
        /// Создаёт маршрут для вопросов конкретного модуля.
        /// </summary>
        public static string ForModuleQuestions(Guid moduleId) => ReplaceUrlSegment(
            ModuleQuestions,
            "moduleId:guid",
            moduleId.ToString());
    }
}
