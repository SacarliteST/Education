namespace Education.Web.Endpoints;

internal static class EducationEndpointGroups
{
    public static IEndpointRouteBuilder MapEducationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAdminProfilesEndpointGroup();
        app.MapCoursesEndpointGroup();
        app.MapModulesEndpointGroup();
        app.MapPracticalsEndpointGroup();
        app.MapQuestionsEndpointGroup();
        app.MapTheoriesEndpointGroup();
        app.MapTestResultsEndpointGroup();
        app.MapGradesEndpointGroup();
        app.MapTaskFilesEndpointGroup();
        app.MapFilesEndpointGroup();

        return app;
    }
}

