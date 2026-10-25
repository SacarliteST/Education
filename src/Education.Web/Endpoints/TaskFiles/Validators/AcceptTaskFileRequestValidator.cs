using Education.Contracts.TaskFiles;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="AcceptTaskFileRequest"/>.</summary>
internal sealed class AcceptTaskFileRequestValidator : AbstractValidator<AcceptTaskFileRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public AcceptTaskFileRequestValidator()
    {
        RuleFor(request => request.Grade)
            .InclusiveBetween(2, 5)
            .WithMessage("Оценка должна быть от 2 до 5.");
    }
}
