namespace TemplateCodeGenerator.ConApp.Generation
{
    internal partial class ItemProperties
    {
        public string SolutionName { get; }
        public string ProjectExtension { get; }
        public string Namespace => $"{SolutionName}{ProjectExtension}";
        public ItemProperties(string solutionName, string projectExtension)
        {
            SolutionName = solutionName;
            ProjectExtension = projectExtension;
        }

        public static string CreateEntityName(Type type) => type.Name;
        public string CreateEntitySubType(Type type)
        {
            return type.FullName!.Replace($"{Namespace}.", string.Empty);
        }
        public static string CreateModelName(Type type) => type.Name;
        public string CreateModelType(Type type)
        {
            return $"{CreateModelNamespace(type)}.{type.Name}";
        }
        public static string CreateEditModelName(Type type)
        {
            return $"{CreateModelName(type)}Edit";
        }
        public string CreateEditModelType(Type type)
        {
            return $"{CreateModelNamespace(type)}.{CreateEditModelName(type)}";
        }

        public static string CreateModelSubType(Type type)
        {
            return $"{CreateModelSubNamespace(type)}.{type.Name}";
        }
        public string CreateModelNamespace(Type type)
        {
            return $"{Namespace}.{CreateModelSubNamespace(type)}";
        }
        public static string CreateModelSubPath(Type type, string postFix, string fileExtension)
        {
            return Path.Combine(CreateModelSubNamespace(type).Replace(".", "\\"), $"{type.Name}{postFix}{fileExtension}");
        }

        public string ConvertEntityToModelType(string typeFullname)
        {
            var result = typeFullname;
            var entitiesFolder = $".{StaticLiterals.EntitiesFolder}.";

            if (result.Contains(entitiesFolder))
            {
                var modelsFolder = $".{StaticLiterals.ModelsFolder}.";

                result = result.Replace(entitiesFolder, modelsFolder);
                result = result.Replace(StaticLiterals.LogicExtension, ProjectExtension);
            }
            return result;
        }

        #region Contracts properties
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
            return $"{SolutionName}{StaticLiterals.LogicExtension}.{CreateContractSubNamespace(type)}";
        }
        public static string CreateContractSubNamespace(Type type)
        {
            var subNamespace = CreateSubNamespaceFromEntityType(type);

            return subNamespace.HasContent() ? $"{StaticLiterals.ContractsFolder}.{subNamespace}" : StaticLiterals.ContractsFolder;
        }
        public static string CreateContractSubPathFromType(Type type, string postFix, string fileExtension)
        {
            return Path.Combine(CreateContractSubNamespace(type).Replace(".", "\\"), $"{CreateContractName(type)}{postFix}{fileExtension}");
        }
        #endregion Contracts properties

        #region Controller properties
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
            var subNamespace = CreateSubNamespaceFromEntityType(type);

            return subNamespace.HasContent() ? $"{StaticLiterals.ControllersFolder}.{subNamespace}" : StaticLiterals.ControllersFolder;
        }
        public static string CreateControllersSubPathFromType(Type type, string postFix, string fileExtension)
        {
            return Path.Combine(CreateControllerSubNamespace(type).Replace(".", "\\"), $"{CreateControllerName(type)}{postFix}{fileExtension}");
        }
        #endregion Controller properties

        #region Facade properties
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
            var subNamespace = CreateSubNamespaceFromEntityType(type);

            return subNamespace.HasContent() ? $"{StaticLiterals.FacadesFolder}.{subNamespace}" : StaticLiterals.FacadesFolder;
        }
        public static string CreateFacadesSubPathFromType(Type type, string postFix, string fileExtension)
        {
            return Path.Combine(CreateFacadeSubNamespace(type).Replace(".", "\\"), $"{CreateFacadeName(type)}{postFix}{fileExtension}");
        }
        #endregion Facade properties

        public static bool IsEntityType(Type type)
        {
            return type.FullName!.Contains($".{StaticLiterals.EntitiesFolder}.");
        }
        public static bool IsModelType(Type type)
        {
            return type.FullName!.Contains($".{StaticLiterals.ModelsFolder}.");
        }
        public static bool IsModelType(string strType)
        {
            return strType.Contains($".{StaticLiterals.ModelsFolder}.");
        }

        public static string CreateModelSubNamespace(Type type)
        {
            var subNamespace = CreateSubNamespaceFromEntityType(type);

            return subNamespace.HasContent() ? $"{StaticLiterals.ModelsFolder}.{subNamespace}" : StaticLiterals.ModelsFolder;
        }

        /// <summary>
        /// Diese Methode ermittelt den Teilnamensraum aus einem Typ.
        /// </summary>
        /// <param name="type">Typ</param>
        /// <returns>Teil-Namensraum</returns>
        public static string CreateSubNamespaceFromType(Type type)
        {
            var result = string.Empty;
            var data = type.Namespace?.Split('.');

            for (var i = 2; i < data?.Length; i++)
            {
                if (string.IsNullOrEmpty(result))
                {
                    result = $"{data[i]}";
                }
                else
                {
                    result = $"{result}.{data[i]}";
                }
            }
            return result;
        }
        /// <summary>
        /// Diese Methode ermittelt den Teilnamensraum aus einem Typ.
        /// </summary>
        /// <param name="type">Typ</param>
        /// <returns>Teil-Namensraum</returns>
        public static string CreateSubNamespaceFromEntityType(Type type)
        {
            var result = CreateSubNamespaceFromType(type);

            if (result.Equals(StaticLiterals.EntitiesFolder))
            {
                result = string.Empty;
            }
            return result.Replace($"{StaticLiterals.EntitiesFolder}.", string.Empty);
        }
    }
}
