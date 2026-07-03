using Education.Domain.Common;
using Education.Domain.Courses;

namespace Education.Domain.Tests;

/// <summary>
/// Вопрос теста.
/// </summary>
public sealed class Question : Entity
{
    /// <summary>
    /// Текст вопроса.
    /// </summary>
    public string Text { get; private set; } = String.Empty;

    /// <summary>
    /// Варианты ответа или дополнительные данные вопроса.
    /// </summary>
    public string Options { get; private set; } = String.Empty;

    /// <summary>
    /// Правильный ответ в формате, зависящем от типа вопроса.
    /// </summary>
    public string Answer { get; private set; } = String.Empty;

    /// <summary>
    /// Вес вопроса в тесте.
    /// </summary>
    public double Weight { get; private set; }

    /// <summary>
    /// Идентификатор типа вопроса.
    /// </summary>
    public Guid QuestionTypeId { get; private set; }

    /// <summary>
    /// Тип вопроса.
    /// </summary>
    public QuestionType QuestionType { get; private set; } = null!;

    /// <summary>
    /// Идентификатор модуля.
    /// </summary>
    public Guid ModuleId { get; private set; }

    /// <summary>
    /// Модуль, к которому относится вопрос.
    /// </summary>
    public Module Module { get; private set; } = null!;

    /// <summary>
    /// Связи вопроса с практическими материалами.
    /// </summary>
    public List<PracticalMaterialBindQuestion> PracticalMaterialBindQuestions { get; private set; } = [];

    private Question()
    {
    }

    /// <summary>
    /// Создает вопрос теста.
    /// </summary>
    /// <param name="moduleId">Идентификатор модуля.</param>
    /// <param name="questionTypeId">Идентификатор типа вопроса.</param>
    /// <param name="text">Текст вопроса.</param>
    /// <param name="options">Варианты ответа или дополнительные данные вопроса.</param>
    /// <param name="answer">Правильный ответ.</param>
    /// <param name="weight">Вес вопроса.</param>
    public Question(Guid moduleId, Guid questionTypeId, string text, string options, string answer, double weight)
    {
        ModuleId = moduleId;
        QuestionTypeId = questionTypeId;
        Text = text;
        Options = options;
        Answer = answer;
        Weight = weight;
    }

    /// <summary>
    /// Обновляет данные вопроса.
    /// </summary>
    /// <param name="questionTypeId">Идентификатор типа вопроса.</param>
    /// <param name="text">Текст вопроса.</param>
    /// <param name="options">Варианты ответа или дополнительные данные вопроса.</param>
    /// <param name="answer">Правильный ответ.</param>
    /// <param name="weight">Вес вопроса.</param>
    public void Update(Guid questionTypeId, string text, string options, string answer, double weight)
    {
        QuestionTypeId = questionTypeId;
        Text = text;
        Options = options;
        Answer = answer;
        Weight = weight;
    }
}

