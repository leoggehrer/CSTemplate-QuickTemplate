//@BaseCode
//MdStart
namespace QuickTemplate.Logic.Models.Account
{
#if ACCOUNT_ON
    /// <summary>
    /// A model class for the login data.
    /// </summary>
    public class LoginSession
    {
        /// <summary>
        /// The reference to the identity.
        /// </summary>
        public int IdentityId { get; set; }
        /// <summary>
        /// The session token.
        /// </summary>
        public string SessionToken { get; set; } = string.Empty;
        /// <summary>
        /// The time of registration.
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// The user name.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// The user email.
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// The login info (optional).
        /// </summary>
        public string? OptionalInfo { get; set; }
        /// <summary>
        /// The login roles.
        /// </summary>
        public Role[] Roles { get; set; } = Array.Empty<Role>();

        internal static LoginSession Create(Entities.Account.LoginSession loginSession)
        {
            var result = new LoginSession();

            result.CopyFrom(loginSession);
            result.Roles = loginSession.Roles.Select(r => Role.Create(r)).ToArray();
            return result;
        }
    }
#endif
}
//MdEnd
