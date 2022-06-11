//@BaseCode
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
        EditModel = 16,

        AccessContract = 32,

        Controller = 64,
    }
}
//MdEnd