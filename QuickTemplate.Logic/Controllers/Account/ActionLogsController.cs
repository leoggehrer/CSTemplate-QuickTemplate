//@BaseCode
//MdStart
#if ACCOUNT_ON
using QuickTemplate.Logic.Entities.Logging;

namespace QuickTemplate.Logic.Controllers.Account
{
    internal sealed partial class ActionLogsController : GenericController<ActionLog>
    {
        public ActionLogsController()
        {
        }

        public ActionLogsController(ControllerObject other) : base(other)
        {
        }
    }
}
#endif
//MdEnd