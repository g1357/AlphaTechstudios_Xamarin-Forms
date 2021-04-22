using ChatApp.Managers.Common;
using ChatApp.Managers.Interfaces;
using ChatApp.Models;
using ChatApp.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Мнеджеры приложения
/// </summary>
namespace ChatApp.Managers
{
    /// <summary>
    /// Менеджер диалогов
    /// </summary>
    public class ConversationsManager : BaseManager, IConversationsManager
    {
        /// <summary>
        /// Хранилище диалогов
        /// </summary>
        private readonly IConversationsRepository conversationsRepository;
        /// <summary>
        /// Хранилище ответов в диалогах
        /// </summary>
        private readonly IConversationRepliesRepository conversationRepliesRepository;

        /// <summary>
        /// Конструктор менеджера диалогов
        /// </summary>
        /// <param name="unitOfWork">Обработчик единицы работы</param>
        /// <param name="conversationsRepository">Хранилище диалогов</param>
        /// <param name="conversationRepliesRepository">Хранилиже ответов в диалогах</param>
        public ConversationsManager(IUnitOfWork unitOfWork,
            IConversationsRepository conversationsRepository,
            IConversationRepliesRepository conversationRepliesRepository)
            : base(unitOfWork) // Передаём обработчик единицы работы в базовый класс
        {
            // Задаём хранидище диалогов
            this.conversationsRepository = conversationsRepository;
            // Задаём хранилиже ответов в диалогах
            this.conversationRepliesRepository = conversationRepliesRepository;
        }

        /// <summary>
        /// Получить все диалоги пользователя по его идентификатору
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Коллекция данных о диалогах</returns>
        public IEnumerable<ConversationModel> GetAllConversationsByUserId(long userId)
        {
            return conversationsRepository.Get(x => x.UserOneID == userId 
                || x.UserTwoID == userId, includeProperties: "ConversationReplies");
        }

        /// <summary>
        /// Получить данные о диалоге пользователя с другим пользователем по их идентификаторам
        /// </summary>
        /// <param name="firstUser">Идентификатор первого пользователя</param>
        /// <param name="secondUser">Идентификатор первого пользователя</param>
        /// <returns>Данные о диалоге или null</returns>
        public ConversationModel GetConversationByUsersId(long firstUser, long secondUser)
        {
            return conversationsRepository.Get(x => (x.UserOneID == firstUser 
                && x.UserTwoID == secondUser) 
                || (x.UserOneID == secondUser && x.UserTwoID == firstUser), 
                includeProperties: "ConversationsReplies").SingleOrDefault();
        }

        /// <summary>
        /// Добавить или обновить диалог двух пользователей по их идентификаторам
        /// </summary>
        /// <param name="firstUser">Идентификатор первого пользователя</param>
        /// <param name="secondUser">Идентификатор первого пользователя</param>
        /// <returns>Идентификатор созданные или обновлённые данные о диалоге</returns>
        public long AddOrUpdateConversation(long firstUser, long secondUser)
        {
            var now = DateTime.UtcNow; // Получаем текущие дату и время
            // Получаем данные о диалоге
            var conversation = GetConversationByUsersId(firstUser, secondUser);
            if (conversation == null) // Если диалога не существует
            {
                // Создаём данные о ноыом диалоге
                conversation = new ConversationModel
                {
                    UserOneID = firstUser,  // Первый пользователь
                    UserTwoID = secondUser, // Второй пользователь
                    CreationDate = now,     // Текущие дата и время создания
                    ModificationDate = now  // Текущие дата и время изменения
                };
                // Добавляем данные о диалоге в хранилище
                conversationsRepository.Insert(conversation);
             
            }
            else // Если диалог существует
            {
                // Изменяем дату изменения на текущую
                conversation.ModificationDate = DateTime.Now; // можно заменить на now
                // Обновляем данные о диалоге в хранилище
                conversationsRepository.Update(conversation);
            }

            // Подтверждаем Единицу работы
            UnitOfWork.Commit();
            // Возвращаем идентификатор диалога
            return conversation.ID;
        }

        /// <summary>
        /// Добавить ответ в диалог пользователя
        /// </summary>
        /// <param name="message">Сообщение, добавляемое в диалог</param>
        /// <param name="conversationId">Идентификатор диалога</param>
        /// <param name="userID">Идентификатор пользователя</param>
        public void AddReply(string message, long conversationId, long userID)
        {
            // Добавля в хранилище ответов в диалогвх новый ответ
            conversationRepliesRepository.Insert(new ConversationReplyModel
            {
                Content = message,  // Сообщение (ответ)
                ConversationID = conversationId,    // Идентификатор диалога
                CreationDate = DateTime.Now,    // Дата создания
                // нужно добавить зменение даты модификации
                SenderUserId = userID   // Идентификатор отправивщего пользователя
            });
            // Подтверждаем Единицу работы
            UnitOfWork.Commit();
        }

    }
}
