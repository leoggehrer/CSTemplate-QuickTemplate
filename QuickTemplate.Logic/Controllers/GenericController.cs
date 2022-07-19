//@BaseCode
//MdStart

using QuickTemplate.Logic.Contracts;
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

        /// <summary>
        /// Gets the internal includes.
        /// </summary>
        /// <returns></returns>
        internal virtual IEnumerable<string> Includes => Array.Empty<string>();

        #region MaxPageSize and Count
        /// <summary>
        /// Gets the maximum page size.
        /// </summary>
        public virtual int MaxPageSize => StaticLiterals.MaxPageSize;

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

            foreach (var includeItem in Includes.Union(includeItems).Distinct())
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

            foreach (var includeItem in Includes.Union(includeItems).Distinct())
            {
                query = query.Include(includeItem);
            }
            return query.Where(predicate).CountAsync();
        }
        #endregion  MaxPageSize and Count

        #region Get
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
        /// Gets all items from the repository.
        /// </summary>
        /// <param name="includeItems">The include items</param>
        /// <returns>All items in accordance with the parameters.</returns>
        public virtual async Task<TEntity[]> GetAllAsync(params string[] includeItems)
        {
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.GetAll).ConfigureAwait(false);
#endif
            return await ExecuteGetAllAsync(includeItems).ConfigureAwait(false);
        }
        /// <summary>
        /// Gets all items from the repository.
        /// </summary>
        /// <param name="orderBy">Sorts the elements of a sequence according to a sort clause.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>All items in accordance with the parameters.</returns>
        public virtual async Task<TEntity[]> GetAllAsync(string orderBy, params string[] includeItems)
        {
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.GetAll).ConfigureAwait(false);
#endif
            return await ExecuteGetAllAsync(orderBy, includeItems).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a subset of items from the repository.
        /// </summary>
        /// <param name="pageIndex">0 based page index.</param>
        /// <param name="pageSize">The pagesize.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>Subset in accordance with the parameters.</returns>
        public virtual async Task<TEntity[]> GetPageListAsync(int pageIndex, int pageSize, params string[] includeItems)
        {
            CheckPageParams(pageIndex, pageSize);
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.GetPageList).ConfigureAwait(false);
#endif
            return await ExecuteGetPageListAsync(pageIndex, pageSize, includeItems).ConfigureAwait(false);
        }
        /// <summary>
        /// Gets a subset of items from the repository.
        /// </summary>
        /// <param name="orderBy">Sorts the elements of a sequence in order according to a key.</param>
        /// <param name="pageIndex">0 based page index.</param>
        /// <param name="pageSize">The pagesize.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>Subset in accordance with the parameters.</returns>
        public virtual async Task<TEntity[]> GetPageListAsync(string orderBy, int pageIndex, int pageSize, params string[] includeItems)
        {
            CheckPageParams(pageIndex, pageSize);
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.GetPageList).ConfigureAwait(false);
#endif
            return await ExecuteGetPageListAsync(orderBy, pageIndex, pageSize, includeItems).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the element of type T with the identification of id (without authorization).
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <returns>The element of the type T with the corresponding identification.</returns>
        internal virtual Task<TEntity?> ExecuteGetByIdAsync(int id)
        {
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in Includes.Distinct())
            {
                query = query.Include(includeItem);
            }
            return query.SingleOrDefaultAsync(e => e.Id == id);
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

            foreach (var includeItem in Includes.Union(includeItems).Distinct())
            {
                query = query.Include(includeItem);
            }
            return query.SingleOrDefaultAsync(e => e.Id == id);
        }

        /// <summary>
        /// Returns all entities in the collection (without authorization).
        /// </summary>
        /// <param name="includeItems">The include items</param>
        /// <returns>All entity collection (with include).</returns>
        internal virtual async Task<TEntity[]> ExecuteGetAllAsync(params string[] includeItems)
        {
            int idx = 0, qryCount;
            var result = new List<TEntity>();
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in Includes.Union(includeItems).Distinct())
            {
                query = query.Include(includeItem);
            }

            do
            {
                var qry = await query.Skip(idx++ * MaxPageSize)
                                     .Take(MaxPageSize)
                                     .AsNoTracking()
                                     .ToArrayAsync()
                                     .ConfigureAwait(false);

                qryCount = result.AddRangeAndCount(qry);
            } while (qryCount == MaxPageSize);
            return result.ToArray();
        }
        /// <summary>
        /// Returns all entities in the collection (without authorization).
        /// </summary>
        /// <param name="orderBy">Sorts the elements of a sequence according to a sort clause.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>All entity collection (with include).</returns>
        internal virtual async Task<TEntity[]> ExecuteGetAllAsync(string orderBy, params string[] includeItems)
        {
            int idx = 0, qryCount;
            var result = new List<TEntity>();
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in Includes.Union(includeItems).Distinct())
            {
                query = query.Include(includeItem);
            }

            do
            {
                var qry = await query.OrderBy(orderBy)
                                     .Skip(idx++ * MaxPageSize)
                                     .Take(MaxPageSize)
                                     .AsNoTracking()
                                     .ToArrayAsync()
                                     .ConfigureAwait(false);

                qryCount = result.AddRangeAndCount(qry);
            } while (qryCount == MaxPageSize);
            return result.ToArray();
        }

        /// <summary>
        /// Returns a subset of items from the repository.
        /// </summary>
        /// <param name="pageIndex">0 based page index.</param>
        /// <param name="pageSize">The pagesize.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>Subset in accordance with the parameters.</returns>
        internal virtual Task<TEntity[]> ExecuteGetPageListAsync(int pageIndex, int pageSize, params string[] includeItems)
        {
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in Includes.Union(includeItems).Distinct())
            {
                query = query.Include(includeItem);
            }
            return query.Skip(pageIndex * pageSize)
                        .Take(pageSize)
                        .AsNoTracking()
                        .ToArrayAsync();
        }
        /// <summary>
        /// Returns a subset of items from the repository.
        /// </summary>
        /// <param name="orderBy">Sorts the elements of a sequence in order according to a key.</param>
        /// <param name="pageIndex">0 based page index.</param>
        /// <param name="pageSize">The pagesize.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>Subset in accordance with the parameters.</returns>
        internal virtual Task<TEntity[]> ExecuteGetPageListAsync(string orderBy, int pageIndex, int pageSize, params string[] includeItems)
        {
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in Includes.Union(includeItems).Distinct())
            {
                query = query.Include(includeItem);
            }
            return query.OrderBy(orderBy)
                        .Skip(pageIndex * pageSize)
                        .Take(pageSize)
                        .AsNoTracking()
                        .ToArrayAsync();
        }
        #endregion Get

        #region Query
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
            int idx = 0, qryCount;
            var result = new List<TEntity>();
            do
            {
                var qry = await ExecuteQueryAsync(predicate, idx, MaxPageSize).ConfigureAwait(false);

                qryCount = result.AddRangeAndCount(qry);
            } while (qryCount == MaxPageSize);
            return result.ToArray();
        }
        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="orderBy">Sorts the elements of a sequence according to a sort clause.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>The filter result.</returns>
        public virtual async Task<TEntity[]> QueryAsync(string predicate, string orderBy, params string[] includeItems)
        {
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.QueryBy).ConfigureAwait(false);
#endif
            int idx = 0, qryCount;
            var result = new List<TEntity>();
            do
            {
                var qry = await ExecuteQueryAsync(predicate, orderBy, idx, MaxPageSize).ConfigureAwait(false);

                qryCount = result.AddRangeAndCount(qry);
            } while (qryCount == MaxPageSize);
            return result.ToArray();
        }
        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="pageIndex">0 based page index.</param>
        /// <param name="pageSize">The pagesize.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>The filter result.</returns>
        public virtual Task<TEntity[]> QueryAsync(string predicate, int pageIndex, int pageSize, params string[] includeItems)
        {
            CheckPageParams(pageIndex, pageSize);
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.QueryBy).ConfigureAwait(false);
#endif
            return ExecuteQueryAsync(predicate, pageIndex, pageSize, includeItems);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="orderBy">Sorts the elements of a sequence according to a sort clause.</param>
        /// <param name="pageIndex">0 based page index.</param>
        /// <param name="pageSize">The pagesize.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>The filter result.</returns>
        public virtual Task<TEntity[]> QueryAsync(string predicate, string orderBy, int pageIndex, int pageSize, params string[] includeItems)
        {
            CheckPageParams(pageIndex, pageSize);
#if ACCOUNT_ON
            await CheckAuthorizationAsync(GetType(), MethodBase.GetCurrentMethod(), AccessType.QueryBy).ConfigureAwait(false);
#endif
            return ExecuteQueryAsync(predicate, orderBy, pageIndex, pageSize, includeItems);
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

            foreach (var includeItem in Includes.Union(includeItems).Distinct())
            {
                query = query.Include(includeItem);
            }
            return query.Where(predicate).AsNoTracking().ToArrayAsync();
        }
        /// <summary>
        /// Filters a sequence of values based on a predicate (without authorization).
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="pageIndex">0 based page index.</param>
        /// <param name="pageSize">The pagesize.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>The filter result.</returns>
        internal virtual Task<TEntity[]> ExecuteQueryAsync(string predicate, int pageIndex, int pageSize, params string[] includeItems)
        {
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in Includes.Union(includeItems).Distinct())
            {
                query = query.Include(includeItem);
            }
            return query.Where(predicate)
                        .Skip(pageIndex * pageSize)
                        .Take(pageSize)
                        .AsNoTracking()
                        .ToArrayAsync();
        }
        /// <summary>
        /// Filters a sequence of values based on a predicate (without authorization).
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="orderBy">Sorts the elements of a sequence according to a sort clause.</param>
        /// <param name="pageIndex">0 based page index.</param>
        /// <param name="pageSize">The pagesize.</param>
        /// <param name="includeItems">The include items</param>
        /// <returns>The filter result.</returns>
        internal virtual Task<TEntity[]> ExecuteQueryAsync(string predicate, string orderBy, int pageIndex, int pageSize, params string[] includeItems)
        {
            var query = EntitySet.AsQueryable();

            foreach (var includeItem in Includes.Union(includeItems).Distinct())
            {
                query = query.Include(includeItem);
            }
            return query.Where(predicate)
                        .OrderBy(orderBy)
                        .Skip(pageIndex * pageSize)
                        .Take(pageSize)
                        .AsNoTracking()
                        .ToArrayAsync();
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

            foreach (var includeItem in Includes.Union(includeItems).Distinct())
            {
                query = query.Include(includeItem);
            }
            return query.Where(predicate).AsNoTracking().ToArrayAsync();
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

        #region Helpers
        internal void CheckPageParams(int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageIndex));
            }
            if (pageSize <= 0 || pageSize > MaxPageSize)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }
        }
        #endregion Helpers
    }
}
//MdEnd