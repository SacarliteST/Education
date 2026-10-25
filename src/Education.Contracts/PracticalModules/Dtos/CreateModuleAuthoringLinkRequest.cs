namespace Education.Contracts.PracticalModules;

/// <summary>
/// Необязательные параметры перехода преподавателя в контур авторинга внешнего модуля.
/// Тело запроса можно не передавать — тогда модуль откроется на своей стартовой странице
/// без кнопки возврата на платформу.
/// </summary>
/// <param name="ReturnPath">
/// Путь на платформе, куда модуль вернёт преподавателя: относительный, начинается с
/// одного «/» (например, <c>/teacher/courses/…/practicals/…</c>). Абсолютные адреса запрещены.
/// </param>
/// <param name="TaskRef">
/// Ссылка на задание модуля (<c>externalTaskRef</c>), которое нужно открыть сразу;
/// допустимы латиница, цифры, «-» и «_».
/// </param>
public sealed record CreateModuleAuthoringLinkRequest(string? ReturnPath, string? TaskRef);
