//@BaseCode
//MdStart
#if ACCOUNT_ON
using QuickTemplate.Logic.Controllers;
using QuickTemplate.Logic.Modules.Account;
using QuickTemplate.Logic.Modules.Exceptions;
using System.Reflection;

namespace QuickTemplate.Logic.Modules.Security
{
    internal static partial class Authorization
    {
        static Authorization()
        {
            ClassConstructing();
            if (string.IsNullOrEmpty(SystemAuthorizationToken))
            {
                SystemAuthorizationToken = Guid.NewGuid().ToString();
            }
            ClassConstructed();
        }
        static partial void ClassConstructing();
        static partial void ClassConstructed();

        internal static int DefaultTimeOutInMinutes { get; private set; } = 90;
        internal static int DefaultTimeOutInSeconds => DefaultTimeOutInMinutes * 60;
        internal static string SystemAuthorizationToken { get; set; }

        internal static Task CheckAuthorizationAsync(string sessionToken, MethodBase? methodBase, AccessType accessType)
        {
            return CheckAuthorizationAsync(sessionToken, methodBase, accessType, string.Empty);
        }
        internal static async Task CheckAuthorizationAsync(string sessionToken, MethodBase? methodBase, AccessType accessType, string infoData)
        {
            _ = methodBase ?? throw new ArgumentNullException(nameof(methodBase));

            bool handled = false;

            BeforeCheckAuthorization(sessionToken, methodBase, accessType, ref handled);
            if (handled == false)
            {
                await CheckAuthorizationInternalAsync(sessionToken, methodBase, accessType, infoData).ConfigureAwait(false);
            }
            AfterCheckAuthorization(sessionToken, methodBase, accessType);
        }

        private static async Task CheckAuthorizationInternalAsync(string sessionToken, MethodBase? methodBase, AccessType accessType, string infoData)
        {
            _ = methodBase ?? throw new ArgumentNullException(nameof(methodBase));

            var originalMethodBase = methodBase.GetAsyncOriginal();

            if (sessionToken.IsNullOrEmpty())
            {
                var authorization = originalMethodBase.GetCustomAttribute<AuthorizeAttribute>();
                var isRequired = authorization?.Required ?? false;

                if (isRequired)
                    throw new AuthorizationException(ErrorType.NotLogedIn);
            }
            else if (sessionToken.Equals(SystemAuthorizationToken) == false)
            {
                var authorization = originalMethodBase.GetCustomAttribute<AuthorizeAttribute>();

                if (authorization != null && authorization.Required)
                {
                    var curSession = await AccountManager.QueryAliveSessionAsync(sessionToken).ConfigureAwait(false);

                    if (curSession == null)
                        throw new AuthorizationException(ErrorType.InvalidSessionToken);

                    if (curSession.IsTimeout)
                        throw new AuthorizationException(ErrorType.AuthorizationTimeOut);

                    var isAuthorized = authorization.Roles.Any() == false
                                    || curSession.Roles.Any(lr => authorization.Roles.Contains(lr.Designation));

                    if (isAuthorized == false)
                        throw new AuthorizationException(ErrorType.NotAuthorized);

                    curSession.LastAccess = DateTime.UtcNow;
#if LOGGING_ON
                    Logging(curSession.IdentityId, originalMethodBase.DeclaringType, originalMethodBase, accessType, infoData);
#endif
                }
            }
        }

        static partial void BeforeCheckAuthorization(string? sessionToken, MethodBase? methodBase, AccessType accessType, ref bool handled);
        static partial void AfterCheckAuthorization(string? sessionToken, MethodBase? methodBase, AccessType accessType);

        internal static async Task CheckAuthorizationAsync(string? sessionToken, Type subjectType, MethodBase methodBase, AccessType accessType, string infoData)
        {
            bool handled = false;

            BeforeCheckAuthorization(sessionToken, subjectType, methodBase, accessType, ref handled);
            if (handled == false)
            {
                await CheckAuthorizationInternalAsync(sessionToken, subjectType, methodBase, accessType, infoData).ConfigureAwait(false);
            }
            AfterCheckAuthorization(sessionToken, subjectType, methodBase, accessType);
        }

        private static async Task CheckAuthorizationInternalAsync(string? sessionToken, Type subjectType, MethodBase methodBase, AccessType accessType, string infoData)
        {
            static AuthorizeAttribute? GetClassAuthorization(Type classType)
            {
                var runType = classType;
                var result = default(AuthorizeAttribute);

                do
                {
                    result = runType.GetCustomAttributes<AuthorizeAttribute>().FirstOrDefault();
                    runType = runType.BaseType;
                } while (result == null && runType != null);
                return result;
            }

            var originalMethodBase = methodBase.GetAsyncOriginal();

            if (string.IsNullOrEmpty(sessionToken))
            {
                var authorization = originalMethodBase.GetCustomAttribute<AuthorizeAttribute>() ?? GetClassAuthorization(subjectType);
                var isRequired = authorization?.Required ?? false;

                if (isRequired)
                {
                    throw new AuthorizationException(ErrorType.NotLogedIn);
                }
            }
            else if (sessionToken.Equals(SystemAuthorizationToken) == false)
            {
                var authorization = originalMethodBase.GetCustomAttribute<AuthorizeAttribute>() ?? GetClassAuthorization(subjectType);

                if (authorization != null && authorization.Required)
                {
                    var curSession = await AccountManager.QueryAliveSessionAsync(sessionToken).ConfigureAwait(false);

                    if (curSession == null)
                        throw new AuthorizationException(ErrorType.InvalidSessionToken);

                    if (curSession.IsTimeout)
                        throw new AuthorizationException(ErrorType.AuthorizationTimeOut);

                    bool isAuthorized = authorization.Roles.Any() == false
                                        || curSession.Roles.Any(lr => authorization.Roles.Contains(lr.Designation));

                    if (isAuthorized == false)
                        throw new AuthorizationException(ErrorType.NotAuthorized);

                    curSession.LastAccess = DateTime.UtcNow;
#if LOGGING_ON
                    Logging(curSession.IdentityId, subjectType, originalMethodBase, accessType, infoData);
#endif
                }
            }
        }

        static partial void BeforeCheckAuthorization(string? sessionToken, Type subjectType, MethodBase methodBase, AccessType accessType, ref bool handled);
        static partial void AfterCheckAuthorization(string? sessionToken, Type subjectType, MethodBase methodBase, AccessType accessType);

#if LOGGING_ON
        private static void Logging(int identityId, Type? subjectType, MethodBase methodBase, AccessType accessType, string info)
        {
            Task.Run(async () =>
            {
                bool handled = false;

                BeforeLogging(subjectType, methodBase, accessType, ref handled);
                if (handled == false)
                {
                    using var actionLogCtrl = new Controllers.Account.ActionLogsController()
                    {
                        SessionToken = SystemAuthorizationToken
                    };
                    var entity = new Entities.Logging.ActionLog
                    {
                        IdentityId = identityId,
                        Time = DateTime.Now,
                        Subject = subjectType?.Name ?? "unknown",
                        Action = $"{methodBase.Name} ({accessType})",
                        Info = info
                    };
                    await actionLogCtrl.InsertAsync(entity).ConfigureAwait(false);
                    await actionLogCtrl.SaveChangesAsync().ConfigureAwait(false);
                }
                AfterLogging(subjectType, methodBase, accessType);
            });
        }
        static partial void BeforeLogging(Type? subjectType, MethodBase methodBase, AccessType accessType, ref bool handled);
        static partial void AfterLogging(Type? subjectType, MethodBase methodBase, AccessType accessType);
#endif
    }
}
#endif
//MdEnd