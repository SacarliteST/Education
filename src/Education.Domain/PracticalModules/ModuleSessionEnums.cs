namespace Education.Domain.PracticalModules;

/// <summary>Строковые значения enum'ов сессии для БД и wire-формата.</summary>
public static class ModuleSessionEnums
{
    /// <summary>Enum → строка (<c>ACTIVE</c>/<c>COMPLETED</c>/<c>EXPIRED</c>).</summary>
    public static string ToWire(this ModuleSessionState state) => state switch
    {
        ModuleSessionState.Completed => "COMPLETED",
        ModuleSessionState.Expired => "EXPIRED",
        _ => "ACTIVE",
    };

    /// <summary>Строка → enum.</summary>
    public static ModuleSessionState ParseState(string? value) => value switch
    {
        "COMPLETED" => ModuleSessionState.Completed,
        "EXPIRED" => ModuleSessionState.Expired,
        _ => ModuleSessionState.Active,
    };

    /// <summary>Enum → строка (<c>completed</c>/<c>timeout</c>/<c>abandoned</c>).</summary>
    public static string ToWire(this ModuleSessionEndReason reason) => reason switch
    {
        ModuleSessionEndReason.Timeout => "timeout",
        ModuleSessionEndReason.Abandoned => "abandoned",
        _ => "completed",
    };

    /// <summary>Строка → enum (<see langword="null"/> сохраняется).</summary>
    public static ModuleSessionEndReason? ParseEndReason(string? value) => value switch
    {
        null => null,
        "timeout" => ModuleSessionEndReason.Timeout,
        "abandoned" => ModuleSessionEndReason.Abandoned,
        _ => ModuleSessionEndReason.Completed,
    };
}
