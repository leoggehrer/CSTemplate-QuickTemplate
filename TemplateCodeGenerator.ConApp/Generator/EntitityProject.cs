//MdStart
using System.Reflection;
using System.Runtime.Loader;

namespace TemplateCodeGenerator.ConApp.Generator
{
    internal partial class EntitityProject
    {
        public SolutionProperties SolutionProperties { get; private set; }
        public string ProjectName => $"{SolutionProperties.SolutionName}{SolutionProperties.LogicPostfix}";
        public string ProjectPath => Path.Combine(SolutionProperties.SolutionPath, ProjectName);

        private EntitityProject(SolutionProperties solutionProperties)
        {
            SolutionProperties = solutionProperties;
        }
        public static EntitityProject Create(SolutionProperties solutionProperties)
        {
            return new(solutionProperties);
        }

        private IEnumerable<Type>? assemblyTypes;
        public IEnumerable<Type> AssemblyTypes
        {
            get
            {
                if (assemblyTypes == null)
                {
                    if (SolutionProperties.LogicAssemblyFilePath.HasContent() && File.Exists(SolutionProperties.LogicAssemblyFilePath))
                    {
                        assemblyTypes = AssemblyLoadContext.Default
                                                           .LoadFromAssemblyPath(SolutionProperties.LogicAssemblyFilePath)
                                                           .GetTypes();
                    }
                }
                return assemblyTypes ?? Array.Empty<Type>();
            }
        }

        public IEnumerable<Type> EnumTypes => AssemblyTypes.Where(t => t.IsEnum);
        public IEnumerable<Type> InterfaceTypes => AssemblyTypes.Where(t => t.IsInterface);
        public IEnumerable<Type> EntityTypes => AssemblyTypes.Where(t => t.IsClass
                                                                      && t.Namespace != null 
                                                                      && t.Namespace!.Contains($".{StaticLiterals.EntitiesFolder}")
                                                                      && t.Name.Equals(StaticLiterals.IdentityEntityName) == false
                                                                      && t.Name.Equals(StaticLiterals.VersionEntityName) == false);
    }
}
//MdEnd