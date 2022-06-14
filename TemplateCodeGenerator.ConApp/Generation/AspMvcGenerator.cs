using System.Reflection;
using System.Text;
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
                    result.Add(CreateFilterModelFromType(type, Common.UnitType.AspMvc, Common.ItemType.FilterModel));
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

        protected virtual IGeneratedItem CreateFilterModelFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var sbHasValue = new StringBuilder();
            var sbToString = new StringBuilder();
            var modelName = CreateFilterModelNameFromType(type);
            var typeProperties = type.GetAllPropertyInfos();
            var filteredProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)) == false
                                                            && IsListType(e.PropertyType) == false
                                                            && (e.PropertyType.IsPrimitive || e.PropertyType == typeof(string)));
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateModelSubPath(type, "Filter", StaticLiterals.CSharpFileExtension),
            };

            result.AddRange(CreateComment(type));
            CreateModelAttributes(type, result.Source);
            result.Add($"public partial class {modelName}");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(modelName));
            result.AddRange(CreatePartialConstrutor("public", modelName));

            foreach (var propertyInfo in filteredProperties)
            {
                if (sbHasValue.Length > 0)
                    sbHasValue.Append(" || ");

                sbToString.Append($"{propertyInfo.Name}: " + "{(" + $"{propertyInfo.Name} != null ? {propertyInfo.Name} : \"---\"" + ")} ");

                sbHasValue.Append($"{propertyInfo.Name} != null");
                CreateModelPropertyAttributes(propertyInfo, result.Source);
                result.AddRange(CreateFilterAutoProperty(propertyInfo));
            }

            if (sbHasValue.Length > 0)
            {
                result.AddRange(CreateComment(type));
                result.Add($"public bool HasValue => {sbHasValue.ToString()};");
            }

            if (sbToString.Length > 0)
            {
                result.AddRange(CreateComment(type));
                result.Add("public override string ToString()");
                result.Add("{");
                result.Add($"return $\"{sbToString.ToString()}\";");
                result.Add("}");
            }
            //public override string ToString()
            //{
            //    return $"Name: {(string.IsNullOrEmpty(Name) == false ? Name : "---")} ShortDescription: {(string.IsNullOrEmpty(ShortDescription) == false ? ShortDescription : "---")} LongDescription: {(string.IsNullOrEmpty(LongDescription) == false ? LongDescription : "---")} State: {(State.HasValue ? State.ToString() : "---")}";
            //}

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
        public static string CreateFilterModelNameFromType(Type type)
        {
            return $"{CreateModelName(type)}Filter";
        }
        public virtual IEnumerable<string> CreateFilterAutoProperty(PropertyInfo propertyInfo)
        {
            var result = new List<string>();
            var propertyType = GetPropertyType(propertyInfo);

            if (propertyType.EndsWith("?") == false)
            {
                propertyType = $"{propertyType}?";
            }
            result.Add(string.Empty);
            result.AddRange(CreateComment(propertyInfo));
            result.Add($"public {propertyType} {propertyInfo.Name}");
            result.Add("{ get; set; }");
            return result;
        }
        partial void CreateModelAttributes(Type type, List<string> source);
    }
}
