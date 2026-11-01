namespace Education.Contracts;

public static partial class ApiRoutes
{
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
        /// Шаблон маршрута списка учебных групп студентов (для преподавателя и администратора).
        /// </summary>
        public const string StudentGroups = PrefixV1 + "/student-groups";

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
        /// Шаблон маршрута постраничного списка студентов для назначения на курс (поиск, фильтр, пагинация).
        /// </summary>
        public const string CourseStudentAssignments = Courses.Course + "/student-assignments";

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
        /// Создаёт маршрут постраничного списка студентов для назначения на курс.
        /// </summary>
        public static string ForCourseStudentAssignments(Guid courseId) => ReplaceUrlSegment(
            CourseStudentAssignments,
            "courseId:guid",
            courseId.ToString());
    }
}
