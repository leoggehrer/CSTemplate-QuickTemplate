//@BaseCode
//MdStart

namespace TemplateCodeGenerator.ConApp.Contracts
{
    public interface IDataContextGenerator
    {
        ISolutionProperties Properties { get; }

        IGeneratedItem CreateDbContext();
    }
}
//MdEnd