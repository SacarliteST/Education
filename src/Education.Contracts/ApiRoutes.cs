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
        public const string Course = CoursesList + "/{courseId:guid}";

        /// <summary>
        /// Шаблон маршрута модулей курса.
        /// </summary>
        public const string CourseModules = Course + "/modules";

        /// <summary>
        /// Шаблон маршрута набора студентов, назначенных на курс.
        /// </summary>
        public const string CourseStudents = Course + "/students";

        /// <summary>
        /// Создаёт маршрут для конкретного курса.
        /// </summary>
        public static string ForCourse(Guid courseId) => ReplaceUrlSegment(Course, "courseId:guid", courseId.ToString());

        /// <summary>
        /// Создаёт маршрут для модулей конкретного курса.
        /// </summary>
        public static string ForCourseModules(Guid courseId) => ReplaceUrlSegment(
            CourseModules,
            "courseId:guid",
            courseId.ToString());

        /// <summary>
        /// Создаёт маршрут набора студентов конкретного курса.
        /// </summary>
        public static string ForCourseStudents(Guid courseId) => ReplaceUrlSegment(
            CourseStudents,
            "courseId:guid",
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
    }

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
        public const string Question = QuestionsList + "/{questionId:guid}";

        /// <summary>
        /// Создаёт маршрут для конкретного вопроса.
        /// </summary>
        public static string ForQuestion(Guid questionId) => ReplaceUrlSegment(
            Question,
            "questionId:guid",
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
        public const string Protocol = ResultsList + "/{testResultId:guid}/protocol";

        /// <summary>
        /// Создаёт маршрут статуса тестирования практического материала.
        /// </summary>
        public static string ForStatus(Guid practicalId) => ReplaceUrlSegment(
            Status,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут старта попытки тестирования.
        /// </summary>
        public static string ForStart(Guid practicalId) => ReplaceUrlSegment(
            Start,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут вопросов текущей попытки.
        /// </summary>
        public static string ForQuestions(Guid practicalId) => ReplaceUrlSegment(
            Questions,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут отправки ответов.
        /// </summary>
        public static string ForSubmit(Guid practicalId) => ReplaceUrlSegment(
            Submit,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут протоколов практического материала.
        /// </summary>
        public static string ForPracticalProtocols(Guid practicalId) => ReplaceUrlSegment(
            PracticalProtocols,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут протоколов практического материала для преподавателя.
        /// </summary>
        public static string ForTeacherPracticalProtocols(Guid practicalId) => ReplaceUrlSegment(
            TeacherPracticalProtocols,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Создаёт маршрут протокола результата тестирования.
        /// </summary>
        public static string ForProtocol(Guid testResultId) => ReplaceUrlSegment(
            Protocol,
            "testResultId:guid",
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
        public static string ForPracticalGrade(Guid practicalId) => ReplaceUrlSegment(
            PracticalGrade,
            "practicalId:guid",
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

    /// <summary>
    /// Шаблоны маршрутов для администрирования локальных учебных профилей.
    /// </summary>
    public static class AdminProfiles
    {
        /// <summary>
        /// Маршрут коллекции локальных учебных профилей.
        /// </summary>
        public const string ProfilesList = PrefixV1 + "/admin/profiles";

        /// <summary>
        /// Шаблон маршрута локального учебного профиля.
        /// </summary>
        public const string Profile = ProfilesList + "/{legacyUserId:guid}";

        /// <summary>
        /// Шаблон маршрута деактивации связи профиля с identity-сервисом.
        /// </summary>
        public const string Deactivate = Profile + "/deactivate";

        /// <summary>
        /// Шаблон маршрута студентов, доступных для назначения на курс.
        /// </summary>
        public const string CourseAssignableStudents = Courses.Course + "/assignable-students";

        /// <summary>
        /// Шаблон маршрута студентов, доступных для назначения на практический материал.
        /// </summary>
        public const string PracticalAssignableStudents = Practicals.Practical + "/assignable-students";

        /// <summary>
        /// Создаёт маршрут локального учебного профиля.
        /// </summary>
        public static string ForProfile(Guid legacyUserId) => ReplaceUrlSegment(
            Profile,
            "legacyUserId:guid",
            legacyUserId.ToString());

        /// <summary>
        /// Создаёт маршрут деактивации связи профиля с identity-сервисом.
        /// </summary>
        public static string ForDeactivate(Guid legacyUserId) => ReplaceUrlSegment(
            Deactivate,
            "legacyUserId:guid",
            legacyUserId.ToString());

        /// <summary>
        /// Создаёт маршрут студентов, доступных для назначения на курс.
        /// </summary>
        public static string ForCourseAssignableStudents(Guid courseId) => ReplaceUrlSegment(
            CourseAssignableStudents,
            "courseId:guid",
            courseId.ToString());

        /// <summary>
        /// Создаёт маршрут студентов, доступных для назначения на практический материал.
        /// </summary>
        public static string ForPracticalAssignableStudents(Guid practicalId) => ReplaceUrlSegment(
            PracticalAssignableStudents,
            "practicalId:guid",
            practicalId.ToString());
    }

    /// <summary>
    /// Шаблоны маршрутов журнала административных действий Education.
    /// </summary>
    public static class AdminEvents
    {
        /// <summary>
        /// Маршрут коллекции событий журнала.
        /// </summary>
        public const string EventsList = PrefixV1 + "/admin/events";
    }

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
        /// Шаблон маршрута приёма оценки от бэкенда модуля (сервер-сервер, <c>X-Service-Key</c>).
        /// </summary>
        public const string SessionComplete = PrefixV1 + "/module-sessions/{sessionId:guid}/complete";

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
        public const string Theory = TheoriesList + "/{theoryId:guid}";

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
        public const string Doc = Docs + "/{docId:guid}";

        /// <summary>
        /// Маршрут коллекции ссылок теоретических материалов.
        /// </summary>
        public const string Links = TheoriesList + "/links";

        /// <summary>
        /// Шаблон маршрута ссылки теоретического материала по идентификатору.
        /// </summary>
        public const string Link = Links + "/{linkId:guid}";

        /// <summary>
        /// Создаёт маршрут для конкретного теоретического материала.
        /// </summary>
        public static string ForTheory(Guid theoryId) => ReplaceUrlSegment(Theory, "theoryId:guid", theoryId.ToString());

        /// <summary>
        /// Создаёт маршрут для документов конкретного теоретического материала.
        /// </summary>
        public static string ForTheoryDocs(Guid theoryId) => ReplaceUrlSegment(
            TheoryDocs,
            "theoryId:guid",
            theoryId.ToString());

        /// <summary>
        /// Создаёт маршрут для ссылок конкретного теоретического материала.
        /// </summary>
        public static string ForTheoryLinks(Guid theoryId) => ReplaceUrlSegment(
            TheoryLinks,
            "theoryId:guid",
            theoryId.ToString());

        /// <summary>
        /// Создаёт маршрут для конкретного документа теоретического материала.
        /// </summary>
        public static string ForDoc(Guid docId) => ReplaceUrlSegment(Doc, "docId:guid", docId.ToString());

        /// <summary>
        /// Создаёт маршрут для конкретной ссылки теоретического материала.
        /// </summary>
        public static string ForLink(Guid linkId) => ReplaceUrlSegment(Link, "linkId:guid", linkId.ToString());
    }

    private static string ReplaceUrlSegment(string template, string name, string value)
    {
        var escapedUri = Uri.EscapeDataString(value);
        return template.Replace('{' + name + '}', escapedUri);
    }
}


