namespace ChatApp.Models.Settings
{
    /// <summary>
    /// Гастройки приложения
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Исходный URL-алрес
        /// </summary>
        public string OriginUrl { get; set; }

        /// <summary>
        /// Секретный ключ
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Эмитент JWT
        /// </summary>
        public string JwtIssuer { get; set; }

        /// <summary>
        /// Мобильная ацдитория JWT
        /// </summary>
        public string JwtMobileAudience { get; set; }

        /// <summary>
        ///  Вёб-аудитория JWT
        /// </summary>
        public string JwtWebAudience { get; set; }
    }
}
