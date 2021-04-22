using ChatApp.Models;

/// <summary>
/// Интерфейсы хранилищ приложения
/// </summary>
namespace ChatApp.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс хранилища ответов диалогов.
    /// На основе обобщённого интерфейса хранилища для модели данных 
    /// ответов диалогов.
    /// </summary>
    public interface IConversationRepliesRepository 
        : IGenericRepository<ConversationReplyModel>
    {
    }
}
