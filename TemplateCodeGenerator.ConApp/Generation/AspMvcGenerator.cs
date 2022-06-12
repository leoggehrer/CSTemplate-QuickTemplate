using TemplateCodeGenerator.ConApp.Contracts;

namespace TemplateCodeGenerator.ConApp.Generation
{
    internal partial class AspMvcGenerator : ModelGenerator
    {
        public AspMvcGenerator(ISolutionProperties solutionProperties) : base(solutionProperties)
        {
        }
        protected override string Extension => StaticLiterals.AspMvsExtension;
        protected override string Namespace => $"{SolutionProperties.SolutionName}{Extension}";

        public virtual IEnumerable<IGeneratedItem> GenerateAll()
        {
            var result = new List<IGeneratedItem>();

            result.AddRange(CreateModels());
            return result;
        }

        public virtual IEnumerable<IGeneratedItem> CreateModels()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type))
                {
                    result.Add(CreateModelFromType(type, Common.UnitType.AspMvc, Common.ItemType.Model));
                    result.Add(CreateLogicModel(type, Common.UnitType.AspMvc, Common.ItemType.Model));
                }
            }
            return result;
        }
        protected virtual IGeneratedItem CreateLogicModel(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateModelSubPath(type, "Inheritance", StaticLiterals.CSharpFileExtension),
            };
            result.Source.Add($"partial class {CreateModelName(type)} : {GetBaseClassByType(type, ModelsFolder)}");
            result.Source.Add("{");
            result.Source.Add("}");
            result.EnvelopeWithANamespace(CreateModelNamespace(type));
            result.FormatCSharpCode();
            return result;
        }
    }
}
