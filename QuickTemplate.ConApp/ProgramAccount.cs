namespace QuickTemplate.ConApp
{
    partial class Program
    {
#if ACCOUNT_ON
        private static string SaUser => "LeoAdmin";
        private static string SaEmail => "LeoAdmin.QuickTemplate@gmx.at";
        private static string SaPwd => "1234LeoAdmin";

        private static string AaUser => "AppAdmin";
        private static string AaEmail => "AppAdmin.QuickTemplate@gmx.at";
        private static string AaPwd => "1234AppAdmin";
        private static string AaRole => "AppAdmin";

        private static string AppUser => "AppUser";
        private static string AppEmail => "AppUser.QuickTemplate@gmx.at";
        private static string AppPwd => "1234AppUser";
        private static string AppRole => "AppUser";

        private static bool EnableJwt => true;

        private static async Task AddAppAccessAsync(string loginEmail, string loginPwd, string user, string email, string pwd, int timeOutInMinutes, bool enableJwtAuth, params string[] roles)
        {
            var login = await Logic.AccountAccess.LogonAsync(loginEmail, loginPwd, string.Empty);

            await Logic.AccountAccess.AddAppAccessAsync(login!.SessionToken, user, email, pwd, timeOutInMinutes, enableJwtAuth, roles);
            await Logic.AccountAccess.LogoutAsync(login!.SessionToken);
        }

        static partial void CreateAccount()
        {
            Task.Run(async () =>
            {
                await Logic.AccountAccess.InitAppAccessAsync(SaUser, SaEmail, SaPwd, true);
                await AddAppAccessAsync(SaEmail, SaPwd, AaUser, AaEmail, AaPwd, 30, EnableJwt, AaRole);
                await AddAppAccessAsync(SaEmail, SaPwd, AppUser, AppEmail, AppPwd, 35, EnableJwt, AppRole);
            }).Wait();
        }
#endif
    }
}
