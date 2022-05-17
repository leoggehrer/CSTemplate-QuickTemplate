//@BaseCode
//MdStart
#if ACCOUNT_ON
using QuickTemplate.Logic;

namespace QuickTemplate.AspMvc.Controllers.Account
{
    public class RolesController : GenericController<Logic.Entities.Account.Role, Models.Account.Role>
    {
        public RolesController(IDataAccess<Logic.Entities.Account.Role> dataAccess) : base(dataAccess)
        {
        }
    }
}
#endif
//MdEnd