/// <summary>
/// Модели данных приложения
/// </summary>
namespace ChatApp.Models
{
    /// <summary>
    /// Модель данных входа в систему
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Адрес электронной почты
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }
    }
}
