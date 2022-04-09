//@BaseCode
//MdStart

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
        /// Returns the element of type T with the identification of id.
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <returns>The element of the type T with the corresponding identification.</returns>
        public virtual ValueTask<TEntity?> GetByIdAsync(int id)
        {
            return EntitySet.FindAsync(id);
        }
        #endregion Queries

        #region Action
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
            BeforeActionExecute(ActionType.Insert, entity);
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
                BeforeActionExecute(ActionType.Insert, entity);
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
        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            BeforeActionExecute(ActionType.Update, entity);
            return Task.Run(() =>
            {
                EntitySet.Update(entity);
                AfterActionExecute(ActionType.Update);
                return entity;
            });
        }
        /// <summary>
        /// The entities are being tracked by the context and exists in the repository, and some or all of its property values have been modified.
        /// </summary>
        /// <param name="entities">The entities which are to be updated.</param>
        /// <returns>The updated entities.</returns>
        public virtual Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                BeforeActionExecute(ActionType.Update, entity);
            }
            return Task.Run(() =>
            {
                EntitySet.UpdateRange(entities);
                AfterActionExecute(ActionType.UpdateArray);
                return entities;
            });
        }
        #endregion Update

        #region Delete
        /// <summary>
        /// Removes the entity from the repository with the appropriate identity.
        /// </summary>
        /// <param name="id">The identification.</param>
        public virtual Task DeleteAsync(int id)
        {
            return Task.Run(() =>
            {
                TEntity? result = EntitySet.Find(id);

                if (result != null)
                {
                    BeforeActionExecute(ActionType.Delete, result);
                    EntitySet.Remove(result);
                    AfterActionExecute(ActionType.Delete);
                }
            });
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