using ChatApp.Mobile.Models;
using ChatApp.Mobile.Services.Interfaces;
using Prism.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Ядро сервисов мобильного приложения
/// </summary>
namespace ChatApp.Mobile.Services.Core
{
    /// <summary>
    /// Сервис работы с пользователями.
    /// Базируется на базовом сервисе.
    /// Реализует интерфейс сервиса пользователей.
    /// </summary>
    public class UsersService : BaseService, IUsersService
    {
        /// <summary>
        /// Конструктор сервиса пользователей.
        /// </summary>
        /// <param name="sessionService">Сервис сессий</param>
        /// <param name="navigationService">Сервис навигации</param>
        public UsersService(ISessionService sessionService,
            INavigationService navigationService)
            : base(sessionService, navigationService)
        {
        }
      
        /// <summary>
        /// Получить список друзей данного пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Коллекция данных о друзьях данного пользователя</returns>
        public async Task<IEnumerable<UserModel>> GetUserFriendsAsync(long userId)
        {
            return await Get<IEnumerable<UserModel>>($"Users/getMyFriends/{userId}");
        }
    }
}
