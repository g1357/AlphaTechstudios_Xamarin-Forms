using System;
namespace ChatApp.Mobile.Models
{
    /// <summary>
    /// Базовая модель данных.
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// Иднтификатор сущности.
        /// Тип: динный целый
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Дата и время создания сущности.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Дата и время последней модификации сущности.
        /// </summary>
        public DateTime ModificationDate { get; set; }
    }
}
