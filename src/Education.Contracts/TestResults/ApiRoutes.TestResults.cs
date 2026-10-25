namespace Education.Contracts;

public static partial class ApiRoutes
{
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
}
