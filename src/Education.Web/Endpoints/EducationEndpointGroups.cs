namespace Education.Web.Endpoints;

public static class EducationEndpointGroups
{
    public static IEndpointRouteBuilder MapEducationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCoursesEndpointGroup();
        app.MapModulesEndpointGroup();
        app.MapPracticalsEndpointGroup();
        app.MapQuestionsEndpointGroup();
        app.MapTheoriesEndpointGroup();
        app.MapTestResultsEndpointGroup();
        app.MapGradesEndpointGroup();

        return app;
    }
}
