using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Модели данных приложения
/// </summary>
namespace ChatApp.Models
{
    /// <summary>
    /// Модель данных о пользователе
    /// </summary>
    public class UserModel : BaseModel
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Адрес электронной почты пользователя
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Жетон проверки пользователя
        /// </summary>
        public string ValidationToken { get; set; }

        /// <summary>
        /// Модификатор входа хэш-функции для кодирования пароля
        /// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Набор данных о диалогах
        /// </summary>
        public ICollection<ConversationModel> Conversations { get; set; }

        /// <summary>
        /// Набор данных о друзьях пользователя
        /// </summary>
        public ICollection<FriendModel> Friends { get; set; }

        /// <summary>
        /// Набор данных о подключениях
        /// </summary>
        public ICollection<ConnectionModel> Connections { get; set; }

        /// <summary>
        /// Жетон обновления жетона доступа
        /// </summary>
        [NotMapped]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Время истечения срока действия жетона доступа
        /// </summary>
        [NotMapped]
        public DateTime TokenExpireTimes { get; set; }

        /// <summary>
        /// Жетон доступа
        /// </summary>
        [NotMapped]
        public string Token { get; set; }
    }
}
