namespace Education.Contracts;

public static partial class ApiRoutes
{
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
}
