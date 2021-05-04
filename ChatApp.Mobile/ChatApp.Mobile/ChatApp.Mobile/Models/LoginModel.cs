using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Mobile.Models
{
    /// <summary>
    ///  Модель данных входа в систему.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Электронная почта пользователя.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string Password { get; set; }
    }
}
