/// <summary>
/// Модели данных приложения
/// </summary>
namespace ChatApp.Models
{
    /// <summary>
    /// Модель данных о друге
    /// </summary>
    public class FriendModel : BaseModel
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// Идентификатор друга пользователя
        /// </summary>
        public long UserFriendId { get; set; }

        /// <summary>
        /// Данные о доуге пользователя
        /// </summary>
        public UserModel UserFriend { get; set; }
    }
}
