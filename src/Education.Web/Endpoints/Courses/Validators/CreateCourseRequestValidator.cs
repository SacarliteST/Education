using Education.Contracts.Courses;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="CreateCourseRequest"/>.</summary>
internal sealed class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public CreateCourseRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Название курса обязательно.")
            .MaximumLength(200)
            .WithMessage("Название курса не должно превышать 200 символов.");

        RuleFor(request => request.Description)
            .NotEmpty()
            .WithMessage("Описание курса обязательно.")
            .MaximumLength(4000)
            .WithMessage("Описание курса не должно превышать 4000 символов.");

        RuleFor(request => request.Date)
            .NotEmpty()
            .WithMessage("Дата курса обязательна.");
    }
}

