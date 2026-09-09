namespace Education.Application.PracticalModules;

/// <summary>Входные данные одного события «цифрового следа» (маппинг Kafka-сообщения в слое Web).</summary>
public sealed record PracticeEventInput(
    Guid SessionId,
    string SessionKey,
    Guid EventId,
    string Kind,
    DateTimeOffset OccurredAt,
    string PayloadJson);

/// <summary>
/// Обработка события «цифрового следа»: проверка секрета сессии, отбрасывание событий
/// неизвестной/терминальной сессии, идемпотентная запись в журнал.
/// </summary>
public interface IPracticeEventHandler
{
    Task HandleAsync(PracticeEventInput input, CancellationToken cancellationToken = default);
}
