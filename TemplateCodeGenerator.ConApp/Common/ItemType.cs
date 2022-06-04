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

        LogicModel = 8,
        WebApiModel = 16,
        AspMvcModel = 32,
        Model = LogicModel + WebApiModel + AspMvcModel,

    }
}
//MdEnd