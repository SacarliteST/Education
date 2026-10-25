using Education.Contracts.Practicals;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="UpdatePracticalStudentsRequest"/>.</summary>
internal sealed class UpdatePracticalStudentsRequestValidator : AbstractValidator<UpdatePracticalStudentsRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public UpdatePracticalStudentsRequestValidator()
    {
        RuleFor(request => request.UserIds)
            .NotNull()
            .WithMessage("Список студентов обязателен.");

        RuleForEach(request => request.UserIds)
            .NotEmpty()
            .WithMessage("Идентификатор студента не может быть пустым.");
    }
}
