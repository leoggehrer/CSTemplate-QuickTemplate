using TemplateCodeGenerator.ConApp.Contracts;
using SL = TemplateCodeGenerator.ConApp.StaticLiterals;

namespace TemplateCodeGenerator.ConApp.Generation
{
    internal partial class LogicGenerator : ModelGenerator
    {
        private ItemProperties? _itemProperties;
        protected override ItemProperties ItemProperties => _itemProperties ??= new ItemProperties(SolutionProperties.SolutionName, StaticLiterals.LogicExtension);

        public LogicGenerator(ISolutionProperties solutionProperties) : base(solutionProperties)
        {
        }

        public virtual IEnumerable<IGeneratedItem> GenerateAll()
        {
            var result = new List<IGeneratedItem>
            {
                CreateDbContext()
            };
            result.AddRange(CreateModels());
            result.AddRange(CreateContracts());
            result.AddRange(CreateControllers());
            result.AddRange(CreateFacades());
            result.Add(CreateFactory());
            return result;
        }

        protected virtual IGeneratedItem CreateDbContext()
        {
            var entityProject = EntityProject.Create(SolutionProperties);
            var dataContextNamespace = $"{ItemProperties.Namespace}.DataContext";
            var result = new Models.GeneratedItem(Common.UnitType.Logic, Common.ItemType.DbContext)
            {
                FullName = $"{dataContextNamespace}.ProjectDbContext",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = $"DataContext\\ProjectDbContextGeneration{StaticLiterals.CSharpFileExtension}",
            };
            result.AddRange(CreateComment());
            result.Add($"partial class ProjectDbContext");
            result.Add("{");

            foreach (var type in entityProject.EntityTypes)
            {
                result.AddRange(CreateComment(type));
                result.Add($"public DbSet<{type.FullName}>? {type.Name}Set" + "{ get; set; }");
            }
            result.Add(string.Empty);

            result.AddRange(CreateComment());
            result.Add("partial void GetGeneratorDbSet<E>(ref DbSet<E>? dbSet, ref bool handled) where E : Entities.IdentityEntity");
            result.Add("{");

            foreach (var type in entityProject.EntityTypes)
            {
                result.Add($"if (typeof(E) == typeof({type.FullName}))");
                result.Add("{");
                result.Add($"dbSet = {type.Name}Set as DbSet<E>;");
                result.Add("handled = true;");
                result.Add("}");
            }
            result.Add("}");

            result.Add("}");
            result.EnvelopeWithANamespace(dataContextNamespace);
            result.FormatCSharpCode();
            return result;
        }

        protected virtual IEnumerable<IGeneratedItem> CreateModels()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type))
                {
                    result.Add(CreateDelegateModelFromType(type, Common.UnitType.Logic, Common.ItemType.Model));
                    result.Add(CreateModelInheritance(type, Common.UnitType.Logic, Common.ItemType.Model));
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

        protected virtual IEnumerable<IGeneratedItem> CreateContracts()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type) && QueryLogicSetting<bool>(Common.ItemType.AccessContract, type, StaticLiterals.Generate, "True"))
                {
                    result.Add(CreateAccessContract(type, Common.UnitType.Logic, Common.ItemType.AccessContract));
                    //result.Add(CreateFacadeContract(type, Common.UnitType.Logic, Common.ItemType.FacadeContract));
                }
            }
            return result;
        }
        protected virtual IGeneratedItem CreateAccessContract(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var contractName = ItemProperties.CreateAccessContractName(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateContractType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateAccessContractSubPathFromType(type, string.Empty, StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            result.Add($"public partial interface {contractName}<T> : Contracts.IDataAccess<T>");
            result.Add("{");
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateContractNamespace(type));
            result.FormatCSharpCode();
            return result;
        }
        protected virtual IGeneratedItem CreateFacadeContract(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var contractName = ItemProperties.CreateFacadeContractName(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateContractType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateFacadeContractSubPathFromType(type, string.Empty, StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            result.Add($"public partial interface {contractName}<T> : Contracts.IDataAccess<T>");
            result.Add("{");
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateContractNamespace(type));
            result.FormatCSharpCode();
            return result;
        }

        private T QueryLogicSetting<T>(Common.ItemType itemType, Type type, string valueName, string defaultValue)
        {
            T result;

            try
            {
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(Common.UnitType.Logic, itemType, CreateEntitiesSubTypeFromType(type), valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {System.Reflection.MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
        }

        protected virtual IEnumerable<IGeneratedItem> CreateControllers()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type) && QueryLogicSetting<bool>(Common.ItemType.Controller, type, StaticLiterals.Generate, "True"))
                {
                    result.Add(CreateControllerFromType(type, Common.UnitType.Logic, Common.ItemType.Controller));
                }
            }
            return result;
        }
        protected virtual IGeneratedItem CreateControllerFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var visibility = QueryLogicSetting<string>(itemType, type, StaticLiterals.Visibility, type.IsPublic ? "public" : "internal");
            var attributes = QueryLogicSetting<string>(itemType, type, StaticLiterals.Attributes, string.Empty);
            var entityType = ItemProperties.CreateEntitySubType(type);
            var genericType = $"Controllers.GenericController";
            var controllerName = ItemProperties.CreateControllerName(type);
            var contractSubType = ItemProperties.CreateAccessContractSubType(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = $"{ItemProperties.CreateControllerType(type)}",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateControllersSubPathFromType(type, string.Empty, StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            CreateControllerAttributes(type, result.Source);
            result.Add($"{(attributes.HasContent() ? $"[{attributes}]" : string.Empty)}");
            result.Add($"{visibility} sealed partial class {controllerName} : {genericType}<{entityType}>, {contractSubType}<{entityType}>");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(controllerName));
            result.AddRange(CreatePartialConstrutor("public", controllerName));
            result.AddRange(CreatePartialConstrutor("public", controllerName, "ControllerObject other", "base(other)", null, false));
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateControllerNamespace(type));
            result.FormatCSharpCode();
            return result;
        }

        protected virtual IEnumerable<IGeneratedItem> CreateFacades()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type) && QueryLogicSetting<bool>(Common.ItemType.Facade, type, StaticLiterals.Generate, "True"))
                {
                    result.Add(CreateFacadeFromType(type, Common.UnitType.Logic, Common.ItemType.Facade));
                }
            }
            return result;
        }
        protected virtual IGeneratedItem CreateFacadeFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var modelType = $"{ItemProperties.CreateModelType(type)}";
            var entityType = ItemProperties.CreateEntitySubType(type);
            var genericType = $"Facades.GenericFacade";
            var facadeName = ItemProperties.CreateFacadeName(type);
            var contractSubType = ItemProperties.CreateFacadeContractSubType(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateFacadeType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateFacadesSubPathFromType(type, string.Empty, StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            CreateControllerAttributes(type, result.Source);
            result.Add($"internal partial class {facadeName} : {genericType}<{modelType}, {entityType}>, {contractSubType}<{modelType}>");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(facadeName));
            result.Add($"new protected {contractSubType}<{type.FullName}> Controller => (base.Controller as {contractSubType}<{type.FullName}>)!;");
            result.AddRange(CreatePartialConstrutor("public", facadeName, null, $"base(new {ItemProperties.CreateControllerSubType(type)}())"));
            result.AddRange(CreatePartialConstrutor("public", facadeName, "Facades.FacadeObject other", $"base(new {ItemProperties.CreateControllerSubType(type)}(other.ControllerObject))", null, false));
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateFacadeNamespace(type));
            result.FormatCSharpCode();
            return result;
        }

        protected virtual IGeneratedItem CreateFactory()
        {
            var entityProject = EntityProject.Create(SolutionProperties);
            var factoryNamespace = $"{ItemProperties.Namespace}";
            var result = new Models.GeneratedItem(Common.UnitType.Logic, Common.ItemType.Factory)
            {
                FullName = $"{factoryNamespace}.Factory",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = $"Factory{StaticLiterals.CSharpFileExtension}",
            };
            result.AddRange(CreateComment());
            result.Add($"public static partial class Factory");
            result.Add("{");

            foreach (var type in entityProject.EntityTypes)
            {

                if (CanCreate(type) && QueryLogicSetting<bool>(Common.ItemType.Factory, type, StaticLiterals.Generate, "True"))
                {
                    var modelType = $"{ItemProperties.CreateModelType(type)}";
                    var contractSubType = ItemProperties.CreateAccessContractSubType(type);
                    var facadeName = ItemProperties.CreateFacadeName(type);
                    var facadeType = ItemProperties.CreateFacadeType(type);

                    result.AddRange(CreateComment(type));
                    result.Add($"public static {contractSubType}<{modelType}> Create{facadeName}() => new {facadeType}();");

                    result.AddRange(CreateComment(type));
                    result.Add($"public static {contractSubType}<{modelType}> Create{facadeName}(Object otherFacade) => new {facadeType}((otherFacade as Facades.FacadeObject)!);");
                }
            }

            result.Add("}");
            result.EnvelopeWithANamespace(factoryNamespace);
            result.FormatCSharpCode();
            return result;
        }

        #region Partial methods
        partial void CreateControllerAttributes(Type type, List<string> codeLines);
        #endregion Partial methods
    }
}
