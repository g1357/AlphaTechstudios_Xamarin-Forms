using ChatApp.DL;
using ChatApp.Repositories.Interfaces;
using System;

/// <summary>
/// Общие классы для хранилищ приложения
/// </summary>
namespace ChatApp.Repositories.Common
{
    /// <summary>
    /// Обработчик единицы работы
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {

        // Контекст базы данных приложения
        private ChatAppContext dbContext;
        // Утилизирован?
        private bool disposed = false;

        /// <summary>
        /// Конструктор обработчика единица работы
        /// </summary>
        /// <param name="context"></param>
        public UnitOfWork(ChatAppContext context)
        {
            // Устанавливаем контекст базы данных
            dbContext = context;
        }

        /// <summary>
        /// Подтвердить изменения.
        /// </summary>
        public void Commit()
        {
            // Сохраняем все изменения в контекстк базы данных
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Освободить
        /// </summary>
        public void Dispose()
        {
            // Освободить
            Dispose(true);
            // Запрос к сборщику мусора CLR не вызывать финализатор для данного объекта
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Освободить.
        /// </summary>
        /// <param name="isDisposed">Освобождён?</param>
        private void Dispose(bool isDisposed)
        {
            if (!disposed)
            {
                if (!isDisposed)
                {
                    // Освободить выделенные ресурлы для данного контекста базы данных 
                    dbContext.Dispose();
                }
            }

            disposed = true;
        }
    }
}
