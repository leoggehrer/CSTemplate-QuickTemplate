//@BaseCode
//MdStart

namespace TemplateCodeGenerator.ConApp.Generator
{
    internal partial class SolutionProperties : Contracts.ISolutionProperties
    {
        #region Project-postfixes
        public static string LogicPostfix => ".Logic";
        public static string WebApiPostfix => ".WebApi";
        public static string AspMvcPostfix => ".AspMvc";
        #endregion Project-postfixes

        public string SolutionPath { get; }
        public string SolutionName { get; }
        public string SolutionFilePath { get; }
        #region ProjectNames
        public IEnumerable<string> ProjectNames => CommonBase.StaticLiterals.ProjectExtensions.Select(e => $"{SolutionName}{e}");

        public string LogicCSProjectFilePath { get; }
        public string LogicAssemblyFilePath { get; }
        public string LogicProjectName => ProjectNames.First(e => e.EndsWith($"{LogicPostfix}"));
        public string LogicSubPath => LogicProjectName;
        public string LogicControllersSubPath => StaticLiterals.ControllersFolder;
        public string LogicEntitiesSubPath => StaticLiterals.EntitiesFolder;
        public string LogicDataContextSubPath => StaticLiterals.DataContextFolder;

        public string WebApiProjectName => ProjectNames.First(e => e.EndsWith($"{WebApiPostfix}"));
        public string WebApiSubPath => WebApiProjectName;
        public string WebApiControllersSubPath => Path.Combine(WebApiSubPath, StaticLiterals.ControllersFolder);

        public string AspMvcAppProjectName => ProjectNames.First(e => e.EndsWith($"{AspMvcPostfix}"));
        public string AspMvcAppSubPath => AspMvcAppProjectName;
        public string AspMvcControllersSubPath => Path.Combine(AspMvcAppSubPath, StaticLiterals.ControllersFolder);
        #endregion ProjectNames

        protected SolutionProperties(string solutionPath)
        {
            SolutionPath = solutionPath;
            SolutionName = GetSolutionName(solutionPath);
            SolutionFilePath = GetSolutionFilePath(solutionPath);
            LogicCSProjectFilePath = GetLogicCSProjectFilePath(solutionPath);
            LogicAssemblyFilePath = GetLogicAssemblyFilePath(solutionPath);
        }

        public static SolutionProperties Create()
        {
            return new SolutionProperties(GetCurrentSolutionPath());
        }
        public static SolutionProperties Create(string solutionPath)
        {
            return new SolutionProperties(solutionPath);
        }

        private static string GetCurrentSolutionPath()
        {
            int endPos = AppContext.BaseDirectory.IndexOf($"{nameof(TemplateCodeGenerator)}", StringComparison.CurrentCultureIgnoreCase);

            return AppContext.BaseDirectory[..endPos];
        }
        private static string GetSolutionName(string solutionPath)
        {
            var fileInfo = new DirectoryInfo(solutionPath).GetFiles().SingleOrDefault(f => f.Extension.Equals(".sln", StringComparison.CurrentCultureIgnoreCase));

            return fileInfo != null ? Path.GetFileNameWithoutExtension(fileInfo.Name) : string.Empty;
        }
        private static string GetSolutionFilePath(string solutionPath)
        {
            var result = default(string);
            var solutionName = GetSolutionName(solutionPath);

            if (Directory.Exists(solutionPath))
            {
                var fileName = $"{solutionName}.sln";
                var fileInfos = new DirectoryInfo(solutionPath).GetFiles(fileName, SearchOption.AllDirectories)
                                                          .Where(f => f.FullName.EndsWith(fileName))
                                                          .OrderByDescending(f => f.LastWriteTime);

                var fileInfo = fileInfos.Where(f => f.FullName.ToLower().Contains("\\ref\\") == false)
                                        .FirstOrDefault();

                if (fileInfo != null)
                {
                    result = fileInfo.FullName;
                }
            }
            return result ?? string.Empty;
        }
        private static string GetLogicCSProjectFilePath(string solutionPath)
        {
            var result = default(string);
            var solutionName = GetSolutionName(solutionPath);
            var projectName = $"{solutionName}{StaticLiterals.LogicExtension}";
            var path = Path.Combine(solutionPath, projectName);

            if (Directory.Exists(path))
            {
                var fileName = $"{projectName}.csproj";
                var fileInfos = new DirectoryInfo(path).GetFiles(fileName, SearchOption.AllDirectories)
                                                          .Where(f => f.FullName.EndsWith(fileName))
                                                          .OrderByDescending(f => f.LastWriteTime);

                var fileInfo = fileInfos.Where(f => f.FullName.ToLower().Contains("\\ref\\") == false)
                                        .FirstOrDefault();

                if (fileInfo != null)
                {
                    result = fileInfo.FullName;
                }
            }
            return result ?? string.Empty;
        }
        private static string GetLogicAssemblyFilePath(string solutionPath)
        {
            var result = default(string);
            var solutionName = GetSolutionName(solutionPath);
            var projectName = $"{solutionName}{StaticLiterals.LogicExtension}";
            var binPath = Path.Combine(solutionPath, projectName, "bin");

            if (Directory.Exists(binPath))
            {
                var fileName = $"{projectName}.dll";
                var fileInfos = new DirectoryInfo(binPath).GetFiles(fileName, SearchOption.AllDirectories)
                                                          .Where(f => f.FullName.EndsWith(fileName))
                                                          .OrderByDescending(f => f.LastWriteTime);

                var fileInfo = fileInfos.Where(f => f.FullName.ToLower().Contains("\\ref\\") == false)
                                        .FirstOrDefault();

                if (fileInfo != null)
                {
                    result = fileInfo.FullName;
                }
            }
            return result ?? string.Empty;
        }
    }
}
//MdEnd