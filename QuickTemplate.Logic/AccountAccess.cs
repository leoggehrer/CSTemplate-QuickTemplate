//@BaseCode
//MdStart
#if ACCOUNT_ON
using QuickTemplate.Logic.Entities.Account;
using QuickTemplate.Logic.Modules.Account;

namespace QuickTemplate.Logic
{
    public static partial class AccountAccess
    {
        public static Task<bool> IsSessionAliveAsync(string sessionToken)
        {
            return AccountManager.IsSessionAliveAsync(sessionToken);
        }

        public static Task InitAppAccessAsync(string name, string email, string password, bool enableJwtAuth)
        {
            return AccountManager.InitAppAccessAsync(name, email, password, enableJwtAuth);
        }
        public static Task AddAppAccessAsync(string sessionToken, string name, string email, string password, bool enableJwtAuth, params string[] roles)
        {
            return AccountManager.AddAppAccessAsync(sessionToken, name, email, password, enableJwtAuth, roles);
        }

        public static Task<LoginSession> LogonAsync(string jsonWebToken)
        {
            return AccountManager.LogonAsync(jsonWebToken);
        }
        public static Task<LoginSession> LogonAsync(string email, string password)
        {
            return AccountManager.LogonAsync(email, password);
        }
        public static Task<LoginSession> LogonAsync(string email, string password, string optionalInfo)
        {
            return AccountManager.LogonAsync(email, password, optionalInfo);
        }

        public static Task<bool> HasRoleAsync(string sessionToken, string role)
        {
            return AccountManager.HasRoleAsync(sessionToken, role);
        }
        public static Task<LoginSession?> QueryLoginAsync(string sessionToken)
        {
            return AccountManager.QueryLoginAsync(sessionToken);
        }
        public static Task<IEnumerable<string>> QueryRolesAsync(string sessionToken)
        {
            return AccountManager.QueryRolesAsync(sessionToken);
        }

        public static Task ChangePasswordAsync(string sessionToken, string oldPassword, string newPassword)
        {
            return AccountManager.ChangePasswordAsync(sessionToken, oldPassword, newPassword);
        }
        public static Task ChangePasswordForAsync(string sessionToken, string email, string newPassword)
        {
            return AccountManager.ChangePasswordAsync(sessionToken, email, newPassword);
        }
        public static Task ResetFailedCountForAsync(string sessionToken, string email)
        {
            return AccountManager.ResetFailedCountForAsync(sessionToken, email);
        }

        public static Task LogoutAsync(string sessionToken)
        {
            return AccountManager.LogoutAsync(sessionToken);
        }
    }
}
#endif
//MdEnd