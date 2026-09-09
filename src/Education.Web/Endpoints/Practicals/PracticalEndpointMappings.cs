using Education.Application.Practicals;
using Education.Contracts.Practicals;
using Education.Domain.Practicals;

namespace Education.Web.Endpoints;

internal static class PracticalEndpointMappings
{
    public static CreatePracticalCommand ToCommand(this CreatePracticalRequest request)
    {
        return new CreatePracticalCommand(request.ModuleId, request.Name);
    }

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

    public static CreateTaskCommand ToCommand(this CreateTaskRequest request, Guid practicalId)
    {
        return new CreateTaskCommand(practicalId, request.Name);
    }

    public static BindPracticalModuleCommand ToCommand(this BindPracticalModuleRequest request, Guid practicalId)
    {
        return new BindPracticalModuleCommand(
            practicalId,
            request.PracticalModuleId,
            request.ExternalTaskRef.Trim(),
            request.TriesCount,
            request.TimeLimitMinutes);
    }

    public static UpdateTaskTextCommand ToCommand(this UpdateTaskTextRequest request)
    {
        return new UpdateTaskTextCommand(request.Text);
    }

    public static PracticalResponse ToResponse(this PracticalMaterial practical)
    {
        return new PracticalResponse(practical.Id, practical.Name);
    }

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
                : null);
    }

    public static TaskResponse ToResponse(this Case task)
    {
        return new TaskResponse(task.Id, task.Name, task.Text);
    }

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
            question.IsSelected);
    }
}

