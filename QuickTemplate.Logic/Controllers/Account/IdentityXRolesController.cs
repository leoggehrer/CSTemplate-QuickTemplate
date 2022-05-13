//@BaseCode
//MdStart
#if ACCOUNT_ON
using QuickTemplate.Logic.Entities.Account;

namespace QuickTemplate.Logic.Controllers.Account
{
    internal sealed partial class IdentityXRolesController : GenericController<Entities.Account.IdentityXRole>
    {
        public IdentityXRolesController()
        {
        }

        public IdentityXRolesController(ControllerObject other) : base(other)
        {
        }
        public async Task<Role[]> QueryIdentityRolesAsync(int identityId)
        {
            var result = new List<Role>();
            var roles = await EntitySet.Where(e => e.IdentityId == identityId)
                                       .Include(e => e.Role)
                                       .Select(e => e.Role)
                                       .ToArrayAsync();
            foreach (var role in roles)
            {
                if (role != null)
                    result.Add(role);
            }
            return result.ToArray();
        }
    }
}
#endif
//MdEnd