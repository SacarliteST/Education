namespace Education.Application.PracticalModules;

/// <summary>
/// Обработка события «цифрового следа»: проверка секрета сессии, отбрасывание событий
/// неизвестной/терминальной сессии, идемпотентная запись в журнал.
/// </summary>
public interface IPracticeEventHandler
{
    /// <summary>Обрабатывает событие: проверяет секрет сессии и записывает событие в журнал.</summary>
    /// <param name="input">Событие, присланное модулем.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task HandleAsync(PracticeEventInput input, CancellationToken cancellationToken = default);
}
