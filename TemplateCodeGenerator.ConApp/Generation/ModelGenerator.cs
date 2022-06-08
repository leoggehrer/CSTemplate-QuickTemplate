//MdStart
using System.Reflection;
using TemplateCodeGenerator.ConApp.Contracts;

namespace TemplateCodeGenerator.ConApp.Generation
{
    internal partial class ModelGenerator : ClassGenerator, IModelGenerator
    {
        #region Models
        internal static string ModelObject => nameof(ModelObject);
        internal static string ModuleModel => nameof(ModuleModel);
        internal static string IdentityModel => nameof(IdentityModel);
        internal static string VersionModel => nameof(VersionModel);
        #endregion Models

        public ModelGenerator(ISolutionProperties solutionProperties)
            : base(solutionProperties)
        {
        }

        public string ModelsFolder { get; } = StaticLiterals.ModelsFolder;
        private string Namespace { get; set; } = string.Empty;
        public string CreateModelType(Type type)
        {
            return $"{CreateModelTypeNamespace(type)}.{type.Name}";
        }
        public string CreateModelTypeNamespace(Type type)
        {
            var modelSubNamespace = $"{ModelsFolder}.{CreateSubNamespaceFromEntityType(type)}";

            return $"{Namespace}.{modelSubNamespace}";
        }
        public string CreateModelTypeSubNamespace(Type type)
        {
            return $"{ModelsFolder}.{CreateSubNamespaceFromEntityType(type)}";
        }
        public string CreateModelSubPathFromType(Type type, string postFix, string fileExtension)
        {
            return Path.Combine(CreateModelTypeSubNamespace(type).Replace(".", "\\"), $"{type.Name}{postFix}{fileExtension}");
        }

        protected virtual bool CanCreate(Type type)
        {
            bool create = true;

            CanCreateModel(type, ref create);
            return create;
        }
        partial void CanCreateModel(Type type, ref bool create);
        partial void CreateModelAttributes(Type type, List<string> codeLines);
        protected virtual void CreateModelPropertyAttributes(PropertyInfo propertyInfo, List<string> codeLines)
        {
            var handled = false;

            BeforeCreateModelPropertyAttributes(propertyInfo, codeLines, ref handled);
            if (handled == false)
            {
            }
            AfterCreateModelPropertyAttributes(propertyInfo, codeLines);
        }

        partial void BeforeCreateModelPropertyAttributes(PropertyInfo propertyInfo, List<string> codeLines, ref bool handled);
        partial void AfterCreateModelPropertyAttributes(PropertyInfo propertyInfo, List<string> codeLines);

        public virtual IEnumerable<IGeneratedItem> GenerateAll()
        {
            var result = new List<IGeneratedItem>();

            result.AddRange(CreateLogicModels());
            result.AddRange(CreateWebApiModels());
            result.AddRange(CreateAspMvcModels());
            return result;
        }

        public virtual IEnumerable<IGeneratedItem> CreateLogicModels()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            Namespace = $"{SolutionProperties.SolutionName}{StaticLiterals.LogicExtension}";
            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type))
                {
                    result.Add(CreateModelFromType(type, Common.UnitType.Logic, Common.ItemType.LogicModel));
                    result.Add(CreateLogicModel(type, Common.UnitType.Logic, Common.ItemType.LogicModel));
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
                SubFilePath = CreateModelSubPathFromType(type, "Inheritance", StaticLiterals.CSharpFileExtension),
            };
            result.Source.Add($"partial class {CreateModelNameFromType(type)} : {GetBaseClassByType(type, ModelsFolder)}");
            result.Source.Add("{");
            result.Source.Add("}");
            result.EnvelopeWithANamespace(CreateModelTypeNamespace(type));
            result.FormatCSharpCode();
            return result;
        }

        public virtual IEnumerable<IGeneratedItem> CreateWebApiModels()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            Namespace = $"{SolutionProperties.SolutionName}{StaticLiterals.WebApiExtension}";
            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type))
                {
                    result.Add(CreateModelFromType(type, Common.UnitType.WebApi, Common.ItemType.WebApiModel));
                    result.Add(CreateWebApiModel(type, Common.UnitType.WebApi, Common.ItemType.WebApiModel));
                }
            }
            return result;
        }
        protected virtual IGeneratedItem CreateWebApiModel(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateModelSubPathFromType(type, "Inheritance", StaticLiterals.CSharpFileExtension),
            };
            result.Source.Add($"partial class {CreateModelNameFromType(type)} : {GetBaseClassByType(type, ModelsFolder)}");
            result.Source.Add("{");
            result.Source.Add("}");
            result.EnvelopeWithANamespace(CreateModelTypeNamespace(type));
            result.FormatCSharpCode();
            return result;
        }

        public virtual IEnumerable<IGeneratedItem> CreateAspMvcModels()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            Namespace = $"{SolutionProperties.SolutionName}{StaticLiterals.AspMvsExtension}";
            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type))
                {
                    result.Add(CreateModelFromType(type, Common.UnitType.AspMvc, Common.ItemType.AspMvcModel));
                    result.Add(CreateAspMvcModel(type, Common.UnitType.AspMvc, Common.ItemType.AspMvcModel));
                }
            }
            return result;
        }
        protected virtual IGeneratedItem CreateAspMvcModel(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateModelSubPathFromType(type, "Inheritance", StaticLiterals.CSharpFileExtension),
            };
            result.Source.Add($"partial class {CreateModelNameFromType(type)} : {GetBaseClassByType(type, ModelsFolder)}");
            result.Source.Add("{");
            result.Source.Add("}");
            result.EnvelopeWithANamespace(CreateModelTypeNamespace(type));
            result.FormatCSharpCode();
            return result;
        }

        protected virtual IGeneratedItem CreateModelFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var modelName = CreateModelNameFromType(type);
            var typeProperties = type.GetAllPropertyInfos();
            var generateProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)) == false) ?? Array.Empty<PropertyInfo>();
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateModelSubPathFromType(type, string.Empty, StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            CreateModelAttributes(type, result.Source);
            result.Add($"public partial class {modelName}");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(modelName));
            result.AddRange(CreatePartialConstrutor("public", modelName));

            foreach (var propertyInfo in generateProperties)
            {
                CreateModelPropertyAttributes(propertyInfo, result.Source);
                result.AddRange(CreateProperty(propertyInfo));
            }
            if (unitType == Common.UnitType.Logic)
            {
                result.AddRange(CreateCopyProperties(type));
                result.AddRange(CreateCopyProperties(type, CreateModelType(type), p => true));
            }
            else if (unitType == Common.UnitType.WebApi)
            {
                result.AddRange(CreateCopyProperties(type, CreateModelType(type), p => true));
            }
            else if (unitType == Common.UnitType.AspMvc)
            {
                result.AddRange(CreateCopyProperties(type, CreateModelType(type), p => true));
            }
            result.AddRange(CreateFactoryMethods(CreateModelType(type), false));
            result.Add("}");
            result.EnvelopeWithANamespace(CreateModelTypeNamespace(type), "using System;");
            result.FormatCSharpCode();
            return result;
        }
        protected virtual IGeneratedItem CreateEditModelFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var modelName = CreateEditModelNameFromType(type);
            var typeProperties = type.GetAllPropertyInfos();
            var filteredProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)));
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateSubFilePathFromType(type, "Models", "", StaticLiterals.CSharpFileExtension),
            };

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
            result.EnvelopeWithANamespace(CreateModelTypeNamespace(type), "using System;");
            result.FormatCSharpCode();
            return result;
        }

        protected string GetBaseClassByType(Type type, string subNamespace)
        {
            var result = string.Empty;

            while (type.BaseType != null 
                   && StaticLiterals.EntityBaseClasses.Any(e => e.Equals(type.BaseType.Name)) == false)
            {
                type = type.BaseType;
            }

            if (type.BaseType != null)
            {
                var idx = StaticLiterals.EntityBaseClasses.IndexOf(e => e.Equals(type.BaseType.Name));

                if (idx > -1 && idx < StaticLiterals.ModelBaseClasses.Length)
                {
                    var ns = Namespace;

                    if (string.IsNullOrEmpty(subNamespace) == false)
                        ns = $"{ns}.{subNamespace}";

                    result = $"{ns}.{StaticLiterals.ModelBaseClasses[idx]}";
                }
            }
            return result;
        }
        protected string CreateModelFullNameFromType(Type type)
        {
            return $"{CreateModelTypeNamespace(type)}.{type.Name}";
        }
    }
}
//MdEnd