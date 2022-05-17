//@BaseCode
//MdStart
using Microsoft.AspNetCore.Mvc;
using QuickTemplate.AspMvc.Modules.Session;
#if ACCOUNT_ON
using Microsoft.AspNetCore.Mvc.Filters;
using QuickTemplate.AspMvc.Controllers.Account;
#endif
namespace QuickTemplate.AspMvc.Controllers
{
    public partial class MvcController : Controller
    {
        static MvcController()
        {
            ClassConstructing();
            ClassConstructed();
        }
        static partial void ClassConstructing();
        static partial void ClassConstructed();

        public MvcController()
        {
            Constructing();
            Constructed();
        }
        partial void Constructing();
        partial void Constructed();

#region SessionInfo
        public bool IsSessionAvailable => HttpContext?.Session != null;
        private ISessionWrapper? sessionInfo = null;
        internal ISessionWrapper SessionInfo => sessionInfo ??= new SessionWrapper(HttpContext.Session);
#endregion SessionInfo

#if ACCOUNT_ON
        protected virtual bool CheckSessionToken { get; set; } = true; 
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (CheckSessionToken && context.Controller?.GetType().Name.Equals(nameof(AccountController)) == false)
            {
                if (SessionInfo.IsAuthenticated == false)
                {
                    ViewBag.Error = ("You are not yet registered. Please log in to the system.");
                    context.Result = new RedirectToActionResult("Logon", "Account", null);
                }
                else if (SessionInfo.IsSessionAlive == false)
                {
                    ViewBag.Error = ("Your session has expired. Please sign in again.");
                    context.Result = new RedirectToActionResult("Logon", "Account", null);
                }
            }
            base.OnActionExecuting(context);
        }
#endif

#region Error-helpers
        protected string GetModelStateError()
        {
            var errors = GetModelStateErrors();

            return string.Join($"{Environment.NewLine}", errors);
        }
        protected string[] GetModelStateErrors()
        {
            var list = new List<string>();
            var errorLists = ModelState.Where(x => x.Value?.Errors.Count > 0)
                                       .Select(x => new { x.Key, x.Value?.Errors });

            foreach (var errorList in errorLists)
            {
                if (errorList.Errors != null)
                {
                    foreach (var error in errorList.Errors)
                    {
                        list.Add($"{errorList.Key}: {error.ErrorMessage}");
                    }
                }
            }
            return list.ToArray();
        }
        protected static string GetExceptionError(Exception source)
        {
            return source.GetError();
        }
#endregion Error-Helpers
    }
}
//MdEnd