namespace Education.Application.PracticalModules;

/// <summary>
/// Обработка события «цифрового следа»: проверка секрета сессии, отбрасывание событий
/// неизвестной/терминальной сессии, идемпотентная запись в журнал.
/// </summary>
public interface IPracticeEventHandler
{
    Task HandleAsync(PracticeEventInput input, CancellationToken cancellationToken = default);
}
