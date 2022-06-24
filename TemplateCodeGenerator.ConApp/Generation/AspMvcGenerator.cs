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
            var viewProperties = GetViewProperties(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = CreateFilterModelType(type),
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateModelSubPath(type, "Filter", StaticLiterals.CSharpFileExtension),
            };

            int idx = 0;
            result.AddRange(CreateComment(type));
            CreateModelAttributes(type, result.Source);
            result.Add($"public partial class {modelName}");
            result.Add("{");
            result.AddRange(CreatePartialStaticConstrutor(modelName));
            result.AddRange(CreatePartialConstrutor("public", modelName));

            foreach (var propertyInfo in viewProperties)
            {
                if (idx++ > 0)
                    sbHasValue.Append(" || ");

                if (propertyInfo.PropertyType == typeof(string))
                {
                    sbToString.Append($"{propertyInfo.Name}: " + "{(" + $"{propertyInfo.Name} ?? \"---\"" + ")} ");
                }
                else
                {
                    sbToString.Append($"{propertyInfo.Name}: " + "{(" + $"{propertyInfo.Name} != null ? {propertyInfo.Name} : \"---\"" + ")} ");
                }
                sbHasValue.Append($"{propertyInfo.Name} != null");
                result.AddRange(CreateFilterAutoProperty(propertyInfo));
            }

            if (sbHasValue.Length > 0)
            {
                result.AddRange(CreateComment(type));
                result.Add($"public bool HasValue => {sbHasValue};");
            }

            result.AddRange(CreateComment(type));
            result.Add("public string CreatePredicate()");
            result.Add("{");
            result.Add("var result = new System.Text.StringBuilder();");
            result.Add(string.Empty);
            foreach (var propertyInfo in viewProperties)
            {
                result.Add($"if ({propertyInfo.Name} != null)");
                result.Add("{");

                result.Add("if (result.Length > 0)");
                result.Add("{");
                result.Add("result.Append(\" || \");");
                result.Add("}");

                if (propertyInfo.PropertyType == typeof(string))
                {
                    result.Add("result.Append($\"(" + $"{propertyInfo.Name} != null && {propertyInfo.Name}.Contains(\\\"" + "{" + $"{propertyInfo.Name}" + "}" + "\\\"))\");");
                }
                else
                {
                    result.Add("result.Append($\"(" + $"{propertyInfo.Name} != null && {propertyInfo.Name} == " + "{" + $"{propertyInfo.Name}" + "})\");");
                }

                result.Add("}");
            }
            result.Add("return result.ToString();");
            result.Add("}");

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
            result.Add(string.Empty);
            result.Add("private static string FilterName => typeof(FilterType).Name;");
            result.Add(string.Empty);
            result.AddRange(CreatePartialConstrutor("public", controllerName, $"{contractType}<{accessType}> other", "base(other)", null, true));

            result.AddRange(CreateComment(type));
            result.Add($"protected override {modelType} ToViewModel({accessType} accessModel, ActionMode actionMode)");
            result.Add("{");
            result.Add($"var handled = false;");
            result.Add($"var result = default({modelType});");
            result.Add("BeforeToViewModel(accessModel, actionMode, ref result, ref handled);");
            result.Add("if (handled == false || result == null)");
            result.Add("{");
            result.Add($"result = {modelType}.Create(accessModel);");
            result.Add("}");
            result.Add("AfterToViewModel(result, actionMode);");
            result.Add("return BeforeView(result, actionMode);");
            result.Add("}");

            result.Add($"partial void BeforeToViewModel({accessType} accessModel, ActionMode actionMode, ref {modelType}? viewModel, ref bool handled);");
            result.Add($"partial void AfterToViewModel({modelType} viewModel, ActionMode actionMode);");

            result.AddRange(CreateComment(type));
            result.Add("public override async Task<IActionResult> Index()");
            result.Add("{");
            result.Add("IActionResult? result;");
            result.Add("var filter = SessionWrapper.Get<FilterType>(FilterName) ?? new FilterType();");
            result.Add(string.Empty);
            result.Add("if (filter.HasValue)");
            result.Add("{");
            result.Add("var predicate = filter.CreatePredicate();");
            result.Add("var accessModels = await DataAccess.QueryAsync(predicate);");
            result.Add(String.Empty);
            result.Add("result = View(AfterQuery(accessModels).Select(e => ToViewModel(e, ActionMode.Index)));");
            result.Add("}");
            result.Add("else");
            result.Add("{");
            result.Add("var accessModels = await DataAccess.GetAllAsync();");
            result.Add(String.Empty);
            result.Add("result = View(AfterQuery(accessModels).Select(e => ToViewModel(e, ActionMode.Index)));");
            result.Add("}");
            result.Add("ViewBag.Filter = filter;");
            result.Add("return result;");
            result.Add("}");

            result.AddRange(CreateComment(type));
            result.Add("public async Task<IActionResult> Filter(FilterType filter)");
            result.Add("{");
            result.Add("IActionResult? result;");
            result.Add(string.Empty);
            result.Add("if (filter.HasValue)");
            result.Add("{");
            result.Add("var predicate = filter.CreatePredicate();");
            result.Add("var accessModels = await DataAccess.QueryAsync(predicate);");
            result.Add(String.Empty);
            result.Add("result = View(\"Index\", AfterQuery(accessModels).Select(e => ToViewModel(e, ActionMode.Index)));");
            result.Add("}");
            result.Add("else");
            result.Add("{");
            result.Add("result = RedirectToAction(\"Index\");");
            result.Add("}");
            result.Add("ViewBag.Filter = filter;");
            result.Add("SessionWrapper.Set<FilterType>(FilterName, filter);");
            result.Add("return result;");
            result.Add("}");

            result.Add("}");
            result.EnvelopeWithANamespace(ItemProperties.CreateControllerNamespace(type), "using Microsoft.AspNetCore.Mvc;", filterModelUsing);
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
        protected static bool IsPrimitiveNullable(PropertyInfo propertyInfo)
        {
            var result = propertyInfo.PropertyType.IsNullableType();

            if (result)
            {
                result = propertyInfo.PropertyType.GetGenericArguments()[0].IsPrimitive;
            }
            return result;
        }
        protected static IEnumerable<PropertyInfo> GetViewProperties(Type type)
        {
            var typeProperties = type.GetAllPropertyInfos();
            var viewProperties = typeProperties.Where(e => StaticLiterals.VersionEntityProperties.Any(p => p.Equals(e.Name)) == false
                                                        && IsListType(e.PropertyType) == false
                                                        && (e.PropertyType.IsPrimitive || IsPrimitiveNullable(e) || e.PropertyType == typeof(string)));

            return viewProperties;
        }
        protected virtual IGeneratedItem CreatePartialTableHeaderView(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var viewProperties = GetViewProperties(type);
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
            result.Add(" <tr>");

            foreach (var item in viewProperties)
            {
                result.Add("  <th>");
                result.Add($"   @Html.DisplayNameFor(model => model.{item.Name})");
                result.Add("  </th>");
            }

            result.Add("  <th></th>");
            result.Add(" </tr>");
            result.Add("</thead>");
            return result;
        }
        protected virtual IGeneratedItem CreatePartialTableRowView(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var viewProperties = GetViewProperties(type);
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
                result.Add(" <td>");
                result.Add($"  @Html.DisplayFor(model => model.{item.Name})");
                result.Add(" </td>");
            }

            result.Add(" <td>");
            result.Add("  @Html.ActionLink(\"Edit\", \"Edit\", new { id=Model.Id }) |");
            result.Add("  @Html.ActionLink(\"Details\", \"Details\", new { id=Model.Id }) |");
            result.Add("  @Html.ActionLink(\"Delete\", \"Delete\", new { id=Model.Id })");
            result.Add(" </td>");
            result.Add("</tr>");
            return result;
        }
        protected virtual IGeneratedItem CreatePartialEditModelView(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var viewProperties = GetViewProperties(type);
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
            result.Add(" <div class=\"col-md-4\">");
            result.Add("  <div asp-validation-summary=\"ModelOnly\" class=\"text-danger\"></div>");
            result.Add("  <input asp-for=\"Id\" type=\"hidden\" />");

            foreach (var item in viewProperties)
            {
                result.Add("  <div class=\"form-group\">");
                result.Add($"   <label asp-for=\"{item.Name}\" class=\"control-label\"></label>");
                if (item.PropertyType == typeof(bool) || item.PropertyType == typeof(bool?))
                {
                    result.Add($"   <input asp-for=\"{item.Name}\" class=\"form-check\" />");
                }
                else
                {
                    result.Add($"   <input asp-for=\"{item.Name}\" class=\"form-control\" />");
                }
                result.Add($"   <span asp-validation-for=\"{item.Name}\" class=\"text-danger\"></span>");
                result.Add("  </div>");
            }

            result.Add(" </div>");
            result.Add("</div>");
            return result;
        }
        protected virtual IGeneratedItem CreatePartialFilterView(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var viewProperties = GetViewProperties(type);
            var modelType = ItemProperties.CreateFilterModelType(type);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = ItemProperties.CreateModelType(type),
                FileExtension = StaticLiterals.CSharpHtmlFileExtension,
                SubFilePath = ItemProperties.CreateViewSubPathFromType(type, "_Filter", StaticLiterals.CSharpHtmlFileExtension),
            };
            result.Add($"@model {modelType}");
            result.Add(string.Empty);

            result.Add("<div class=\"row\">");
            result.Add(" <div class=\"col-md-4\">");
            result.Add("  <form asp-action=\"Filter\">");
            result.Add("   <div asp-validation-summary=\"ModelOnly\" class=\"text-danger\"></div>");

            foreach (var item in viewProperties)
            {
                result.Add("   <div class=\"form-group\">");
                result.Add($"    <label asp-for=\"{item.Name}\" class=\"control-label\"></label>");
                if (item.PropertyType == typeof(bool) || item.PropertyType == typeof(bool?))
                {
                    result.Add($"    <input asp-for=\"{item.Name}\" class=\"form-check\" />");
                }
                else
                {
                    result.Add($"    <input asp-for=\"{item.Name}\" class=\"form-control\" />");
                }
                result.Add("   </div>");
            }

            result.Add("   <p></p>");
            result.Add("   <div class=\"form-group\">");
            result.Add("    <input type=\"submit\" value=\"Apply\" class=\"btn btn-primary\" />");
            result.Add("   </div>");
            result.Add("  </form>");

            result.Add(" </div>");
            result.Add("</div>");
            return result;
        }
        protected virtual IGeneratedItem CreatePartialDisplayModelView(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var viewProperties = GetViewProperties(type);
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
                result.Add(" <dt class=\"col-sm-2\">");
                result.Add($"  @Html.DisplayNameFor(model => model.{item.Name})");
                result.Add(" </dt>");
                result.Add(" <dd class=\"col-sm-10\">");
                result.Add($"  @Html.DisplayFor(model => model.{item.Name})");
                result.Add(" </dd>");
            }

            result.Add("</dl>");
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
