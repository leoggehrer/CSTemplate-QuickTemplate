//@BaseCode
//MdStart
#if ACCOUNT_ON
using Microsoft.AspNetCore.Mvc;
using QuickTemplate.Logic;
using QuickTemplate.Logic.Entities.Account;
using FilterType = QuickTemplate.AspMvc.Models.Account.RoleFilter;

namespace QuickTemplate.AspMvc.Controllers.Account
{
    public partial class RolesController : GenericController<Logic.Entities.Account.Role, Models.Account.Role>
    {
        private static string FilterName => typeof(FilterType).Name;
        public RolesController(IDataAccess<Logic.Entities.Account.Role> dataAccess) : base(dataAccess)
        {
        }

        protected override Role[] AfterQuery(Role[] accessModels)
        {
            return base.AfterQuery(accessModels);
        }

        public override Task<IActionResult> Index()
        {
            ViewBag.Filter = SessionWrapper.Get<FilterType>(FilterName) ?? new FilterType();

            return base.Index();
        }
        public async Task<IActionResult> Filter(FilterType filter)
        {
            var accessModels = await DataAccess.GetAllAsync();

            ViewBag.Filter = filter;
            SessionWrapper.Set<FilterType>(FilterName, filter);
            return View("Index", AfterQuery(accessModels).Select(e => ToViewModel(e, ActionMode.Index)));
        }
    }
}
#endif
//MdEnd