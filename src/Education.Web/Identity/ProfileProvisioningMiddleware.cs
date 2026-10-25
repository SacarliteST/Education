using Education.Application.Users;

namespace Education.Web.Identity;

/// <summary>
/// Гарантирует учебный профиль для любого аутентифицированного запроса (в том числе администратора, чьи ручки
/// профиль не запрашивают) и синхронизирует ФИО и роль из токена. Результат кэшируется на запрос в резолвере,
/// поэтому сервисы не платят за повторную проверку. Отсутствие профиля здесь не блокирует запрос: решение
/// принимает конкретная ручка (403 там, где профиль нужен).
/// </summary>
public sealed class ProfileProvisioningMiddleware
{
    private readonly RequestDelegate next;

    /// <summary>Создаёт экземпляр класса <see cref="ProfileProvisioningMiddleware"/>.</summary>
    /// <param name="next">Следующий обработчик конвейера.</param>
    public ProfileProvisioningMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    /// <summary>Выполняет проверку профиля и передаёт запрос дальше.</summary>
    public async Task InvokeAsync(HttpContext context, IEducationUserResolver resolver)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            try
            {
                await resolver.ResolveCurrentLegacyUserIdAsync(context.RequestAborted);
            }
            catch (EducationUserLinkNotFoundException)
            {
                // Профиль не создан/отключён — ответ определит эндпоинт.
            }
            catch (UnauthenticatedEducationUserException)
            {
                // Токен без валидного идентификатора пользователя.
            }
        }

        await next(context);
    }
}
