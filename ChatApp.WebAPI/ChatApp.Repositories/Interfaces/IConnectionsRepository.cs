using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Интерфейсы хранилищ приложения
/// </summary>
namespace ChatApp.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс хранилища подключений.
    /// На основе обобщённого интерфейса хранилища для модели данных подключения.
    /// </summary>
    public interface IConnectionsRepository : IGenericRepository<ConnectionModel>
    {
    }
}
