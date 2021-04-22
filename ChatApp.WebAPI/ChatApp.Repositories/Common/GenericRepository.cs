using ChatApp.DL;
using ChatApp.Models;
using ChatApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

/// <summary>
/// Общие классы для хранилищей приложения
/// </summary>
namespace ChatApp.Repositories.Common
{
    /// <summary>
    /// Обобщённое хранилище.
    /// </summary>
    /// <typeparam name="TEntity">Тип сужностей хранилища</typeparam>
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : BaseModel
    {
        // Контекст базы данных приложения
        protected ChatAppContext DbCxt;
        // Набор данных
        protected DbSet<TEntity> DbSet;

        /// <summary>
        /// Конструктор обобщённого хранилища
        /// </summary>
        /// <param name="context">Контекст дбазы анных приложения</param>
        public GenericRepository(ChatAppContext context)
        {
            // Сохраняем контекст базы данных приложения
            DbCxt = context;
            // Выбираем набор данных для типа сущности из контекста
            DbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// Удалить сущность а наборе данных по идентификатору сущности.
        /// </summary>
        /// <param name="id">Идентификатор записи (сущности)</param>
        public void Delete(object id)
        {
            // Находим сущность в наборе данных по идентификатору
            TEntity entityToDelete = DbSet.Find(id);
            // Удаляем сущность
            this.Delete(entityToDelete);
        }

        /// <summary>
        /// Удалить заданную сущность в наборе данных.
        /// </summary>
        /// <param name="entityToDelete">Сущность для удаления</param>
        public void Delete(TEntity entityToDelete)
        {
            // Если состояние сущности - отключено, то
            if (DbCxt.Entry(entityToDelete).State == EntityState.Detached)
            {
                // Подключить сущность к набору данных
                DbSet.Attach(entityToDelete);
            }

            // Удалить заданную сущность из набора данных
            DbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// Получить коллекцию сущностей по заданым аргументам.
        /// </summary>
        /// <param name="filter">Фильтр отбора сущностей</param>
        /// <param name="orderBy">Правила упорядочивания</param>
        /// <param name="includeProperties">Включённые свойства</param>
        /// <returns>Коллекция сущностей</returns>
        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
            string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        /// <summary>
        /// Получить сущность по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность</returns>
        public TEntity GetByID(object id)
        {
            // Вернуть найденную по идентификатору сущность
            return DbSet.Find(id);
        }

        /// <summary>
        /// Вставить сущность в набор данных.
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <returns>Идентификатор добавленной сущности</returns>
        public long Insert(TEntity entity)
        {
            // Добавляем сущность в набор данных
            DbSet.Add(entity);
            // Вохвращаем идентификатор сущности
            return entity.ID;
        }

        /// <summary>
        /// Обновить сущность в наборе данных.
        /// </summary>
        /// <param name="entityToUpdate">Сущность для обновления</param>
        public void Update(TEntity entityToUpdate)
        {
            // Подключаем обновляемую сущность к набору данных
            DbSet.Attach(entityToUpdate);
            // Устанавливаем состояние сущности в "изменена"
            DbCxt.Entry(entityToUpdate).State = EntityState.Modified;
        }

        /// <summary>
        /// Обновить коллекцию сущностей.
        /// </summary>
        /// <param name="entitiesToUpdate">Коллекция суностей для обновления</param>
        public void UpdateAll(IEnumerable<TEntity> entitiesToUpdate)
        {
            // Перебираем сущности в коллекции
            foreach (var entity in entitiesToUpdate)
            {
                // Обновляем каждую сущность в наборе данных
                this.Update(entity);
            }
        }
    }
}
