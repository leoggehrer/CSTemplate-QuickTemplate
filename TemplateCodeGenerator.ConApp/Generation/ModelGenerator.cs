//MdStart
using System.Reflection;
using TemplateCodeGenerator.ConApp.Contracts;

namespace TemplateCodeGenerator.ConApp.Generation
{
    internal abstract partial class ModelGenerator : ClassGenerator
    {
        public ModelGenerator(ISolutionProperties solutionProperties)
            : base(solutionProperties)
        {
        }

        public string ModelsFolder { get; } = StaticLiterals.ModelsFolder;
        protected abstract string Extension { get; }
        protected abstract string Namespace { get; }
        public string CreateModelType(Type type)
        {
            return $"{CreateModelNamespace(type)}.{type.Name}";
        }
        public string CreateModelSubType(Type type)
        {
            return $"{CreateModelSubNamespace(type)}.{type.Name}";
        }
        public string CreateModelNamespace(Type type)
        {
            return $"{Namespace}.{CreateModelSubNamespace(type)}";
        }
        public string CreateModelSubNamespace(Type type)
        {
            return $"{ModelsFolder}.{CreateSubNamespaceFromEntityType(type)}";
        }
        public string CreateModelSubPath(Type type, string postFix, string fileExtension)
        {
            return Path.Combine(CreateModelSubNamespace(type).Replace(".", "\\"), $"{type.Name}{postFix}{fileExtension}");
        }

        protected static bool IsEntityType(Type type)
        {
            return type.FullName!.Contains($".{StaticLiterals.EntitiesFolder}.");
        }
        protected static bool IsModelType(Type type)
        {
            return type.FullName!.Contains($".{StaticLiterals.ModelsFolder}.");
        }
        protected static bool IsModelType(string strType)
        {
            return strType.Contains($".{StaticLiterals.ModelsFolder}.");
        }
        protected string ConvertEntityToModelType(string typeFullname)
        {
            var result = typeFullname;
            var entitiesFolder = $".{StaticLiterals.EntitiesFolder}.";

            if (result.Contains(entitiesFolder))
            {
                var modelsFolder = $".{StaticLiterals.ModelsFolder}.";

                result = result.Replace(entitiesFolder, modelsFolder);
                result = result.Replace(StaticLiterals.LogicExtension, Extension);
            }
            return result;
        }

        #region overrides
        public override string GetPropertyType(PropertyInfo propertyInfo)
        {
            var result = base.GetPropertyType(propertyInfo);

            return ConvertEntityToModelType(result);
        }
        protected override string CopyProperty(string copyType, PropertyInfo propertyInfo)
        {
            string? result = null;

            if (IsModelType(copyType) == false && IsEntityType(propertyInfo.PropertyType))
            {
                if (IsListType(propertyInfo.PropertyType))
                {
                    result = $"{propertyInfo.Name} = other.{propertyInfo.Name}.Select(e => {ConvertEntityToModelType(propertyInfo.PropertyType.GenericTypeArguments[0].FullName!)}.Create(e)).ToList();";
                }
                else
                {
                    result = $"{propertyInfo.Name} = other.{propertyInfo.Name} != null ? {ConvertEntityToModelType(propertyInfo.PropertyType.FullName!)}.Create(other.{propertyInfo.Name}) : null;";
                }
            }
            return result ?? base.CopyProperty(copyType, propertyInfo);
        }
        public override IEnumerable<string> CreateDelegateAutoGet(PropertyInfo propertyInfo, string delegateObjectName, PropertyInfo delegatePropertyInfo)
        {
            var result = new List<string>();
            var propertyType = GetPropertyType(propertyInfo);
            var delegateProperty = $"{delegateObjectName}.{delegatePropertyInfo.Name}";

            if (IsModelType(propertyType))
            {
                if (IsListType(propertyInfo.PropertyType))
                {
                    result.Add($"get => {delegateProperty}.Select(e => {ConvertEntityToModelType(propertyInfo.PropertyType.GenericTypeArguments[0].FullName!)}.Create(e)).ToList();");
                }
                else
                {
                    result.Add($"get => {delegateProperty} != null ? {propertyType.Replace("?", string.Empty)}.Create({delegateProperty}) : null;");
                }
            }
            else
            {
                result.Add($"get => {delegateObjectName}.{delegatePropertyInfo.Name};");
            }
            return result;
        }
        public override IEnumerable<string> CreateDelegateAutoSet(PropertyInfo propertyInfo, string delegateObjectName, PropertyInfo delegatePropertyInfo)
        {
            var result = new List<string>();
            var propertyType = GetPropertyType(propertyInfo);
            var delegateProperty = $"{delegateObjectName}.{delegatePropertyInfo.Name}";

            if (IsModelType(propertyType))
            {
                if (IsListType(propertyInfo.PropertyType))
                {
                    result.Add($"set => {delegateProperty} = value.Select(e => e.{delegateObjectName}).ToList();");
                }
                else
                {
                    result.Add($"set => {delegateProperty} = value?.{delegateObjectName};");
                }
            }
            else
            {
                result.Add($"set => { delegateObjectName}.{ delegatePropertyInfo.Name} = value;");
            }
            return result;
        }
        #endregion overrides

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

        protected virtual IGeneratedItem CreateModelFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var modelName = CreateModelName(type);
            var typeProperties = type.GetAllPropertyInfos();
            var generateProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)) == false) ?? Array.Empty<PropertyInfo>();
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateModelSubPath(type, string.Empty, StaticLiterals.CSharpFileExtension),
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
                result.AddRange(CreateCopyProperties("internal", type, type.FullName!));
                result.AddRange(CreateCopyProperties("public", type, CreateModelType(type)));
            }
            else if (unitType == Common.UnitType.WebApi)
            {
                result.AddRange(CreateCopyProperties("public", type, CreateModelType(type)));
            }
            else if (unitType == Common.UnitType.AspMvc)
            {
                result.AddRange(CreateCopyProperties("public", type, CreateModelType(type), p => true));
            }
            result.AddRange(CreateEquals(type));
            result.AddRange(CreateGetHashCode(type));
            result.AddRange(CreateFactoryMethods(CreateModelType(type), false));
            result.Add("}");
            result.EnvelopeWithANamespace(CreateModelNamespace(type), "using System;");
            result.FormatCSharpCode();
            return result;
        }
        protected virtual IGeneratedItem CreateDelegateModelFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var modelName = CreateModelName(type);
            var typeProperties = type.GetAllPropertyInfos();
            var generateProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)) == false) ?? Array.Empty<PropertyInfo>();
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = CreateModelSubPath(type, string.Empty, StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            CreateModelAttributes(type, result.Source);
            result.Add($"public partial class {modelName}");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(modelName));
            result.AddRange(CreatePartialConstrutor("public", modelName));

            result.Add(string.Empty);
            result.Add($"new internal {type.FullName} Source");
            result.Add("{");
            result.Add($"get => ({type.FullName})(_source ??= new {type.FullName}());");
            result.Add("set => _source = value;");
            result.Add("}");
            result.Add(string.Empty);

            foreach (var propertyInfo in generateProperties)
            {
                CreateModelPropertyAttributes(propertyInfo, result.Source);
                result.AddRange(CreateDelegateProperty(propertyInfo, "Source", propertyInfo));
            }
            if (unitType == Common.UnitType.Logic)
            {
                result.AddRange(CreateDelegateCopyProperties("internal", type, type.FullName!));
                result.AddRange(CreateDelegateCopyProperties("public", type, CreateModelType(type)));
            }
            else if (unitType == Common.UnitType.WebApi)
            {
                result.AddRange(CreateCopyProperties("public", type, CreateModelType(type)));
            }
            else if (unitType == Common.UnitType.AspMvc)
            {
                result.AddRange(CreateCopyProperties("public", type, CreateModelType(type), p => true));
            }
            result.AddRange(CreateEquals(type));
            result.AddRange(CreateGetHashCode(type));
            result.AddRange(CreateDelegateFactoryMethods(CreateModelType(type), type.FullName!, false));
            result.Add("}");
            result.EnvelopeWithANamespace(CreateModelNamespace(type), "using System;");
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
            return $"{CreateModelNamespace(type)}.{type.Name}";
        }
    }
}
//MdEnd