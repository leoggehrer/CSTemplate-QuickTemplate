//@BaseCode
//MdStart

namespace TemplateCodeGenerator.ConApp.Contracts
{
    public interface IFactoryGenerator
    {
        ISolutionProperties Properties { get; }

        IGeneratedItem CreateLogicFactory();
        IGeneratedItem CreateAdapterFactory();
        IGeneratedItem CreateThirdPartyFactory();
    }
}
//MdEnd