namespace Education.Application.PracticalModules;

/// <summary>Лимит запусков внешней практики исчерпан (учитываются и <c>EXPIRED</c>).</summary>
public sealed class TriesExhaustedException() : Exception("Попытки исчерпаны.");
