//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace QuickTemplate.Logic.Controllers.Account
{
    internal sealed partial class RolesController : GenericController<Entities.Account.Role>
    {
        public RolesController()
        {
        }

        public RolesController(ControllerObject other) : base(other)
        {
        }
    }
}
#endif
//MdEnd