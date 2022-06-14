//MdStart
using System.Collections.Generic;

namespace TemplateCodeGenerator.ConApp.Contracts
{
    public interface IGeneratedItem
    {
        Common.UnitType UnitType { get; }
        Common.ItemType ItemType { get; }
        string FullName { get; }
        string SubFilePath { get; }
        string FileExtension { get; }
        IEnumerable<string> SourceCode { get; }
    }
}
//MdEnd