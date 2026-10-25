using Education.Domain.Common;

namespace Education.Domain.PracticalModules;

/// <summary>
/// Запись «цифрового следа» — одно действие студента в модуле. Что считать действием,
/// решает модуль; ядро <see cref="Payload"/> не интерпретирует. Дедупликация — по <see cref="Entity.Id"/>
/// (это <c>eventId</c> из Kafka-сообщения).
/// </summary>
public sealed class PracticalTaskEvent : Entity
{
    /// <summary>Сессия, к которой относится событие.</summary>
    public Guid SessionId { get; private set; }

    /// <summary>Произвольная строка-тип от модуля («sql_submit» и т.п.).</summary>
    public string Kind { get; private set; } = String.Empty;

    /// <summary>Момент действия по данным модуля.</summary>
    public DateTimeOffset OccurredAt { get; private set; }

    /// <summary>Максимальный размер markdown в байтах UTF-8; сверх — усекается, событие не отбрасывается.</summary>
    public const int MaxMarkdownBytes = 64 * 1024;

    /// <summary>Маркер, дописываемый в конец усечённого markdown.</summary>
    public const string MarkdownTruncationMarker = "\n\n_… текст усечён_";

    /// <summary>Произвольный JSON от модуля (jsonb).</summary>
    public string Payload { get; private set; } = "{}";

    /// <summary>Готовый человекочитаемый разбор события от модуля (markdown), если модуль его прислал.</summary>
    public string? Markdown { get; private set; }

    private PracticalTaskEvent()
    {
    }

    /// <summary>Создаёт событие «цифрового следа» сессии.</summary>
    /// <param name="eventId">Идентификатор события (генерирует модуль, служит ключом идемпотентности).</param>
    /// <param name="sessionId">Идентификатор сессии.</param>
    /// <param name="kind">Вид события.</param>
    /// <param name="occurredAt">Момент события по часам модуля.</param>
    /// <param name="payload">Полезная нагрузка в формате JSON.</param>
    /// <param name="markdown">Готовый разбор события в markdown, если модуль его прислал.</param>
    public PracticalTaskEvent(
        Guid eventId,
        Guid sessionId,
        string kind,
        DateTimeOffset occurredAt,
        string payload,
        string? markdown = null)
    {
        Id = eventId;
        SessionId = sessionId;
        Kind = kind;
        OccurredAt = occurredAt;
        Payload = String.IsNullOrWhiteSpace(payload) ? "{}" : payload;
        Markdown = NormalizeMarkdown(markdown);
    }

    /// <summary>Приводит markdown к виду для хранения: пустой текст становится <see langword="null"/>, длина ограничивается.</summary>
    /// <param name="markdown">Исходный текст.</param>
    /// <returns>Нормализованный текст или <see langword="null"/>.</returns>
    internal static string? NormalizeMarkdown(string? markdown)
    {
        if (String.IsNullOrWhiteSpace(markdown))
        {
            return null;
        }

        var encoding = System.Text.Encoding.UTF8;
        if (encoding.GetByteCount(markdown) <= MaxMarkdownBytes)
        {
            return markdown;
        }

        var budget = MaxMarkdownBytes - encoding.GetByteCount(MarkdownTruncationMarker);
        int low = 0, high = markdown.Length;
        while (low < high)
        {
            var middle = (low + high + 1) / 2;
            if (encoding.GetByteCount(markdown.AsSpan(0, middle)) <= budget)
            {
                low = middle;
            }
            else
            {
                high = middle - 1;
            }
        }

        var length = low;

        // Не режем суррогатную пару посередине.
        if (length > 0 && Char.IsHighSurrogate(markdown[length - 1]))
        {
            length--;
        }

        return markdown[..length] + MarkdownTruncationMarker;
    }
}
