//@BaseCode
//MdStart
namespace TemplateCodeGenerator.ConApp.Common
{
    [Flags]
    public enum UnitType : long
    {
        General = 1,

        Contracts = 2 * General,
        Logic = 2 * Contracts,
        Transfer = 2 * Logic,
        Adapters = 2 * Transfer,
        WebApi = 2 * Adapters,

        BaseUnits = Contracts + Logic + Transfer + Adapters + WebApi,

        AspMvcApp = 2 * WebApi,
        BlazorServerApp = 2 * AspMvcApp,
        TelerikBlazorServerApp = 2 * BlazorServerApp,
        AngularApp = 2 * BlazorServerApp,

        NoneApps = 0,
        AllApps = AspMvcApp + BlazorServerApp + AngularApp,
    }
}
//MdEnd