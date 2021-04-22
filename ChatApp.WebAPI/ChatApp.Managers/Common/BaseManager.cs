using ChatApp.Repositories.Interfaces;

/// <summary>
/// Общие классы Менеджеров
/// </summary>
namespace ChatApp.Managers.Common
{
    /// <summary>
    /// Базовый класс для Менеджера
    /// </summary>
    public class BaseManager
    {
        /// <summary>
        /// Поле, содержащее обработчик Единицы работы
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Конструктор базоваго класса Менеджера
        /// </summary>
        /// <param name="unitOfWork">Обработчик единицы работы</param>
        public BaseManager(IUnitOfWork unitOfWork)
        {
            // Задаём обработчика Единицы работы
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Свойство "только для чтения" возвращает обработчик Единицы работы
        /// </summary>
        protected IUnitOfWork UnitOfWork => unitOfWork;
    }
}
