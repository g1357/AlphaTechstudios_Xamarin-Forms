using ChatApp.Models;
using System.Collections.Generic;

/// <summary>
/// Интерфейсы Менеджеров приложения
/// </summary>
namespace ChatApp.Managers.Interfaces
{
    /// <summary>
    /// Интерфейс Менеджера диалогов
    /// </summary>
    public interface IConversationsManager
    {
        /// <summary>
        /// Получить все диалоги пользователя по его дентификатору
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Коллекция данных о диалогах</returns>
        IEnumerable<ConversationModel> GetAllConversationsByUserId(long userId);

        /// <summary>
        /// Получить данные о диалоге пользователя с другим пользователем по их идентификаторам
        /// </summary>
        /// <param name="firstUser">Идентификатор первого пользователя</param>
        /// <param name="secondUser">Идентификатор первого пользователя</param>
        /// <returns>Данные о диалоге</returns>
        ConversationModel GetConversationByUsersId(long firstUser, long secondUser);

        /// <summary>
        /// Добавить или обновить диалог двух пользователей по их идентификаторам
        /// </summary>
        /// <param name="firstUser">Идентификатор первого пользователя</param>
        /// <param name="secondUser">Идентификатор первого пользователя</param>
        /// <returns>Созданные или обновлённые данные о диалоге</returns>
        long AddOrUpdateConversation(long firstUser, long secondUser);

        /// <summary>
        /// Добавить ответ в диалог пользователя
        /// </summary>
        /// <param name="message">Сообщение, добавляемое в диалог</param>
        /// <param name="conversationId">Идентификатор диалога</param>
        /// <param name="userID">Идентификатор пользователя</param>
        void AddReply(string message, long conversationId, long userID);
    }
}
