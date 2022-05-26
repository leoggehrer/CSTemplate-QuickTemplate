//@BaseCode
//MdStart
#if ACCOUNT_ON
using Microsoft.AspNetCore.Mvc;
using QuickTemplate.AspMvc.Models.Account;

namespace QuickTemplate.AspMvc.Controllers.Account
{
    public partial class AccountController : MvcController
    {
        static AccountController()
        {
            ClassConstructing();
            ClassConstructed();
        }
        static partial void ClassConstructing();
        static partial void ClassConstructed();

        public AccountController()
            : base()
        {
            Constructing();
            Constructed();
        }
        partial void Constructing();
        partial void Constructed();

        public IActionResult Logon(string? returnUrl = null, string? error = null)
        {
            var handled = false;
            var viewName = nameof(Logon);
            var viewModel = new LogonViewModel()
            {
                ReturnUrl = returnUrl,
            };

            BeforeLogon(viewModel, ref handled);
            if (handled == false)
            {
                SessionWrapper.ReturnUrl = returnUrl;
                if (error.HasContent())
                    ViewBag.Error = error;
            }
            AfterLogon(viewModel, ref viewName);
            return View(viewName, viewModel);
        }
        partial void BeforeLogon(LogonViewModel model, ref bool handled);
        partial void AfterLogon(LogonViewModel model, ref string viewName);

        [HttpGet]
        [ActionName("Index")]
        public virtual IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Logon")]
        public async Task<IActionResult> LogonAsync(LogonViewModel viewModel)
        {
            if (ModelState.IsValid == false)
            {
                return View(viewModel);
            }
            bool handled = false;
            var action = SessionWrapper.ReturnAction ?? "Index";
            var controller = SessionWrapper.ReturnController ?? "Home";

            BeforeDoLogon(viewModel, ref handled);
            if (handled == false)
            {
                try
                {
                    await ExecuteLogonAsync(viewModel).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.GetError();
                    return View(viewModel);
                }
            }
            AfterDoLogon(viewModel, ref action, ref controller);
            if (viewModel.ReturnUrl != null)
            {
                return Redirect(viewModel.ReturnUrl);
            }
            return RedirectToAction(action, controller);
        }
        partial void BeforeDoLogon(LogonViewModel model, ref bool handled);
        partial void AfterDoLogon(LogonViewModel model, ref string action, ref string controller);

        public IActionResult LogonRemote(string? returnUrl = null, string? error = null)
        {
            var handled = false;
            var viewName = nameof(LogonRemote);
            var viewModel = new LogonViewModel()
            {
                ReturnUrl = returnUrl,
            };

            BeforeLogonRemote(viewModel, ref handled);
            if (handled == false)
            {
                SessionWrapper.ReturnUrl = returnUrl;
                if (error.HasContent())
                    ViewBag.Error = error;
            }
            AfterLogonRemote(viewModel, ref viewName);
            return View(viewName, viewModel);
        }
        partial void BeforeLogonRemote(LogonViewModel model, ref bool handled);
        partial void AfterLogonRemote(LogonViewModel model, ref string viewName);

        [ActionName("Logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            if (SessionWrapper.LoginSession != null)
            {
                bool handled = false;

                BeforeLogout(ref handled);
                if (handled == false)
                {
                    await Logic.AccountAccess.LogoutAsync(SessionWrapper.LoginSession.SessionToken).ConfigureAwait(false);
                    SessionWrapper.LoginSession = null;
                }
                AfterLogout();
            }
            return RedirectToAction("Index", "Home");
        }
        partial void BeforeLogout(ref bool handled);
        partial void AfterLogout();

        public IActionResult ChangePassword()
        {
            var handled = false;
            var viewModel = new ChangePasswordViewModel();
            var viewName = "ChangePassword";

            BeforeChangePassword(viewModel, ref handled);
            if (handled == false)
            {
                if (SessionWrapper.LoginSession == null
                    || SessionWrapper.LoginSession.LogoutTime.HasValue)
                {
                    return RedirectToAction("Logon", new { returnUrl = "ChangePassword" });
                }
                viewModel.UserName = SessionWrapper.LoginSession?.Name ?? string.Empty;
                viewModel.Email = SessionWrapper.LoginSession?.Email ?? String.Empty;
            }
            AfterChangePassword(viewModel, ref viewName);
            return View(viewName, viewModel);
        }
        partial void BeforeChangePassword(ChangePasswordViewModel model, ref bool handled);
        partial void AfterChangePassword(ChangePasswordViewModel model, ref string viewName);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel viewModel)
        {
            if (ModelState.IsValid == false)
            {
                return View(viewModel);
            }
            bool handled = false;
            var viewName = "ConfirmationChangePassword";

            BeforeDoChangePassword(viewModel, ref handled);
            if (handled == false)
            {
                if (SessionWrapper.LoginSession == null
                    || SessionWrapper.LoginSession.LogoutTime.HasValue)
                {
                    return RedirectToAction("Logon", new { returnUrl = "ChangePassword" });
                }

                try
                {
                    await Logic.AccountAccess.ChangePasswordAsync(SessionWrapper.LoginSession?.SessionToken ?? string.Empty, viewModel.OldPassword, viewModel.NewPassword).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.GetError();
                    return View("ChangePassword", viewModel);
                }
            }
            AfterDoChangePassword(viewModel, ref viewName);
            return View(viewName);
        }
        partial void BeforeDoChangePassword(ChangePasswordViewModel model, ref bool handled);
        partial void AfterDoChangePassword(ChangePasswordViewModel model, ref string viewName);

        public IActionResult ResetPassword()
        {
            var handled = false;
            var viewModel = new ResetPasswordViewModel();
            var viewName = "ResetPassword";

            BeforeResetPassword(viewModel, ref handled);
            if (handled == false)
            {
                if (SessionWrapper.LoginSession == null
                    || SessionWrapper.LoginSession.LogoutTime.HasValue)
                {
                    return RedirectToAction("Logon", new { returnUrl = "ChangePassword" });
                }
            }
            AfterResetPassword(viewModel, ref viewName);
            return View(viewName, viewModel);
        }
        partial void BeforeResetPassword(ResetPasswordViewModel model, ref bool handled);
        partial void AfterResetPassword(ResetPasswordViewModel model, ref string viewName);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid == false)
            {
                return View(viewModel);
            }
            bool handled = false;
            var viewName = "ConfirmationResetPassword";

            BeforeDoResetPassword(viewModel, ref handled);
            if (SessionWrapper.LoginSession == null
                || SessionWrapper.LoginSession.LogoutTime.HasValue)
            {
                return RedirectToAction("Logon", new { returnUrl = "ResetPassword" });
            }

            try
            {
                await Logic.AccountAccess.ChangePasswordForAsync(SessionWrapper.SessionToken, viewModel.Email, viewModel.ConfirmPassword).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.GetError();
                return View("ResetPassword", viewModel);
            }
            AfterDoResetPassword(viewModel, ref viewName);
            return View(viewName);
        }
        partial void BeforeDoResetPassword(ResetPasswordViewModel model, ref bool handled);
        partial void AfterDoResetPassword(ResetPasswordViewModel model, ref string viewName);

        private async Task ExecuteLogonAsync(LogonViewModel viewModel)
        {
            try
            {
                var internLogin = await Logic.AccountAccess.LogonAsync(viewModel.Email, viewModel.Password).ConfigureAwait(false);
                var loginSession = new LoginSession();

                if (internLogin != null)
                {
                    loginSession.CopyFrom(internLogin);
                }
                SessionWrapper.LoginSession = loginSession;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.GetError();
                throw;
            }
        }
    }
}
#endif
//MdEnd
