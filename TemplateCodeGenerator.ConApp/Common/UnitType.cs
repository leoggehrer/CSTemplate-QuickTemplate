//MdStart
namespace TemplateCodeGenerator.ConApp.Common
{
    [Flags]
    public enum UnitType : long
    {
        General = 1,

        Logic = 2 * General,
        WebApi = 2 * Logic,
        AspMvc = 2 * WebApi,
    }
}
//MdEnd