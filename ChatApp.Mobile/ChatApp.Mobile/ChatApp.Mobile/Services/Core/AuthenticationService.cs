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
        /// <param name="sessionService">Сервис сессий</param>
        /// <param name="navigationService">Сервис навигации</param>
        public AuthenticationService(ISessionService sessionService,
            INavigationService navigationService)
            // Вызываем конструктор базовогокласса и передаём ему параметру
            : base(sessionService, navigationService)
        {
        }

        /// <summary>
        /// Войти в систему
        /// </summary>
        /// <param name="loginDto">Данные для входа в систему</param>
        /// <returns>Данные о пользователе с очищенным паролем, при успешнов входе в систему.
        /// null в противном случае</returns>
        public async Task<UserModel> Login(LoginModel loginDto)
        {
            // Возвращаем результат выполнения асинхронного запроса к вёб сервису командой POST
            // передающей в качестве аргумента данные для входа и возвращаущие данные о пользователе
            return await Post<UserModel, LoginModel>("Users/login", loginDto);
        }

        /// <summary>
        /// Выйти из системы.
        /// </summary>
        /// <returns>Этот метод ничего не возвращает.</returns>
        public async Task LogOut()
        {
            await SessionService.LogOut();
        }

        /// <summary>
        /// Обновить жетон доступа.
        /// </summary>
        /// <param name="token">Данные о жетоне</param>
        /// <returns>Данные о пользователес очищенным паролем, при успешной операции.
        /// null ghb ytelfxt/</returns>
        public async Task<UserModel> RefreshToken(string token)
        {
            return await PostRefresh<UserModel, string>("Users/refresh", token);
        }
    }
}
