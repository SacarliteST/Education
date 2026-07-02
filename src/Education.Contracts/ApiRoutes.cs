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
        public static string ForModule(long moduleId) => ReplaceUrlSegment(Module, "moduleId:long", moduleId.ToString());

        /// <summary>
        /// Создаёт маршрут для теоретических материалов конкретного модуля.
        /// </summary>
        public static string ForModuleTheories(long moduleId) => ReplaceUrlSegment(
            ModuleTheories,
            "moduleId:long",
            moduleId.ToString());

        /// <summary>
        /// Создаёт маршрут для практических материалов конкретного модуля.
        /// </summary>
        public static string ForModulePracticals(long moduleId) => ReplaceUrlSegment(
            ModulePracticals,
            "moduleId:long",
            moduleId.ToString());

        /// <summary>
        /// Создаёт маршрут для вопросов конкретного модуля.
        /// </summary>
        public static string ForModuleQuestions(long moduleId) => ReplaceUrlSegment(
            ModuleQuestions,
            "moduleId:long",
            moduleId.ToString());
    }

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
        public const string Practical = PracticalsList + "/{practicalId:long}";

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
        /// Создаёт маршрут для конкретного практического материала.
        /// </summary>
        public static string ForPractical(long practicalId) => ReplaceUrlSegment(
            Practical,
            "practicalId:long",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут для настройки вопросов практического материала.
        /// </summary>
        public static string ForQuestions(long practicalId) => ReplaceUrlSegment(
            Questions,
            "practicalId:long",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут публикации практического материала.
        /// </summary>
        public static string ForPublish(long practicalId) => ReplaceUrlSegment(
            Publish,
            "practicalId:long",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут заданий практического материала.
        /// </summary>
        public static string ForTasks(long practicalId) => ReplaceUrlSegment(
            Tasks,
            "practicalId:long",
            practicalId.ToString());
    }

    /// <summary>
    /// Шаблоны маршрутов для работы с вопросами.
    /// </summary>
    public static class Questions
    {
        /// <summary>
        /// Маршрут коллекции вопросов.
        /// </summary>
        public const string QuestionsList = PrefixV1 + "/questions";

        /// <summary>
        /// Шаблон маршрута вопроса по идентификатору.
        /// </summary>
        public const string Question = QuestionsList + "/{questionId:long}";

        /// <summary>
        /// Создаёт маршрут для конкретного вопроса.
        /// </summary>
        public static string ForQuestion(long questionId) => ReplaceUrlSegment(
            Question,
            "questionId:long",
            questionId.ToString());
    }

    /// <summary>
    /// Шаблоны маршрутов для работы с результатами тестирования.
    /// </summary>
    public static class TestResults
    {
        /// <summary>
        /// Шаблон маршрута статуса тестирования практического материала.
        /// </summary>
        public const string Status = Practicals.Practical + "/test-status";

        /// <summary>
        /// Шаблон маршрута старта попытки тестирования.
        /// </summary>
        public const string Start = Practicals.Practical + "/test/start";

        /// <summary>
        /// Шаблон маршрута вопросов текущей попытки.
        /// </summary>
        public const string Questions = Practicals.Practical + "/test/questions";

        /// <summary>
        /// Шаблон маршрута отправки ответов.
        /// </summary>
        public const string Submit = Practicals.Practical + "/test/submit";

        /// <summary>
        /// Шаблон маршрута протоколов практического материала.
        /// </summary>
        public const string PracticalProtocols = Practicals.Practical + "/protocols";

        /// <summary>
        /// Шаблон маршрута протоколов практического материала для преподавателя.
        /// </summary>
        public const string TeacherPracticalProtocols = PracticalProtocols + "/teacher";

        /// <summary>
        /// Маршрут коллекции результатов тестирования.
        /// </summary>
        public const string ResultsList = PrefixV1 + "/test-results";

        /// <summary>
        /// Шаблон маршрута протокола результата тестирования.
        /// </summary>
        public const string Protocol = ResultsList + "/{testResultId:long}/protocol";

        /// <summary>
        /// Создаёт маршрут статуса тестирования практического материала.
        /// </summary>
        public static string ForStatus(long practicalId) => ReplaceUrlSegment(
            Status,
            "practicalId:long",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут старта попытки тестирования.
        /// </summary>
        public static string ForStart(long practicalId) => ReplaceUrlSegment(
            Start,
            "practicalId:long",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут вопросов текущей попытки.
        /// </summary>
        public static string ForQuestions(long practicalId) => ReplaceUrlSegment(
            Questions,
            "practicalId:long",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут отправки ответов.
        /// </summary>
        public static string ForSubmit(long practicalId) => ReplaceUrlSegment(
            Submit,
            "practicalId:long",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут протоколов практического материала.
        /// </summary>
        public static string ForPracticalProtocols(long practicalId) => ReplaceUrlSegment(
            PracticalProtocols,
            "practicalId:long",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут протоколов практического материала для преподавателя.
        /// </summary>
        public static string ForTeacherPracticalProtocols(long practicalId) => ReplaceUrlSegment(
            TeacherPracticalProtocols,
            "practicalId:long",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут протокола результата тестирования.
        /// </summary>
        public static string ForProtocol(long testResultId) => ReplaceUrlSegment(
            Protocol,
            "testResultId:long",
            testResultId.ToString());
    }

    /// <summary>
    /// Шаблоны маршрутов для работы с оценками.
    /// </summary>
    public static class Grades
    {
        /// <summary>
        /// Шаблон маршрута итоговой оценки за практический материал.
        /// </summary>
        public const string PracticalGrade = Practicals.Practical + "/grade";

        /// <summary>
        /// Создаёт маршрут итоговой оценки за практический материал.
        /// </summary>
        public static string ForPracticalGrade(long practicalId) => ReplaceUrlSegment(
            PracticalGrade,
            "practicalId:long",
            practicalId.ToString());
    }

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

    /// <summary>
    /// Шаблоны маршрутов для работы с файлами сдачи заданий.
    /// </summary>
    public static class TaskFiles
    {
        /// <summary>
        /// Маршрут файла сдачи текущего студента по заданию.
        /// </summary>
        public const string StudentTaskFile = PrefixV1 + "/tasks/{taskId:long}/file";

        /// <summary>
        /// Маршрут файлов сдачи указанного задания для преподавателя.
        /// </summary>
        public const string TaskFilesByTask = PrefixV1 + "/tasks/{taskId:long}/files";

        /// <summary>
        /// Маршрут файлов сдачи практического материала для преподавателя.
        /// </summary>
        public const string PracticalTaskFiles = Practicals.Practical + "/task-files";

        /// <summary>
        /// Шаблон маршрута файла сдачи по идентификатору.
        /// </summary>
        public const string TaskFile = PrefixV1 + "/task-files/{taskFileId:long}";

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
        public static string ForStudentTaskFile(long taskId) => ReplaceUrlSegment(
            StudentTaskFile,
            "taskId:long",
            taskId.ToString());

        /// <summary>
        /// Создаёт маршрут файлов сдачи указанного задания.
        /// </summary>
        public static string ForTaskFilesByTask(long taskId) => ReplaceUrlSegment(
            TaskFilesByTask,
            "taskId:long",
            taskId.ToString());

        /// <summary>
        /// Создаёт маршрут файлов сдачи практического материала.
        /// </summary>
        public static string ForPracticalTaskFiles(long practicalId) => ReplaceUrlSegment(
            PracticalTaskFiles,
            "practicalId:long",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут комментариев файла сдачи.
        /// </summary>
        public static string ForComments(long taskFileId) => ReplaceUrlSegment(
            Comments,
            "taskFileId:long",
            taskFileId.ToString());

        /// <summary>
        /// Создаёт маршрут принятия файла сдачи.
        /// </summary>
        public static string ForAccept(long taskFileId) => ReplaceUrlSegment(
            Accept,
            "taskFileId:long",
            taskFileId.ToString());
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
