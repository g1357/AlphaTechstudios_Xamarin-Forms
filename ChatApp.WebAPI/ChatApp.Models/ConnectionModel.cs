/// <summary>
/// Модели данных приложения
/// </summary>
namespace ChatApp.Models
{
    /// <summary>
    /// Модель данных подключения
    /// </summary>
    public class ConnectionModel : BaseModel
    {
        /// <summary>
        /// Идентификатор подключения
        /// </summary>
        public string ConnectionID { get; set; }

        /// <summary>
        /// Агент пользователя
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Флаг наличия активного подключения
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// Данные о пользователе
        /// </summary>
        public UserModel User { get; set; }

    }
}
