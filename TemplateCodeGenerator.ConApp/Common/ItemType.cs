//MdStart

namespace TemplateCodeGenerator.ConApp.Common
{
    [Flags]
    public enum ItemType : ulong
    {
        DbContext = 1,
        Factory = 2 * DbContext,

        Entity = 2 * Factory,

        Model = 2 * Entity,
        EditModel = 2 * Model,
        FilterModel = 2 * EditModel,

        AccessContract = 2 * FilterModel,
        Controller = 2 * AccessContract,

        FacadeContract = 2 * Controller,
        Facade = 2 * FacadeContract,

        AddServices = 2 * Facade,

        View = 2 * AddServices,
    }
}
//MdEnd