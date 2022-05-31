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
#if ACCOUNT_ON
    using System.Reflection;
    [Modules.Security.Authorize]
#endif
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
        /// <summary>
        /// Creates an instance
        /// </summary>
        public GenericController()
            : base(new DataContext.ProjectDbContext())
        {

        }
        /// <summary>
        /// Creates an instance
        /// </summary>
        /// <param name="other">A reference to an other controller</param>
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
        public virtual async Task<int> CountAsync()
        {
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.QueryCount).ConfigureAwait(false);
#endif
            return await ExecuteCountAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// Gets the number of quantity in the collection (without authorization).
        /// </summary>
        /// <returns>Number of entities in the collection.</returns>
        internal virtual Task<int> ExecuteCountAsync()
        {
            return EntitySet.CountAsync();
        }
        /// <summary>
        /// Returns the number of quantity in the collection based on a predicate.
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>Number of entities in the collection.</returns>
        public virtual async Task<int> CountAsync(string predicate, params string[] includeItems)
        {
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.QueryCountBy).ConfigureAwait(false);
#endif
            return await ExecuteCountAsync(predicate, includeItems).ConfigureAwait(false);
        }
        /// <summary>
        /// Returns the number of quantity in the collection based on a predicate (without authorization).
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>Number of entities in the collection.</returns>
        internal virtual Task<int> ExecuteCountAsync(string predicate, params string[] includeItems)
        {
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in includeItems)
            {
                query = query.Include(includeItem);
            }
            return query.Where(predicate).CountAsync();
        }

        /// <summary>
        /// Returns the number of quantity in the collection based on a predicate (without authorization).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>Number of entities in the collection.</returns>
        internal virtual Task<int> ExecuteCountAsync(Expression<Func<TEntity, bool>> predicate, params string[] includeItems)
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
        /// Returns all of the entities in the collection.
        /// </summary>
        /// <returns>All of the entitities in the collection.</returns>
        public virtual async Task<TEntity[]> GetAllAsync()
        {
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.GetAll).ConfigureAwait(false);
#endif
            return await ExecuteGetAllAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// Returns all interfaces of the entities in the collection (without authorization).
        /// </summary>
        /// <returns>All interfaces of the entity collection.</returns>
        internal virtual Task<TEntity[]> ExecuteGetAllAsync()
        {
            return EntitySet.AsNoTracking().ToArrayAsync();
        }
        /// <summary>
        /// Returns all interfaces of the entities in the collection.
        /// </summary>
        /// <param name="includeItems">The include items</param>
        /// <returns>All interfaces of the entity collection (with include).</returns>
        public virtual async Task<TEntity[]> GetAllAsync(params string[] includeItems)
        {
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.GetAll).ConfigureAwait(false);
#endif
            return await ExecuteGetAllAsync(includeItems).ConfigureAwait(false);
        }
        /// <summary>
        /// Returns all interfaces of the entities in the collection (without authorization).
        /// </summary>
        /// <param name="includeItems">The include items</param>
        /// <returns>All interfaces of the entity collection (with include).</returns>
        internal virtual Task<TEntity[]> ExecuteGetAllAsync(params string[] includeItems)
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
        public virtual async Task<TEntity[]> QueryAsync(string predicate, params string[] includeItems)
        {
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.QueryBy).ConfigureAwait(false);
#endif
            return await ExecuteQueryAsync(predicate, includeItems).ConfigureAwait(false);
        }
        /// <summary>
        /// Filters a sequence of values based on a predicate (without authorization).
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>The filter result.</returns>
        internal virtual Task<TEntity[]> ExecuteQueryAsync(string predicate, params string[] includeItems)
        {
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in includeItems)
            {
                query = query.Include(includeItem);
            }
            return query.Where(predicate).ToArrayAsync();
        }
        /// <summary>
        /// Filters a sequence of values based on a predicate (without authorization).
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>The filter result.</returns>
        internal virtual Task<TEntity[]> ExecuteQueryAsync(Expression<Func<TEntity, bool>> predicate, params string[] includeItems)
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
        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.GetBy).ConfigureAwait(false);
#endif
            return await ExecuteGetByIdAsync(id).ConfigureAwait(false);
        }
        /// <summary>
        /// Returns the element of type T with the identification of id (without authorization).
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <returns>The element of the type T with the corresponding identification.</returns>
        internal virtual ValueTask<TEntity?> ExecuteGetByIdAsync(int id)
        {
            return EntitySet.FindAsync(id);
        }
        /// <summary>
        /// Returns the element of type T with the identification of id.
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>The element of the type T with the corresponding identification (with includes).</returns>
        public virtual async Task<TEntity?> GetByIdAsync(int id, params string[] includeItems)
        {
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.GetBy).ConfigureAwait(false);
#endif
            return await ExecuteGetByIdAsync(id, includeItems).ConfigureAwait(false);
        }
        /// <summary>
        /// Returns the element of type T with the identification of id (without authorization).
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>The element of the type T with the corresponding identification (with includes).</returns>
        internal virtual Task<TEntity?> ExecuteGetByIdAsync(int id, params string[] includeItems)
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
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.Insert).ConfigureAwait(false);
#endif
            return await ExecuteInsertAsync(entity).ConfigureAwait(false);
        }
        /// <summary>
        /// The entity is being tracked by the context but does not yet exist in the repository (without authorization). 
        /// </summary>
        /// <param name="entity">The entity which is to be inserted.</param>
        /// <returns>The inserted entity.</returns>
        internal virtual async Task<TEntity> ExecuteInsertAsync(TEntity entity)
        {
            ValidateEntity(ActionType.Insert, entity);
            BeforeActionExecute(ActionType.Insert, entity);
            await BeforeActionExecuteAsync(ActionType.Insert, entity).ConfigureAwait(false);
            BeforeExecuteInsert(entity);
            await EntitySet.AddAsync(entity).ConfigureAwait(false);
            AfterExecuteInsert(entity);
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
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.InsertArray).ConfigureAwait(false);
#endif
            return await ExecuteInsertAsync(entities).ConfigureAwait(false);
        }
        /// <summary>
        /// The entities are being tracked by the context but does not yet exist in the repository (without authorization). 
        /// </summary>
        /// <param name="entities">The entities which are to be inserted.</param>
        /// <returns>The inserted entities.</returns>
        internal virtual async Task<IEnumerable<TEntity>> ExecuteInsertAsync(IEnumerable<TEntity> entities)
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
        partial void BeforeExecuteInsert(TEntity entity);
        partial void AfterExecuteInsert(TEntity entity);

        #endregion Insert

        #region Update
        /// <summary>
        /// The entity is being tracked by the context and exists in the repository, and some or all of its property values have been modified.
        /// </summary>
        /// <param name="entity">The entity which is to be updated.</param>
        /// <returns>The the modified entity.</returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.Update).ConfigureAwait(false);
#endif
            return await ExecuteUpdateAsync(entity).ConfigureAwait(false);
        }
        /// <summary>
        /// The entity is being tracked by the context and exists in the repository, and some or all of its property values have been modified (without authorization).
        /// </summary>
        /// <param name="entity">The entity which is to be updated.</param>
        /// <returns>The the modified entity.</returns>
        internal virtual async Task<TEntity> ExecuteUpdateAsync(TEntity entity)
        {
            ValidateEntity(ActionType.Update, entity);
            BeforeActionExecute(ActionType.Update, entity);
            await BeforeActionExecuteAsync(ActionType.Update, entity).ConfigureAwait(false);
            BeforeExecuteUpdate(entity);
            EntitySet.Update(entity);
            AfterExecuteUpdate(entity);
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
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.UpdateArray).ConfigureAwait(false);
#endif
            return await ExecuteUpdateAsync(entities).ConfigureAwait(false);
        }
        /// <summary>
        /// The entities are being tracked by the context and exists in the repository, and some or all of its property values have been modified.
        /// </summary>
        /// <param name="entities">The entities which are to be updated.</param>
        /// <returns>The updated entities.</returns>
        internal virtual async Task<IEnumerable<TEntity>> ExecuteUpdateAsync(IEnumerable<TEntity> entities)
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
        partial void BeforeExecuteUpdate(TEntity entity);
        partial void AfterExecuteUpdate(TEntity entity);
        #endregion Update

        #region Delete
        /// <summary>
        /// Removes the entity from the repository with the appropriate identity.
        /// </summary>
        /// <param name="id">The identification.</param>
        public virtual async Task DeleteAsync(int id)
        {
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.Delete).ConfigureAwait(false);
#endif
            await ExecuteDeleteAsync(id).ConfigureAwait(false);
        }
        /// <summary>
        /// Removes the entity from the repository with the appropriate identity (without authorization).
        /// </summary>
        /// <param name="id">The identification.</param>
        internal virtual async Task ExecuteDeleteAsync(int id)
        {
            TEntity? entity = await EntitySet.FindAsync(id).ConfigureAwait(false);

            if (entity != null)
            {
                ValidateEntity(ActionType.Delete, entity);
                BeforeActionExecute(ActionType.Delete, entity);
                await BeforeActionExecuteAsync(ActionType.Delete, entity).ConfigureAwait(false);
                BeforeExecuteDelete(entity);
                EntitySet.Remove(entity);
                AfterExecuteDelete(entity);
                AfterActionExecute(ActionType.Delete);
            }
        }
        partial void BeforeExecuteDelete(TEntity entity);
        partial void AfterExecuteDelete(TEntity entity);
        #endregion Delete

        #region SaveChanges
        /// <summary>
        /// Saves any changes in the underlying persistence.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database.</returns>
        public async Task<int> SaveChangesAsync()
        {
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.Save).ConfigureAwait(false);
#endif
            return await ExecuteSaveChangesAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// Saves any changes in the underlying persistence (without authorization).
        /// </summary>
        /// <returns>The number of state entries written to the underlying database.</returns>
        internal async Task<int> ExecuteSaveChangesAsync()
        {
            var result = 0;

            if (Context != null)
            {
                BeforeActionExecute(ActionType.Save);
                BeforeExecuteSaveChanges();
                result = await Context.SaveChangesAsync().ConfigureAwait(false);
                AfterExecuteSaveChanges();
                AfterActionExecute(ActionType.Save);
            }
            return result;
        }
        partial void BeforeExecuteSaveChanges();
        partial void AfterExecuteSaveChanges();
        #endregion SaveChanges
    }
}
//MdEnd