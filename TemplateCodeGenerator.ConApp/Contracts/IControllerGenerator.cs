//@BaseCode
//MdStart
using System.Collections.Generic;

namespace TemplateCodeGenerator.ConApp.Contracts
{
    public interface IControllerGenerator
    {
        ISolutionProperties Properties { get; }

        IEnumerable<IGeneratedItem> CreateBusinessControllers();
        IEnumerable<IGeneratedItem> CreatePersistenceControllers();
        IEnumerable<IGeneratedItem> CreateShadowControllers();

        IEnumerable<IGeneratedItem> CreateWebApiControllers();

        IEnumerable<IGeneratedItem> CreateAspMvcControllers();
    }
}
//MdEnd