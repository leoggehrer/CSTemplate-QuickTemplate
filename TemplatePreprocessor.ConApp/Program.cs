//@BaseCode
//MdStart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace TemplatePreprocessor.ConApp
{
    internal partial class Program
    {
        #region Class-Constructors
        static Program()
        {
            ClassConstructing();
            HomePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                        Environment.OSVersion.Platform == PlatformID.MacOSX)
                       ? Environment.GetEnvironmentVariable("HOME")
                       : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

            UserPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            SourcePath = GetCurrentSolutionPath();
            Directives = new string[]
            {
                "ACCOUNT_OFF",
                "LOGGING_OFF",
                "REVISION_OFF"
            };
            ClassConstructed();
        }
        static partial void ClassConstructing();
        static partial void ClassConstructed();
        #endregion Class-Constructors
        private static string? HomePath { get; set; }
        private static string UserPath { get; set; }
        private static string SourcePath { get; set; }
        private static string[] Directives { get; set; }

        private static void Main(/*string[] args*/)
        {
            Console.WriteLine(nameof(TemplatePreprocessor));

            RunApp();
        }

        private static void RunApp()
        {
            var input = string.Empty;

            while (input.Equals("x") == false)
            {
                var sourceSolutionName = GetSolutionNameByPath(SourcePath);

                Console.Clear();
                Console.WriteLine("Solution Preprocessor");
                Console.WriteLine("=====================");
                Console.WriteLine();
                Console.WriteLine($"Set directives '{sourceSolutionName}' from: {SourcePath}");
                Console.WriteLine();
                Console.WriteLine("[1] Change source path");
                Console.WriteLine("[2] Start copy process");
                Console.WriteLine("[x|X] Exit");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Choose: ");
                input = Console.ReadLine()?.ToLower() ?? String.Empty;

                if (input.Equals("1"))
                {
                    Console.Write("Enter source path: ");
                    var path = Console.ReadLine();

                    if (string.IsNullOrEmpty(path) == false)
                    {
                        SourcePath = path;
                    }
                }
                else if (input.Equals("2"))
                {
                    SetPreprocessorDirectivesInProjectFiles(SourcePath, Directives);
                    EditPreprocessorDirectivesInRazorFiles(SourcePath, Directives);
                }
                Console.ResetColor();
            }
        }

        private static void PrintSolutionDirectives(string path, params string[] excludeDirectives)
        {
            var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var idx = 0;
                var lines = File.ReadAllLines(file, Encoding.Default);

                foreach (var line in lines)
                {
                    if (line.Trim().StartsWith("#if ") && excludeDirectives.Any(e => line.Contains(e)) == false)
                    {
                        var message = $"{line} in line {idx} of the {file} file";

                        //Console.WriteLine(message);
                        //Debug.WriteLine(message);
                    }
                    idx++;
                }
            }
        }

        private static string GetCurrentSolutionPath()
        {
            int endPos = AppContext.BaseDirectory
                                   .IndexOf($"{nameof(TemplatePreprocessor)}", StringComparison.CurrentCultureIgnoreCase);
            var result = AppContext.BaseDirectory[..endPos];

            while (result.EndsWith("/"))
            {
                result = result[0..^1];
            }
            while (result.EndsWith("\\"))
            {
                result = result[0..^1];
            }
            return result;
        }
        private static string GetCurrentSolutionName()
        {
            var solutionPath = GetCurrentSolutionPath();

            return GetSolutionNameByFile(solutionPath);
        }
        private static string GetSolutionNameByPath(string solutionPath)
        {
            return solutionPath.Split(new char[] { '\\', '/' })
                               .Where(e => string.IsNullOrEmpty(e) == false)
                               .Last();
        }
        private static string GetSolutionNameByFile(string solutionPath)
        {
            var fileInfo = new DirectoryInfo(solutionPath).GetFiles()
                                                          .SingleOrDefault(f => f.Extension.Equals(".sln", StringComparison.CurrentCultureIgnoreCase));

            return fileInfo != null ? Path.GetFileNameWithoutExtension(fileInfo.Name) : string.Empty;
        }

        private static int SetPreprocessorDirectivesInProjectFiles(string path, params string[] directiveItems)
        {
            var files = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);
            var directives = string.Join(";", directiveItems);

            foreach (var file in files)
            {
                var hasChanged = false;
                var result = new List<string>();
                var lines = File.ReadAllLines(file, Encoding.Default);

                foreach (var line in lines)
                {
                    if (line.Contains("<DefineConstants>", "</DefineConstants>"))
                    {
                        hasChanged = true;
                        result.Add(line.ReplaceBetween("<DefineConstants>", "</DefineConstants>", directives));
                    }
                    else
                    {
                        result.Add(line);
                    }
                }
                if (hasChanged == false && directives.Length > 0)
                {
                    var insertIdx = result.FindIndex(e => e.Contains("</PropertyGroup>"));

                    insertIdx = insertIdx < 0 ? result.Count - 2 : insertIdx;
                    hasChanged = true;

                    result.InsertRange(insertIdx + 1, new string[]
                        {
                            string.Empty,
                            "  <PropertyGroup>",
                            $"    <DefineConstants>{directives}</DefineConstants>",
                            "  </PropertyGroup>",
                        });
                }
                if (hasChanged)
                {
                    File.WriteAllLines(file, result.ToArray(), Encoding.Default);
                }
            }
            return files.Length;
        }
        private static void EditPreprocessorDirectivesInRazorFiles(string path, params string[] directiveItems)
        {
            foreach (var directive in directiveItems)
            {
                var analyzeDirective = directive.ToUpper();

                if (analyzeDirective.EndsWith("_OFF", StringComparison.CurrentCultureIgnoreCase))
                {
                    SetPreprocessorDirectivesCommentsInRazorFiles(path, directive.Replace("_OFF", "_ON"));
                    RemovePreprocessorDirectivesCommentsInRazorFiles(path, directive);
                }
                else if (analyzeDirective.EndsWith("_ON", StringComparison.CurrentCultureIgnoreCase))
                {
                    SetPreprocessorDirectivesCommentsInRazorFiles(path, directive.Replace("_ON", "_OFF"));
                    RemovePreprocessorDirectivesCommentsInRazorFiles(path, directive);
                }
            }
        }
        private static void SetPreprocessorDirectivesCommentsInRazorFiles(string path, params string[] directiveItems)
        {
            var files = Directory.GetFiles(path, "*.cshtml", SearchOption.AllDirectories);

            foreach (var directive in directiveItems)
            {
                foreach (var file in files)
                {
                    var startIndex = 0;
                    var hasChanged = false;
                    var result = string.Empty;
                    var text = File.ReadAllLines(file, Encoding.Default)
                                   .Select(l =>
                                   {
                                       if (l.Contains($"@*#if {directive}*@") || l.Contains("@*#endif*@"))
                                       {
                                           l = l.Trim();
                                       }
                                       return l;
                                   }).ToText();

                    foreach (var tag in text.GetAllTags($"@*#if {directive}*@", "@*#endif*@"))
                    {
                        if (tag.StartTagIndex > startIndex)
                        {
                            result += text.Partialstring(startIndex, tag.StartTagIndex - 1);
                            result += tag.StartTag;
                            if (tag.InnerText.Trim().StartsWith("@*"))
                            {
                                result += tag.InnerText;
                            }
                            else
                            {
                                hasChanged = true;
                                result += /*Environment.NewLine + */"@*";
                                result += tag.InnerText;
                                result += "*@";// + Environment.NewLine;
                            }
                            result += tag.EndTag;
                            startIndex += tag.EndTagIndex + tag.EndTag.Length;
                        }
                    }
                    if (hasChanged && startIndex < text.Length)
                    {
                        result += text.Partialstring(startIndex, text.Length);
                    }
                    if (hasChanged)
                    {
                        File.WriteAllText(file, result, Encoding.Default);
                    }
                }
            }
        }
        private static void RemovePreprocessorDirectivesCommentsInRazorFiles(string path, params string[] directiveItems)
        {
            var files = Directory.GetFiles(path, "*.cshtml", SearchOption.AllDirectories);

            foreach (var directive in directiveItems)
            {
                foreach (var file in files)
                {
                    var startIndex = 0;
                    var hasChanged = false;
                    var result = string.Empty;
                    var text = File.ReadAllText(file, Encoding.Default);

                    foreach (var tag in text.GetAllTags($"@*#if {directive}*@", "@*#endif*@"))
                    {
                        if (tag.StartTagIndex > startIndex)
                        {
                            result += text.Partialstring(startIndex, tag.StartTagIndex - 1);
                            result += tag.StartTag;
                            var innerText = tag.InnerText.Trim(Environment.NewLine.ToCharArray());
                            if (innerText.Trim().StartsWith("@*") && innerText.Trim().EndsWith("*@"))
                            {
                                hasChanged = true;
                                result += innerText.Partialstring(2, innerText.Length - 5);
                                result += Environment.NewLine;
                            }
                            else
                            {
                                result += tag.InnerText;
                            }
                            startIndex += tag.EndTagIndex + tag.EndTag.Length;
                            result += tag.EndTag;
                        }
                    }
                    if (hasChanged && startIndex < text.Length)
                    {
                        result += text.Partialstring(startIndex, text.Length);
                    }
                    if (hasChanged)
                    {
                        File.WriteAllText(file, result, Encoding.Default);
                    }
                }
            }
        }
        private static void ReplacePreprocessorDirectivesInRazorFiles(string path, params string[] directiveItems)
        {
            var files = Directory.GetFiles(path, "*.cshtml", SearchOption.AllDirectories);

            foreach (var directive in directiveItems)
            {
                var labels = new[] { $"#if {directive}", "#endif" };

                foreach (var file in files)
                {
                    var hasChanged = false;
                    var result = new List<string>();
                    var lines = File.ReadAllLines(file, Encoding.Default);

                    foreach (var line in lines)
                    {
                        var targetLine = line;

                        foreach (var label in labels)
                        {
                            if (line.StartsWith(label))
                            {
                                hasChanged = true;
                                targetLine = line.Replace(label, $"@*{label}*@");
                            }
                        }
                        result.Add(targetLine);
                    }
                    if (hasChanged)
                    {
                        File.WriteAllLines(file, result, Encoding.Default);
                    }
                }
            }
        }
    }
}
//MdEnd