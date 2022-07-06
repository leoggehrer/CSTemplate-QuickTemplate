//MdStart

namespace TemplateCodeGenerator.ConApp.Common
{
    [Flags]
    public enum ItemType : ulong
    {
        Entity = 1,
        Model = 2 * Entity,
        EditModel = 2 * Model,
        FilterModel = 2 * EditModel,

        AccessContract = 2 * FilterModel,
        FacadeContract = 2 * AccessContract,

        Property = 2 * FacadeContract,

        DbContext = 2 * Property,

        Controller = 2 * DbContext,
        Facade = 2 * Controller,
        Factory = 2 * Facade,

        AddServices = 2 * Factory,
        View = 2 * AddServices,
    }
}
//MdEnd