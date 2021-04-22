using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Модели данных приложения
/// </summary>
namespace ChatApp.Models
{
    /// <summary>
    /// Базовая модель данных
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// Идентификатор модели данных
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Дата и время создания сущности
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Дата и время последней модификации данных
        /// </summary>
        public DateTime ModificationDate { get; set; }

    }
}
