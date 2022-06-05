//@BaseCode
//MdStart
namespace TemplateCodeGenerator.ConApp.Contracts
{
    public interface IModelGenerator
    {
        ISolutionProperties Properties { get; }
        string ModelsFolder { get; }

        string CreateModelsNamespace();
        string CreateModelTypeNamespace(Type type);

        IEnumerable<IGeneratedItem> GenerateAll();
        IEnumerable<IGeneratedItem> CreateLogicModels();
        IEnumerable<IGeneratedItem> CreateWebApiModels();
        IEnumerable<IGeneratedItem> CreateAspMvcModels();
    }
}
//MdEnd