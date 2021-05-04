using System;
using System.Collections.Generic;

namespace ChatApp.Mobile.Models
{
    /// <summary>
    /// Модель данных о пользователе.
    /// Базируется на базовой моднли данных.
    /// </summary>
    public class UserModel : BaseModel
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Адрес электронной почты пользователя.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Хэш пароля пользователя.
        /// Поле возвращается очищенным во многих запросах.
        /// Ни пароль, ни его хэш никоглда не возвращаются сервисом.
        /// </summary>
        public string Password { get; set; }

        public ICollection<ConversationModel> Conversations { get; set; }

        /// <summary>
        /// Жетон обновления жетона доступа.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Дата и время истечения срока действия жетона доступа.
        /// </summary>
        public DateTime TokenExpireTimes { get; set; }

        /// <summary>
        /// Жетон доступа.
        /// </summary>
        public string Token { get; set; }
    }
}
