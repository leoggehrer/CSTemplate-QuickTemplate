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
        private static readonly string[] SourceLabels = new string[] { StaticLiterals.BaseCodeLabel };
        private static readonly string[] TargetLabels = new string[] { StaticLiterals.CodeCopyLabel };
        #endregion Properties

        static void Main(/*string[] args*/)
        {
            RunApp();
        }

        #region Console methods
        private static readonly bool canBusyPrint = true;
        private static bool runBusyProgress = false;
        private static void RunApp()
        {
            bool running = false;

            do
            {
                var input = string.Empty;
                var handled = false;
                var targetPaths = new List<string>();

                BeforeGetTargetPaths(SourcePath, targetPaths, ref handled);
                if (handled == false)
                {
                    TargetPaths = GetQuickTemplateProjects(SourcePath);
                }
                else
                {
                    TargetPaths = TargetPaths.ToArray();
                }
                PrintHeader(SourcePath, TargetPaths);

                Console.Write($"Generating [1..{TargetPaths.Length}|X...Quit]: ");
                input = Console.ReadLine()?.ToLower();
                PrintBusyProgress();
                running = input?.Equals("x") == false;
                if (running)
                {
                    if (input != null && input.Equals("a"))
                    {
                        foreach (var item in TargetLabels)
                        {
                            //BalancingSolutions(SourcePath, SourceLabels, TargetPaths, TargetLabels);
                        }
                    }
                    else
                    {
                        var numbers = input?.Trim()
                                            .Split(',').Where(s => Int32.TryParse(s, out int n))
                                            .Select(s => Int32.Parse(s))
                                            .ToArray();

                        foreach (var number in numbers ?? Array.Empty<int>())
                        {
                            if (number == TargetPaths.Length + 1)
                            {
                                foreach (var item in TargetLabels)
                                {
                                    //BalancingSolutions(SourcePath, SourceLabels, TargetPaths, TargetLabels);
                                }
                            }
                            else if (number > 0 && number <= TargetPaths.Length)
                            {
                                //BalancingSolutions(SourcePath, SourceLabels, new string[] { TargetPaths[number - 1] }, TargetLabels);
                            }
                        }
                    }
                }
                runBusyProgress = false;
            } while (running);
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
        private static string[] GetQuickTemplateProjects(string sourcePath)
        {
            var directoryInfo = new DirectoryInfo(sourcePath);
            var parentDirectory = directoryInfo.Parent != null ? directoryInfo.Parent.FullName : SourcePath;
            var qtDirectories = Directory.GetDirectories(parentDirectory, "QT*", SearchOption.AllDirectories)
                                         .Where(d => d.Replace(UserPath, String.Empty).Contains('.') == false)
                                         .ToList();
            return qtDirectories.ToArray();
        }
        #endregion

        #region Partial methods
        static partial void BeforeGetTargetPaths(string sourcePath, List<string> targetPaths, ref bool handled);
        static partial void AfterGetTargetPaths(string sourcePath, List<string> targetPaths);
        #endregion Partial methods
    }
}