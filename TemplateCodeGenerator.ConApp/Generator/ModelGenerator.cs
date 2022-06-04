//MdStart
using System.Reflection;
using TemplateCodeGenerator.ConApp.Generator;

namespace TemplateCodeGenerator.ConApp.Generation
{
    internal partial class ModelGenerator : ClassGenerator, Contracts.IModelGenerator
    {
        #region Models
        internal static string ModelObject => nameof(ModelObject);
        internal static string ModuleModel => nameof(ModuleModel);
        internal static string IdentityModel => nameof(IdentityModel);
        internal static string VersionModel => nameof(VersionModel);
        #endregion Models

        protected ModelGenerator(SolutionProperties solutionProperties)
            : base(solutionProperties)
        {
        }

        public string AppModelsNameSpace { get; }
        public string ModelsFolder { get; }

        public string CreateModelsNamespace(Type type)
        {
            return $"{AppModelsNameSpace}.{GeneratorObject.CreateSubNamespaceFromType(type)}";
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

        public virtual IEnumerable<Contracts.IGeneratedItem> GenerateAll()
        {
            var result = new List<Contracts.IGeneratedItem>();

            result.AddRange(CreateLogicModels());
            //result.AddRange(CreateWebApiModels());
            //result.AddRange(CreateAspMvcModels());
            return result;
        }

        public virtual IEnumerable<Contracts.IGeneratedItem> CreateLogicModels()
        {
            var result = new List<Contracts.IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type))
                {
                    result.Add(CreateModelFromType(type, Common.UnitType.Logic, Common.ItemType.AspMvcModel));
                    result.Add(CreateLogicModel(type, Common.UnitType.Logic));
                }
            }
            return result;
        }
        protected virtual Contracts.IGeneratedItem CreateLogicModel(Type type, Common.UnitType unitType)
        {
            var result = new Models.GeneratedItem(unitType, Common.ItemType.LogicModel)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateSubFilePathFromInterface(type, ModelsFolder, "Inheritance", StaticLiterals.CSharpFileExtension),
            };
            result.Source.Add($"partial class {CreateModelNameFromType(type)} : {GetBaseClassByType(type)}");
            result.Source.Add("{");
            result.Source.Add("}");
            result.EnvelopeWithANamespace(CreateModelsNamespace(type));
            result.FormatCSharpCode();
            return result;
        }

        public virtual IEnumerable<Contracts.IGeneratedItem> CreateWebApiModels()
        {
            var result = new List<Contracts.IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type))
                {
                    result.Add(CreateModelFromType(type, Common.UnitType.WebApi, Common.ItemType.WebApiModel));
                    result.Add(CreateWebApiModel(type, Common.UnitType.WebApi));
                }
            }
            return result;
        }
        protected virtual Contracts.IGeneratedItem CreateWebApiModel(Type type, Common.UnitType unitType)
        {
            var result = new Models.GeneratedItem(unitType, Common.ItemType.WebApiModel)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateSubFilePathFromInterface(type, ModelsFolder, "Inheritance", StaticLiterals.CSharpFileExtension),
            };
            result.Source.Add($"partial class {CreateModelNameFromType(type)} : {GetBaseClassByType(type)}");
            result.Source.Add("{");
            result.Source.Add("}");
            result.EnvelopeWithANamespace(CreateModelsNamespace(type));
            result.FormatCSharpCode();
            return result;
        }

        public virtual IEnumerable<Contracts.IGeneratedItem> CreateAspMvcModels()
        {
            var result = new List<Contracts.IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type))
                {
                    result.Add(CreateModelFromType(type, Common.UnitType.AspMvc, Common.ItemType.AspMvcModel));
                    //result.Add(CreateAspMvcModel(type, Common.UnitType.AspMvc));
                }
            }
            return result;
        }
        //protected virtual Contracts.IGeneratedItem CreateAspMvcModel(Type type, Common.UnitType unitType)
        //{
        //    var result = new Models.GeneratedItem(unitType, Common.ItemType.PersistenceModel)
        //    {
        //        FullName = CreateModelFullNameFromType(type),
        //        FileExtension = StaticLiterals.CSharpFileExtension,
        //        SubFilePath = CreateSubFilePathFromInterface(type, ModelsFolder, "Inheritance", StaticLiterals.CSharpFileExtension),
        //    };
        //    result.Source.Add($"partial class {CreateModelNameFromType(type)} : {GetBaseClassByContract(type)}");
        //    result.Source.Add("{");
        //    result.Source.Add("}");
        //    result.EnvelopeWithANamespace(CreateModelsNamespace(type));
        //    result.FormatCSharpCode();
        //    return result;
        //}

        protected virtual Contracts.IGeneratedItem CreateModelFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var modelName = CreateModelNameFromType(type);
            var typeProperties = type.GetAllPropertyInfos();
            var generateProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name))) ?? Array.Empty<PropertyInfo>();
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateSubFilePathFromInterface(type, "Models", "", StaticLiterals.CSharpFileExtension),
            };
            CreateModelAttributes(type, result.Source);
            result.Add($"public partial class {modelName} : {type.FullName}");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(modelName));
            result.AddRange(CreatePartialConstrutor("public", modelName));

            foreach (var propertyInfo in generateProperties)
            {
                CreateModelPropertyAttributes(propertyInfo, result.Source);
                result.AddRange(CreateProperty(propertyInfo));
            }
            result.AddRange(CreateCopyProperties(type));
            result.AddRange(CreateFactoryMethods(type, false));
            result.Add("}");
            result.EnvelopeWithANamespace(CreateModelsNamespace(type), "using System;");
            result.FormatCSharpCode();
            return result;
        }
        protected virtual Contracts.IGeneratedItem CreateEditModelFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var modelName = CreateEditModelNameFromType(type);
            var typeProperties = type.GetAllPropertyInfos();
            var filteredProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)));
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateSubFilePathFromInterface(type, "Models", "", StaticLiterals.CSharpFileExtension),
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
            result.EnvelopeWithANamespace(CreateModelsNamespace(type), "using System;");
            result.FormatCSharpCode();
            return result;
        }

        protected string GetBaseClassByType(Type type)
        {
            var result = string.Empty;

            while (type.BaseType != null)
            {
                result = type.BaseType.Name;
                type = type.BaseType;
            }
            return result;
        }
        protected string CreateModelFullNameFromType(Type type)
        {
            var modelName = CreateModelNameFromType(type);
            var subNamespace = $"{(ModelsFolder.HasContent() ? $"{ModelsFolder}." : string.Empty)}";
            var result = type?.FullName?.Replace(type.Name, modelName);

            return result?.Replace(".Contracts.", subNamespace) ?? string.Empty;
        }
    }
}
//MdEnd