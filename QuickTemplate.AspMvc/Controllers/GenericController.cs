//@BaseCode
//MdStart
#nullable disable
using Microsoft.AspNetCore.Mvc;

namespace QuickTemplate.AspMvc.Controllers
{
    public abstract class GenericController<TEntity, TModel> : Controller
        where TEntity : Logic.Entities.IdentityEntity, new()
        where TModel : class, new()
    {
        protected Logic.Controllers.GenericController<TEntity> Controller { get; init; }

        protected GenericController(Logic.Controllers.GenericController<TEntity> controller)
        {
            this.Controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        protected virtual TModel ToModel(TEntity entity)
        {
            var result = new TModel();

            result.CopyFrom(entity);
            return BeforeView(result);
        }
        protected virtual TEntity ToEntity(TModel model)
        {
            var result = new TEntity();

            result.CopyFrom(model);
            return result;
        }
        protected virtual TModel BeforeView(TModel model) => model;

        // GET: Item
        public virtual async Task<IActionResult> Index()
        {
            var entities = await Controller.GetAllAsync();

            return View(entities.Select(e => ToModel(e)));
        }

        // GET: Item/Details/5
        public virtual async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await Controller.GetByIdAsync(id.Value);
            if (genre == null)
            {
                return NotFound();
            }
            return View(ToModel(genre));
        }

        // GET: Item/Create
        public virtual IActionResult Create()
        {
            var entity = new TEntity();

            return View(ToModel(entity));
        }

        // POST: Item/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(TModel model)
        {
            TEntity entity = ToEntity(model);

            if (ModelState.IsValid)
            {
                try
                {
                    await Controller.InsertAsync(entity);
                    await Controller.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;

                    if (ex.InnerException != null)
                    {
                        ViewBag.Error = ex.InnerException.Message;
                    }
                }
            }
            return View(ToModel(entity));
        }

        // GET: Item/Edit/5
        public virtual async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await Controller.GetByIdAsync(id.Value);

            if (entity == null)
            {
                return NotFound();
            }
            return View(ToModel(entity));
        }

        // POST: Item/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(int id, TModel model)
        {
            var entity = await Controller.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            entity.CopyFrom(model);

            if (ModelState.IsValid)
            {
                try
                {
                    entity = await Controller.UpdateAsync(entity);
                    await Controller.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;

                    if (ex.InnerException != null)
                    {
                        ViewBag.Error = ex.InnerException.Message;
                    }
                }
            }
            return string.IsNullOrEmpty(ViewBag.Error) ? RedirectToAction(nameof(Index)) : View(ToModel(entity));
        }

        // GET: Item/Delete/5
        public virtual async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await Controller.GetByIdAsync(id.Value);

            if (entity == null)
            {
                return NotFound();
            }
            return View(ToModel(entity));
        }

        // POST: Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await Controller.GetByIdAsync(id);

            if (entity != null)
            {
                try
                {
                    await Controller.DeleteAsync(id);
                    await Controller.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;

                    if (ex.InnerException != null)
                    {
                        ViewBag.Error = ex.InnerException.Message;
                    }
                }
            }
            return ViewBag.Error != null ? View(ToModel(entity)) : RedirectToAction(nameof(Index));
        }

        protected override void Dispose(bool disposing)
        {
            Controller?.Dispose();
            base.Dispose(disposing);
        }
    }
}
//MdEnd