//@BaseCode
//MdStart
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace QuickTemplate.WebApi.Controllers
{
    /// <summary>
    /// A base class for an MVC controller without view support.
    /// </summary>
    public abstract partial class ApiControllerBase : ControllerBase
    {
        static ApiControllerBase()
        {
            ClassConstructing();
            ClassConstructed();
        }
        static partial void ClassConstructing();
        static partial void ClassConstructed();
        /// <summary>
        /// Initializes a new instance of the ApiControllerBase class.
        /// </summary>
        protected ApiControllerBase()
        {
            Constructing();
            Constructed();
        }
        partial void Constructing();
        partial void Constructed();
#if ACCOUNT_ON
        /// <summary>
        /// Retrieves the session token from the header.
        /// </summary>
        /// <returns>The session token.</returns>
        protected string GetSessionToken()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"];

            return Task.Run(async() => await GetSessionTokenAsync(authHeader)).Result;
        }
        /// <summary>
        /// Retrieves the session token from the header.
        /// </summary>
        /// <returns>The session token.</returns>
        protected Task<string> GetSessionTokenAsync()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"];

            return GetSessionTokenAsync(authHeader);
        }
        /// <summary>
        /// Retrieves the session token from the header.
        /// </summary>
        /// <param name="authHeader">The authorization header.</param>
        /// <returns>The session token.</returns>
        protected static async Task<string> GetSessionTokenAsync(string authHeader)
        {
            string result = string.Empty;

            if (authHeader.HasContent())
            {
                if (authHeader.StartsWith("Bearer"))
                {
                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    var encodedToken = authHeader["Bearer ".Length..].Trim();

                    result = encoding.GetString(Convert.FromBase64String(encodedToken));
                }
                else if (authHeader.StartsWith("Basic"))
                {
                    var encodedUseridPassword = authHeader["Basic ".Length..].Trim();
                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    var useridPassword = encoding.GetString(Convert.FromBase64String(encodedUseridPassword));

                    var seperatorIndex = useridPassword.IndexOf(':');
                    var userid = useridPassword[..seperatorIndex];
                    var password = useridPassword[(seperatorIndex + 1)..];
                    var login = await Logic.AccountAccess.LogonAsync(userid, password, string.Empty).ConfigureAwait(false);

                    result = login.SessionToken;
                }
                else if (authHeader.StartsWith("SessionToken"))
                {
                    result = authHeader["SessionToken ".Length..].Trim();
                }
                else
                {
                    result = authHeader;
                }
            }
            return result;
        }
#endif
    }
}
//MdEnd