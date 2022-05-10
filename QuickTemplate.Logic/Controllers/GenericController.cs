//@BaseCode
//MdStart

using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace QuickTemplate.Logic.Controllers
{
    /// <summary>
    /// This class provides the CRUD operations for an entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for which the operations are available.</typeparam>
    public abstract partial class GenericController<TEntity> : ControllerObject, IDataAccess<TEntity>
        where TEntity : Entities.IdentityEntity, new()
    {
        protected enum ActionType
        {
            Insert,
            InsertArray,
            Update,
            UpdateArray,
            Delete,
            Save,
        }
        static GenericController()
        {
            BeforeClassInitialize();

            AfterClassInitialize();
        }
        static partial void BeforeClassInitialize();
        static partial void AfterClassInitialize();

        private DbSet<TEntity>? dbSet = null;
        public GenericController()
            : base(new DataContext.ProjectDbContext())
        {

        }
        public GenericController(ControllerObject other)
            : base(other)
        {

        }

        internal DbSet<TEntity> EntitySet
        {
            get
            {
                if (dbSet == null)
                {
                    if (Context != null)
                    {
                        dbSet = Context.GetDbSet<TEntity>();
                    }
                    else
                    {
                        using var ctx = new DataContext.ProjectDbContext();

                        dbSet = ctx.GetDbSet<TEntity>();

                    }
                }
                return dbSet;
            }
        }

        #region Count
        /// <summary>
        /// Gets the number of quantity in the collection.
        /// </summary>
        /// <returns>Number of entities in the collection.</returns>
        public virtual Task<int> CountAsync()
        {
            return EntitySet.CountAsync();
        }
        /// <summary>
        /// Returns the number of quantity in the collection based on a predicate.
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>Number of entities in the collection.</returns>
        public virtual Task<int> CountAsync(string predicate, params string[] includeItems)
        {
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in includeItems)
            {
                query = query.Include(includeItem);
            }
            return query.Where(predicate).CountAsync();
        }
        /// <summary>
        /// Returns the number of quantity in the collection based on a predicate.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>Number of entities in the collection.</returns>
        internal virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, params string[] includeItems)
        {
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in includeItems)
            {
                query = query.Include(includeItem);
            }
            return query.Where(predicate).CountAsync();
        }
        #endregion Count

        #region Queries
        /// <summary>
        /// Returns all interfaces of the entities in the collection.
        /// </summary>
        /// <returns>All interfaces of the entity collection.</returns>
        public virtual Task<TEntity[]> GetAllAsync()
        {
            return EntitySet.AsNoTracking().ToArrayAsync();
        }
        /// <summary>
        /// Returns all interfaces of the entities in the collection.
        /// </summary>
        /// <param name="includeItems">The include items</param>
        /// <returns>All interfaces of the entity collection (with include).</returns>
        public virtual Task<TEntity[]> GetAllAsync(params string[] includeItems)
        {
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in includeItems)
            {
                query = query.Include(includeItem);
            }
            return query.AsNoTracking().ToArrayAsync();
        }
        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>The filter result.</returns>
        public virtual Task<TEntity[]> QueryAsync(string predicate, params string[] includeItems)
        {
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in includeItems)
            {
                query = query.Include(includeItem);
            }
            return query.Where(predicate).ToArrayAsync();
        }
        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>The filter result.</returns>
        internal virtual Task<TEntity[]> QueryAsync(Expression<Func<TEntity, bool>> predicate, params string[] includeItems)
        {
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in includeItems)
            {
                query = query.Include(includeItem);
            }
            return query.Where(predicate).ToArrayAsync();
        }
        /// <summary>
        /// Returns the element of type T with the identification of id.
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <returns>The element of the type T with the corresponding identification.</returns>
        public virtual ValueTask<TEntity?> GetByIdAsync(int id)
        {
            return EntitySet.FindAsync(id);
        }
        /// <summary>
        /// Returns the element of type T with the identification of id.
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>The element of the type T with the corresponding identification (with includes).</returns>
        public virtual Task<TEntity?> GetByIdAsync(int id, params string[] includeItems)
        {
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in includeItems)
            {
                query = query.Include(includeItem);
            }
            return query.FirstOrDefaultAsync(e => e.Id == id);
        }
        #endregion Queries

        #region Action
        /// <summary>
        /// This method is called before an action is performed.
        /// </summary>
        /// <param name="actionType">Action types such as insert, edit, etc.</param>
        /// <param name="entity">The entity that the action affects.</param>
        protected virtual void ValidateEntity(ActionType actionType, TEntity entity)
        {

        }
        /// <summary>
        /// This method is called before an action is performed.
        /// </summary>
        /// <param name="actionType">Action types such as save, etc.</param>
        protected virtual void BeforeActionExecute(ActionType actionType)
        {

        }
        /// <summary>
        /// This method is called before an action is performed.
        /// </summary>
        /// <param name="actionType">Action types such as insert, edit, etc.</param>
        /// <param name="entity">The entity that the action affects.</param>
        protected virtual void BeforeActionExecute(ActionType actionType, TEntity entity)
        {

        }
        /// <summary>
        /// This method is called before an action is performed.
        /// </summary>
        /// <param name="actionType">Action types such as insert, edit, etc.</param>
        /// <param name="entity">The entity that the action affects.</param>
        protected virtual Task BeforeActionExecuteAsync(ActionType actionType, TEntity entity)
        {
            return Task.FromResult(0);
        }
        /// <summary>
        /// This method is called after an action is performed.
        /// </summary>
        /// <param name="actionType">Action types such as insert, edit, etc.</param>
        protected virtual void AfterActionExecute(ActionType actionType)
        {

        }
        #endregion Action

        #region Insert
        /// <summary>
        /// The entity is being tracked by the context but does not yet exist in the repository. 
        /// </summary>
        /// <param name="entity">The entity which is to be inserted.</param>
        /// <returns>The inserted entity.</returns>
        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            ValidateEntity(ActionType.Insert, entity);
            BeforeActionExecute(ActionType.Insert, entity);
            await BeforeActionExecuteAsync(ActionType.Insert, entity).ConfigureAwait(false);
            await EntitySet.AddAsync(entity).ConfigureAwait(false);
            AfterActionExecute(ActionType.Insert);
            return entity;
        }
        /// <summary>
        /// The entities are being tracked by the context but does not yet exist in the repository. 
        /// </summary>
        /// <param name="entities">The entities which are to be inserted.</param>
        /// <returns>The inserted entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                ValidateEntity(ActionType.Insert, entity);
                BeforeActionExecute(ActionType.Insert, entity);
                await BeforeActionExecuteAsync(ActionType.Insert, entity).ConfigureAwait(false);
            }
            await EntitySet.AddRangeAsync(entities).ConfigureAwait(false);
            AfterActionExecute(ActionType.InsertArray);
            return entities;
        }
        #endregion Insert

        #region Update
        /// <summary>
        /// The entity is being tracked by the context and exists in the repository, and some or all of its property values have been modified.
        /// </summary>
        /// <param name="entity">The entity which is to be updated.</param>
        /// <returns>The the modified entity.</returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            ValidateEntity(ActionType.Update, entity);
            BeforeActionExecute(ActionType.Update, entity);
            await BeforeActionExecuteAsync(ActionType.Update, entity).ConfigureAwait(false);
            EntitySet.Update(entity);
            AfterActionExecute(ActionType.Update);
            return entity;
        }
        /// <summary>
        /// The entities are being tracked by the context and exists in the repository, and some or all of its property values have been modified.
        /// </summary>
        /// <param name="entities">The entities which are to be updated.</param>
        /// <returns>The updated entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                ValidateEntity(ActionType.Update, entity);
                BeforeActionExecute(ActionType.Update, entity);
                await BeforeActionExecuteAsync(ActionType.Update, entity).ConfigureAwait(false);
            }
            EntitySet.UpdateRange(entities);
            AfterActionExecute(ActionType.UpdateArray);
            return entities;
        }
        #endregion Update

        #region Delete
        /// <summary>
        /// Removes the entity from the repository with the appropriate identity.
        /// </summary>
        /// <param name="id">The identification.</param>
        public virtual async Task DeleteAsync(int id)
        {
            TEntity? entity = await EntitySet.FindAsync(id).ConfigureAwait(false);

            if (entity != null)
            {
                ValidateEntity(ActionType.Delete, entity);
                BeforeActionExecute(ActionType.Delete, entity);
                await BeforeActionExecuteAsync(ActionType.Update, entity).ConfigureAwait(false);
                EntitySet.Remove(entity);
                AfterActionExecute(ActionType.Delete);
            }
        }
        #endregion Delete

        #region SaveChanges
        /// <summary>
        /// Saves any changes in the underlying persistence.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database.</returns>
        public async Task<int> SaveChangesAsync()
        {
            var result = 0;

            if (Context != null)
            {
                BeforeActionExecute(ActionType.Save);
                result = await Context.SaveChangesAsync().ConfigureAwait(false);
                AfterActionExecute(ActionType.Save);
            }
            return result;
        }
        #endregion SaveChanges
    }
}
//MdEnd