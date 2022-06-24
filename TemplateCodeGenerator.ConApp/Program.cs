using System.Diagnostics;
using TemplateCodeGenerator.ConApp.Generation;

namespace TemplateCodeGenerator.ConApp
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
            TargetPaths = Array.Empty<string>();
            ClassConstructed();
        }
        static partial void ClassConstructing();
        static partial void ClassConstructed();

        #region Properties
        private static string? HomePath { get; set; }
        private static string UserPath { get; set; }
        private static string SourcePath { get; set; }
        private static string[] TargetPaths { get; set; }
        private static string[] SearchPatterns => StaticLiterals.SourceFileExtensions.Split('|');
        #endregion Properties

        static void Main(/*string[] args*/)
        {
            RunApp();
        }

        private static void TestForecolor()
        {
            var savecolor = Console.ForegroundColor;
            var first = Console.ForegroundColor.FirstEnum();
            var run = first;

            do
            {
                Console.ForegroundColor = savecolor;
                Console.Write($"Color - {run,-14}: ");
                Console.ForegroundColor = run;
                Console.WriteLine("Hallo, das ist ein color Test!");
                run = run.NextEnum();
            } while (first != run);
        }

        #region Console methods
        private static readonly bool canBusyPrint = true;
        private static bool runBusyProgress = false;
        private static void RunApp()
        {
            var input = string.Empty;
            var saveForeColor = Console.ForegroundColor;

            while (input.Equals("x") == false)
            {
                var menuIndex = 0;
                var maxWaiting = 10 * 60 * 1000;    // 10 minutes
                var sourceSolutionName = GetSolutionNameByPath(SourcePath);

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Template Code Generator");
                Console.WriteLine("=======================");
                Console.WriteLine();
                Console.WriteLine($"Code generation for '{sourceSolutionName}' from: {SourcePath}");
                Console.WriteLine();
                Console.WriteLine($"[{++menuIndex}] Change source path");
                Console.WriteLine($"[{++menuIndex}] Compile solution...");
                Console.WriteLine($"[{++menuIndex}] Compile logic project...");
                Console.WriteLine($"[{++menuIndex}] Delete generation files...");
                Console.WriteLine($"[{++menuIndex}] Start code generation...");
                Console.WriteLine("[x|X] Exit");
                Console.WriteLine();
                Console.Write("Choose: ");

                input = Console.ReadLine()?.ToLower() ?? String.Empty;
                Console.ForegroundColor = saveForeColor;
                if (Int32.TryParse(input, out var select))
                {
                    var solutionProperties = SolutionProperties.Create(SourcePath);

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
                    if (select == 2)
                    {
                        var counter = 0;
                        var startCompilePath = Path.Combine(Path.GetTempPath(), solutionProperties.SolutionName);
                        var compilePath = startCompilePath;
                        bool deleteError;

                        do
                        {
                            deleteError = false;
                            if (Directory.Exists(compilePath))
                            {
                                try
                                {
                                    Directory.Delete(compilePath, true);
                                }
                                catch
                                {
                                    deleteError = true;
                                    compilePath = $"{startCompilePath}{++counter}";
                                }
                            }
                        } while (deleteError != false);

                        var arguments = $"build \"{solutionProperties.SolutionFilePath}\" -c Release -o {compilePath}";
                        Console.WriteLine(arguments);
                        Debug.WriteLine($"dotnet.exe {arguments}");

                        var csprojStartInfo = new ProcessStartInfo("dotnet.exe")
                        {
                            Arguments = arguments,
                            //WorkingDirectory = projectPath,
                            UseShellExecute = false
                        };
                        Process.Start(csprojStartInfo)?.WaitForExit(maxWaiting);
                        solutionProperties.CompilePath = compilePath;
                        if (select == 2)
                        {
                            Console.Write("Press any key ");
                            Console.ReadKey();
                        }
                    }
                    if (select == 3 || select == 5)
                    {
                        var counter = 0;
                        var startCompilePath = Path.Combine(Path.GetTempPath(), solutionProperties.SolutionName);
                        var compilePath = startCompilePath;
                        bool deleteError;

                        do
                        {
                            deleteError = false;
                            if (Directory.Exists(compilePath))
                            {
                                try
                                {
                                    Directory.Delete(compilePath, true);
                                }
                                catch
                                {
                                    deleteError = true;
                                    compilePath = $"{startCompilePath}{++counter}";
                                }
                            }
                        } while (deleteError != false);

                        var arguments = $"build \"{solutionProperties.LogicCSProjectFilePath}\" -c Release -o {compilePath}";
                        Console.WriteLine(arguments);
                        Debug.WriteLine($"dotnet.exe {arguments}");

                        var csprojStartInfo = new ProcessStartInfo("dotnet.exe")
                        {
                            Arguments = arguments,
                            //WorkingDirectory = projectPath,
                            UseShellExecute = false
                        };
                        Process.Start(csprojStartInfo)?.WaitForExit(maxWaiting);
                        solutionProperties.CompilePath = compilePath;
                        if (select == 2)
                        {
                            Console.Write("Press any key ");
                            Console.ReadKey();
                        }
                    }
                    if (select == 4)
                    {
                        Generator.DeleteGenerationFiles(SourcePath);
                    }
                    if (select == 5)
                    {
                        var generatedItems = Generator.Generate(solutionProperties);

                        Generator.DeleteGenerationFiles(SourcePath);
                        Writer.WriteAll(SourcePath, solutionProperties, generatedItems);
                    }
                }
            }
        }
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
        private static void PrintHeader(string sourcePath, string[] targetPaths)
        {
            var index = 0;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Template Code Generator");
            Console.WriteLine("=======================");
            Console.WriteLine();
            Console.WriteLine($"Source: {sourcePath}");
            Console.WriteLine();
            foreach (var target in targetPaths)
            {
                Console.WriteLine($"   Generation for: [{++index,2}] {target}");
            }
            Console.WriteLine("   Generation for: [ a] ALL");
            Console.WriteLine();

            if (Directory.Exists(sourcePath) == false)
            {
                Console.WriteLine($"Source-Path '{sourcePath}' not exists");
            }
            foreach (var item in targetPaths)
            {
                if (Directory.Exists(item) == false)
                {
                    Console.WriteLine($"   Target-Path '{item}' not exists");
                }
            }
            Console.WriteLine();
        }
        #endregion Console methods

        #region Helpers
        private static string GetCurrentSolutionPath()
        {
            int endPos = AppContext.BaseDirectory
                                   .IndexOf($"{nameof(TemplateCodeGenerator)}", StringComparison.CurrentCultureIgnoreCase);

            return AppContext.BaseDirectory[..endPos];
        }
        private static string GetSolutionNameByPath(string solutionPath)
        {
            return solutionPath.Split(new char[] { '\\', '/' })
                               .Where(e => string.IsNullOrEmpty(e) == false)
                               .Last();
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
        #endregion Helpers

        #region Partial methods
        static partial void BeforeGetTargetPaths(string sourcePath, List<string> targetPaths, ref bool handled);
        static partial void AfterGetTargetPaths(string sourcePath, List<string> targetPaths);
        #endregion Partial methods
    }
}