//@BaseCode
//MdStart
using QuickTemplate.AspMvc.Extensions;

namespace QuickTemplate.AspMvc.Modules.Session
{
    public partial class SessionWrapper : ISessionWrapper
    {
        private ISession Session { get; }

        public SessionWrapper(ISession session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        #region General
        public bool HasValue(string key)
        {
            return Session.TryGetValue(key, out _);
        }
        public void Remove(string key)
        {
            Session.Remove(key);
        }
        #endregion General

        #region Object-Access
        public void SetValue(string key, object value)
        {
            Session.Set(key, value);
        }
        public object? GetValue(string key)
        {
            return Session.Get<object>(key);
        }
        #endregion Object-Access

        #region Int-Access
        public void SetIntValue(string key, int value)
        {
            Session.SetInt32(key, value);
        }
        public int? GetIntValue(string key)
        {
            return Session.GetInt32(key);
        }
        #endregion Int-Access

        #region String-Access
        public void SetStringValue(string key, string value)
        {
            Session.SetString(key, value);
        }
        public string? GetStringValue(string key)
        {
            return Session.GetString(key);
        }
        public string GetStringValue(string key, string defaultValue)
        {
            var result = Session.GetString(key);

            return string.IsNullOrEmpty(result) ? defaultValue : result;
        }
        #endregion String-Access

        #region Properties
        public string? ReturnUrl
        {
            get
            {
                return GetStringValue(nameof(ReturnUrl));
            }
            set
            {
                SetStringValue(nameof(ReturnUrl), value ?? string.Empty);
            }
        }
        public string? ReturnController
        {
            get
            {
                return GetStringValue(nameof(ReturnController));
            }
            set
            {
                SetStringValue(nameof(ReturnController), value ?? string.Empty);
            }
        }
        public string? ReturnAction
        {
            get
            {
                return GetStringValue(nameof(ReturnAction));
            }
            set
            {
                SetStringValue(nameof(ReturnAction), value ?? string.Empty);
            }
        }
        public string? Hint
        {
            get
            {
                return GetStringValue(nameof(Hint));
            }
            set
            {
                SetStringValue(nameof(Hint), value ?? string.Empty);
            }
        }
        public string? Error
        {
            get
            {
                return GetStringValue(nameof(Error));
            }
            set
            {
                SetStringValue(nameof(Error), value ?? string.Empty);
            }
        }
        #endregion Properties

#if ACCOUNT_ON
        #region Authentication
        public Models.Persistence.Account.LoginSession? LoginSession
        {
            get => Session.Get<Models.Persistence.Account.LoginSession>(nameof(LoginSession));
            set => Session.Set(nameof(LoginSession), value);
        }
        public string SessionToken
        {
            get
            {
                var loginSession = LoginSession;

                return loginSession != null ? loginSession.SessionToken : string.Empty;
            }
        }
        public bool IsAuthenticated
        {
            get
            {
                return LoginSession != null;
            }
        }
        public bool IsSessionAlive
        {
            get
            {
                var result = false;
                var loginSession = LoginSession;

                if (IsAuthenticated)
                {
                    var accMngr = new Adapters.Modules.Account.AccountManager();

                    result = Task.Run(async () => await accMngr.IsSessionAliveAsync(SessionToken).ConfigureAwait(false)).Result;
                }
                return result;
            }
        }
        public bool HasRole(string role, params string[] further)
        {
            var result = false;
            var loginSession = LoginSession;

            if (loginSession != null)
            {
                var accMngr = new Adapters.Modules.Account.AccountManager();

                result = Task.Run(async () => await accMngr.HasRoleAsync(loginSession.SessionToken, role).ConfigureAwait(false)).Result;
                for (int i = 0; result == false && i < further.Length; i++)
                {
                    result = Task.Run(async () => await accMngr.HasRoleAsync(loginSession.SessionToken, further[i]).ConfigureAwait(false)).Result;
                }
            }
            return result;
        }
        #endregion Authentication
#endif
    }
}
//MdEnd
