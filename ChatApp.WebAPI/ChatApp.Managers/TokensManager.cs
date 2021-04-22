using ChatApp.Managers.Common;
using ChatApp.Managers.Interfaces;
using ChatApp.Models;
using ChatApp.Repositories.Interfaces;
using System;
using System.Linq;

/// <summary>
/// Менеджеры приложения
/// </summary>
namespace ChatApp.Managers
{
    /// <summary>
    /// Менеджер жетонов
    /// </summary>
    public class TokensManager : BaseManager, ITokensManager
    {
        /// <summary>
        /// Хранилище жетонов
        /// </summary>
        private readonly ITokensRepository tokensRepository;

        /// <summary>
        /// Конструктор Менеждера жетонов
        /// </summary>
        /// <param name="unitOfWork">Обработчик Единицы работы</param>
        /// <param name="tokensRepository">Хранилище жетонов</param>
        public TokensManager(IUnitOfWork unitOfWork, ITokensRepository tokensRepository)
              : base(unitOfWork)
        {
            this.tokensRepository = tokensRepository;
        }

        /// <summary>
        /// Получить данные о жетоне обновления жетона доступа.
        /// </summary>
        /// <param name="refreshToken">Жетон обновления</param>
        /// <param name="userAgent">Агент пользователя</param>
        /// <returns>Данные о жетоне обновления жетона доступа или null,
        /// если жетон обновления отсутствует</returns>
        public RefreshTokenModel GetRefreshToken(string refreshToken, string userAgent)
        {
            // Определяем входящий источник. Если Аген пользователя не имеет значения, то "Mobile",
            // иначе "Web"ю
            string incomeSource = string.IsNullOrEmpty(userAgent) ? "Mobile" : "Web";

            // Возвращаем данные о жетоне обновления из хранилища, при соврадении жетона обновления 
            // и агента пользователя
            return tokensRepository.Get(x => 
                x.RefreshToken == refreshToken && x.UserAgent == incomeSource).SingleOrDefault();
        }

        /// <summary>
        /// Получить данные о токене обновления токена доступа по адресу электронной почты
        /// и агенту пользователя.
        /// </summary>
        /// <param name="email">Адрес электронной почты пользователя</param>
        /// <param name="userAgent">Тип агента пользоватея</param>
        /// <returns>>Данные о жетоне обновления жетона доступа или null,
        /// если жетон обновления отсутствует</returns>
        public RefreshTokenModel GetRefreshTokenByEmail(string email, string userAgent)
        {
            // Определяем входящий источник. Если Аген пользователя не имеет значения, то "Mobile",
            // иначе "Web"ю
            string incomeSource = string.IsNullOrEmpty(userAgent) ? "Mobile" : "Web";

            // Возвращаем данные о жетоне обновления из хранилища, при соврадении адреса электронной 
            // почты пользователя и агента пользователя
            return tokensRepository.Get(x => x.UserEmail == email && x.UserAgent == incomeSource)
                .SingleOrDefault();
        }

        /// <summary>
        /// Обновить жетон обновления жетона доступа
        /// </summary>
        /// <param name="refreshToekn">Жетон обновления</param>
        /// <param name="userAgent">Тип агента пользователя</param>
        /// <returns>Обновлённые данные о жетоне обновления</returns>
        public RefreshTokenModel UpdateToken(string refreshToekn, string userAgent)
        {
            // Получаем данные о жетоне обновления по жетону обновления и типу агента пользователя
            var tokenModel = GetRefreshToken(refreshToekn, userAgent);
            if (tokenModel == null) // Tсли жетон отсутствует
            {
                return null;    // Возвращаем null
            }

            // Генерируем новый жетон обновления
            tokenModel.RefreshToken = Guid.NewGuid().ToString();
            // Изменяем дату и время изменения жетона
            tokenModel.ModificationDate = DateTime.Now;
            // Обнвляем данные о жетоне обновления в хранилище
            tokensRepository.Update(tokenModel);
            // Подтверждаем Единицу работы
            UnitOfWork.Commit();
            // Возвращаем новые данные о жетоне обновления
            return tokenModel;
        }

        /// <summary>
        /// Добавить жетон обновления.
        /// </summary>
        /// <param name="userEmail">Адрес электронной почты пользователя</param>
        /// <param name="ipAddress">IP адрес</param>
        /// <param name="userAgent">Агент пользователя</param>
        /// <returns>Данные созданного жетона доступа</returns>
        public RefreshTokenModel AddToken(string userEmail, string ipAddress, string userAgent)
        {
            var now = DateTime.Now; // Берём текущую дату и время
            // Создаём новый жетон обновления
            RefreshTokenModel tokenModel = new RefreshTokenModel
            {
                UserEmail = userEmail,  // Адрес электронной почты пользователя
                RefreshToken = Guid.NewGuid().ToString(), // Сгенерированный жетон
                ModificationDate = now, // Текущеие дата и время
                CreationDate = now, // Текущее дата и время
                IpAddress = ipAddress,  // IP адрес
                // Тип агента пользователя "Mobile" или "Web"
                UserAgent = string.IsNullOrEmpty(userAgent) ? "Mobile" : "Web"
            };
            // Добавдяем данные о токене в хранилище
            tokensRepository.Insert(tokenModel);
            // Подтверждаем Единицу работы
            UnitOfWork.Commit();

            // Возвращаем созданный жетон обновления
            return tokenModel;
        }
    }
}
