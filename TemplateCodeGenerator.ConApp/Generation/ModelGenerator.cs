//MdStart
using System.Reflection;
using TemplateCodeGenerator.ConApp.Contracts;

namespace TemplateCodeGenerator.ConApp.Generation
{
    internal abstract partial class ModelGenerator : ClassGenerator
    {
        protected abstract ItemProperties ItemProperties { get; }
        public ModelGenerator(ISolutionProperties solutionProperties)
            : base(solutionProperties)
        {
        }

        #region overrides
        public override string GetPropertyType(PropertyInfo propertyInfo)
        {
            var result = base.GetPropertyType(propertyInfo);

            return ItemProperties.ConvertEntityToModelType(result);
        }
        protected override string CopyProperty(string copyType, PropertyInfo propertyInfo)
        {
            string? result = null;

            if (ItemProperties.IsModelType(copyType) == false && ItemProperties.IsEntityType(propertyInfo.PropertyType))
            {
                if (IsArrayType(propertyInfo.PropertyType))
                {
                    result = $"{propertyInfo.Name} = other.{propertyInfo.Name}.Select(e => {ItemProperties.ConvertEntityToModelType(propertyInfo.PropertyType.GetElementType()!.FullName!)}.Create((object)e)).ToArray();";
                }
                else if (IsListType(propertyInfo.PropertyType))
                {
                    result = $"{propertyInfo.Name} = other.{propertyInfo.Name}.Select(e => {ItemProperties.ConvertEntityToModelType(propertyInfo.PropertyType.GenericTypeArguments[0].FullName!)}.Create((object)e)).ToList();";
                }
                else
                {
                    result = $"{propertyInfo.Name} = other.{propertyInfo.Name} != null ? {ItemProperties.ConvertEntityToModelType(propertyInfo.PropertyType.FullName!)}.Create((object)other.{propertyInfo.Name}) : null;";
                }
            }
            return result ?? base.CopyProperty(copyType, propertyInfo);
        }
        public override IEnumerable<string> CreateDelegateAutoGet(PropertyInfo propertyInfo, string delegateObjectName, PropertyInfo delegatePropertyInfo)
        {
            var result = new List<string>();
            var propertyType = GetPropertyType(propertyInfo);
            var delegateProperty = $"{delegateObjectName}.{delegatePropertyInfo.Name}";

            if (ItemProperties.IsModelType(propertyType))
            {
                if (IsArrayType(propertyInfo.PropertyType))
                {
                    result.Add($"get => {delegateProperty}.Select(e => {ItemProperties.ConvertEntityToModelType(propertyInfo.PropertyType.GetElementType()!.FullName!)}.Create(e)).ToArray();");
                }
                else if (IsListType(propertyInfo.PropertyType))
                {
                    result.Add($"get => {delegateProperty}.Select(e => {ItemProperties.ConvertEntityToModelType(propertyInfo.PropertyType.GenericTypeArguments[0].FullName!)}.Create(e)).ToList();");
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

            if (ItemProperties.IsModelType(propertyType))
            {
                if (IsArrayType(propertyInfo.PropertyType))
                {
                    result.Add($"set => {delegateProperty} = value.Select(e => e.{delegateObjectName}).ToArray();");
                }
                else if (IsListType(propertyInfo.PropertyType))
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

        private T QueryModelSetting<T>(Common.UnitType unitType, Common.ItemType itemType, Type type, string valueName, string defaultValue)
        {
            T result;

            try
            {
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(unitType, itemType, CreateEntitiesSubTypeFromType(type), valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
        }
        private T QueryModelSetting<T>(Common.UnitType unitType, Common.ItemType itemType, Type type, string itemSubName, string valueName, string defaultValue)
        {
            T result;

            try
            {
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(unitType, itemType, $"{CreateEntitiesSubTypeFromType(type)}.{itemSubName}", valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
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

        protected virtual IGeneratedItem CreateModelFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var modelName = CreateModelName(type);
            var visibility = QueryModelSetting<string>(unitType, itemType, type, StaticLiterals.Visibility, "public");
            var attributes = QueryModelSetting<string>(unitType, itemType, type, StaticLiterals.Attributes, string.Empty);
            var typeProperties = type.GetAllPropertyInfos();
            var generateProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)) == false) ?? Array.Empty<PropertyInfo>();
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateModelFullNameFromType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateModelSubPath(type, string.Empty, StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            CreateModelAttributes(type, result.Source);
            result.Add($"{(attributes.HasContent() ? $"[{attributes}]" : string.Empty)}");
            result.Add($"{visibility} partial class {modelName}");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(modelName));
            result.AddRange(CreatePartialConstrutor("public", modelName));

            foreach (var propertyInfo in generateProperties)
            {
                var propertyAttributes = QueryModelSetting<string>(unitType, Common.ItemType.Property, type, propertyInfo.Name, StaticLiterals.Attributes, string.Empty);

                result.AddRange(CreateComment(propertyInfo));
                CreateModelPropertyAttributes(propertyInfo, result.Source);
                result.Add($"{(propertyAttributes.HasContent() ? $"[{propertyAttributes}]" : string.Empty)}");
                result.AddRange(CreateProperty(type, propertyInfo));
            }

            result.AddRange(CreateFactoryMethod(false, ItemProperties.CreateModelType(type)));

            if (unitType == Common.UnitType.Logic)
            {
                result.AddRange(CreateFactoryMethod(false, ItemProperties.CreateModelType(type), type.FullName!));

                result.AddRange(CreateCopyProperties("internal", type, type.FullName!));
                result.AddRange(CreateCopyProperties("public", type, ItemProperties.CreateModelType(type)));
            }
            else if (unitType == Common.UnitType.WebApi)
            {
                if (type.IsPublic)
                {
                    result.AddRange(CreateFactoryMethod(false, ItemProperties.CreateModelType(type), type.FullName!));

                    result.AddRange(CreateCopyProperties("public", type, type.FullName!));
                    result.AddRange(CreateCopyProperties("public", type, ItemProperties.CreateModelType(type)));
                }
                else
                {
                    result.AddRange(CreateCopyProperties("public", type, ItemProperties.CreateModelType(type)));
                }
            }
            else if (unitType == Common.UnitType.AspMvc)
            {
                if (type.IsPublic)
                {
                    result.AddRange(CreateFactoryMethod(false, ItemProperties.CreateModelType(type), type.FullName!));

                    result.AddRange(CreateCopyProperties("public", type, type.FullName!));
                    result.AddRange(CreateCopyProperties("public", type, ItemProperties.CreateModelType(type)));
                }
                else
                {
                    result.AddRange(CreateCopyProperties("public", type, ItemProperties.CreateModelType(type)));
                }
            }
            result.AddRange(CreateEquals(type));
            result.AddRange(CreateGetHashCode(type));
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateModelNamespace(type), "using System;");
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
                SubFilePath = ItemProperties.CreateModelSubPath(type, string.Empty, StaticLiterals.CSharpFileExtension),
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
                result.AddRange(CreateDelegateCopyProperties("public", type, ItemProperties.CreateModelType(type)));
            }
            else if (unitType == Common.UnitType.WebApi)
            {
                result.AddRange(CreateCopyProperties("public", type, ItemProperties.CreateModelType(type)));
            }
            else if (unitType == Common.UnitType.AspMvc)
            {
                result.AddRange(CreateCopyProperties("public", type, ItemProperties.CreateModelType(type), p => true));
            }
            result.AddRange(CreateEquals(type));
            result.AddRange(CreateGetHashCode(type));
            result.AddRange(CreateDelegateFactoryMethods(ItemProperties.CreateModelType(type), type.FullName!, false));
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateModelNamespace(type), "using System;");
            result.FormatCSharpCode();
            return result;
        }

        protected string GetBaseClassByType(Type type, string subNamespace)
        {
            var result = "object";

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
                    var ns = ItemProperties.Namespace;

                    if (string.IsNullOrEmpty(subNamespace) == false)
                        ns = $"{ns}.{subNamespace}";

                    result = $"{ns}.{StaticLiterals.ModelBaseClasses[idx]}";
                }
            }
            return result;
        }
        protected string CreateModelFullNameFromType(Type type)
        {
            return $"{ItemProperties.CreateModelNamespace(type)}.{type.Name}";
        }
    }
}
//MdEnd