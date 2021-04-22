namespace ChatApp.Models
{
    /// <summary>
    /// Модель данных жетона обновления жетона оступа
    /// </summary>
    public class RefreshTokenModel : BaseModel
    {
        /// <summary>
        /// Адрес электронной почты пользователя
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Жетон обновления жетона доступа
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// IP адрес
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Агент пользователя
        /// </summary>
        public string UserAgent { get; set; }
    }
}
