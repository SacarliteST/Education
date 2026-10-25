namespace Education.Web.Endpoints;

/// <summary>Регистрация всех групп маршрутов Education API.</summary>
internal static class EducationEndpointGroups
{
    /// <summary>Регистрирует маршруты всех разделов API.</summary>
    /// <param name="app">Построитель маршрутов.</param>
    /// <returns>Тот же построитель маршрутов.</returns>
    public static IEndpointRouteBuilder MapEducationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAdminProfilesEndpointGroup();
        app.MapProfileEndpointGroup();
        app.MapPracticalModulesEndpointGroup();
        app.MapModuleSessionsEndpointGroup();
        app.MapCoursesEndpointGroup();
        app.MapClassWindowsEndpointGroup();
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

