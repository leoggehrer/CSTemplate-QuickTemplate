//@BaseCode
//MdStart

using System.Collections.Generic;
using CommonStaticLiterals = CommonBase.StaticLiterals;

namespace TemplateCodeGenerator.ConApp
{
    public static partial class StaticLiterals
    {
        public static string LogicExtension => ".Logic";
        public static string SourceFileExtensions => CommonStaticLiterals.SourceFileExtensions;
        public static string CSharpFileExtension => CommonStaticLiterals.CSharpFileExtension;
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

        #region Entity names
        public static string IdentityEntityName => "IdentityEntity";
        public static string VersionEntityName => "VersionEntity";
        public static string[] IdentityEntityProperties => new string[] { "Id" };
        public static string[] VersionEntityProperties => new string[] { "Id", "RowVersion" };
        #endregion Entity names

        #region Folders
        public static string EntitiesFolder => "Entities";
        public static string DataContextFolder => "DataContext";
        public static string ControllersFolder => "Controllers";
        public static string ModelsFolder => "Models";
        public static string ModulesFolder => "Modules";
        #endregion Folders

        public static string EntitiesLabel => "Entities";
        public static string ModulesLabel => "Modules";

        public static string DelegatePropertyName => "DelegateObject";
    }
}
//MdEnd