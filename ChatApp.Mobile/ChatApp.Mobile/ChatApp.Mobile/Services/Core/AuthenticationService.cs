using ChatApp.Mobile.Models;
using ChatApp.Mobile.Services.Interfaces;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Ядро сервисов мобильного приложения
/// </summary>
namespace ChatApp.Mobile.Services.Core
{
    /// <summary>
    /// Сервис аутентификации
    /// </summary>
    public class AuthenticationService : BaseService, IAuthenticationService
    {
        /// <summary>
        /// Конструктор сервиса аутентификации
        /// </summary>
        /// <param name="sessionService"></param>
        /// <param name="navigationService"></param>
        public AuthenticationService(ISessionService sessionService,
            INavigationService navigationService)
            // Вызываем конструктор базовогокласса и передаём ему параметру
            : base(sessionService, navigationService)
        {
        }

        /// <summary>
        /// Makes login for user.
        /// </summary>
        /// <param name="loginDto">The login dto.</param>
        /// <returns>The authenticated user.Null if not.</returns>
        public async Task<UserModel> Login(LoginModel loginDto)
        {
            // Возвращаем результат выполнения асинхронного запроса к вёб сервису командой POST
            // передающей в качестве аргумента данные для входа и возвращаущие данные о пользователе
            return await Post<UserModel, LoginModel>("Users/login", loginDto);
        }

        /// <summary>
        /// Logout user.
        /// </summary>
        /// <returns>This methode returns nothing.</returns>
        public async Task LogOut()
        {
            await SessionService.LogOut();
        }

        public async Task<UserModel> RefreshToken(string token)
        {
            return await PostRefresh<UserModel, string>("Users/refresh", token);
        }
    }
}
