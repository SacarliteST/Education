using Education.Application.Theories;
using Education.Contracts.Theories;
using Education.Domain.Materials;

namespace Education.Web.Endpoints;

/// <summary>Преобразование данных раздела «теории» между контрактами API и слоем приложения.</summary>
internal static class TheoryEndpointMappings
{
    /// <summary>Преобразует <see cref="CreateTheoryRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    public static CreateTheoryCommand ToCommand(this CreateTheoryRequest request)
    {
        return new CreateTheoryCommand(request.ModuleId, request.Name);
    }

    /// <summary>Преобразует <see cref="UpdateTheoryTitleRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    public static UpdateTheoryTitleCommand ToCommand(this UpdateTheoryTitleRequest request)
    {
        return new UpdateTheoryTitleCommand(request.Title);
    }

    /// <summary>Преобразует <see cref="UpdateTheoryTextRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    public static UpdateTheoryTextCommand ToCommand(this UpdateTheoryTextRequest request)
    {
        return new UpdateTheoryTextCommand(request.Text);
    }

    /// <summary>Преобразует <see cref="CreateTheoryLinkRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    public static CreateTheoryLinkCommand ToCommand(this CreateTheoryLinkRequest request)
    {
        return new CreateTheoryLinkCommand(request.TheoryMaterialId, request.Link, request.Description);
    }

    /// <summary>Преобразует <see cref="TheoreticalMaterial"/> в результат.</summary>
    /// <param name="theory">Исходные данные.</param>
    public static TheoryListItemResponse ToListItemResponse(this TheoreticalMaterial theory)
    {
        return new TheoryListItemResponse(theory.Id, theory.Name);
    }

    /// <summary>Преобразует <see cref="TheoreticalMaterial"/> в результат.</summary>
    /// <param name="theory">Исходные данные.</param>
    public static TheoryTextResponse ToTextResponse(this TheoreticalMaterial theory)
    {
        return new TheoryTextResponse(theory.Text, theory.Name);
    }

    /// <summary>Преобразует <see cref="TheoreticalMaterialFile"/> в ответ API.</summary>
    /// <param name="file">Исходные данные.</param>
    public static TheoryDocumentResponse ToResponse(this TheoreticalMaterialFile file)
    {
        return new TheoryDocumentResponse(file.Id, file.Path, file.Description, file.OriginalFileName);
    }

    /// <summary>Преобразует <see cref="TheoreticalMaterialLink"/> в ответ API.</summary>
    /// <param name="link">Исходные данные.</param>
    public static TheoryLinkResponse ToResponse(this TheoreticalMaterialLink link)
    {
        return new TheoryLinkResponse(link.Id, link.Link, link.Description);
    }
}

