using Education.Contracts.Practicals;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Валидатор запроса пересбора набора заданий студента.</summary>
internal sealed class RegenerateAssignmentsRequestValidator : AbstractValidator<RegenerateAssignmentsRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public RegenerateAssignmentsRequestValidator()
    {
        RuleFor(request => request.StudentId)
            .NotEmpty()
            .WithMessage("Идентификатор студента обязателен.");

        RuleFor(request => request.ClassWindowId)
            .NotEmpty()
            .WithMessage("Идентификатор пары обязателен.");
    }
}
