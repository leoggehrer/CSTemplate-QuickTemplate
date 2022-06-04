//@BaseCode
//MdStart
using System.Collections.Generic;

namespace TemplateCodeGenerator.ConApp.Contracts
{
    public interface IEntityGenerator
    {
        ISolutionProperties Properties { get; }

        IEnumerable<IGeneratedItem> GenerateAll();
        IEnumerable<IGeneratedItem> CreateBusinessEntities();
        IEnumerable<IGeneratedItem> CreateModulesEntities();
        IEnumerable<IGeneratedItem> CreatePersistenceEntities();
        IEnumerable<IGeneratedItem> CreateShadowEntities();
    }
}
//MdEnd