namespace Education.Contracts.ClassWindows;

/// <summary>Запрос продления пары.</summary>
/// <param name="Minutes">На сколько минут продлить (1–240).</param>
public sealed record ExtendClassWindowRequest(int Minutes);
