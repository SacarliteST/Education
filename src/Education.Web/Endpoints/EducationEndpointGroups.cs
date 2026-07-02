namespace Education.Web.Endpoints;

public static class EducationEndpointGroups
{
    public static IEndpointRouteBuilder MapEducationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCoursesEndpointGroup();
        app.MapModulesEndpointGroup();
        app.MapTheoriesEndpointGroup();

        return app;
    }
}
