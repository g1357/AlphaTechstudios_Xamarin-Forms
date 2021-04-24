using ChatApp.Managers.Common;
using ChatApp.Managers.Extensions;
using ChatApp.Managers.Interfaces;
using ChatApp.Models;
using ChatApp.Models.Settings;
using ChatApp.Repositories.Interfaces;
using ChatApp.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

/// <summary>
/// Менеджеры приложения
/// </summary>
namespace ChatApp.Managers
{
    /// <summary>
    /// Менеджер пользователей
    /// </summary>
    public class UsersManager : BaseManager, IUsersManager
    {
        // Хранилище данных о пользователях
        private readonly IUsersRepository usersRepository;
        // Хранилище данных о жетонов обновления
        private ITokensManager tokensManager;
        // Хранилище данных о друзьях
        private readonly IFriendsRepository friendsRepository;
        // Хранилище данных о подключениях
        private readonly IConnectionsRepository connectionsRepository;
        // Настройки приложения
        private readonly AppSettings appSettings;

        /// <summary>
        /// Конструктор менеджера пользоватеелй.
        /// </summary>
        /// <param name="unitOfWork">Обработчик Единицы работы</param>
        /// <param name="usersRepository">Хранилище пользователей</param>
        /// <param name="tokensManager">Хранилище токенов обновления</param>
        /// <param name="friendsRepository">Хранилище друзей</param>
        /// <param name="connectionsRepository">Хранилище подключений</param>
        /// <param name="appSettings">Настройки приложения</param>
        public UsersManager(IUnitOfWork unitOfWork,
            IUsersRepository usersRepository,
            ITokensManager tokensManager,
            IFriendsRepository friendsRepository,
            IConnectionsRepository connectionsRepository,
            IOptions<AppSettings> appSettings)
            : base(unitOfWork)
        {
            this.usersRepository = usersRepository;
            this.tokensManager = tokensManager;
            this.friendsRepository = friendsRepository;
            this.connectionsRepository = connectionsRepository;
            this.appSettings = appSettings.Value;
        }

        /// <summary>
        /// Добавить пользователя.
        /// </summary>
        /// <param name="userModel">Данные о пользователе</param>
        /// <returns>Идентификатор пользователя или -100, если пользователь уже существует</returns>
        public long InsertUser(UserModel userModel)
        {
            // Преобразуем адрес электронной почты к строчным буквам
            userModel.Email = userModel.Email.ToLower();
            // Получаем данные о пользователе по адресу электронной почты
            var user = GetUserByEmail(userModel.Email);
            if (user != null) // Если пользователь найден
            {
                return -100;    // Возвращаем код -100
            }

            var now = DateTime.Now; // Берём текущуие дату и время
            var salt = PasswordHasher.GenerateSalt();   // Генерируем модификатор пароля
            // Вычисляем хэш-код пароля
            var pwdHash = Convert.ToBase64String(PasswordHasher.ComputeHash(userModel.Password, salt));

            userModel.CreationDate = now;   // Задаём дату и время создания
            userModel.ModificationDate = now;   // Задаём дату и время изменения
            // генерируем жетон проверки пользователя
            userModel.ValidationToken = Guid.NewGuid().ToString();
            // Преобразуем модификатор пароля в текстовый вид
            userModel.PasswordSalt = Convert.ToBase64String(salt);
            // Устанавливаем хэш пароля
            userModel.Password = pwdHash;

            // Добавляем пользователя в хранилище пользователей
            var modelId = usersRepository.Insert(userModel);
            // Подтверждаем Единицу работы
            UnitOfWork.Commit();
            // Возвращаем идентификатор созданного пользователя
            return modelId;
        }

        /// <summary>
        /// Получить данные о пользователе по идентификатору.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Данные о пользователе с очищенным паролдем или null</returns>
        public UserModel GetUserById(long userId)
        {
            // Возвращаем данные о выбранным по идентификатору пользователе с очищенным паролем
            // или null при отсутствии такого пользователя
            return usersRepository.Get(x => 
                x.ID == userId, includeProperties: "Friends")
                .SingleOrDefault()
                .WithoutPassword();
        }

        /// <summary>
        /// Получить данные о пользователе по адресу электронной почты.
        /// </summary>
        /// <param name="email">Адрес электронной почты пользователя</param>
        /// <returns>Данные о пользователе с очищенным паролдем или null</returns>
        public UserModel GetUserByEmail(string email)
        {
            // Возвращаем данные о выбранным по электронной почте  пользователе с очищенным паролем
            // или null при отсутствии такого пользователя
            return usersRepository.Get(x => 
                x.Email == email, includeProperties: "Friends,Connections")
                .SingleOrDefault()
                .WithoutPassword();

        }

        /// <summary>
        /// Добавить подключение к пользователю.
        /// </summary>
        /// <param name="conversationModel"Данные о подключении></param>
        public void AddUserConnections(ConnectionModel conversationModel)
        {
            // Доавляем данные о полключении в хранилище
            connectionsRepository.Insert(conversationModel);
            // Подтверждаем Единицу работы
            UnitOfWork.Commit();
            //var dbUserModel = usersRepository.GetByID(userId);
            //dbUserModel.Connections.Add(conversationModel);
            //usersRepository.Update(dbUserModel);
            //UnitOfWork.Commit();
        }

        /// <summary>
        /// Обновить состояния подключений пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="status">Состояние</param>
        /// <param name="connectionID">Идентификатор подключения</param>
        public void UpdateUserConnectionsStatus(long userId, bool status, 
            string connectionID)
        {
            // Получаес данные о подключении по идентификатору пользователя 
            // и идентификатору подключения или null, при отсутствии данных
            var connection = connectionsRepository.Get(x => 
                x.ConnectionID == connectionID && x.UserID == userId).SingleOrDefault();
            if (connection != null) // Если данные присутствуют
            {
                // Изменяем состояние подключения
                connection.IsConnected = status;
                // Обновляем данные о подключении в хранилище
                connectionsRepository.Update(connection);
                // Подтверждаем Единицу работы
                UnitOfWork.Commit();
            }
        }

        /// <summary>
        /// Вход в систему
        /// </summary>
        /// <param name="loginModel">Данные о пользователя</param>
        /// <param name="httpContext">Контекст HTTP</param>
        /// <returns>Данные о пользователе или null, при неудачном входе в систему</returns>
        public UserModel Login(LoginModel loginModel, HttpContext httpContext)
        {
            double minutes = 2d;    // Две минуты
            // Преобразуем адрес электронной почтвы пользовате в строчные буквы
            loginModel.Email = loginModel.Email.ToLower();
            DateTime now = DateTime.UtcNow; // Текущие дата и время
            // Извлекаем IP адрес из контекста HTTP
            var ipAddress = httpContext.Connection.RemoteIpAddress;
            // Извоекаем агента пользователя из контекста HTTP
            var userAgent = httpContext.Request.Headers["User-Agent"];

            // Выбираем из хранилища данных о пользователепо адресу электронной почты
            // и пустому жетону проверки
            var user = usersRepository.Get(x => 
                x.Email == loginModel.Email && !string.IsNullOrEmpty(x.ValidationToken))
                .SingleOrDefault();

            if (user != null)   // Если пользователь найден
            {
                // Проверяем пароль пользователя
                bool verified = PasswordHasher.VerifyPassword(loginModel.Password, 
                    user.PasswordSalt, user.Password);
                if (verified) // Если пароль совпал
                {
                    // Получаем жетон обновления по адресу электронной почты пользователя
                    var refreshToken = tokensManager.GetRefreshTokenByEmail(loginModel.Email,
                        userAgent);
                    // Проверка на хакерскую атаку или сбой
                    // Если жетон обновления существует и IP адреса при получении жетона обновления и текущий совпадают
                    if (refreshToken != null && refreshToken.IpAddress != ipAddress.ToString())
                    {
                        // TODO Send notification to the user and the admin => May be  Hacker attck.
                    }

                    if (refreshToken != null)   // Если жетон существует
                    {
                        // Получаем обновлённый жетон обновления
                        refreshToken = tokensManager.UpdateToken(refreshToken.RefreshToken, 
                            userAgent);
                    }
                    else // Если жетон отсутствует
                    {
                        // Получаем добавленный новый жетон обновления
                        refreshToken = tokensManager.AddToken(loginModel.Email, 
                            ipAddress.ToString(), userAgent);
                    }

                    // Генерируем жетон доступа
                    string newToken = GenerateToken(userAgent, user, now, minutes);
                    // Задаём в данных о пользователе жетон обновления
                    user.RefreshToken = refreshToken.RefreshToken;
                    // Задаём время истечения срока действия жетона
                    user.TokenExpireTimes = now.AddMinutes(minutes);
                    // Задаём жетон доступа
                    user.Token = newToken;
                    // Возвращаем данные о пользователе с очищенным паролем
                    return user.WithoutPassword();
                }
                else // Если пароль не совпал
                {
                    // TODO BG: LOG login failed email; address ip and user agent. 
                }
            }
            return null; // Возвращаем null при несовпадении паролей или отсутствии пользователя
        }

        /// <summary>
        /// ОБновить жетон доступа
        /// </summary>
        /// <param name="refreshToken">Токен обновления</param>
        /// <param name="httpContext">Контекст HTTP</param>
        /// <returns>Данные о пользователе с очищенным паролем или null,
        /// при любой ошибке</returns>
        public UserModel RefreshToken(string refreshToken, HttpContext httpContext)
        {
            DateTime now = DateTime.UtcNow; // Получаем текущие дату и время
            double minutes = 2d;    // две минуты
            // Извлекаем из контекста HTTP тип агента пользователя
            var userAgent = httpContext.Request.Headers["User-Agent"];
            // Извлекаем из контекста HTTP IP адрес пользователя
            var ipAddress = httpContext.Connection.RemoteIpAddress;
            // Извлекаем из хранилища данные о жетоне обновления
            var refToken = tokensManager.GetRefreshToken(refreshToken, userAgent);

            if (refToken == null)   // Если жетон обновления не найден
            {
                //logger.LogWarning($"Probable hacker attack attempt for refresh token: {refreshToken}, IP : {ipAddress}");
                return null;    // Возвращаем null
            }

            // Если IP адреса из контекста HTTP и жетона обновления не совпадают
            if (ipAddress.ToString() != refToken.IpAddress)
            {
                //logger.LogWarning($"Token not found token: {refreshToken} ");
                return null;    // Возвращаем null
            }

            // Извлекаем из хранилища данные о пользователе по адресу электронной почты 
            // с очищенным паролем
            var user = GetUserByEmail(refToken.UserEmail).WithoutPassword();
            if (user == null)   // Если пользоваитель не найден
            {
                //logger.LogWarning($" user not found for token: {refreshToken} ");
                return null;    // Возвращаем null
            }

            // Генерируем новый эетон доступа
            string newToken = GenerateToken(userAgent, user, now, minutes);
            // Обновляем жетон обновления для текущего типа агента пользователя
            refToken = tokensManager.UpdateToken(refToken.RefreshToken, userAgent);
            // Устанавливам для пользователя жетон обновления
            user.RefreshToken = refToken.RefreshToken;
            // Устанавливаем время истечения срока действия жетона
            user.TokenExpireTimes = now.AddMinutes(minutes);    // 2 минуты
            // Устанавливаем новый жетон доступа
            user.Token = newToken;
            // Возвращаем данные о пользоваителе
            return user;
        }

        /// <summary>
        /// Получить коллекцию моих друзей в виде данных о полизователях.
        /// </summary>
        /// <param name="userID">Идентификатор пользователя</param>
        /// <returns>Коллекция данных о друзьях или null, при их отсутствии</returns>
        public IEnumerable<UserModel> GetMyFriends(long userID)
        {
            // Возвращаем коллекцию данных о пользователях из хранилища друзей
            // для заданного пользователя. Отбираем друзей.
            return friendsRepository.Get(x => 
                x.UserID == userID, includeProperties: "UserFriend")
                .Select(x => x.UserFriend);
        }

        /// <summary>
        /// Генерировать жетон
        /// </summary>
        /// <param name="userAgent">Тип агента пользователя</param>
        /// <param name="user">Данные о пользователе</param>
        /// <param name="now">Сейчас</param>
        /// <param name="minutes">Срок жизни жетона в минутах</param>
        /// <returns>JWT жетон</returns>
        private string GenerateToken(string userAgent, UserModel user, 
            DateTime now, double minutes)
        {

            // Создаём обработчик эетона JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            // Кодируем секретный ключ
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);
            // Создаём описатель жетона
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                // Задаём издателя жетона
                Issuer = appSettings.JwtIssuer,
                // Задаём аудиторию "Mobile" или "Web"
                Audience = string.IsNullOrEmpty(userAgent) ? 
                    appSettings.JwtMobileAudience : appSettings.JwtWebAudience,
                // Задаём утверждения идентификации
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // Электронная почта
                    new Claim(ClaimTypes.Email, user.Email),
                    // Идентификатор
                    new Claim("Id", user.ID.ToString()),
                }),
                // Задаём время через которое истекает срок действия жетона
                Expires = now.AddMinutes(minutes),  // 2 минуты от текущего момента
                // Создаём ключ подписывания жетона
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // Создаём JWT жетон
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // Возвращаем жетон в текстовом виде
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Получить данные о пользователе по идентификатору подключения
        /// </summary>
        /// <param name="connectionId">Идентификатор подключения</param>
        /// <returns>Данные о пользователе с очищенным паролем или null, 
        /// при отсутствии или ошибке</returns>
        public UserModel GetUserByConnectionId(string connectionId)
        {
            // Возвращаем данные о поделючении из хранилища по идентификатору подключения.
            // Выбираем данные о пользователе, если они есть.
            return connectionsRepository.Get(x => 
                x.ConnectionID == connectionId, includeProperties: "User")
                .SingleOrDefault()?.User;
        }
    }
}
