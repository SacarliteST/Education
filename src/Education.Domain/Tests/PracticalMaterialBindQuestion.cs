using Education.Domain.Common;
using Education.Domain.Practicals;

namespace Education.Domain.Tests;

public sealed class PracticalMaterialBindQuestion : Entity
{
    public Guid QuestionId { get; private set; }
    public Question Question { get; private set; } = null!;
    public Guid PracticalMaterialId { get; private set; }
    public PracticalMaterial PracticalMaterial { get; private set; } = null!;
    public List<Answer> Answers { get; private set; } = [];

    private PracticalMaterialBindQuestion()
    {
    }

    public PracticalMaterialBindQuestion(Guid practicalMaterialId, Guid questionId)
    {
        PracticalMaterialId = practicalMaterialId;
        QuestionId = questionId;
    }
}

