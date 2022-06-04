//@BaseCode
//MdStart
using TemplateCodeGenerator.ConApp.Common;

namespace TemplateCodeGenerator.ConApp.Contracts
{
    public interface IModelGenerator
    {
        ISolutionProperties Properties { get; }

        string ModelsFolder { get; }
        string AppModelsNameSpace { get; }

        IEnumerable<IGeneratedItem> GenerateAll();
        IEnumerable<IGeneratedItem> CreateLogicModels();
        IEnumerable<IGeneratedItem> CreateWebApiModels();
        IEnumerable<IGeneratedItem> CreateAspMvcModels();
    }
}
//MdEnd