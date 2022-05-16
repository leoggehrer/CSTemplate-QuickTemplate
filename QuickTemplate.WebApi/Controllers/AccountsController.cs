//@BaseCode
//MdStart
#if ACCOUNT_ON
using Microsoft.AspNetCore.Mvc;

namespace QuickTemplate.WebApi.Controllers
{
    /// <summary>
    /// A base class for an MVC controller without view support.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        /// <summary>  
        /// This method checks the login data email/password and, if correct, returns a logon session.  
        /// </summary>  
        /// <param name="email">The user email.</param>  
        /// <param name="password">The password.</param>  
        /// <param name="info">Logoninfo (optional)</param>  
        /// <returns>The logon session object.</returns>  
        [HttpGet("logon", Name = nameof(LogonByAsync))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Logic.Models.Account.LoginSession>> LogonByAsync(
            [FromQuery(Name = "email")] string email,
            [FromQuery(Name = "password")] string password,
            [FromQuery(Name = "info")] string? info)
        {
            var result = await Logic.AccountAccess.LogonAsync(email, password, info ?? string.Empty);

            return Ok(result);
        }

        /// <summary>  
        /// This query determines the payments depending on the parameters.  
        /// </summary>  
        /// <param name="sessionToken">The sessionToken.</param>  
        /// <returns>The logon session object.</returns>  
        [HttpGet("logout", Name = nameof(LogoutByAsync))]
        public Task LogoutByAsync([FromQuery(Name = "sessionToken")] string sessionToken)
        {
            return Logic.AccountAccess.LogoutAsync(sessionToken);
        }
    }
}
#endif
//MdEnd