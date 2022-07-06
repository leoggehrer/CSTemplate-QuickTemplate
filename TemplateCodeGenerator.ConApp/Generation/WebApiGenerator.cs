using TemplateCodeGenerator.ConApp.Contracts;

namespace TemplateCodeGenerator.ConApp.Generation
{
    internal partial class WebApiGenerator : ModelGenerator
    {
        private ItemProperties? _itemProperties;
        protected override ItemProperties ItemProperties => _itemProperties ??= new ItemProperties(SolutionProperties.SolutionName, StaticLiterals.WebApiExtension);
        public WebApiGenerator(ISolutionProperties solutionProperties) : base(solutionProperties)
        {
        }

        public virtual IEnumerable<IGeneratedItem> GenerateAll()
        {
            var result = new List<IGeneratedItem>();

            result.AddRange(CreateModels());
            result.AddRange(CreateControllers());
            result.Add(CreateAddServices());
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
                    result.Add(CreateModelInheritance(type, Common.UnitType.WebApi, Common.ItemType.Model));
                    result.Add(CreateEditModelFromType(type, Common.UnitType.WebApi, Common.ItemType.EditModel));
                }
            }
            return result;
        }
        protected virtual IGeneratedItem CreateModelInheritance(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateModelSubPath(type, "Inheritance", StaticLiterals.CSharpFileExtension),
            };
            result.Source.Add($"partial class {CreateModelName(type)} : {GetBaseClassByType(type, StaticLiterals.ModelsFolder)}");
            result.Source.Add("{");
            result.Source.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateModelNamespace(type));
            result.FormatCSharpCode();
            return result;
        }

        protected virtual IGeneratedItem CreateEditModelFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var modelName = ItemProperties.CreateEditModelName(type);
            var typeProperties = type.GetAllPropertyInfos();
            var filteredProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)) == false
                                                            && IsListType(e.PropertyType) == false);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateModelSubPath(type, "Edit", StaticLiterals.CSharpFileExtension),
            };

            result.AddRange(CreateComment(type));
            CreateModelAttributes(type, result.Source);
            result.Add($"public partial class {modelName}");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(modelName));
            result.AddRange(CreatePartialConstrutor("public", modelName));

            foreach (var propertyInfo in filteredProperties.Where(pi => pi.CanWrite))
            {
                result.AddRange(CreateComment(propertyInfo));
                CreateModelPropertyAttributes(propertyInfo, result.Source);
                result.AddRange(CreateProperty(type, propertyInfo));
            }
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateModelNamespace(type), "using System;");
            result.FormatCSharpCode();
            return result;
        }

        protected virtual IEnumerable<IGeneratedItem> CreateControllers()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type))
                {
                    result.Add(CreateControllerFromType(type, Common.UnitType.WebApi, Common.ItemType.Controller));
                }
            }
            return result;
        }
        protected virtual IGeneratedItem CreateControllerFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var visibility = "public";
            var accessType = ItemProperties.CreateEntitySubType(type);
            var genericType = $"Controllers.GenericController";
            var modelType = ItemProperties.CreateModelType(type);
            var editModelType = ItemProperties.CreateEditModelType(type);
            var controllerName = ItemProperties.CreateControllerName(type);
            var contractType = ItemProperties.CreateContractType(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = $"{ItemProperties.CreateControllerType(type)}",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateControllersSubPathFromType(type, string.Empty, StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            CreateControllerAttributes(type, result.Source);
            result.Add($"{visibility} sealed partial class {controllerName} : {genericType}<{accessType}, {editModelType}, {modelType}>");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(controllerName));
            result.AddRange(CreatePartialConstrutor("public", controllerName, $"{contractType}<{accessType}> other", "base(other)", null, true));

            result.AddRange(CreateComment(type));
            result.Add($"protected override {modelType} ToOutModel({accessType} accessModel)");
            result.Add("{");
            result.Add($"var handled = false;");
            result.Add($"var result = default({modelType});");
            result.Add("BeforeToOutModel(accessModel, ref result, ref handled);");
            result.Add("if (handled == false || result == null)");
            result.Add("{");
            result.Add($"result = {modelType}.Create(accessModel);");
            result.Add("}");
            result.Add("AfterToOutModel(result);");
            result.Add($"return result;");
            result.Add("}");

            result.Add($"partial void BeforeToOutModel({accessType} accessModel, ref {modelType}? outModel, ref bool handled);");
            result.Add($"partial void AfterToOutModel({modelType} outModel);");

            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateControllerNamespace(type));
            result.FormatCSharpCode();
            return result;
        }

        protected virtual IGeneratedItem CreateAddServices()
        {
            var entityProject = EntityProject.Create(SolutionProperties);
            var result = new Models.GeneratedItem(Common.UnitType.WebApi, Common.ItemType.AddServices)
            {
                FullName = $"{ItemProperties.Namespace}.Program",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = $"ProgramGeneration{StaticLiterals.CSharpFileExtension}",
            };
            result.AddRange(CreateComment());
            result.Add("partial class Program");
            result.Add("{");
            result.Add("static partial void AddServices(WebApplicationBuilder builder)");
            result.Add("{");
            foreach (var type in entityProject.EntityTypes)
            {
                var accessType = ItemProperties.CreateEntitySubType(type);
                var contractType = ItemProperties.CreateContractType(type);
                var controllerType = ItemProperties.CreateLogicControllerType(type);

                result.Add($"builder.Services.AddTransient<{contractType}<{accessType}>, {controllerType}>();");
            }
            result.Add("}");
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.Namespace);
            result.FormatCSharpCode();
            return result;
        }

        #region Partial methods
        partial void CreateModelAttributes(Type type, List<string> source);
        partial void CreateControllerAttributes(Type type, List<string> codeLines);
        #endregion Partial methods
    }
}
