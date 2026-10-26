using System.Text.Json;

namespace Education.Application.PracticalModules;

/// <summary>Чтение настройки банка заданий из конфигурации модуля: <c>{ "trainingBank": { "enabled": true, "launchPath": "/student/launch" } }</c>.</summary>
public static class TrainingBankConfiguration
{
    /// <summary>Путь входа в SPA модуля по умолчанию.</summary>
    public const string DefaultLaunchPath = "/student/launch";

    /// <summary>Пытается прочитать настройку банка.</summary>
    /// <param name="configuration">JSON конфигурации модуля.</param>
    /// <param name="launchPath">Относительный путь входа в SPA модуля, если банк включён.</param>
    /// <returns><see langword="true"/>, если банк включён и путь входа корректен.</returns>
    public static bool TryRead(string configuration, out string launchPath)
    {
        launchPath = DefaultLaunchPath;
        try
        {
            using var document = JsonDocument.Parse(configuration);
            if (!document.RootElement.TryGetProperty("trainingBank", out var bank)
                || bank.ValueKind != JsonValueKind.Object
                || !bank.TryGetProperty("enabled", out var enabled)
                || enabled.ValueKind != JsonValueKind.True)
            {
                return false;
            }

            if (bank.TryGetProperty("launchPath", out var path) && path.ValueKind == JsonValueKind.String)
            {
                var value = path.GetString()!;
                if (!IsRelativePath(value))
                {
                    return false;
                }

                launchPath = value;
            }

            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool IsRelativePath(string value) =>
        value.StartsWith('/')
        && !value.StartsWith("//", StringComparison.Ordinal)
        && !value.Contains('\\')
        && !value.Any(Char.IsControl);
}
