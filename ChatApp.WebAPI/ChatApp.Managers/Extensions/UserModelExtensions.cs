using ChatApp.Models;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Расширения для Менеджеров
/// </summary>
namespace ChatApp.Managers.Extensions
{
    /// <summary>
    /// Расширения модели данных о пользователях
    /// </summary>
    public static class UserModelExtensions
    {
        /// <summary>
        /// Очистка пароля в наборе пользователей
        /// </summary>
        /// <param name="users">Коллекция данных о пользователях</param>
        /// <returns>Коллекция данных о пользователях с очищенными паролями</returns>
        public static IEnumerable<UserModel> WithoutPassword(this IEnumerable<UserModel> users)
        {
            // Возвращаем коллекцию данных о пользователях, в которой к каждому элементу данных
            // о пользователях применём метод очистки пароля.
            return users.Select(x => x.WithoutPassword());
        }

        /// <summary>
        /// Очистка пароля в данных о пользователе
        /// </summary>
        /// <param name="userModel">Данные о пользователе</param>
        /// <returns>Данные о пользователе с очищенным паролем</returns>
        public static UserModel WithoutPassword(this UserModel userModel)
        {
            if (userModel == null) // Если данные не существуют
            {
                return null; // Вернуть null
            }
            userModel.Password = null; // Очислить поле пароля
            return userModel; // Вернуть данные о пользователе
        }
    }
}
