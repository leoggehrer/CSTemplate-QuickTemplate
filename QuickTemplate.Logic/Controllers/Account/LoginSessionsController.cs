//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace QuickTemplate.Logic.Controllers.Account
{
    [Modules.Security.Authorize("SysAdmin", "AppAdmin")]
    internal sealed partial class LoginSessionsController : GenericController<Entities.Account.LoginSession>
    {
        public LoginSessionsController()
        {
        }

        public LoginSessionsController(ControllerObject other) : base(other)
        {
        }

        protected override void BeforeActionExecute(ActionType actionType, Entities.Account.LoginSession entity)
        {
            if (actionType == ActionType.Insert)
            {
                entity.SessionToken = Guid.NewGuid().ToString();
                entity.LoginTime = entity.LastAccess = DateTime.UtcNow;
            }
            base.BeforeActionExecute(actionType, entity);
        }
        public Task<Entities.Account.LoginSession[]> QueryOpenLoginSessionsAsync()
        {
            return EntitySet.Where(e => e.LogoutTime.HasValue == false)
                            .Include(e => e.Identity)
                            .ToArrayAsync();
        }
    }
}
#endif
//MdEnd