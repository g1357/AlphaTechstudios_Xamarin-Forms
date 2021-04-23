using ChatApp.Mobile.Models;
using ChatApp.Mobile.Services.Interfaces;
using Newtonsoft.Json;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Ядро сервисов мобильного приложения
/// </summary>
namespace ChatApp.Mobile.Services.Core
{
    /// <summary>
    /// Базовый сервис для остальных сервисов
    /// </summary>
    public class BaseService
    {
        // Создаём простой системный семафор, позволяющий доступ только из одного потока
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        // Сервис сессий
        protected readonly ISessionService SessionService;
        // Сервис навигации
        private readonly INavigationService navigationService;

        /// <summary>
        /// Gets or sets the base address.
        /// </summary>
        protected string BaseAddress { get; set; }

        /// <summary>
        /// Gets or sets the BaseRoute.
        /// </summary>
        protected string BaseRoute { get; set; }

        /// <summary>
        /// Вернуть базовый URL
        /// Gets the base address.
        /// </summary>
        protected string BaseUrl => $"{BaseAddress}{BaseRoute}/";

        /// <summary>
        /// Конструктор базового сервиса с одним параметром.
        /// </summary>
        /// <param name="sessionService">Сервис сессий</param>
        public BaseService(ISessionService sessionService)
        {
            // Инициируем пути
            InitRoutes();
            // Сохраняем сервис сессий для дальнейшего использования
            SessionService = sessionService;
        }

        /// <summary>
        /// Конструтор базового сервиса с двумя параметрами
        /// </summary>
        /// <param name="sessionService">Сервис сессий</param>
        /// <param name="navigationService">Сервис навигации</param>
        public BaseService(ISessionService sessionService, INavigationService navigationService)
        {
            // Инициируем пути
            InitRoutes();
            // Сохраняем сервис сессий для дальнейшего использования
            SessionService = sessionService;
            // Сохраняем сервис навигации для дальнейшего использования
            this.navigationService = navigationService;
        }


        /// <summary>
        /// Gets httpClient.
        /// </summary>
        /// <returns>The hhtpClient.</returns>
        protected async Task<HttpClient> GetClient(bool withoutToken = false)
        {
            return await GetClient(BaseUrl, withoutToken);
        }

        /// <summary>
        /// Инициировать пути
        /// </summary>
        private void InitRoutes()
        {
            // Задаём базовый путь
            BaseRoute = "/api";
            // Задаём базовый адрес
            //BaseAddress = "http://192.168.1.36:45459";
            // Для эмклятора Android задаём IP адрес
            BaseAddress = "http://10.0.2.2:45459";
        }

        /// <summary>
        /// Получить обновлённого клиента HTTP
        /// </summary>
        /// <returns></returns>
        private HttpClient GetClientRefresh()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            return client;
        }

        /// <summary>
        /// Gets httpClient from URL.
        /// </summary>
        /// <param name="baseUrl">The url.</param>
        /// <returns>The httpClient.</returns>
        protected async Task<HttpClient> GetClient(string baseUrl, bool withoutToken = false)
        {
            // Флаг "Освобождать семафор при ошибке".
            // Начальное значение "не освобождать" так как семафор не используется
            bool release = false;
            // Создаём слиента HTTP
            HttpClient client = new HttpClient();
            // Задаём клиенту базовый адрес
            client.BaseAddress = new Uri(baseUrl);

            if (withoutToken) // Если запрос должен передаваться без жетона, то
            {
                // Возвращаем созданного клиента
                return client;
            }
            // Получаем данные о пользователе из защищённого хранилища приложения
            var user = await SessionService.GetConnectedUser();
            // Получаем данные о жетоне из защищённого хранилища приложения
            var token = await SessionService.GetToken();

            try
            {
                if (user != null) // Если пользователь подключён, то
                {
                    // Если срок действия жетона +30 секунд мнгьше текущих даты и времени, то
                    if (token.TokenExpireTime.AddSeconds(30) <= DateTime.UtcNow)
                    {   // Нужно получить новый жетон

                        // Ждём освобождения семафора и занимаем его
                        await semaphoreSlim.WaitAsync();
                        // Симафор используется, при ошибке его надо освободить, чтобы избежать клинча
                        release = true;
                        // Получаем данные о пользователе из защищённого хранилища приложения
                        user = await SessionService.GetConnectedUser();
                        // Получаем данные о жетоне из защищённого хранилища приложения
                        token = await SessionService.GetToken();

                        // Если срок действия жетона +30 секунд мнгьше текущих даты и времени, то
                        if (token.TokenExpireTime.AddSeconds(30) <= DateTime.UtcNow)
                        {
                            // Посылаем запрос вёб сервису на обновление жетона доступа 
                            //и получаем данные о пользователе с очищенным паролем или null, при ошибке
                            user = await PostRefresh<UserModel, string>("Users/refresh", token.RefreshToken);
                            if (user != null) // Если получение данных прощло успешно, то
                            {
                                // Сохраняем в защищённом хранилище приложения данные о пользователе
                                await SessionService.SetConnectedUser(user);
                                // Сохраняем в защищённом хранилище приложения данные о жетоне
                                await SessionService.SetToken(new TokenModel
                                {
                                    Token = user.Token,
                                    RefreshToken = user.RefreshToken,
                                    TokenExpireTime = user.TokenExpireTimes
                                });
                                // Получаем данные о жетоне из защищённого хранилища приложения
                                token = await SessionService.GetToken();
                            }
                            else
                            {
                                // Очищаем данные о подключёном пользователе в защищённом хранилище
                                await SessionService.SetConnectedUser(null);
                                // Очищаем данные о жетоне в защищённом хранилище
                                await SessionService.SetToken(null);
                                // Переходим на страницу ввхлда в систему
                                await navigationService.NavigateAsync("../LoginPage");
                            }
                        }
                        // Освоюождаем семафор
                        semaphoreSlim.Release();
                        // Семафор освобождён и меняем значение флага
                        release = false;
                    }
                    // Устанавливаем жетон в заголовке запроса HTTP
                    client.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", token.Token);
                }
            }
            catch (Exception exp)
            {
                // Если семафор нужно освободить, то
                if (release)
                {
                    // Освобождаем семафор
                    semaphoreSlim.Release();
                }
            }
            // Возвращаем клиента для работы с вёб сервисом
            // В зависимости от входного параметра с жетоном или без
            return client;
        }

        /// <summary>
        /// Gets the response from url.
        /// </summary>
        /// <param name="url">The service url.</param>
        /// <returns>The reponse.</returns>
        protected async Task Get(string url)
        {
            using (HttpClient client = await GetClient())
            {
                try
                {
                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        // TODO log.
                    }
                }
                catch (HttpRequestException ex)
                {
                    // TODO log.
                }
            }
        }

        /// <summary>
        /// Gets reponse from url.
        /// </summary>
        /// <typeparam name="T">The reponse type.</typeparam>
        /// <param name="url">The service url.</param>
        /// <returns>The service's reponse.</returns>
        protected async Task<T> Get<T>(string url, bool withoutToken = false)
        {
            using (HttpClient client = await GetClient(withoutToken))
            {
                try
                {
                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        return default;
                    }
                    else
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(result);
                    }
                }
                catch (HttpRequestException exp)
                {
                    return default;
                }
            }
        }

        /// <summary>
        /// Posts the data.
        /// </summary>
        /// <typeparam name="T">The response Type</typeparam>
        /// <typeparam name="M">The model Type</typeparam>
        /// <param name="url">The service url.</param>
        /// <param name="model">The model object.</param>
        /// <returns>The response.</returns>
        protected async Task<T> Post<T, M>(string url, M model)
        {
            var content = JsonConvert.SerializeObject(model);
            HttpContent contentPost = new StringContent(content, Encoding.UTF8, "application/json");
            using (HttpClient client = await GetClient())
            {
                try
                {
                    var response = await client.PostAsync(url, contentPost);

                    if (!response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();

                        return default;
                    }
                    else
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(result);
                    }
                }
                catch (Exception exp)
                {
                    return default;
                }
            }
        }

        protected async Task<bool> Post(string url, MultipartFormDataContent model)
        {
            using (HttpClient client = await this.GetClient())
            {
                try
                {
                    var response = await client.PostAsync(url, model);


                    if (!response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        return false;
                    }
                    else
                    {
                        var result = await response.Content.ReadAsByteArrayAsync();
                        return true;
                    }
                }
                catch (Exception exp)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="M"></typeparam>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected async Task<T> PostRefresh<T, M>(string url, M model)
        {
            // Сериализуем данные в JSON
            var content = JsonConvert.SerializeObject(model);
            // Создаём контент HTTP
            HttpContent contentPost = new StringContent(content, Encoding.UTF8, "application/json");
            // Создаём клиента HTTP
            using (HttpClient client = this.GetClientRefresh())
            {
                try
                {
                    // Отправка запроса POST по заданному URL и с указанным содержимым
                    var response = await client.PostAsync(url, contentPost);

                    // Если код состояния ответа не успешный, то
                    if (!response.IsSuccessStatusCode)
                    {
                        // Получаем в строковом формате содержимое тела ответа
                        var result = await response.Content.ReadAsStringAsync();
                        // Возвращаем значение по умолчанию типа результата
                        return default(T);
                    }
                    else
                    {
                        // Получаем в строковом формате содержимое тела ответа
                        var result = await response.Content.ReadAsStringAsync();
                        // Возвращаем десериализованный из JSON тело ответа на запрос
                        return JsonConvert.DeserializeObject<T>(result);
                    }
                }
                catch (Exception exp)
                {
                    // Возвращаем значение по умолчанию типа результата
                    return default(T);
                }
            }
        }

        /// <summary>
        /// Puts the data.
        /// </summary>
        /// <typeparam name="T">The response Type</typeparam>
        /// <typeparam name="M">The model Type</typeparam>
        /// <param name="url">The service url.</param>
        /// <param name="model">The model object.</param>
        /// <returns>The response.</returns>
        protected async Task<T> PutAsync<T, M>(string url, M model)
        {
            var content = JsonConvert.SerializeObject(model);
            HttpContent contentPost = new StringContent(content, Encoding.UTF8, "application/json");
            using (HttpClient client = await GetClient())
            {
                try
                {
                    var response = await client.PutAsync(url, contentPost);

                    if (!response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        return default;
                    }
                    else
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(result);
                    }
                }
                catch (Exception exp)
                {
                    return default;
                }
            }
        }

    }
}
