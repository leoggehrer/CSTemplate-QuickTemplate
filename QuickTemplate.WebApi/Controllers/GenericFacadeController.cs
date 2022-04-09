//@BaseCode
//MdStart
using Microsoft.AspNetCore.Mvc;

namespace QuickTemplate.WebApi.Controllers
{
    /// <summary>
    /// A generic one for the standard CRUD operations.
    /// </summary>
    /// <typeparam name="TFasadeModel">The type of entity</typeparam>
    /// <typeparam name="TEditModel">The type of edit model</typeparam>
    /// <typeparam name="TOutModel">The type of model</typeparam>
    [ApiController]
    [Route("api/[controller]")]
    public abstract partial class GenericFacadeController<TFasadeModel, TEditModel, TOutModel> : ControllerBase, IDisposable
        where TFasadeModel : class, Logic.IIdentifyable, new()
        where TEditModel : class, new()
        where TOutModel : class, new()
    {
        private bool disposedValue;

        protected Logic.IDataAccess<TFasadeModel> Fasade { get; init; }

        internal GenericFacadeController(Logic.IDataAccess<TFasadeModel> fasade)
        {
            Fasade = fasade;
        }
        /// <summary>
        /// Converts an entity to a model and copies all properties of the same name from the entity to the model.
        /// </summary>
        /// <param name="entity">The entity to be converted</param>
        /// <returns>The model with the property values of the same name</returns>
        protected virtual TOutModel ToOutModel(TFasadeModel entity)
        {
            var result = new TOutModel();

            result.CopyFrom(entity);
            return result;
        }
        /// <summary>
        /// Converts all entities to models and copies all properties of the same name from an entity to the model.
        /// </summary>
        /// <param name="entities">The entities to be converted</param>
        /// <returns>The models</returns>
        protected virtual IEnumerable<TOutModel> ToOutModel(IEnumerable<TFasadeModel> entities)
        {
            var result = new List<TOutModel>();

            foreach (var entity in entities)
            {
                result.Add(ToOutModel(entity));
            }
            return result;
        }

        /// <summary>
        /// Gets a list of models
        /// </summary>
        /// <returns>List of models</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TOutModel>>> GetAsync()
        {
            var entities = await Fasade.GetAllAsync();

            return Ok(ToOutModel(entities));
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
        public virtual async Task<ActionResult<TOutModel?>> GetAsync(int id)
        {
            var entity = await Fasade.GetByIdAsync(id);

            return entity == null ? NotFound() : Ok(ToOutModel(entity));
        }

        /// <summary>
        /// Adds a model.
        /// </summary>
        /// <param name="model">Model to add</param>
        /// <returns>Data about the created model (including primary key)</returns>
        /// <response code="201">Model created</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public virtual async Task<ActionResult<TOutModel>> PostAsync([FromBody] TEditModel model)
        {
            var entity = new TFasadeModel();

            entity.CopyFrom(model);
            var insertEntity = await Fasade.InsertAsync(entity);

            await Fasade.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = entity.Id }, ToOutModel(insertEntity));
        }

        /// <summary>
        /// Updates a model
        /// </summary>
        /// <param name="id">Id of the model to update</param>
        /// <param name="model">Data to update</param>
        /// <returns>Data about the updated model</returns>
        /// <response code="200">Model updated</response>
        /// <response code="404">Model not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TOutModel>> PutAsync(int id, [FromBody] TEditModel model)
        {
            var entity = await Fasade.GetByIdAsync(id);

            if (entity != null)
            {
                entity.CopyFrom(model);
                await Fasade.UpdateAsync(entity);
                await Fasade.SaveChangesAsync();
            }
            return entity == null ? NotFound() : Ok(ToOutModel(entity));
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
            var entity = await Fasade.GetByIdAsync(id);

            if (entity != null)
            {
                await Fasade.DeleteAsync(entity.Id);
                await Fasade.SaveChangesAsync();
            }
            return entity == null ? NotFound() : NoContent();
        }

        #region Dispose pattern
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Fasade.Dispose();
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