//MdStart
using TemplateCodeGenerator.ConApp.Common;

namespace TemplateCodeGenerator.ConApp.Models
{
    internal partial class GeneratedItem : Contracts.IGeneratedItem
    {
        public GeneratedItem()
        {

        }
        public GeneratedItem(UnitType unitType, ItemType itemType)
        {
            UnitType = unitType;
            ItemType = itemType;
        }
        public UnitType UnitType { get; }
        public ItemType ItemType { get; }
        public string FullName { get; init; } = string.Empty;
        public string SubFilePath { get; init; } = string.Empty;
        public string FileExtension { get; init; } = string.Empty;
        public IEnumerable<string> SourceCode => Source;

        public List<string> Source { get; } = new List<string>();

        public void Add(string item)
        {
            Source.Add(item);
        }
        public void AddRange(IEnumerable<string> collection)
        {
            Source.AddRange(collection);
        }
        public void EnvelopeWithANamespace(string nameSpace, params string[] usings)
        {
            var codeLines = new List<string>();

            if (nameSpace.HasContent())
            {
                codeLines.Add($"namespace {nameSpace}");
                codeLines.Add("{");
                codeLines.AddRange(usings);
            }
            codeLines.AddRange(Source.Eject());
            if (nameSpace.HasContent())
            {
                codeLines.Add("}");
            }
            Source.AddRange(codeLines);
        }
        public void FormatCSharpCode(bool removeBlockComments = false, bool removeLineComments = false)
        {
            Source.AddRange(Source.Eject().FormatCSharpCode(removeBlockComments, removeLineComments));
        }
        public override string ToString()
        {
            return $"{UnitType,-15}{ItemType,-20}{FullName,-30}{SubFilePath,-50}";
        }
    }
}
//MdEnd