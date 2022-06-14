using TemplateCodeGenerator.ConApp.Contracts;

namespace TemplateCodeGenerator.ConApp.Generation
{
    internal partial class WebApiGenerator : ModelGenerator
    {
        public WebApiGenerator(ISolutionProperties solutionProperties) : base(solutionProperties)
        {
        }
        protected override string Extension => StaticLiterals.WebApiExtension;
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
                    result.Add(CreateModelFromType(type, Common.UnitType.WebApi, Common.ItemType.Model));
                    result.Add(CreateLogicModel(type, Common.UnitType.WebApi, Common.ItemType.Model));
                    result.Add(CreateEditModelFromType(type, Common.UnitType.WebApi, Common.ItemType.EditModel));
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

        protected virtual IGeneratedItem CreateEditModelFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var modelName = CreateEditModelNameFromType(type);
            var typeProperties = type.GetAllPropertyInfos();
            var filteredProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)) == false
                                                            && IsListType(e.PropertyType) == false);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateModelSubPath(type, "Edit", StaticLiterals.CSharpFileExtension),
            };

            result.AddRange(CreateComment(type));
            CreateModelAttributes(type, result.Source);
            result.Add($"public partial class {modelName}");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(modelName));
            result.AddRange(CreatePartialConstrutor("public", modelName));

            foreach (var propertyInfo in filteredProperties.Where(pi => pi.CanWrite))
            {
                CreateModelPropertyAttributes(propertyInfo, result.Source);
                result.AddRange(CreateProperty(propertyInfo));
            }
            result.Add("}");
            result.EnvelopeWithANamespace(CreateModelNamespace(type), "using System;");
            result.FormatCSharpCode();
            return result;
        }

        /// <summary>
        /// Diese Methode ermittelt den Edit-Model Namen aus seinem Schnittstellen Typ.
        /// </summary>
        /// <param name="type">Schnittstellen-Typ</param>
        /// <returns>Name des Models.</returns>
        public static string CreateEditModelNameFromType(Type type)
        {
            return $"{CreateModelName(type)}Edit";
        }
        partial void CreateModelAttributes(Type type, List<string> source);
    }
}
