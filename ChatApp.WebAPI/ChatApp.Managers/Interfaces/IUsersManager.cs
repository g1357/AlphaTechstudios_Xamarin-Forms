using ChatApp.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

/// <summary>
/// Интерфейсы Менеджеров прилоления
/// </summary>
namespace ChatApp.Managers.Interfaces
{
    /// <summary>
    /// Интерфейс менеджера пользователей
    /// </summary>
    public interface IUsersManager
    {
        /// <summary>
        /// Получить данные о пользователе по идентификатору.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Данные о пользователе с очищенным паролдем или null</returns>
        UserModel GetUserById(long userId);

        /// <summary>
        /// Получить данные о пользователе по адресу электронной почты.
        /// </summary>
        /// <param name="email">Адрес электронной почты пользователя</param>
        /// <returns>Данные о пользователе с очищенным паролдем или null</returns>
        UserModel GetUserByEmail(string email);

        /// <summary>
        /// Добавить подключение к пользователю.
        /// </summary>
        /// <param name="conversationModel"Данные о подключении></param>
        void AddUserConnections(ConnectionModel conversationModel);

        /// <summary>
        /// Обновить состояния подключений пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="status">Состояние</param>
        /// <param name="connectionID">Идентификатор подключения</param>
        void UpdateUserConnectionsStatus(long userId, bool status, string connectionID);

        /// <summary>
        /// Вход в систему
        /// </summary>
        /// <param name="loginModel">Данные о пользователя</param>
        /// <param name="httpContext">Контекст HTTP</param>
        /// <returns>Данные о пользователе или null, при неудачном входе в систему</returns>
        UserModel Login(LoginModel loginModel, HttpContext httpContext);

        /// <summary>
        /// ОБновить жетон доступа
        /// </summary>
        /// <param name="refreshToken">Токен обновления</param>
        /// <param name="httpContext">Контекст HTTP</param>
        /// <returns>Данные о пользователе с очищенным паролем или null,
        /// при любой ошибке</returns>
        UserModel RefreshToken(string refreshToken, HttpContext httpContext);

        /// <summary>
        /// Добавить пользователя.
        /// </summary>
        /// <param name="userModel">Данные о пользователе</param>
        /// <returns>Идентификатор пользователя или -100, если пользователь уже существует</returns>
        long InsertUser(UserModel userModel);

        /// <summary>
        /// Получить коллекцию моих друзей в виде данных о полизователях.
        /// </summary>
        /// <param name="userID">Идентификатор пользователя</param>
        /// <returns>Коллекция данных о друзьях или null, при их отсутствии</returns>
        IEnumerable<UserModel> GetMyFriends(long userID);

        /// <summary>
        /// Получить данные о пользователе по идентификатору подключения
        /// </summary>
        /// <param name="connectionId">Идентификатор подключения</param>
        /// <returns>Данные о пользователе с очищенным паролем или null, 
        /// при отсутствии или ошибке</returns>
        UserModel GetUserByConnectionId(string connectionId);
    }
}
