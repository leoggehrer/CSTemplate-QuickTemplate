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
            Console.WriteLine(nameof(TemplateCopier));

            var input = string.Empty;
            var targetSolutionName = "TargetSolution";

            while (input.Equals("x") == false)
            {
                var sourceSolutionName = GetSolutionNameByPath(SourcePath);
                var sourceProjects = StaticLiterals.SolutionProjects
                                                   .Concat(StaticLiterals.ProjectExtensions.Select(e => $"{sourceSolutionName}{e}"));

                Console.Clear();
                Console.WriteLine("Solution copier!");
                Console.WriteLine("================");
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
                    Console.Write("Enter target path: ");
                    var path = Console.ReadLine();

                    if (string.IsNullOrEmpty(path) == false)
                    {
                        TargetPath = path;
                    }
                }
                else if (input.Equals("3"))
                {
                    Console.Write("Enter target solution name: ");
                    targetSolutionName = Console.ReadLine() ?? String.Empty;
                }
                else if (input.Equals("4"))
                {
                    var sc = new Copier();

                    PrintBusyProgress();
                    sc.Copy(SourcePath, Path.Combine(TargetPath, targetSolutionName), sourceProjects);
                    runBusyProgress = false;
                }
                Console.ResetColor();
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
    }
}