using ChatApp.Managers.Extensions;
using ChatApp.Managers.Interfaces;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Контроллеры Web API
/// </summary>
namespace ChatApp.WebAPI.Controllers
{
    /// <summary>
    /// Контроллер работы с пользователями
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // Менеджер пользователей
        private readonly IUsersManager usersManager;

        /// <summary>
        /// Конструктор контроллера пользователей.
        /// </summary>
        /// <param name="usersManager">Менеджер пользователей</param>
        public UsersController(IUsersManager usersManager)
        {
            // Сохраняем менеджера пользователей для дальнейшего использования
            this.usersManager = usersManager;
        }

        /// <summary>
        /// Добавить пользователя.
        /// </summary>
        /// <param name="userModel">Данные о пользователе. Берутся из тела запроса.</param>
        [AllowAnonymous] // Доступ к методу возможен без проверки
        [HttpPost("Insert")]
        public IActionResult InserUser([FromBody] UserModel userModel)
        {
            try // Начало блока перехвата исключений
            {
                // Добавляем пользователя в хранилище
                var response = usersManager.InsertUser(userModel);
                // Если пользователь добавлен (ID установлен в значение больше 0)
                if (userModel.ID > 0)
                {
                    // Подучаем данные о пользователе из хранилища по идентификатору
                    var user = usersManager.GetUserById(userModel.ID);
                }
                // Возвращаем "Ok" и bltynbabrfnjh gjkmpjdfntkz
                return Ok(response);
            }
            catch (Exception exp) // Перехватываем возникшие исключения
            {
                // Возвращаем "Плохой запрос"
                return BadRequest();
            }
        }

        /// <summary>
        /// Войти в систему.
        /// </summary>
        /// <param name="loginModel">Данные для входа. Берутся из тела запроса.</param>
        /// <returns>Результат действия</returns>
        [AllowAnonymous] // Доступ к методу возможен без проверки
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            try // Начало блока перехвата исключений
            {
                // Ппроверяем данные пользователя по данным из хранилища
                var userModel = usersManager.Login(loginModel, HttpContext);

                // Если данные пользователя не получениы или жетон доступа не пустой, то
                if (userModel == null || string.IsNullOrEmpty(userModel.ValidationToken))
                {
                    // Возвращаем "Плохой запрос"
                    // с сообщением "Имя пользователя и пароль неправильные"
                    return BadRequest(new { message = "Login or password is incorrect" });
                }

                // Возвращаем "Ok" и данные опользователе с очищенным паролем
                return Ok(userModel.WithoutPassword());
            }
            catch (Exception exp) // Перехватываем возникшие исключения
            {
                // Возвращаем "Плохой запрос"
                return BadRequest();
            }
        }

        /// <summary>
        /// Обновить жетон доступа по жетону обновления.
        /// </summary>
        /// <param name="refreshToken">Жетон обновления. Берётся из тела запроса.</param>
        /// <returns>Результат действия</returns>
        [AllowAnonymous] // Доступ к методу возможен без проверки
        [HttpPost("refresh")]
        public IActionResult RefreshToken([FromBody] string refreshToken)
        {
            try // Начало блока перехвата исключений
            {
                // Обновляем жетон доступа и получаем обновлённые данные о аользователе
                var userModel = usersManager.RefreshToken(refreshToken, HttpContext);
                // Если операция неуспешна, то
                if (userModel == null)
                {
                    // TODO Log Error Refresh Token not found.
                    // Возвращаем "Пользователь не авторизован"
                    return Unauthorized();
                }
                // ВОзвращаем "Ok" и данные о пользователе с очищенным паролем
                return Ok(userModel);
            }
            catch (Exception exp) // Перехватываем возникшие исключения
            {
                // Возвращаем "Плохой запрос"
                return BadRequest();
            }
        }

        /// <summary>
        /// Получить данные о друзьях пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя. Берётся из строки запроса.</param>
        /// <returns>Результат действия</returns>
        [HttpGet("getMyFriends/{userId}")]
        public IActionResult GetMyFriends(long userId)
        {
            try // Начало блока перехвата исключений
            {
                // Возвращаем "Ok" и коллекцию данных о моих друзьях
                // в виде даннфх о пользователях с очищенными паролями
                return Ok(usersManager.GetMyFriends(userId));
            }
            catch (Exception exp) // Перехватываем возникшие исключения
            {
                // Возвращаем "Плохой запрос"
                return BadRequest();
            }
        }
    }
}
