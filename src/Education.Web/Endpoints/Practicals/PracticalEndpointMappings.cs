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
        long practicalId)
    {
        return new ConfigurePracticalQuestionsCommand(
            practicalId,
            request.QuestionIds,
            request.TriesCount,
            request.PercentForFive,
            request.PercentForFour,
            request.PercentForThree);
    }

    public static PracticalResponse ToResponse(this PracticalMaterial practical)
    {
        return new PracticalResponse(practical.Id, practical.Name);
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
