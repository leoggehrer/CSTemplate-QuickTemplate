//@BaseCode
//MdStart
#if ACCOUNT_ON && REVISION_ON

namespace QuickTemplate.Logic.Controllers.Revision
{
    internal partial class HistoriesController : GenericController<Entities.Revision.History>
    {
        public HistoriesController()
        {
        }

        public HistoriesController(ControllerObject other) : base(other)
        {
        }
    }
}
#endif
//MdEnd