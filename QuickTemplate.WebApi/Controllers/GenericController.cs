//@BaseCode
//MdStart
using Microsoft.AspNetCore.Mvc;
using QuickTemplate.Logic.Contracts;

namespace QuickTemplate.WebApi.Controllers
{
    /// <summary>
    /// A generic one for the standard CRUD operations.
    /// </summary>
    /// <typeparam name="TAccessModel">The type of access model</typeparam>
    /// <typeparam name="TEditModel">The type of edit model</typeparam>
    /// <typeparam name="TOutModel">The type of output model</typeparam>
    [ApiController]
    [Route("api/[controller]")]
    public abstract partial class GenericController<TAccessModel, TEditModel, TOutModel> : ApiControllerBase, IDisposable
        where TAccessModel : class, IIdentifyable, new()
        where TEditModel : class, new()
        where TOutModel : class, new()
    {
        private bool disposedValue = false;
#if ACCOUNT_ON
        private bool initSessionToken = false;
#endif
        private IDataAccess<TAccessModel>? _dataAccess;

        /// <summary>
        /// This property controls access to the logic operations.
        /// </summary>
        protected IDataAccess<TAccessModel> DataAccess
        {
            get
            {
#if ACCOUNT_ON
                if (initSessionToken == false)
                {
                    initSessionToken = true;
                    _dataAccess!.SessionToken = GetSessionToken();
                }
#endif
                return _dataAccess!;
            }
            init => _dataAccess = value;
        }

        internal GenericController(IDataAccess<TAccessModel> dataAccess)
        {
            DataAccess = dataAccess;
        }
        /// <summary>
        /// Converts an accessModel to a model and copies all properties of the same name from the accessModel to the model.
        /// </summary>
        /// <param name="accessModel">The accessModel to be converted</param>
        /// <returns>The model with the property values of the same name</returns>
        protected virtual TOutModel ToOutModel(TAccessModel accessModel)
        {
            var result = new TOutModel();

            result.CopyFrom(accessModel);
            return result;
        }
        /// <summary>
        /// Converts all entities to models and copies all properties of the same name from an accessModel to the model.
        /// </summary>
        /// <param name="accessModels">The entities to be converted</param>
        /// <returns>The models</returns>
        protected virtual IEnumerable<TOutModel> ToOutModel(IEnumerable<TAccessModel> accessModels)
        {
            var result = new List<TOutModel>();

            foreach (var accessModel in accessModels)
            {
                result.Add(ToOutModel(accessModel));
            }
            return result;
        }

        /// <summary>
        /// Get a single model by Id.
        /// </summary>
        /// <param name="id">Id of the model to get</param>
        /// <response code="200">Model found</response>
        /// <response code="404">Model not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TOutModel?>> GetAsync(
            [FromQuery(Name = "id")] int id)
        {
            var accessModel = await DataAccess.GetByIdAsync(id);

            return accessModel == null ? NotFound() : Ok(ToOutModel(accessModel));
        }

        /// <summary>
        /// Gets a list of models.
        /// </summary>
        /// <returns>List of models</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TOutModel>>> GetAsync()
        {
            var accessModels = await DataAccess.GetAllAsync();

            return Ok(ToOutModel(accessModels));
        }

        /// <summary>
        /// Returns all entities in the collection.
        /// </summary>
        /// <param name="orderBy">Sorts the elements of a sequence according to a sort clause.</param>
        /// <returns>All entities of the collection.</returns>
        [HttpGet("/api/[controller]/Sorted")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TOutModel>>> GetAsync(
            [FromQuery(Name = "orderBy")] string orderBy)
        {
            var accessModels = await DataAccess.GetAllAsync(orderBy);

            return Ok(ToOutModel(accessModels));
        }

        /// <summary>
        /// Gets a subset of items from the repository.
        /// </summary>
        /// <param name="index">0 based page index.</param>
        /// <param name="size">The pagesize.</param>
        /// <returns>Subset in accordance with the parameters.</returns>
        [HttpGet("/api/[controller]/GetPage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TOutModel>>> GetPageListAsync(
            [FromQuery(Name = "index")] int index,
            [FromQuery(Name = "size")] int size)
        {
            var accessModels = await DataAccess.GetPageListAsync(index, size);

            return Ok(ToOutModel(accessModels));
        }

        /// <summary>
        /// Gets a subset of items from the repository.
        /// </summary>
        /// <param name="orderBy">Sorts the elements of a sequence according to a sort clause.</param>
        /// <param name="index">0 based page index.</param>
        /// <param name="size">The pagesize.</param>
        /// <returns>Subset in accordance with the parameters.</returns>
        [HttpGet("/api/[controller]/GetSortedPage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TOutModel>>> GetPageListAsync(
            [FromQuery(Name = "orderBy")] string orderBy,
            [FromQuery(Name = "index")] int index,
            [FromQuery(Name = "size")] int size)
        {
            var accessModels = await DataAccess.GetPageListAsync(orderBy, index, size);

            return Ok(ToOutModel(accessModels));
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <returns>The filter result.</returns>
        [HttpGet("/api/[controller]/Query")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TOutModel>>> QueryAllAsync(
            [FromQuery(Name = "predicate")] string predicate)
        {
            var entities = await DataAccess.QueryAsync(predicate);

            return Ok(entities.Select(e => ToOutModel(e)));
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="orderBy">Sorts the elements of a sequence according to a sort clause.</param>
        /// <returns>The filter result.</returns>
        [HttpGet("/api/[controller]/QuerySorted")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TOutModel>>> QueryAllAsync(
            [FromQuery(Name = "predicate")] string predicate,
            [FromQuery(Name = "orderBy")] string orderBy)
        {
            var entities = await DataAccess.QueryAsync(predicate, orderBy);

            return Ok(entities.Select(e => ToOutModel(e)));
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="index">0 based page index.</param>
        /// <param name="size">The pagesize.</param>
        /// <returns>The filter result.</returns>
        [HttpGet("/api/[controller]/QueryPage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TOutModel>>> QueryAsync(
            [FromQuery(Name = "predicate")] string predicate,
            [FromQuery(Name = "index")] int index,
            [FromQuery(Name = "size")] int size)
        {
            var entities = await DataAccess.QueryAsync(predicate, index, size);

            return Ok(entities.Select(e => ToOutModel(e)));
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A string to test each element for a condition.</param>
        /// <param name="orderBy">Sorts the elements of a sequence according to a sort clause.</param>
        /// <param name="index">0 based page index.</param>
        /// <param name="size">The pagesize.</param>
        /// <returns>The filter result.</returns>
        [HttpGet("/api/[controller]/QuerySortedPage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TOutModel>>> QueryAsync(
            [FromQuery(Name = "predicate")] string predicate,
            [FromQuery(Name = "orderBy")] string orderBy,
            [FromQuery(Name = "index")] int index,
            [FromQuery(Name = "size")] int size)
        {
            var entities = await DataAccess.QueryAsync(predicate, orderBy, index, size);

            return Ok(entities.Select(e => ToOutModel(e)));
        }

        /// <summary>
        /// Adds a model.
        /// </summary>
        /// <param name="editModel">Model to add</param>
        /// <returns>Data about the created model (including primary key)</returns>
        /// <response code="201">Model created</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public virtual async Task<ActionResult<TOutModel>> PostAsync([FromBody] TEditModel editModel)
        {
            var accessModel = new TAccessModel();

            accessModel.CopyFrom(editModel);
            var insertaccessModel = await DataAccess.InsertAsync(accessModel);

            await DataAccess.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = accessModel.Id }, ToOutModel(insertaccessModel));
        }

        /// <summary>
        /// Updates a model
        /// </summary>
        /// <param name="id">Id of the model to update</param>
        /// <param name="editModel">Data to update</param>
        /// <returns>Data about the updated model</returns>
        /// <response code="200">Model updated</response>
        /// <response code="404">Model not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TOutModel>> PutAsync(int id, [FromBody] TEditModel editModel)
        {
            var accessModel = await DataAccess.GetByIdAsync(id);

            if (accessModel != null)
            {
                accessModel.CopyFrom(editModel);
                await DataAccess.UpdateAsync(accessModel);
                await DataAccess.SaveChangesAsync();
            }
            return accessModel == null ? NotFound() : Ok(ToOutModel(accessModel));
        }

        /// <summary>
        /// Delete a single model by Id
        /// </summary>
        /// <param name="id">Id of the model to delete</param>
        /// <response code="204">Model deleted</response>
        /// <response code="404">Model not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult> DeleteAsync(int id)
        {
            var accessModel = await DataAccess.GetByIdAsync(id);

            if (accessModel != null)
            {
                await DataAccess.DeleteAsync(accessModel.Id);
                await DataAccess.SaveChangesAsync();
            }
            return accessModel == null ? NotFound() : NoContent();
        }

        #region Dispose pattern
        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing">If true, the method has been called directly or indirectly by a user.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _dataAccess?.Dispose();
                    _dataAccess = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GenericController()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        /// <summary>
        /// A derived class should not be able to override this method.
        /// </summary>
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