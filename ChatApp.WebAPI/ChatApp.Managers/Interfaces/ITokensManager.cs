using ChatApp.Models;

/// <summary>
/// Интерфейсы Менеджеров приложения
/// </summary>
namespace ChatApp.Managers.Interfaces
{
    /// <summary>
    /// Интерфейс менеджера жетонов доступа
    /// </summary>
    public interface ITokensManager
    {
        /// <summary>
        /// Добавить жетон.
        /// </summary>
        /// <param name="userEmail">Адрес электронной почты пользователя</param>
        /// <param name="ipAddress">IP адрес</param>
        /// <param name="userAgent">Агент пользователя</param>
        /// <returns>Данные созданного жетона доступа</returns>
        RefreshTokenModel AddToken(string userEmail, string ipAddress, string userAgent);

        /// <summary>
        /// Получить данные о жетон обновления жетона доступа по жетону обновления и
        /// агенту пользователя.
        /// </summary>
        /// <param name="refreshToken">Токен обновления</param>
        /// <param name="userAgent">Агент пользователя ("Mobile", "Web" или ничего)</param>
        /// <returns>Данные о жетоне обновления</returns>
        RefreshTokenModel GetRefreshToken(string refreshToken, string userAgent);

        /// <summary>
        /// Получить данные о токене обновления токена доступа по адресу электронной почты
        /// и агенту пользователя.
        /// </summary>
        /// <param name="email">Адрес электронной почты пользователя</param>
        /// <param name="userAgent">Тип агента пользоватея</param>
        /// <returns>>Данные о жетоне обновления жетона доступа</returns>
        RefreshTokenModel GetRefreshTokenByEmail(string email, string userAgent);

        /// <summary>
        /// Обновить жетон обновления жетона доступа
        /// </summary>
        /// <param name="refreshToekn">Жетон обновления</param>
        /// <param name="userAgent">Тип агента пользователя</param>
        /// <returns>Обновлённые данные о жетоне обновления</returns>
        RefreshTokenModel UpdateToken(string refreshToekn, string userAgent);
    }
}
