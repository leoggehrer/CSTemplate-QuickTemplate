using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace TemplateCopier.ConApp
{
    internal partial class Copier
    {
        private static char Separator => ';';
        public static Action<string> Logger { get; set; } = s => System.Diagnostics.Debug.WriteLine(s);

        private static string DockerfileName => "dockerfile";
        private static string DockerComposefileName => "docker-compose.yml";
        private static readonly string[] IgnoreFolders = new string[]
        {
            "\\.vs"
            ,"\\.vs\\"
            ,"\\.git"
            ,"\\.git\\"
            ,"\\bin"
            ,"\\bin\\"
            ,"\\obj"
            ,"\\obj\\"
            ,"\\node_modules\\"
        };
        private static readonly string[] IgnoreFileFolders = new string[]
        {
            "\\.vs\\"
            ,"\\.git\\"
            ,"\\bin\\"
            ,"\\obj\\"
            ,"\\node_modules\\"
            ,"\\Migrations\\"
        };
        private static string[] ReplaceExtensions { get; } = new string[]
        {
            ".asax"
            ,".config"
            ,".cs"
            ,".cshtml"
            ,".csproj"
            ,".css"
            ,".html"
            ,".js"
            ,".json"
            ,".less"
            ,".sln"
            ,".tt"
            ,".txt"
            ,".csv"
            ,".xml"
            ,".razor"
            ,".md"
            ,".template"
            ,".xaml"
        };
        private static string[] ProjectExtensions { get; } = new string[]
        {
            ".asax"
            ,".config"
            ,".cs"
            ,".cshtml"
            ,".csproj"
            ,".css"
            ,".html"
            ,".js"
            ,".json"
            ,".less"
            ,".tt"
            ,".txt"
            ,".xml"
            ,".razor"
            ,".md"
            ,".template"
            ,".jpg"
            ,".png"
            ,".xaml"
        };
        private static string[] SolutionExtenions { get; } = new string[]
        {
            ".jpg"
            ,".png"
            ,".drawio"
            ,".md"
            ,".txt"
            ,".csv"
            ,".json"
            ,".xlsx"
            ,".docx"
            ,".pdf"
            ,".yml"
            ,".cd"
        };
        private List<string> Extensions { get; } = new List<string>();
        private List<string> ProjectGuids { get; } = new List<string>();
        public void Copy(string sourceDirectory, string targetDirectory, IEnumerable<string> sourceProjets)
        {
            if (string.IsNullOrWhiteSpace(sourceDirectory) == true)
                throw new ArgumentException(null, nameof(sourceDirectory));

            if (string.IsNullOrWhiteSpace(targetDirectory) == true)
                throw new ArgumentException(null, nameof(targetDirectory));

            Logger($"Source-Project: {sourceDirectory}");
            Logger($"Target-Directory: {targetDirectory}");

            if (sourceDirectory.Equals(targetDirectory) == false)
            {
                Logger("Running");
                var result = CreateTemplate(sourceDirectory, targetDirectory, sourceProjets);

                foreach (var ext in Extensions.OrderBy(i => i))
                {
                    System.Diagnostics.Debug.WriteLine($",\"{ext}\"");
                }

                if (result)
                {
                    Logger("Finished!");
                }
                else
                {
                    Logger("Not finished! There are some errors!");
                }
            }
        }

        private bool CreateTemplate(string sourceSolutionDirectory, string targetSolutionDirectory, IEnumerable<string> sourceProjets)
        {
            if (Directory.Exists(targetSolutionDirectory) == false)
            {
                Directory.CreateDirectory(targetSolutionDirectory);
            }

            var sourceFolderName = new DirectoryInfo(sourceSolutionDirectory).Name;
            var targetFolderName = new DirectoryInfo(targetSolutionDirectory).Name;

            CopySolutionStructure(sourceSolutionDirectory, targetSolutionDirectory, sourceProjets);

            foreach (var directory in Directory.GetDirectories(sourceSolutionDirectory, "*", SearchOption.AllDirectories))
            {
                var subFolder = directory.Replace(sourceSolutionDirectory, string.Empty);

                if ((IgnoreFolders.Any(i => subFolder.EndsWith(i) || subFolder.Contains(i)) == false)
                    && sourceProjets.Any(i => subFolder.EndsWith(i)))
                {
                    subFolder = subFolder.Replace(sourceFolderName, targetFolderName);

                    CopyProjectDirectoryWorkFiles(directory, sourceSolutionDirectory, targetSolutionDirectory);
                }
            }
            return true;
        }
        private void CopySolutionStructure(string sourceSolutionDirectory, string targetSolutionDirectory, IEnumerable<string> sourceProjects)
        {
            var sourceSolutionFolder = new DirectoryInfo(sourceSolutionDirectory).Name;
            var sourceSolutionFilePath = Directory.GetFiles(sourceSolutionDirectory, $"*{StaticLiterals.SolutionFileExtension}", SearchOption.AllDirectories)
                                                  .FirstOrDefault(f => f.EndsWith($"{sourceSolutionFolder}{StaticLiterals.SolutionFileExtension}", StringComparison.CurrentCultureIgnoreCase)) ?? String.Empty;
            var sourceSolutionPath = Path.GetDirectoryName(sourceSolutionFilePath);
            var targetSolutionFolder = new DirectoryInfo(targetSolutionDirectory).Name;
            var targetSolutionPath = targetSolutionDirectory;
            var targetSolutionFilePath = CreateTargetFilePath(sourceSolutionFilePath, sourceSolutionDirectory, targetSolutionDirectory);

            CopySolutionFile(sourceSolutionFilePath, targetSolutionFilePath, sourceSolutionFolder, targetSolutionFolder, sourceProjects);
            CopySolutionFiles(sourceSolutionDirectory, targetSolutionDirectory);
            CopySolutionProjectFiles(sourceSolutionDirectory, targetSolutionDirectory, sourceProjects);
        }
        private static string[] SplitProjectEntry(TagInfo tag)
        {
            var result = new List<string>();
            var removeItems = new[] { " ", "\t" };
            var data = tag.FullText.RemoveAll(removeItems)
                                   .Split($"{Environment.NewLine}")
                                   .Where(e => e.HasContent());

            result.AddRange(data);
            return result.ToArray();
        }
        private static bool IsSolutionEntry(IEnumerable<string> entryItems)
        {
            entryItems.CheckArgument(nameof(entryItems));

            return entryItems.Count() > 1 && entryItems.ElementAt(1).StartsWith("ProjectSection(SolutionItems)");
        }
        private static IEnumerable<string> ConvertSolutionEntry(IEnumerable<string> entryItems)
        {
            entryItems.CheckArgument(nameof(entryItems));

            var result = new List<string>();
            var items = entryItems.ToArray();

            result.Add(items[0]);
            result.Add($"\t{items[1]}");

            for (int i = 2; i < items.Length - 1; i++)
            {
                var item = items[i];

                if (item.Contains('='))
                {
                    var data = item.Split("=");

                    if (SolutionExtenions.Any(e => e.Equals(Path.GetExtension(data[0]))))
                    {
                        result.Add($"\t\t{item}");
                    }
                    else
                    {
                        result.Add(item);
                    }
                }
                else
                {
                    result.Add("\t" + item);
                }
            }
            result.Add(items[^1]);
            return result;
        }
        private static IEnumerable<string> ConvertProjectEntry(IEnumerable<string> entryItems, string sourceSolutionName, string targetSolutionName, IEnumerable<string> sourceProjects)
        {
            entryItems.CheckArgument(nameof(entryItems));
            sourceProjects.CheckArgument(nameof(sourceProjects));

            var result = new List<string>();
            var items = entryItems.ToArray();
            var regex = new Regex(sourceSolutionName, RegexOptions.IgnoreCase);

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];

                if (item.StartsWith("Project("))
                {
                    var data = item.Split(new string[] { "=", "," }, StringSplitOptions.None);

                    if (data.Length > 1 && sourceProjects.Any(e => e.Equals(data[1].RemoveAll("\""))))
                    {
                        result.Add("Project(\"{" + Guid.NewGuid().ToString().ToUpper() + "}\") = ");
                        for (int j = 1; j < data.Length; j++)
                        {
                            result[^1] = $"{result[^1]}{(j > 1 ? ", " : string.Empty)}" + $"{regex.Replace(data[j], targetSolutionName)}";
                        }
                        result.Add("EndProject");
                    }
                }
            }
            return result;
        }
        private static void CopySolutionFile(string solutionSourceFilePath, string targetSolutionFilePath, string sourceSolutionName, string targetSolutionName, IEnumerable<string> sourceProjects)
        {
            var targetText = new StringBuilder();
            var targetLines = new List<string>();
            var sourceText = File.ReadAllText(solutionSourceFilePath, Encoding.Default);
            var projectTags = sourceText.GetAllTags(new string[] { "Project(", $"EndProject" });

            if (projectTags.Any())
            {
                targetText.Append(sourceText[..projectTags.First().StartTagIndex]);
                foreach (var tag in projectTags)
                {
                    var entryItems = SplitProjectEntry(tag);

                    if (IsSolutionEntry(entryItems))
                    {
                        targetLines.AddRange(ConvertSolutionEntry(entryItems));
                    }
                    else // it is a project entry
                    {
                        targetLines.AddRange(ConvertProjectEntry(entryItems, sourceSolutionName, targetSolutionName, sourceProjects));
                    }
                }
                targetText.Append(targetLines.ToText());

                var globalTags = sourceText[projectTags.Last().EndIndex..]
                                                       .GetAllTags("GlobalSection(", "EndGlobalSection");

                targetText.AppendLine("Global");
                foreach (var tag in globalTags)
                {
                    if (tag.FullText.Contains("GlobalSection(ProjectConfigurationPlatforms) = postSolution"))
                    {
                        var data = tag.FullText.Split(Environment.NewLine);

                        if (data.Any())
                            targetText.AppendLine($"\t{data[0]}");

                        for (int i = 1; i < data.Length - 1; i++)
                        {
                            var guid = data[i].Partialstring("{", "}");

                            if (targetLines.Any(e => e.Contains(guid)))
                            {
                                targetText.AppendLine($"{data[i]}");
                            }
                        }

                        if (data.Any())
                            targetText.AppendLine($"{data[^1]}");
                    }
                    else
                    {
                        targetText.Append('\t');
                        targetText.AppendLine(tag.FullText);
                    }
                }
                targetText.AppendLine("EndGlobal");
            }
            File.WriteAllText(targetSolutionFilePath, targetText.ToString(), Encoding.Default);
        }
        private void CopySolutionFiles(string sourceSolutionDirectory, string targetSolutionDirectory)
        {
            var sourceSolutionFolder = new DirectoryInfo(sourceSolutionDirectory).Name;
            var targetSolutionFolder = new DirectoryInfo(targetSolutionDirectory).Name;

            foreach (var sourceFile in new DirectoryInfo(sourceSolutionDirectory).GetFiles("*", SearchOption.TopDirectoryOnly)
                                                                                 .Where(f => SolutionExtenions.Any(e => e.Equals(f.Extension, StringComparison.CurrentCultureIgnoreCase))))
            {
                var targetFilePath = CreateTargetFilePath(sourceFile.FullName, sourceSolutionDirectory, targetSolutionDirectory);

                CopyFile(sourceFile.FullName, targetFilePath, sourceSolutionFolder, targetSolutionFolder);
            }
        }

        private void CopySolutionProjectFiles(string sourceSolutionDirectory, string targetSolutionDirectory, IEnumerable<string> sourceProjects)
        {
            var projectFilePath = string.Empty;
            var sourceSolutionFolder = new DirectoryInfo(sourceSolutionDirectory).Name;
            var targetSolutionFolder = new DirectoryInfo(targetSolutionDirectory).Name;

            foreach (var sourceFile in new DirectoryInfo(sourceSolutionDirectory).GetFiles($"*{StaticLiterals.ProjectFileExtension}", SearchOption.AllDirectories))
            {
                var directoryName = sourceFile.DirectoryName?.ToLower();

                if (sourceProjects.Any(e => directoryName != null && directoryName.EndsWith(e.ToLower())))
                {
                    var targetFilePath = CreateTargetFilePath(sourceFile.FullName, sourceSolutionDirectory, targetSolutionDirectory);

                    CopyFile(sourceFile.FullName, targetFilePath, sourceSolutionFolder, targetSolutionFolder);
                }
            }
            if (string.IsNullOrEmpty(projectFilePath) == false)
            {
                ReplaceProjectGuids(projectFilePath);
            }
        }
        private void CopyProjectDirectoryWorkFiles(string sourceDirectory, string sourceSolutionDirectory, string targetSolutionDirectory)
        {
            var projectFilePath = string.Empty;
            var sourceSolutionFolder = new DirectoryInfo(sourceSolutionDirectory).Name;
            var targetSolutionFolder = new DirectoryInfo(targetSolutionDirectory).Name;
            var sourceFiles = new DirectoryInfo(sourceDirectory).GetFiles("*", SearchOption.AllDirectories)
                                                                .Where(f => IgnoreFileFolders.Any(i => f.FullName.ToLower().Contains(i.ToLower())) == false
                                                                         && (f.Name.Equals("dockerfile", StringComparison.CurrentCultureIgnoreCase) || ProjectExtensions.Any(i => i.Equals(Path.GetExtension(f.Name)))));

            foreach (var sourceFile in sourceFiles)
            {
                var targetFilePath = CreateTargetFilePath(sourceFile.FullName, sourceSolutionDirectory, targetSolutionDirectory);

                CopyFile(sourceFile.FullName, targetFilePath, sourceSolutionFolder, targetSolutionFolder);
            }
        }
        private void CopyFile(string sourceFilePath, string targetFilePath, string sourceSolutionName, string targetSolutionName)
        {
            var extension = Path.GetExtension(sourceFilePath);
            var targetDirectory = Path.GetDirectoryName(targetFilePath);

            if (Extensions.SingleOrDefault(i => i.Equals(extension, StringComparison.CurrentCultureIgnoreCase)) == null)
            {
                Extensions.Add(extension);
            }

            if (targetDirectory != null && Directory.Exists(targetDirectory) == false)
            {
                Directory.CreateDirectory(targetDirectory);
            }

            if (sourceFilePath.EndsWith(DockerfileName, StringComparison.CurrentCultureIgnoreCase))
            {
                var sourceLines = File.ReadAllLines(sourceFilePath, Encoding.Default);
                var targetLines = sourceLines.Select(l => l.Replace(sourceSolutionName, targetSolutionName));

                File.WriteAllLines(targetFilePath, targetLines.ToArray(), Encoding.Default);
            }
            else if (sourceFilePath.EndsWith(DockerComposefileName, StringComparison.CurrentCultureIgnoreCase))
            {
                var sourceLines = File.ReadAllLines(sourceFilePath, Encoding.Default);
                var targetLines = sourceLines.Select(l => l.Replace(sourceSolutionName, targetSolutionName))
                                             .Select(l => l.Replace(sourceSolutionName.ToLower(), targetSolutionName.ToLower()));

                File.WriteAllLines(targetFilePath, targetLines.ToArray(), Encoding.Default);
            }
            else if (ReplaceExtensions.SingleOrDefault(i => i.Equals(extension, StringComparison.CurrentCultureIgnoreCase)) != null)
            {
                var targetLines = new List<string>();
                var sourceLines = File.ReadAllLines(sourceFilePath, Encoding.Default);
                var regex = new Regex(sourceSolutionName, RegexOptions.IgnoreCase);

                if (sourceFilePath.EndsWith("BlazorApp.csproj"))
                {
                    for (int i = 0; i < sourceLines.Length; i++)
                    {
                        var sourceLine = sourceLines[i];

                        if (sourceLine.TrimStart().StartsWith("<UserSecretsId>"))
                        {
                            sourceLine = $"    <UserSecretsId>{Guid.NewGuid()}</UserSecretsId>";
                            sourceLines[i] = sourceLine;
                        }
                    }
                }

                if (sourceLines.Any()
                    && sourceLines.First().Contains(StaticLiterals.IgnoreLabel) == false
                    && sourceLines.First().Contains(StaticLiterals.GeneratedCodeLabel) == false)
                {
                    foreach (var sourceLine in sourceLines)
                    {
                        var targetLine = regex.Replace(sourceLine, targetSolutionName);

                        targetLine = targetLine.Replace(StaticLiterals.BaseCodeLabel, StaticLiterals.CodeCopyLabel);
                        targetLines.Add(targetLine);
                    }
                    File.WriteAllLines(targetFilePath, targetLines.ToArray(), Encoding.UTF8);
                }
            }
            else if (File.Exists(targetFilePath) == false)
            {
                File.Copy(sourceFilePath, targetFilePath);
            }
        }

        private static string CreateTargetFilePath(string sourceFilePath, string sourceSolutionDirectory, string targetSolutionDirectory)
        {
            var result = targetSolutionDirectory;
            var sourceSolutionFolder = new DirectoryInfo(sourceSolutionDirectory).Name;
            var targetSolutionFolder = new DirectoryInfo(targetSolutionDirectory).Name;
            var subSourceFilePath = sourceFilePath.Replace(sourceSolutionDirectory, string.Empty);

            foreach (var item in subSourceFilePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrEmpty(item) == false)
                {
                    result = Path.Combine(result, item.Replace(sourceSolutionFolder, targetSolutionFolder));
                }
            }
            return result;
        }
        private void ReplaceProjectGuids(string filePath)
        {
            var xml = new XmlDocument();

            xml.Load(filePath);

            if (xml.DocumentElement != null)
            {
                foreach (XmlNode node in xml.DocumentElement.ChildNodes)
                {
                    // first node is the url ... have to go to nexted loc node
                    foreach (XmlNode item in node)
                    {
                        if (item.Name.Equals("ProjectGuid") == true)
                        {
                            string newGuid = Guid.NewGuid().ToString().ToUpper();

                            ProjectGuids.Add($"{item.InnerText}{Separator}{newGuid}");

                            item.InnerText = "{" + newGuid + "}";
                        }
                    }
                }
            }
            xml.Save(filePath);
        }
    }
}
