using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Application Data Layer.
/// Слой данных приложения
/// </summary>
namespace ChatApp.DL
{
    /// <summary>
    /// Контекст данных
    /// </summary>
    public class ChatAppContext : DbContext
    {
        /// <summary>
        /// Конструктор объекта контекста данных
        /// </summary>
        /// <param name="dbContextOptions"></param>
        public ChatAppContext(DbContextOptions<ChatAppContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        /// <summary>
        /// Набор данных о пользователях
        /// </summary>
        public DbSet<UserModel> Users { get; set; }

        /// <summary>
        /// Набор данных о друзьях
        /// </summary>
        public DbSet<FriendModel> Friends { get; set; }

        /// <summary>
        /// Набор данных о диалогах
        /// </summary>
        public DbSet<ConversationModel> Conversations { get; set; }

        /// <summary>
        /// Набор данных об ответах на диалоги
        /// </summary>
        public DbSet<ConversationReplyModel> ConversationReplies { get; set; }

        /// <summary>
        /// Набор данных о подключениях
        /// </summary>
        public DbSet<ConnectionModel> Connections { get; set; }

        /// <summary>
        /// Набор данных о жетонах обновления жетонов доступа
        /// </summary>
        public DbSet<RefreshTokenModel> RefreshTokens { get; set; }

        /// <summary>
        /// Переопределение метода базового класса, чтобы настроить базу данных (и другие 
        /// параметры) для использования в этом контексте.
        /// Этот метод вызывается для каждого экземпляра создаваемого контекста.
        /// Базовая реализация ничего не делает (поэтому не используем ": base()").
        /// </summary>
        /// <param name="optionsBuilder">Построитель опций, используетсяй для создания или
        /// изменения параметров для этого контекста. Базы данных (и другие
        /// расширения) обычно определяют методы расширения для этого объекта, 
        /// которые позволяют настроить контекст.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Задаёт отключение отслеживания изменений для запросов LINQ.
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
}
