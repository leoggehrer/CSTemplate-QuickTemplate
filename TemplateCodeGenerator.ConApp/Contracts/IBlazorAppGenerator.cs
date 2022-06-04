//@BaseCode
//MdStart

using System.Collections.Generic;

namespace TemplateCodeGenerator.ConApp.Contracts
{
    public interface IBlazorAppGenerator : IModelGenerator
    {
        IEnumerable<IGeneratedItem> CreateBusinessIndexPages();
        IEnumerable<IGeneratedItem> CreatePersistenceIndexPages();
        IEnumerable<IGeneratedItem> CreateShadowIndexPages();
    }
}
//MdEnd