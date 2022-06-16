using System.Reflection;
using System.Text;
using TemplateCodeGenerator.ConApp.Contracts;

namespace TemplateCodeGenerator.ConApp.Generation
{
    internal partial class AspMvcGenerator : ModelGenerator
    {
        private ItemProperties? _itemProperties;
        protected override ItemProperties ItemProperties => _itemProperties ??= new ItemProperties(SolutionProperties.SolutionName, StaticLiterals.AspMvsExtension);
        public AspMvcGenerator(ISolutionProperties solutionProperties) : base(solutionProperties)
        {
        }

        public virtual IEnumerable<IGeneratedItem> GenerateAll()
        {
            var result = new List<IGeneratedItem>();

            result.AddRange(CreateModels());
            result.AddRange(CreateControllers());
            result.Add(CreateAddServices());
            result.AddRange(CreateViews());
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
                    result.Add(CreateModelInheritance(type, Common.UnitType.AspMvc, Common.ItemType.Model));
                    result.Add(CreateFilterModelFromType(type, Common.UnitType.AspMvc, Common.ItemType.FilterModel));
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

        protected virtual IGeneratedItem CreateFilterModelFromType(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var sbHasValue = new StringBuilder();
            var sbToString = new StringBuilder();
            var modelName = CreateFilterModelName(type);
            var typeProperties = type.GetAllPropertyInfos();
            var filteredProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)) == false
                                                            && IsListType(e.PropertyType) == false
                                                            && (e.PropertyType.IsPrimitive || e.PropertyType == typeof(string)));
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateFilterModelType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateModelSubPath(type, "Filter", StaticLiterals.CSharpFileExtension),
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
                result.Add($"public bool HasValue => {sbHasValue};");
            }

            if (sbToString.Length > 0)
            {
                result.AddRange(CreateComment(type));
                result.Add("public override string ToString()");
                result.Add("{");
                result.Add($"return $\"{sbToString}\";");
                result.Add("}");
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
                    result.Add(CreateControllerFromType(type, Common.UnitType.AspMvc, Common.ItemType.Controller));
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
            var controllerName = ItemProperties.CreateControllerName(type);
            var contractType = ItemProperties.CreateContractType(type);
            var filterModelUsing = $"using FilterType = {CreateFilterModelType(type)};";
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = $"{ItemProperties.CreateControllerType(type)}",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateControllersSubPathFromType(type, string.Empty, StaticLiterals.CSharpFileExtension),
            };
            result.AddRange(CreateComment(type));
            CreateControllerAttributes(type, result.Source);
            result.Add($"{visibility} sealed partial class {controllerName} : {genericType}<{accessType}, {modelType}>");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(controllerName));
            result.AddRange(CreatePartialConstrutor("public", controllerName, $"{contractType}<{accessType}> other", "base(other)", null, true));
            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateControllerNamespace(type), filterModelUsing);
            result.FormatCSharpCode();
            return result;
        }

        protected virtual IGeneratedItem CreateAddServices()
        {
            var entityProject = EntityProject.Create(SolutionProperties);
            var result = new Models.GeneratedItem(Common.UnitType.AspMvc, Common.ItemType.AddServices)
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

        protected virtual IEnumerable<IGeneratedItem> CreateViews()
        {
            var result = new List<IGeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.EntityTypes)
            {
                if (CanCreate(type))
                {
                    result.Add(CreatePartialTableHeaderView(type, Common.UnitType.AspMvc, Common.ItemType.View));
                    result.Add(CreatePartialTableRowView(type, Common.UnitType.AspMvc, Common.ItemType.View));
                    result.Add(CreatePartialEditModelView(type, Common.UnitType.AspMvc, Common.ItemType.View));
                    result.Add(CreatePartialDisplayModelView(type, Common.UnitType.AspMvc, Common.ItemType.View));
                    result.Add(CreatePartialFilterView(type, Common.UnitType.AspMvc, Common.ItemType.View));
                }
            }
            return result;
        }
        protected virtual IGeneratedItem CreatePartialTableHeaderView(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var typeProperties = type.GetAllPropertyInfos();
            var viewProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)) == false
                                                        && IsListType(e.PropertyType) == false
                                                        && (e.PropertyType.IsPrimitive || e.PropertyType == typeof(string)));
            var modelType = ItemProperties.CreateModelType(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateModelType(type),
                FileExtension = StaticLiterals.CSharpHtmlFileExtension,
                SubFilePath = ItemProperties.CreateViewSubPathFromType(type, "_TableHeader", StaticLiterals.CSharpHtmlFileExtension),
            };
            result.Add($"@model IEnumerable<{modelType}>");
            result.Add(string.Empty);
            result.Add("<thead>");
            result.Add("    <tr>");

            foreach (var item in viewProperties)
            {
                result.Add("        <th>");
                result.Add($"            @Html.DisplayNameFor(model => model.{item.Name})");
                result.Add("        </th>");
            }

            result.Add("        <th></th>");
            result.Add("    </tr>");
            result.Add("</thead>");
            result.FormatCSharpCode();
            return result;
        }
        protected virtual IGeneratedItem CreatePartialTableRowView(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var typeProperties = type.GetAllPropertyInfos();
            var viewProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)) == false
                                                        && IsListType(e.PropertyType) == false
                                                        && (e.PropertyType.IsPrimitive || e.PropertyType == typeof(string)));
            var modelType = ItemProperties.CreateModelType(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateModelType(type),
                FileExtension = StaticLiterals.CSharpHtmlFileExtension,
                SubFilePath = ItemProperties.CreateViewSubPathFromType(type, "_TableRow", StaticLiterals.CSharpHtmlFileExtension),
            };
            result.Add($"@model {modelType}");
            result.Add(string.Empty);
            result.Add("<tr>");

            foreach (var item in viewProperties)
            {
                result.Add("    <td>");
                result.Add($"        @Html.DisplayFor(model => model.{item.Name})");
                result.Add("    </td>");
            }

            result.Add("    <td>");
            result.Add("        @Html.ActionLink(\"Edit\", \"Edit\", new { id=Model.Id }) |");
            result.Add("        @Html.ActionLink(\"Details\", \"Details\", new { id=Model.Id }) |");
            result.Add("    @Html.ActionLink(\"Delete\", \"Delete\", new { id=Model.Id })");
            result.Add("    </td>");
            result.Add("</tr>");
            result.FormatCSharpCode();
            return result;
        }
        protected virtual IGeneratedItem CreatePartialEditModelView(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var typeProperties = type.GetAllPropertyInfos();
            var viewProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)) == false
                                                        && IsListType(e.PropertyType) == false
                                                        && (e.PropertyType.IsPrimitive || e.PropertyType == typeof(string)));
            var modelType = ItemProperties.CreateModelType(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateModelType(type),
                FileExtension = StaticLiterals.CSharpHtmlFileExtension,
                SubFilePath = ItemProperties.CreateViewSubPathFromType(type, "_EditModel", StaticLiterals.CSharpHtmlFileExtension),
            };
            result.Add($"@model {modelType}");
            result.Add(string.Empty);

            result.Add("<div class=\"row\">");
            result.Add("    <div class=\"col-md-4\">");
            result.Add("        <div asp-validation-summary=\"ModelOnly\" class=\"text-danger\"></div>");
            result.Add("        <input asp-for=\"Id\" type=\"hidden\" />");

            foreach (var item in viewProperties)
            {
                result.Add("        <div class=\"form-group\">");
                result.Add($"            <label asp-for=\"{item.Name}\" class=\"control-label\"></label>");
                result.Add($"            <input asp-for=\"{item.Name}\" class=\"form-control\" />");
                result.Add($"            <span asp-validation-for=\"{item.Name}\" class=\"text-danger\"></span>");
                result.Add("        </div>");
            }

            result.Add("    </div>");
            result.Add("</div>");
            result.FormatCSharpCode();
            return result;
        }
        protected virtual IGeneratedItem CreatePartialFilterView(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var typeProperties = type.GetAllPropertyInfos();
            var viewProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)) == false
                                                        && IsListType(e.PropertyType) == false
                                                        && (e.PropertyType.IsPrimitive || e.PropertyType == typeof(string)));
            var modelType = ItemProperties.CreateModelType(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateModelType(type),
                FileExtension = StaticLiterals.CSharpHtmlFileExtension,
                SubFilePath = ItemProperties.CreateViewSubPathFromType(type, "_Filter", StaticLiterals.CSharpHtmlFileExtension),
            };
            result.Add($"@model {modelType}");
            result.Add(string.Empty);

            result.Add("<div class=\"row\">");
            result.Add("    <div class=\"col-md-4\">");
            result.Add("        <div asp-validation-summary=\"ModelOnly\" class=\"text-danger\"></div>");

            foreach (var item in viewProperties)
            {
                result.Add("        <div class=\"form-group\">");
                result.Add($"            <label asp-for=\"{item.Name}\" class=\"control-label\"></label>");
                result.Add($"            <input asp-for=\"{item.Name}\" class=\"form-control\" />");
                result.Add("        </div>");
            }

            result.Add("    </div>");
            result.Add("</div>");
            result.FormatCSharpCode();
            return result;
        }

        protected virtual IGeneratedItem CreatePartialDisplayModelView(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var typeProperties = type.GetAllPropertyInfos();
            var viewProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)) == false
                                                        && IsListType(e.PropertyType) == false
                                                        && (e.PropertyType.IsPrimitive || e.PropertyType == typeof(string)));
            var modelType = ItemProperties.CreateModelType(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateModelType(type),
                FileExtension = StaticLiterals.CSharpHtmlFileExtension,
                SubFilePath = ItemProperties.CreateViewSubPathFromType(type, "_DisplayModel", StaticLiterals.CSharpHtmlFileExtension),
            };
            result.Add($"@model {modelType}");
            result.Add(string.Empty);

            result.Add("<dl class=\"row\">");

            foreach (var item in viewProperties)
            {
                result.Add("    <dt class=\"col-sm-2\">");
                result.Add($"        @Html.DisplayNameFor(model => model.{item.Name})");
                result.Add("    </dt>");
                result.Add("    <dd class=\"col-sm-10\">");
                result.Add($"        @Html.DisplayFor(model => model.{item.Name})");
                result.Add("    </dd>");
            }

            result.Add("</dl>");
            result.FormatCSharpCode();
            return result;
        }

        public static string CreateFilterModelName(Type type)
        {
            return $"{CreateModelName(type)}Filter";
        }
        protected string CreateFilterModelType(Type type)
        {
            return $"{ItemProperties.Namespace}.{ItemProperties.CreateModelSubNamespace(type)}.{CreateFilterModelName(type)}";
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

        #region Partial methods
        partial void CreateModelAttributes(Type type, List<string> source);
        partial void CreateControllerAttributes(Type type, List<string> codeLines);
        #endregion Partial methods
    }
}
