using Education.Application.Theories;
using Education.Contracts.Theories;
using Education.Domain.Materials;

namespace Education.Web.Endpoints;

internal static class TheoryEndpointMappings
{
    public static CreateTheoryCommand ToCommand(this CreateTheoryRequest request)
    {
        return new CreateTheoryCommand(request.ModuleId, request.Name);
    }

    public static UpdateTheoryTitleCommand ToCommand(this UpdateTheoryTitleRequest request)
    {
        return new UpdateTheoryTitleCommand(request.Title);
    }

    public static UpdateTheoryTextCommand ToCommand(this UpdateTheoryTextRequest request)
    {
        return new UpdateTheoryTextCommand(request.Text);
    }

    public static CreateTheoryLinkCommand ToCommand(this CreateTheoryLinkRequest request)
    {
        return new CreateTheoryLinkCommand(request.TheoryMaterialId, request.Link, request.Description);
    }

    public static TheoryListItemResponse ToListItemResponse(this TheoreticalMaterial theory)
    {
        return new TheoryListItemResponse(theory.Id, theory.Name);
    }

    public static TheoryTextResponse ToTextResponse(this TheoreticalMaterial theory)
    {
        return new TheoryTextResponse(theory.Text, theory.Name);
    }

    public static TheoryDocumentResponse ToResponse(this TheoreticalMaterialFile file)
    {
        return new TheoryDocumentResponse(file.Id, file.Path, file.Description, GetPublicFileName(file.Path));
    }

    public static TheoryLinkResponse ToResponse(this TheoreticalMaterialLink link)
    {
        return new TheoryLinkResponse(link.Id, link.Link, link.Description);
    }

    private static string GetPublicFileName(string path)
    {
        var name = Path.GetFileName(path);
        var lastUnderscore = name.LastIndexOf('_');

        return lastUnderscore <= 0 ? name : name[..lastUnderscore] + Path.GetExtension(name);
    }
}
