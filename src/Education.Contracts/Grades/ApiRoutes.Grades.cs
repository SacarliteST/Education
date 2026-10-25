namespace Education.Contracts;

public static partial class ApiRoutes
{
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

        /// <summary>
        /// Шаблон маршрута оценок всех студентов по практике (для преподавателя).
        /// </summary>
        public const string PracticalStudentGrades = Practicals.Practical + "/student-grades";

        /// <summary>
        /// Создаёт маршрут оценок студентов по практике.
        /// </summary>
        public static string ForPracticalStudentGrades(Guid practicalId) => ReplaceUrlSegment(
            PracticalStudentGrades,
            "practicalId:guid",
            practicalId.ToString());

        /// <summary>
        /// Шаблон маршрута ведомости курса (студенты × практики, для преподавателя).
        /// </summary>
        public const string CourseGradebook = Courses.Course + "/gradebook";

        /// <summary>
        /// Создаёт маршрут ведомости курса.
        /// </summary>
        public static string ForCourseGradebook(Guid courseId) => ReplaceUrlSegment(
            CourseGradebook,
            "courseId:guid",
            courseId.ToString());
    }
}
