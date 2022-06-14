using TemplateCodeGenerator.ConApp.Contracts;

namespace TemplateCodeGenerator.ConApp.Generation
{
    internal partial class LogicGenerator : ModelGenerator
    {
        public LogicGenerator(ISolutionProperties solutionProperties) : base(solutionProperties)
        {
        }

        protected override string Extension => StaticLiterals.LogicExtension;
        protected override string Namespace => $"{SolutionProperties.SolutionName}{Extension}";

        public string CreateEntitySubType(Type type)
        {
            return type.FullName!.Replace($"{Namespace}.", string.Empty);
        }

        public static string CreateContractName(Type type)
        {
            return $"I{type.Name.CreatePluralWord()}Access";
        }
        public string CreateContractType(Type type)
        {
            return $"{CreateContractNamespace(type)}.{CreateContractName(type)}";
        }
        public static string CreateContractSubType(Type type)
        {
            return $"{CreateContractSubNamespace(type)}.{CreateContractName(type)}";
        }
        public string CreateContractNamespace(Type type)
        {
            return $"{Namespace}.{CreateContractSubNamespace(type)}";
        }
        public static string CreateContractSubNamespace(Type type)
        {
            return $"{StaticLiterals.ContractsFolder}.{CreateSubNamespaceFromEntityType(type)}";
        }
        public static string CreateContractSubPathFromType(Type type, string postFix, string fileExtension)
        {
            return Path.Combine(CreateContractSubNamespace(type).Replace(".", "\\"), $"{CreateContractName(type)}{postFix}{fileExtension}");
        }

        public static string CreateControllerName(Type type)
        {
            return $"{type.Name.CreatePluralWord()}Controller";
        }
        public string CreateControllerType(Type type)
        {
            return $"{CreateControllerNamespace(type)}.{CreateControllerName(type)}";
        }
        public static string CreateControllerSubType(Type type)
        {
            return $"{CreateControllerSubNamespace(type)}.{CreateControllerName(type)}";
        }
        public string CreateControllerNamespace(Type type)
        {
            return $"{Namespace}.{CreateControllerSubNamespace(type)}";
        }
        public static string CreateControllerSubNamespace(Type type)
        {
            return $"{StaticLiterals.ControllersFolder}.{CreateSubNamespaceFromEntityType(type)}";
        }
        public static string CreateControllersSubPathFromType(Type type, string postFix, string fileExtension)
        {
            return Path.Combine(CreateControllerSubNamespace(type).Replace(".", "\\"), $"{CreateControllerName(type)}{postFix}{fileExtension}");
        }

        public static string CreateFacadeName(Type type)
        {
            return $"{type.Name.CreatePluralWord()}Facade";
        }
        public string CreateFacadeType(Type type)
        {
            return $"{CreateFacadeNamespace(type)}.{CreateFacadeName(type)}";
        }
        public static string CreateFacadeSubType(Type type)
        {
            return $"{CreateFacadeSubNamespace(type)}.{CreateFacadeName(type)}";
        }
        public string CreateFacadeNamespace(Type type)
        {
            return $"{Namespace}.{CreateFacadeSubNamespace(type)}";
        }
        public static string CreateFacadeSubNamespace(Type type)
        {
            return $"{StaticLiterals.FacadesFolder}.{CreateSubNamespaceFromEntityType(type)}";
        }
        public static string CreateFacadesSubPathFromType(Type type, string postFix, string fileExtension)
        {
            return Path.Combine(CreateFacadeSubNamespace(type).Replace(".", "\\"), $"{CreateFacadeName(type)}{postFix}{fileExtension}");
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
            var dataContextNamespace = $"{Namespace}.DataContext";
            var result = new Models.GeneratedItem(Common.UnitType.Logic, Common.ItemType.DbContext)
            {
                FullName = $"{dataContextNamespace}.ProjectDbContext",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = $"DataContext\\ProjectDbContextGenerationst{StaticLiterals.CSharpFileExtension}",
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
                    result.Add(CreateLogicModel(type, Common.UnitType.Logic, Common.ItemType.Model));
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
        protected virtual IEnumerable<IGeneratedItem> CreateContracts()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type))
                {
                    result.Add(CreateContractFromType(type, Common.UnitType.Logic, Common.ItemType.Model));
                }
            }
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
                    result.Add(CreateControllerFromType(type, Common.UnitType.Logic, Common.ItemType.Controller));
                }
            }
            return result;
        }
        protected virtual IEnumerable<IGeneratedItem> CreateFacades()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type))
                {
                    result.Add(CreateFacadeFromType(type, Common.UnitType.Logic, Common.ItemType.Facade));
                }
            }
            return result;
        }

        protected virtual IGeneratedItem CreateContractFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var contractName = CreateContractName(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateContractType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateContractSubPathFromType(type, string.Empty, StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            result.Add($"public partial interface {contractName}<T> : Contracts.IDataAccess<T>");
            result.Add("{");
            result.Add("}");
            result.EnvelopeWithANamespace(CreateContractNamespace(type));
            result.FormatCSharpCode();
            return result;
        }
        protected virtual IGeneratedItem CreateControllerFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var visibility = type.IsPublic ? "public" : "internal";
            var entityType = CreateEntitySubType(type);
            var genericType = $"Controllers.GenericController";
            var controllerName = CreateControllerName(type);
            var contractSubType = CreateContractSubType(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = $"{CreateControllerType(type)}",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateControllersSubPathFromType(type, string.Empty, StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            CreateControllerAttributes(type, result.Source);
            result.Add($"{visibility} sealed partial class {controllerName} : {genericType}<{entityType}>, {contractSubType}<{entityType}>");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(controllerName));
            result.AddRange(CreatePartialConstrutor("public", controllerName));
            result.AddRange(CreatePartialConstrutor("public", controllerName, "ControllerObject other", "base(other)", null, false));
            result.Add("}");
            result.EnvelopeWithANamespace(CreateControllerNamespace(type));
            result.FormatCSharpCode();
            return result;
        }
        protected virtual IGeneratedItem CreateFacadeFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var modelType = $"{CreateModelType(type)}";
            var entityType = CreateEntitySubType(type);
            var genericType = $"Facades.GenericFacade";
            var facadeName = CreateFacadeName(type);
            var contractSubType = CreateContractSubType(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateFacadeType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateFacadesSubPathFromType(type, string.Empty, StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            CreateControllerAttributes(type, result.Source);
            result.Add($"internal partial class {facadeName} : {genericType}<{modelType}, {entityType}>, {contractSubType}<{modelType}>");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(facadeName));
            result.Add($"new protected {contractSubType}<{type.FullName}> Controller => (base.Controller as {contractSubType}<{type.FullName}>)!;");
            result.AddRange(CreatePartialConstrutor("public", facadeName, null, $"base(new {CreateControllerSubType(type)}())"));
            result.AddRange(CreatePartialConstrutor("public", facadeName, "Facades.FacadeObject other", $"base(new {CreateControllerSubType(type)}(other.ControllerObject))", null, false));
            result.Add("}");
            result.EnvelopeWithANamespace(CreateFacadeNamespace(type));
            result.FormatCSharpCode();
            return result;
        }

        protected virtual IGeneratedItem CreateFactory()
        {
            var entityProject = EntityProject.Create(SolutionProperties);
            var factoryNamespace = $"{Namespace}";
            var result = new Models.GeneratedItem(Common.UnitType.Logic, Common.ItemType.DbContext)
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
                var modelType = $"{CreateModelType(type)}";
                var contractSubType = CreateContractSubType(type);
                var facadeName = CreateFacadeName(type);
                var facadeType = CreateFacadeType(type);

                result.AddRange(CreateComment(type));
                result.Add($"public static {contractSubType}<{modelType}> Create{facadeName}() => new {facadeType}();");

                result.AddRange(CreateComment(type));
                result.Add($"public static {contractSubType}<{modelType}> Create{facadeName}(Object otherFacade) => new {facadeType}((otherFacade as Facades.FacadeObject)!);");
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
