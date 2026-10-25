namespace Education.Contracts;

public static partial class ApiRoutes
{
    /// <summary>
    /// Шаблоны маршрутов профиля текущего пользователя.
    /// </summary>
    public static class Profile
    {
        /// <summary>
        /// Маршрут учебного профиля текущего пользователя.
        /// </summary>
        public const string Me = PrefixV1 + "/profile";
    }
}
