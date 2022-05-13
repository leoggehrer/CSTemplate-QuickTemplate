//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace QuickTemplate.Logic.Controllers.Account
{
    internal sealed partial class IdentitiesController : GenericController<Entities.Account.Identity>
    {
        public IdentitiesController()
        {
        }

        public IdentitiesController(ControllerObject other) : base(other)
        {
        }
        public Task<Entities.Account.Identity?> GetValidIdentityByEmail(string email)
        {
            return EntitySet.Include(e => e.IdentityXRoles)
                                 .ThenInclude(e => e.Role)
                                 .FirstOrDefaultAsync(e => e.State == Modules.Common.State.Active
                                                        && e.AccessFailedCount < 4
                                                        && e.Email.ToLower() == email.ToLower());
        }
    }
}
#endif
//MdEnd