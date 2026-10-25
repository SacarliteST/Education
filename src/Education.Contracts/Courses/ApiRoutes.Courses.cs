namespace Education.Contracts;

public static partial class ApiRoutes
{
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

        /// <summary>Пары курса: список и запуск.</summary>
        public const string ClassWindows = Course + "/class-windows";

        /// <summary>Режим «практика только на парах».</summary>
        public const string PracticeAccess = Course + "/practice-access";

        /// <summary>Состояние доступа студента к практике курса.</summary>
        public const string ClassAccess = Course + "/class-access";

        /// <summary>Продление пары.</summary>
        public const string ClassWindowExtend = PrefixV1 + "/class-windows/{windowId:guid}/extend";

        /// <summary>Досрочное завершение пары.</summary>
        public const string ClassWindowFinish = PrefixV1 + "/class-windows/{windowId:guid}/finish";

        /// <summary>
        /// Шаблон маршрута модулей курса.
        /// </summary>
        public const string CourseModules = Course + "/modules";

        /// <summary>
        /// Шаблон маршрута набора студентов, назначенных на курс.
        /// </summary>
        public const string CourseStudents = Course + "/students";

        /// <summary>
        /// Шаблон маршрута точечного изменения набора студентов курса (добавить и убрать).
        /// </summary>
        public const string CourseStudentChanges = CourseStudents + "/changes";

        /// <summary>
        /// Создаёт маршрут для конкретного курса.
        /// </summary>
        public static string ForCourse(Guid courseId) => ReplaceUrlSegment(Course, "courseId:guid", courseId.ToString());

        /// <summary>Маршрут пар курса.</summary>
        public static string ForClassWindows(Guid courseId) => ReplaceUrlSegment(ClassWindows, "courseId:guid", courseId.ToString());

        /// <summary>Маршрут режима «практика только на парах».</summary>
        public static string ForPracticeAccess(Guid courseId) => ReplaceUrlSegment(PracticeAccess, "courseId:guid", courseId.ToString());

        /// <summary>Маршрут состояния доступа студента.</summary>
        public static string ForClassAccess(Guid courseId) => ReplaceUrlSegment(ClassAccess, "courseId:guid", courseId.ToString());

        /// <summary>Маршрут продления пары.</summary>
        public static string ForClassWindowExtend(Guid windowId) => ReplaceUrlSegment(ClassWindowExtend, "windowId:guid", windowId.ToString());

        /// <summary>Маршрут завершения пары.</summary>
        public static string ForClassWindowFinish(Guid windowId) => ReplaceUrlSegment(ClassWindowFinish, "windowId:guid", windowId.ToString());

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

        /// <summary>
        /// Создаёт маршрут точечного изменения набора студентов конкретного курса.
        /// </summary>
        public static string ForCourseStudentChanges(Guid courseId) => ReplaceUrlSegment(
            CourseStudentChanges,
            "courseId:guid",
            courseId.ToString());
    }
}
