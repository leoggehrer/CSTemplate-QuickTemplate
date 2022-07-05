using TemplateCodeGenerator.ConApp.Common;

namespace TemplateCodeGenerator.ConApp.Models
{
    internal class GenerationSetting
    {
        public UnitType UnitType { get; set; }
        public ItemType ItemType { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
