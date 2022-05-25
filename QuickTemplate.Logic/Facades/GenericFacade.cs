//@BaseCode
//MdStart

using QuickTemplate.Logic.Controllers;

namespace QuickTemplate.Logic.Facades
{
    /// <summary>
    /// Generic facade for mapping entity types to model types.
    /// </summary>
    /// <typeparam name="TModel">The model type as public type</typeparam>
    /// <typeparam name="TEntity">The entity type for the internal controller</typeparam>
    internal abstract partial class GenericFacade<TModel, TEntity> : FacadeObject, IDataAccess<TModel>
        where TModel : Models.IdentityModel, new()
        where TEntity : Entities.IdentityEntity, new()
    {
        private bool disposedValue;

        protected GenericController<TEntity> Controller { get; init; }
        protected GenericFacade(GenericController<TEntity> controller)
            : base(controller)
        {
            Controller = controller;
        }
        /// <summary>
        /// Converts the entity type to the facade type.
        /// </summary>
        /// <param name="entity">Entity type</param>
        /// <returns>The facade type</returns>
        internal virtual TModel ToModel(TEntity entity)
        {
            var result = new TModel();

            result.CopyFrom(entity);
            return result;
        }
        /// <summary>
        /// Converts the model type to the entity type.
        /// </summary>
        /// <param name="model">Model type</param>
        /// <returns>The entity type</returns>
        internal virtual TEntity ToEntity(TModel model)
        {
            var result = new TEntity();

            result.CopyFrom(model);
            return result;
        }

#if ACCOUNT_ON
        #region SessionToken
        /// <summary>
        /// Sets the session token.
        /// </summary>
        public string SessionToken
        {
            set
            {
                Controller.SessionToken = value;
            }
        }
        #endregion SessionToken
#endif

        #region Count
        public virtual Task<int> CountAsync()
        {
            return Controller.CountAsync();
        }
        public virtual Task<int> CountAsync(string predicate, params string[] includeItems)
        {
            return Controller.CountAsync(predicate, includeItems);
        }
        #endregion Count

        #region Queries
        public virtual async Task<TModel[]> GetAllAsync()
        {
            var entities = await Controller.GetAllAsync().ConfigureAwait(false);

            return entities.Select(e => ToModel(e)).ToArray();
        }
        public virtual async Task<TModel[]> GetAllAsync(params string[] includeItems)
        {
            var entities = await Controller.GetAllAsync(includeItems).ConfigureAwait(false);

            return entities.Select(e => ToModel(e)).ToArray();
        }
        public virtual async Task<TModel?> GetByIdAsync(int id)
        {
            var entity = await Controller.GetByIdAsync(id).ConfigureAwait(false);

            return entity != null ? ToModel(entity) : null;
        }
        public virtual async Task<TModel?> GetByIdAsync(int id, params string[] includeItems)
        {
            var entity = await Controller.GetByIdAsync(id, includeItems).ConfigureAwait(false);

            return entity != null ? ToModel(entity) : null;
        }
        public virtual async Task<TModel[]> QueryAsync(string predicate, params string[] includeItems)
        {
            var entities = await Controller.QueryAsync(predicate, includeItems).ConfigureAwait(false);

            return entities.Select(e => ToModel(e)).ToArray();
        }
        #endregion Queries

        #region Insert
        public virtual async Task<TModel> InsertAsync(TModel model)
        {
            var entity = await Controller.InsertAsync(ToEntity(model)).ConfigureAwait(false);

            return ToModel(entity);
        }
        public virtual async Task<IEnumerable<TModel>> InsertAsync(IEnumerable<TModel> models)
        {
            var entities = await Controller.InsertAsync(models.Select(f => ToEntity(f))).ConfigureAwait(false);

            return entities.Select(e => ToModel(e));
        }
        #endregion Insert

        #region Update
        public virtual async Task<TModel> UpdateAsync(TModel model)
        {
            var entity = await Controller.GetByIdAsync(model.Id).ConfigureAwait(false);

            _ = entity ?? throw new Exception("Entity not found.");

            entity.CopyFrom(model);
            entity = await Controller.UpdateAsync(entity).ConfigureAwait(false);

            return ToModel(entity);
        }
        public virtual async Task<IEnumerable<TModel>> UpdateAsync(IEnumerable<TModel> models)
        {
            var result = new List<TModel>();

            foreach (var model in models)
            {
                var updateModel = await UpdateAsync(model).ConfigureAwait(false);

                result.Add(updateModel);
            }
            return result;
        }
        #endregion Update

        #region Delete
        public virtual Task DeleteAsync(int id)
        {
            return Controller.DeleteAsync(id);
        }
        #endregion Delete

        #region SaveChanges
        public Task<int> SaveChangesAsync()
        {
            return Controller.SaveChangesAsync();
        }
        #endregion SaveChanges

        #region Dispose pattern
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Controller.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GenericFacade()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion Dispose pattern
    }
}
//MdEnd