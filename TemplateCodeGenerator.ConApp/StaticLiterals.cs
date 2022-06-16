//MdStart
using CommonStaticLiterals = CommonBase.StaticLiterals;

namespace TemplateCodeGenerator.ConApp
{
    public static partial class StaticLiterals
    {
        public static string GeneratedCodeFileName => "_GeneratedCode.cs";
        public static string SourceFileExtensions => CommonStaticLiterals.SourceFileExtensions;
        public static string CSharpFileExtension => CommonStaticLiterals.CSharpFileExtension;
        public static string CSharpHtmlFileExtension => $"{CommonStaticLiterals.CSharpFileExtension}html";
        public static string GeneratedCodeLabel => CommonStaticLiterals.GeneratedCodeLabel;
        public static string CustomizedAndGeneratedCodeLabel => CommonStaticLiterals.CustomizedAndGeneratedCodeLabel;

        public static IDictionary<string, string> SourceFileHeaders { get; } = new Dictionary<string, string>()
        {
            {".css", $"/*{GeneratedCodeLabel}*/" },
            {".cs", $"//{GeneratedCodeLabel}" },
            {".ts", $"//{GeneratedCodeLabel}" },
            {".cshtml", $"@*{GeneratedCodeLabel}*@" },
            {".razor", $"@*{GeneratedCodeLabel}*@" },
            {".razor.cs", $"//{GeneratedCodeLabel}" },
        };

        #region Project Extensions
        public static string LogicExtension => ".Logic";
        public static string WebApiExtension => ".WebApi";
        public static string AspMvsExtension => ".AspMvc";
        #endregion Project Extensions

        #region Entity properties
        public static string IdentityEntityName => "IdentityEntity";
        public static string VersionEntityName => "VersionEntity";
        public static string[] EntityBaseClasses => new string[] { VersionEntityName, IdentityEntityName };
        public static string[] IdentityEntityProperties => new string[] { "Id" };
        public static string[] VersionEntityProperties => new string[] { "Id", "RowVersion" };
        #endregion Entity properties

        #region Model properties
        public static string IdentityModelName => "IdentityModel";
        public static string VersionModelName => "VersionModel";
        public static string[] ModelBaseClasses => new string[] { VersionModelName, IdentityModelName };
        public static string[] IdentityModelProperties => new string[] { "Id" };
        public static string[] VersionModelProperties => new string[] { "Id", "RowVersion" };
        #endregion Model properties

        #region Folders
        public static string EntitiesFolder => "Entities";
        public static string ModelsFolder => "Models";
        public static string ContractsFolder => "Contracts";
        public static string DataContextFolder => "DataContext";
        public static string ControllersFolder => "Controllers";
        public static string FacadesFolder => "Facades";
        public static string ViewsFolder => "Views";
        #endregion Folders
    }
}
//MdEnd