using System.Diagnostics;

namespace TemplateCopier.ConApp
{
    internal partial class Program
    {
        static Program()
        {
            ClassConstructing();
            HomePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                        Environment.OSVersion.Platform == PlatformID.MacOSX)
                       ? Environment.GetEnvironmentVariable("HOME")
                       : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

            UserPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            SourcePath = GetCurrentSolutionPath();
            TargetPath = Directory.GetParent(SourcePath)?.FullName ?? String.Empty;
            ClassConstructed();
        }
        static partial void ClassConstructing();
        static partial void ClassConstructed();

        private static string? HomePath { get; set; }
        private static string UserPath { get; set; }
        private static string SourcePath { get; set; }
        private static string TargetPath { get; set; }
        private static void Main(/*string[] args*/)
        {
            RunApp();
        }

        private static void RunApp()
        {
            var input = string.Empty;
            var targetSolutionName = "TargetSolution";
            var saveForeColor = Console.ForegroundColor;

            while (input.Equals("x") == false)
            {
                var sourceSolutionName = GetSolutionNameByPath(SourcePath);
                var sourceProjects = StaticLiterals.SolutionProjects
                                                   .Concat(StaticLiterals.ProjectExtensions.Select(e => $"{sourceSolutionName}{e}"));

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Template Copier");
                Console.WriteLine("===============");
                Console.WriteLine();
                Console.WriteLine($"Copy '{sourceSolutionName}' from: {SourcePath}");
                Console.WriteLine($"Copy to '{targetSolutionName}':   {Path.Combine(TargetPath, targetSolutionName)}");
                Console.WriteLine();
                Console.WriteLine("[1] Change source path");
                Console.WriteLine("[2] Change target path");
                Console.WriteLine("[3] Change target solution name");
                Console.WriteLine("[4] Start copy process");
                Console.WriteLine("[x|X] Exit");
                Console.WriteLine();
                Console.Write("Choose: ");

                input = Console.ReadLine()?.ToLower() ?? String.Empty;
                Console.ForegroundColor = saveForeColor;
                Console.WriteLine();
                if (Int32.TryParse(input, out var select))
                {
                    if (select == 1)
                    {
                        var solutionPath = GetCurrentSolutionPath();
                        var qtProjects = GetQuickTemplateProjects(solutionPath).Union(new[] { solutionPath }).ToArray();

                        for (int i = 0; i < qtProjects.Length; i++)
                        {
                            if (i == 0)
                                Console.WriteLine();

                            Console.WriteLine($"Change path to: [{i + 1}] {qtProjects[i]}");
                        }
                        Console.WriteLine();
                        Console.Write("Select or enter source path: ");
                        var selectOrPath = Console.ReadLine();

                        if (Int32.TryParse(selectOrPath, out int number))
                        {
                            if ((number - 1) >= 0 && (number - 1) < qtProjects.Length)
                            {
                                SourcePath = qtProjects[number - 1];
                            }
                        }
                        else if (string.IsNullOrEmpty(selectOrPath) == false)
                        {
                            SourcePath = selectOrPath;
                        }
                    }
                    else if (select == 2)
                    {
                        var solutionPath = GetCurrentSolutionPath();
                        var qtProjects = GetQuickTemplateProjects(solutionPath).Union(new[] { solutionPath }).ToArray();
                        var qtPaths = qtProjects.Select(p => GetParentDirectory(p)).Distinct().OrderBy(p => p).ToArray();

                        for (int i = 0; i < qtPaths.Length; i++)
                        {
                            if (i == 0)
                                Console.WriteLine();

                            Console.WriteLine($"Change path to: [{i + 1}] {qtPaths[i]}");
                        }
                        Console.WriteLine();
                        Console.Write("Select or enter target path: ");
                        var selectOrPath = Console.ReadLine();

                        if (Int32.TryParse(selectOrPath, out int number))
                        {
                            if ((number - 1) >= 0 && (number - 1) < qtPaths.Length)
                            {
                                TargetPath = qtPaths[number - 1];
                            }
                        }
                        else if (string.IsNullOrEmpty(selectOrPath) == false)
                        {
                            TargetPath = selectOrPath;
                        }
                    }
                    else if (select == 3)
                    {
                        Console.Write("Enter target solution name: ");
                        targetSolutionName = Console.ReadLine() ?? String.Empty;
                    }
                    else if (select == 4)
                    {
                        var sc = new Copier();
                        var targetSolutionPath = Path.Combine(TargetPath, targetSolutionName);

                        PrintBusyProgress();
                        sc.Copy(SourcePath, targetSolutionPath, sourceProjects);
                        runBusyProgress = false;

                        OpenSolutionFolder(targetSolutionPath);
                    }
                    Console.ResetColor();
                }
            }
        }

        private static readonly bool canBusyPrint = true;
        private static bool runBusyProgress = false;
        private static void PrintBusyProgress()
        {
            Console.WriteLine();
            runBusyProgress = true;
            Task.Factory.StartNew(async () =>
            {
                while (runBusyProgress)
                {
                    if (canBusyPrint)
                    {
                        Console.Write(".");
                    }
                    await Task.Delay(250).ConfigureAwait(false);
                }
            });
        }
        private static string GetParentDirectory(string path)
        {
            var result = Directory.GetParent(path);

            return result != null ? result.FullName : path;
        }
        private static string GetCurrentSolutionPath()
        {
            int endPos = AppContext.BaseDirectory
                                   .IndexOf($"{nameof(TemplateCopier)}", StringComparison.CurrentCultureIgnoreCase);
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
        private static string[] GetQuickTemplateProjects(string sourcePath)
        {
            var directoryInfo = new DirectoryInfo(sourcePath);
            var parentDirectory = directoryInfo.Parent != null ? directoryInfo.Parent.FullName : SourcePath;
            var qtDirectories = Directory.GetDirectories(parentDirectory, "QT*", SearchOption.AllDirectories)
                                         .Where(d => d.Replace(UserPath, String.Empty).Contains('.') == false)
                                         .ToList();
            return qtDirectories.ToArray();
        }

        #region CLI Argument methods
        private static void OpenSolutionFolder(string solutionPath)
        {
            Process.Start(new ProcessStartInfo()
            {
                WorkingDirectory = solutionPath,
                FileName = "explorer",
                Arguments = solutionPath,
                CreateNoWindow = true,
            });
        }
        #endregion

    }
}