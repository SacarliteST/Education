using Education.Contracts.Questions;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class CreateQuestionRequestValidator : AbstractValidator<CreateQuestionRequest>
{
    public CreateQuestionRequestValidator()
    {
        RuleFor(request => request.ModuleId)
            .NotEmpty()
            .WithMessage("Идентификатор модуля обязателен.");

        RuleFor(request => request.Type)
            .NotEmpty()
            .WithMessage("Тип вопроса обязателен.");

        RuleFor(request => request.Text)
            .NotEmpty()
            .WithMessage("Текст вопроса обязателен.");

        RuleFor(request => request.Body)
            .NotEmpty()
            .WithMessage("Тело вопроса обязательно.");

        RuleFor(request => request.Answer)
            .NotEmpty()
            .WithMessage("Правильный ответ обязателен.");

        RuleFor(request => request.Weight)
            .GreaterThan(0)
            .WithMessage("Вес вопроса должен быть больше 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Вес вопроса не должен превышать 100.");
    }
}

internal sealed class UpdateQuestionRequestValidator : AbstractValidator<UpdateQuestionRequest>
{
    public UpdateQuestionRequestValidator()
    {
        RuleFor(request => request.Type)
            .NotEmpty()
            .WithMessage("Тип вопроса обязателен.");

        RuleFor(request => request.Text)
            .NotEmpty()
            .WithMessage("Текст вопроса обязателен.");

        RuleFor(request => request.Body)
            .NotEmpty()
            .WithMessage("Тело вопроса обязательно.");

        RuleFor(request => request.Answer)
            .NotEmpty()
            .WithMessage("Правильный ответ обязателен.");

        RuleFor(request => request.Weight)
            .GreaterThan(0)
            .WithMessage("Вес вопроса должен быть больше 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Вес вопроса не должен превышать 100.");
    }
}

