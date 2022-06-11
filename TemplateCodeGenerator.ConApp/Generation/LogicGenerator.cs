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

        public string CreateContractNamespace(Type type)
        {
            var modelSubNamespace = $"{StaticLiterals.ContractsFolder}.{CreateSubNamespaceFromEntityType(type)}";

            return $"{Namespace}.{modelSubNamespace}";
        }
        public string CreateControllerNamespace(Type type)
        {
            var modelSubNamespace = $"{StaticLiterals.ControllersFolder}.{CreateSubNamespaceFromEntityType(type)}";

            return $"{Namespace}.{modelSubNamespace}";
        }

        public static string CreateContractSubPathFromType(Type type, string postFix, string fileExtension)
        {
            return Path.Combine(StaticLiterals.ContractsFolder, CreateSubNamespaceFromEntityType(type).Replace(".", "\\"), $"{type.Name}{postFix}{fileExtension}");
        }
        public static string CreateControllersSubPathFromType(Type type, string postFix, string fileExtension)
        {
            return Path.Combine(StaticLiterals.ControllersFolder, CreateSubNamespaceFromEntityType(type).Replace(".", "\\"), $"{type.Name}{postFix}{fileExtension}");
        }
        public static string CreateFacadesSubPathFromType(Type type, string postFix, string fileExtension)
        {
            return Path.Combine(StaticLiterals.FacadesFolder, CreateSubNamespaceFromEntityType(type).Replace(".", "\\"), $"{type.Name}{postFix}{fileExtension}");
        }

        public virtual IEnumerable<IGeneratedItem> GenerateAll()
        {
            var result = new List<IGeneratedItem>();

            result.AddRange(CreateModels());
            result.AddRange(CreateContracts());
            result.AddRange(CreateControllers());
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
                    result.Add(CreateModelFromType(type, Common.UnitType.Logic, Common.ItemType.Model));
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
                SubFilePath = CreateModelSubPathFromType(type, "Inheritance", StaticLiterals.CSharpFileExtension),
            };
            result.Source.Add($"partial class {CreateModelNameFromType(type)} : {GetBaseClassByType(type, ModelsFolder)}");
            result.Source.Add("{");
            result.Source.Add("}");
            result.EnvelopeWithANamespace(CreateModelTypeNamespace(type));
            result.FormatCSharpCode();
            return result;
        }
        public virtual IEnumerable<IGeneratedItem> CreateContracts()
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
        public virtual IEnumerable<IGeneratedItem> CreateControllers()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type))
                {
                    result.Add(CreateControllersFromType(type, Common.UnitType.Logic, Common.ItemType.Controller));
                }
            }
            return result;
        }

        protected virtual IGeneratedItem CreateContractFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var contractName = $"I{type.Name}Access";
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateContractSubPathFromType(type, "Access", StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            result.Add($"public partial interface {contractName}<T> : Contracts.IDataAccess<T>");
            result.Add("{");
            result.Add("}");
            result.EnvelopeWithANamespace(CreateContractNamespace(type), "using System;");
            result.FormatCSharpCode();
            return result;
        }
        partial void CreateControllerAttributes(Type type, List<string> codeLines);
        protected virtual IGeneratedItem CreateControllerFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var modelName = CreateModelNameFromType(type);
            var entityName = type.FullName;
            var controllerName = $"{modelName}Controller";
            var genericControllerName = $"Controllers.GenericController";
            var contractName = $"{StaticLiterals.ContractsFolder}.{CreateSubNamespaceFromEntityType(type)}.I{type.Name}Access";
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateControllersSubPathFromType(type, "Controller", StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            CreateControllerAttributes(type, result.Source);
            result.Add($"internal partial class {controllerName} : {genericControllerName}<{entityName}>, {contractName}<{entityName}>");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(controllerName));
            result.AddRange(CreatePartialConstrutor("public", controllerName));
            result.AddRange(CreatePartialConstrutor("public", controllerName, "ControllerObject other", "base(other)", null, false));
            result.Add("}");
            result.EnvelopeWithANamespace(CreateControllerNamespace(type), "using System;");
            result.FormatCSharpCode();
            return result;
        }
        protected virtual IGeneratedItem CreateContrFacadeFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var modelName = CreateModelNameFromType(type);
            var entityName = type.FullName;
            var facadeName = $"{modelName}Facade";
            var genericFacadeName = $"Facades.GenericFacade";
            var contractName = $"{StaticLiterals.ContractsFolder}.{CreateSubNamespaceFromEntityType(type)}.I{type.Name}Access";
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateFacadesSubPathFromType(type, "Facade", StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            CreateControllerAttributes(type, result.Source);
            result.Add($"internal partial class {facadeName} : {genericFacadeName}<{entityName}, {modelName}>, {contractName}<{modelName}>");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(facadeName));
            result.AddRange(CreatePartialConstrutor("public", facadeName));
            result.AddRange(CreatePartialConstrutor("public", facadeName, "ControllerObject other", "base(other)", null, false));
            result.Add("}");
            result.EnvelopeWithANamespace(CreateControllerNamespace(type), "using System;");
            result.FormatCSharpCode();
            return result;
        }
    }
}
