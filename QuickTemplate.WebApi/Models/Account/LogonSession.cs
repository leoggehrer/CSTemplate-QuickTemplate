//@BaseCode
//MdStart
namespace QuickTemplate.WebApi.Models.Account
{
#if ACCOUNT_ON
    /// <summary>
    /// A model class for the login data.
    /// </summary>
    public class LogonSession
    {
        /// <summary>
        /// The reference to the identity.
        /// </summary>
        public int IdentityId { get; set; }
        /// <summary>
        /// The session token.
        /// </summary>
        public string? SessionToken { get; set; }
        /// <summary>
        /// The time of registration.
        /// </summary>
        public DateTime? LoginTime { get; set; }

        /// <summary>
        /// The user name.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// The user email.
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// The login info (optional).
        /// </summary>
        public string? OptionalInfo { get; set; }
    }
#endif
}
//MdEnd