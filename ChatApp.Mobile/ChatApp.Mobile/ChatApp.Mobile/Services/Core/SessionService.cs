using ChatApp.Mobile.Models;
using ChatApp.Mobile.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

/// <summary>
/// Ядро сервисов мобильного приложения
/// </summary>
namespace ChatApp.Mobile.Services.Core
{
    /// <summary>
    /// Сервис сессий
    /// </summary>
    public class SessionService : ISessionService
    {
        /// <summary>
        /// Gets connecetd user From SecuredStorage.
        /// </summary>
        /// <returns>The current user Model.</returns>
        public async Task<UserModel> GetConnectedUser()
        {
            string content = string.Empty;
            try //
            {
                // Получаем сериализованные данные подключёного пользователя из безопасного хранилища
                content = await SecureStorage.GetAsync("ConnectedUser");
            }
            catch (Exception exp)
            {

            }
            // Если контент не пустой,
            // то десериализуем данные из JSON в объект данных о пользователе
            return string.IsNullOrEmpty(content) ? null 
                : JsonConvert.DeserializeObject<UserModel>(content);
        }

        /// <summary>
        /// Gets Token From SecuredStorage.
        /// </summary>
        /// <returns>The current user Model.</returns>
        public async Task<TokenModel> GetToken()
        {
            // Инициализируем контент пустой строкой
            string content = string.Empty;
            try
            {
                // Получаем сериализованные данные о жетоне из безопасного хранилища
                content = await SecureStorage.GetAsync("Token");
            }
            catch (Exception exp)
            {

            }
            // Если контент не пустой,
            // то десериализуем данные из JSON в объект данных о жетоне
            return string.IsNullOrEmpty(content) ? null 
                : JsonConvert.DeserializeObject<TokenModel>(content);
        }

        /// <summary>
        /// Save the conneceted user in the secured storage.
        /// </summary>
        /// <param name="userModel">The connecetd userModel.</param>
        /// <returns>This method returns nothing.</returns>
        public async Task SetConnectedUser(UserModel userModel)
        {
            // Сериализуем данные о пользователе в JSON
            string content = JsonConvert.SerializeObject(userModel);
            // Сохраняем данные о пользователе в безопасном хранилище
            // с ключом "ConnectedUser" (подключённый пользователь)
            await SecureStorage.SetAsync("ConnectedUser", content);
        }

        /// <summary>
        /// Sets user authentication and refresh tokens.
        /// </summary>
        /// <param name="tokenModel">The token model.</param>
        /// <returns>This method returns nothing</returns>
        public async Task SetToken(TokenModel tokenModel)
        {
            // Сериализуем данные о жетоне в JSON
            string content = JsonConvert.SerializeObject(tokenModel);
            // Сохраняем данные о жетоне в безопасном хранилище
            // с ключом "Token" (жетон)
            await SecureStorage.SetAsync("Token", content);
        }

        /// <summary>
        /// Delete the user data from the secured storage.
        /// </summary>
        /// <returns>This methode returns nothing.</returns>
        public async Task LogOut()
        {
            // Очищаем в безопасном хранилище значение подключённого пользователя
            await SecureStorage.SetAsync("ConnectedUser", string.Empty);
            // Очищаем в безопасном хранилище значение жетона
            await SecureStorage.SetAsync("Token", string.Empty);
        }

        /// <summary>
        /// Gets the key value from data
        /// </summary>
        /// <typeparam name="T">The data with class type.</typeparam>
        /// <param name="key">The target key.</param>
        /// <returns>The key value.</returns>
        public T GetValue<T>(string key) where T : class
        {
            // Получаем персональные значения для указанного ключа
            // со значением по умолчанию null
            var content = Preferences.Get(key, null);
            // Еcли контент не пустой, по выполняем десериализацию из JSON
            return string.IsNullOrEmpty(content) ? null 
                : JsonConvert.DeserializeObject<T>(content);
        }

        /// <summary>
        /// Gets the key value from data
        /// </summary>
        /// <typeparam name="T">The data with struct type.</typeparam>
        /// <param name="key">The target key.</param>
        /// <returns>The key value.</returns>
        public T GetStructValue<T>(string key) where T : struct
        {
            // Получаем персональные настройки для указанного ключа
            // со значением по умолчанию null
            var content = Preferences.Get(key, null);
            // Еcли контент не пустой, по выполняем десериализацию из JSON
            return JsonConvert.DeserializeObject<T>(content);
        }

        /// <summary>
        /// Sets the value for the target key.
        /// </summary>
        /// <typeparam name="T">The data type.</typeparam>
        /// <param name="key">The target key.</param>
        /// <param name="value">Thge value to save.</param>
        public void SetValue<T>(string key, T value)
        {
            // Сериализуем в JSON указанное значение
            string content = JsonConvert.SerializeObject(value);
            // Сохраняем сериализованные данные в персональных настройка
            // с казанным ключок
            Preferences.Set(key, content);
        }
    }
}
