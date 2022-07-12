﻿//@BaseCode
//MdStart
#nullable disable
using Microsoft.AspNetCore.Mvc;
using QuickTemplate.Logic.Contracts;
#if ACCOUNT_ON
using Microsoft.AspNetCore.Mvc.Filters;
#endif

namespace QuickTemplate.AspMvc.Controllers
{
    /// <summary>
    /// A generic one for the standard CRUD operations.
    /// </summary>
    /// <typeparam name="TAccessModel">The type of access model</typeparam>
    /// <typeparam name="TViewModel">The type of view model</typeparam>
    public abstract partial class GenericController<TAccessModel, TViewModel> : MvcController
        where TAccessModel : IIdentifyable, new()
        where TViewModel : class, new()
    {
        public enum ActionMode : int
        {
            Index,
            Details,
            ViewCreate,
            Create,
            ViewEdit,
            Edit,
            ViewDelete,
            Delete,
        }
        protected IDataAccess<TAccessModel> DataAccess { get; init; }

        protected GenericController(IDataAccess<TAccessModel> dataAccess)
        {
            this.DataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

#if ACCOUNT_ON
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            DataAccess.SessionToken = SessionWrapper.SessionToken;
        }
#endif
        protected virtual RedirectToActionResult RedirectAfterAction(ActionMode actionMode, TAccessModel accessModel)
        {
            return RedirectToAction(nameof(Index));
        }
        protected virtual TAccessModel ToAccessModel(TViewModel viewModel)
        {
            var result = new TAccessModel();

            result.CopyFrom(viewModel);
            return result;
        }
        protected virtual TViewModel ToViewModel(TAccessModel accessModel, ActionMode actionMode)
        {
            var result = new TViewModel();

            result.CopyFrom(accessModel);
            return BeforeView(result, actionMode);
        }

        protected virtual IEnumerable<TAccessModel> AfterQuery(IEnumerable<TAccessModel> accessModels) => accessModels;
        protected virtual TViewModel BeforeView(TViewModel viewModel, ActionMode actionMode) => viewModel;
        protected virtual IEnumerable<TViewModel> BeforeView(IEnumerable<TViewModel> viewModels, ActionMode actionMode) => viewModels;

        // GET: Item
        public virtual async Task<IActionResult> Index()
        {
            var accessModels = await DataAccess.GetAllAsync();
            var viewModels = AfterQuery(accessModels).Select(e => ToViewModel(e, ActionMode.Index)).ToArray();

            return View(BeforeView(viewModels, ActionMode.Index));
        }

        // GET: Item/Details/5
        public virtual async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accessModel = await DataAccess.GetByIdAsync(id.Value);

            if (accessModel == null)
            {
                return NotFound();
            }
            return View(ToViewModel(accessModel, ActionMode.Details));
        }

        // GET: Item/Create
        public virtual IActionResult Create()
        {
            var accessModel = new TAccessModel();

            return View(ToViewModel(accessModel, ActionMode.ViewCreate));
        }

        // POST: Item/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(TViewModel viewModel)
        {
            TAccessModel accessModel = ToAccessModel(viewModel);

            if (ModelState.IsValid)
            {
                try
                {
                    accessModel = await DataAccess.InsertAsync(accessModel);
                    await DataAccess.SaveChangesAsync();
                    return RedirectAfterAction(ActionMode.Create, accessModel);
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
            else
            {
                ViewBag.Error = string.Join("; ", ModelState.Values
                                      .SelectMany(x => x.Errors)
                                      .Select(x => x.ErrorMessage));
            }
            return View(ToViewModel(accessModel, ActionMode.Index));
        }

        // GET: Item/Edit/5
        public virtual async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accessModel = await DataAccess.GetByIdAsync(id.Value);

            if (accessModel == null)
            {
                return NotFound();
            }
            return View(ToViewModel(accessModel, ActionMode.ViewEdit));
        }

        // POST: Item/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(int id, TViewModel viewModel)
        {
            var accessModel = await DataAccess.GetByIdAsync(id);

            if (accessModel == null)
            {
                return NotFound();
            }

            accessModel.CopyFrom(viewModel);

            if (ModelState.IsValid)
            {
                try
                {
                    accessModel = await DataAccess.UpdateAsync(accessModel);
                    await DataAccess.SaveChangesAsync();
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
            else
            {
                ViewBag.Error = string.Join("; ", ModelState.Values
                                      .SelectMany(x => x.Errors)
                                      .Select(x => x.ErrorMessage));
            }
            return string.IsNullOrEmpty(ViewBag.Error) ? RedirectAfterAction(ActionMode.Edit, accessModel) : View(ToViewModel(accessModel, ActionMode.Edit));
        }

        // GET: Item/Delete/5
        public virtual async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accessModel = await DataAccess.GetByIdAsync(id.Value);

            if (accessModel == null)
            {
                return NotFound();
            }
            return View(ToViewModel(accessModel, ActionMode.ViewDelete));
        }

        // POST: Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accessModel = await DataAccess.GetByIdAsync(id);

            if (accessModel != null)
            {
                try
                {
                    await DataAccess.DeleteAsync(id);
                    await DataAccess.SaveChangesAsync();
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
            return ViewBag.Error != null ? View(ToViewModel(accessModel, ActionMode.Delete)) : RedirectAfterAction(ActionMode.Delete, accessModel);
        }

        protected override void Dispose(bool disposing)
        {
            DataAccess?.Dispose();
            base.Dispose(disposing);
        }
    }
}
//MdEnd