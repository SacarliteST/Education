using Education.Contracts.Practicals;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class UpdatePracticalStudentsRequestValidator : AbstractValidator<UpdatePracticalStudentsRequest>
{
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
