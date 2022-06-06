//MdStart
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace TemplateCodeGenerator.ConApp.Git
{
    public partial class GitIgnoreManager
    {
        private static string[] Paths { get; set; } = Array.Empty<string>();
        private static string[] SearchPatterns => StaticLiterals.SourceFileExtensions.Split('|');
        private static string GitIgnoreFile => ".gitignore";

        private static string BeginGitIgnoreBlock => "#QnSCodeIgnoreStart";
        private static string EndGitIgnoreBlock => "#QnSCodeIgnoreEnd";

        public static void Run(string exlusionDirectory)
        {
            BuildPaths(exlusionDirectory);

            foreach (var path in Paths)
            {
                var gitIgnoreResult = new List<string>();

                // Delete all generated files
                Parallel.ForEach(SearchPatterns, searchPattern =>
                {
                    var sourceFiles = GetSourceCodeFiles(path, searchPattern, new string[] { StaticLiterals.GeneratedCodeLabel });
                    var gitIngoredFiles = GetGitIgnoredEntries(sourceFiles, path);

                    lock (gitIgnoreResult)
                    {
                        gitIgnoreResult.AddRange(gitIngoredFiles);
                    }

                });
                WriteGitIgnoreFiles(gitIgnoreResult, path);
            }
        }

        private static void BuildPaths(string exlusionDirectory)
        {
            var cp = Environment.CurrentDirectory.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            var sp = cp.TakeTo(x => x.Equals(exlusionDirectory));

            // Set the solution Path
            Paths = new string[] { Path.Combine(sp.ToArray()) };
        }
        private static IEnumerable<string> GetSourceCodeFiles(string path, string searchPattern, string[] labels)
        {
            var result = new ConcurrentBag<string>();
            var files = Directory.EnumerateFiles(path, searchPattern, SearchOption.AllDirectories);

            Parallel.ForEach(files, file =>
            {
                using var streamReader = new StreamReader(file, Encoding.Default);
                var line = streamReader.ReadLine();

                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (labels.Any(l => line.Contains(l)))
                    {
                        result.Add(file);
                    }
                }
            });
            return result;
        }
        private static IEnumerable<string> GetGitIgnoredEntries(IEnumerable<string> files, string path)
        {
            var result = new List<string>();

            foreach (var file in files)
            {
                var gitIgnoreFile = file.Replace(path, "");

                if (!string.IsNullOrWhiteSpace(gitIgnoreFile))
                {
                    result.Add(gitIgnoreFile.Replace(@"\", "/"));
                }
            }
            return result;
        }
        private static void WriteGitIgnoreFiles(IEnumerable<string> files, string path)
        {
            var gitIgnoreFilePath = Path.Combine(path, GitIgnoreFile);

            if (File.Exists(gitIgnoreFilePath))
            {
                var lines = File.ReadAllLines(gitIgnoreFilePath);
                var start = 0;
                var end = 0;

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];

                    if (line == BeginGitIgnoreBlock)
                    {
                        start = i;
                    }

                    if (start > 0 && line == EndGitIgnoreBlock)
                    {
                        end = i;
                    }
                }

                var result = new List<string>();

                if (end > start && start > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i < start || i > end)
                        {
                            result.Add(lines[i]);
                        }
                        else
                        {
                            i = end;
                            result.Add(BeginGitIgnoreBlock);
                            result.AddRange(files);
                            result.Add(EndGitIgnoreBlock);
                        }
                    }
                }
                else
                {
                    result.AddRange(lines);
                    result.Add(BeginGitIgnoreBlock);
                    result.AddRange(files);
                    result.Add(EndGitIgnoreBlock);
                }

                File.WriteAllLines(gitIgnoreFilePath, result);

                var processInfo = new ProcessStartInfo();
                var anyCommand = "git rm -r --cached .";

                processInfo.UseShellExecute = true;
                processInfo.WorkingDirectory = path;
                processInfo.FileName = @"C:\Windows\System32\cmd.exe";
                processInfo.Arguments = anyCommand;
                processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(processInfo);
            }
        }
    }
}
//MdEnd