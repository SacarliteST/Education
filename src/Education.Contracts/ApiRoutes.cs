namespace Education.Contracts;

/// <summary>
/// Централизованные шаблоны маршрутов API.
/// </summary>
public static class ApiRoutes
{
    /// <summary>
    /// Общий версионированный префикс API.
    /// </summary>
    public const string PrefixV1 = "api/v1";

    /// <summary>
    /// Шаблоны маршрутов для работы с курсами.
    /// </summary>
    public static class Courses
    {
        /// <summary>
        /// Маршрут коллекции курсов.
        /// </summary>
        public const string CoursesList = PrefixV1 + "/courses";

        /// <summary>
        /// Маршрут курсов текущего преподавателя.
        /// </summary>
        public const string TeacherCourses = CoursesList + "/teacher";

        /// <summary>
        /// Маршрут назначенных курсов текущего студента.
        /// </summary>
        public const string StudentCourses = CoursesList + "/student";

        /// <summary>
        /// Шаблон маршрута курса по идентификатору.
        /// </summary>
        public const string Course = CoursesList + "/{courseId:long}";

        /// <summary>
        /// Шаблон маршрута модулей курса.
        /// </summary>
        public const string CourseModules = Course + "/modules";

        /// <summary>
        /// Создаёт маршрут для конкретного курса.
        /// </summary>
        public static string ForCourse(long courseId) => ReplaceUrlSegment(Course, "courseId:long", courseId.ToString());

        /// <summary>
        /// Создаёт маршрут для модулей конкретного курса.
        /// </summary>
        public static string ForCourseModules(long courseId) => ReplaceUrlSegment(
            CourseModules,
            "courseId:long",
            courseId.ToString());
    }

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
        public const string Module = ModulesList + "/{moduleId:long}";

        /// <summary>
        /// Шаблон маршрута теоретических материалов модуля.
        /// </summary>
        public const string ModuleTheories = Module + "/theories";

        /// <summary>
        /// Создаёт маршрут для конкретного модуля.
        /// </summary>
        public static string ForModule(long moduleId) => ReplaceUrlSegment(Module, "moduleId:long", moduleId.ToString());

        /// <summary>
        /// Создаёт маршрут для теоретических материалов конкретного модуля.
        /// </summary>
        public static string ForModuleTheories(long moduleId) => ReplaceUrlSegment(
            ModuleTheories,
            "moduleId:long",
            moduleId.ToString());
    }

    /// <summary>
    /// Шаблоны маршрутов для работы с теоретическими материалами.
    /// </summary>
    public static class Theories
    {
        /// <summary>
        /// Маршрут коллекции теоретических материалов.
        /// </summary>
        public const string TheoriesList = PrefixV1 + "/theories";

        /// <summary>
        /// Шаблон маршрута теоретического материала по идентификатору.
        /// </summary>
        public const string Theory = TheoriesList + "/{theoryId:long}";

        /// <summary>
        /// Шаблон маршрута документов теоретического материала.
        /// </summary>
        public const string TheoryDocs = Theory + "/docs";

        /// <summary>
        /// Шаблон маршрута ссылок теоретического материала.
        /// </summary>
        public const string TheoryLinks = Theory + "/links";

        /// <summary>
        /// Шаблон маршрута обновления заголовка теоретического материала.
        /// </summary>
        public const string TheoryTitle = Theory + "/title";

        /// <summary>
        /// Шаблон маршрута обновления текста теоретического материала.
        /// </summary>
        public const string TheoryText = Theory + "/text";

        /// <summary>
        /// Маршрут коллекции документов теоретических материалов.
        /// </summary>
        public const string Docs = TheoriesList + "/docs";

        /// <summary>
        /// Шаблон маршрута документа теоретического материала по идентификатору.
        /// </summary>
        public const string Doc = Docs + "/{docId:long}";

        /// <summary>
        /// Маршрут коллекции ссылок теоретических материалов.
        /// </summary>
        public const string Links = TheoriesList + "/links";

        /// <summary>
        /// Шаблон маршрута ссылки теоретического материала по идентификатору.
        /// </summary>
        public const string Link = Links + "/{linkId:long}";

        /// <summary>
        /// Создаёт маршрут для конкретного теоретического материала.
        /// </summary>
        public static string ForTheory(long theoryId) => ReplaceUrlSegment(Theory, "theoryId:long", theoryId.ToString());

        /// <summary>
        /// Создаёт маршрут для документов конкретного теоретического материала.
        /// </summary>
        public static string ForTheoryDocs(long theoryId) => ReplaceUrlSegment(
            TheoryDocs,
            "theoryId:long",
            theoryId.ToString());

        /// <summary>
        /// Создаёт маршрут для ссылок конкретного теоретического материала.
        /// </summary>
        public static string ForTheoryLinks(long theoryId) => ReplaceUrlSegment(
            TheoryLinks,
            "theoryId:long",
            theoryId.ToString());

        /// <summary>
        /// Создаёт маршрут для конкретного документа теоретического материала.
        /// </summary>
        public static string ForDoc(long docId) => ReplaceUrlSegment(Doc, "docId:long", docId.ToString());

        /// <summary>
        /// Создаёт маршрут для конкретной ссылки теоретического материала.
        /// </summary>
        public static string ForLink(long linkId) => ReplaceUrlSegment(Link, "linkId:long", linkId.ToString());
    }

    private static string ReplaceUrlSegment(string template, string name, string value)
    {
        var escapedUri = Uri.EscapeDataString(value);
        return template.Replace('{' + name + '}', escapedUri);
    }
}
