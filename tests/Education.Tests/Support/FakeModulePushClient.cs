using Education.Application.PracticalModules;

namespace Education.Tests.Support;

/// <summary>Запоминает пуши сессий, которые платформа отправила бы бэкенду модуля.</summary>
public sealed class FakeModulePushClient : IModulePushClient
{
    /// <summary>Все принятые пуши в порядке отправки.</summary>
    public List<ModuleSessionPush> Pushes { get; } = [];

    /// <summary>Имитировать недоступность модуля: пуш завершается <see cref="ModulePushFailedException"/>.</summary>
    public bool ThrowUnavailable { get; init; }

    /// <inheritdoc />
    public Task PushAsync(
        string sessionsEndpoint,
        string moduleSlug,
        ModuleSessionPush push,
        CancellationToken cancellationToken = default)
    {
        if (ThrowUnavailable)
        {
            throw new ModulePushFailedException("test: module unavailable");
        }

        Pushes.Add(push);
        return Task.CompletedTask;
    }
}
