//MdStart

namespace TemplateCodeGenerator.ConApp.Common
{
    [Flags]
    public enum ItemType : ulong
    {
        DbContext = 1,
        Factory = 2,

        Entity = 4,

        Model = 8,
        EditModel = 2 * Model,
        FilterModel = 2 * EditModel,

        AccessContract = 2 * FilterModel,

        Controller = 64,
        Facade = 2 * Controller,
    }
}
//MdEnd