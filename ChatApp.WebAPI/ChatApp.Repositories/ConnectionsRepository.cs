using ChatApp.DL;
using ChatApp.Models;
using ChatApp.Repositories.Common;
using ChatApp.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Хранилища приложения
/// </summary>
namespace ChatApp.Repositories
{
    /// <summary>
    /// Хранилище подключений.
    /// На основе обобщённого хранилища для модели данных подключения
    /// и интерфейса хранилища подключений
    /// </summary>
    public class ConnectionsRepository 
        : GenericRepository<ConnectionModel>, IConnectionsRepository
    {
        /// <summary>
        /// Конструктор хранилища подключений
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public ConnectionsRepository(ChatAppContext context) 
            : base(context) // Передаём в базовый класс
        {
        }
    }
}
