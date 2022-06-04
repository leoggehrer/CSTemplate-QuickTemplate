//@BaseCode
//MdStart

namespace TemplateCodeGenerator.ConApp.Contracts
{
    public interface IAngularAppGenerator
    {
        ISolutionProperties Properties { get; }

        IEnumerable<IGeneratedItem> CreateEnums();
        IEnumerable<IGeneratedItem> CreateBusinessContracts();
        IEnumerable<IGeneratedItem> CreateModulesContracts();
        IEnumerable<IGeneratedItem> CreatePersistenceContracts();
        IEnumerable<IGeneratedItem> CreateShadowContracts();
    }
}
//MdEnd