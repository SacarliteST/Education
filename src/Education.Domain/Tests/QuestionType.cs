using Education.Domain.Common;

namespace Education.Domain.Tests;

public sealed class QuestionType : Entity
{
    public string Name { get; private set; } = String.Empty;

    private QuestionType()
    {
    }

    public QuestionType(long id, string name)
    {
        Id = id;
        Name = name;
    }
}
