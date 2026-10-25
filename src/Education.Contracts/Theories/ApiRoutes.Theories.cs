namespace Education.Contracts;

public static partial class ApiRoutes
{
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
}
