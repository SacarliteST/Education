using Education.Application.Practicals;
using Education.Contracts.Practicals;
using Education.Domain.Practicals;

namespace Education.Web.Endpoints;

/// <summary>Преобразование данных раздела «практики» между контрактами API и слоем приложения.</summary>
internal static class PracticalEndpointMappings
{
    /// <summary>Преобразует <see cref="CreatePracticalRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    public static CreatePracticalCommand ToCommand(this CreatePracticalRequest request)
    {
        return new CreatePracticalCommand(request.ModuleId, request.Name);
    }

    /// <summary>Преобразует <see cref="ConfigurePracticalQuestionsRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    /// <param name="practicalId">Идентификатор практики.</param>
    public static ConfigurePracticalQuestionsCommand ToCommand(
        this ConfigurePracticalQuestionsRequest request,
        Guid practicalId)
    {
        return new ConfigurePracticalQuestionsCommand(
            practicalId,
            request.QuestionIds,
            request.TriesCount,
            request.PercentForFive,
            request.PercentForFour,
            request.PercentForThree);
    }

    /// <summary>Преобразует <see cref="CreateTaskRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    /// <param name="practicalId">Идентификатор практики.</param>
    public static CreateTaskCommand ToCommand(this CreateTaskRequest request, Guid practicalId)
    {
        return new CreateTaskCommand(practicalId, request.Name);
    }

    /// <summary>Преобразует <see cref="BindPracticalModuleRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    /// <param name="practicalId">Идентификатор практики.</param>
    public static BindPracticalModuleCommand ToCommand(this BindPracticalModuleRequest request, Guid practicalId)
    {
        return new BindPracticalModuleCommand(
            practicalId,
            request.PracticalModuleId,
            request.ExternalTaskRef.Trim(),
            request.TriesCount,
            request.TimeLimitMinutes,
            request.AttemptsPerSession);
    }

    /// <summary>Преобразует <see cref="UpdateTaskTextRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    public static UpdateTaskTextCommand ToCommand(this UpdateTaskTextRequest request)
    {
        return new UpdateTaskTextCommand(request.Text);
    }

    /// <summary>Преобразует <see cref="PracticalMaterial"/> в ответ API.</summary>
    /// <param name="practical">Исходные данные.</param>
    public static PracticalResponse ToResponse(this PracticalMaterial practical)
    {
        return new PracticalResponse(practical.Id, practical.Name);
    }

    /// <summary>Преобразует <see cref="PracticalDetail"/> в ответ API.</summary>
    /// <param name="detail">Исходные данные.</param>
    public static PracticalDetailResponse ToResponse(this PracticalDetail detail)
    {
        return new PracticalDetailResponse(
            detail.Id,
            detail.Name,
            detail.Kind.ToWire(),
            detail.IsPublic,
            detail.TriesCount,
            detail.TimeLimitMinutes,
            detail.ModuleBinding is { } binding
                ? new ExternalModuleBindingResponse(
                    binding.PracticalModuleId,
                    binding.PracticalModuleSlug,
                    binding.PracticalModuleName,
                    binding.TaskId,
                    binding.ExternalTaskRef)
                : null,
            detail.AttemptsPerSession);
    }

    /// <summary>Преобразует <see cref="Case"/> в ответ API.</summary>
    /// <param name="task">Исходные данные.</param>
    public static TaskResponse ToResponse(this Case task)
    {
        return new TaskResponse(task.Id, task.Name, task.Text);
    }

    /// <summary>Преобразует <see cref="PracticalQuestionsSetup"/> в ответ API.</summary>
    /// <param name="setup">Исходные данные.</param>
    public static PracticalQuestionsSetupResponse ToResponse(this PracticalQuestionsSetup setup)
    {
        return new PracticalQuestionsSetupResponse(
            setup.Questions.Select(question => question.ToResponse()).ToList(),
            setup.IsPublic,
            setup.TriesCount,
            setup.PercentForFive,
            setup.PercentForFour,
            setup.PercentForThree);
    }

    private static SelectableQuestionResponse ToResponse(this SelectableQuestion question)
    {
        return new SelectableQuestionResponse(
            question.Id,
            question.Text,
            question.Type,
            question.Body,
            question.Weight,
            question.IsSelected);
    }
}

