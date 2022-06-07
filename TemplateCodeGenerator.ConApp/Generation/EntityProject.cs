//MdStart
using System.Runtime.Loader;
using TemplateCodeGenerator.ConApp.Contracts;

namespace TemplateCodeGenerator.ConApp.Generation
{
    internal partial class EntityProject
    {
        public ISolutionProperties SolutionProperties { get; private set; }
        public string ProjectName => $"{SolutionProperties.SolutionName}{SolutionProperties.LogicPostfix}";
        public string ProjectPath => Path.Combine(SolutionProperties.SolutionPath, ProjectName);

        private EntityProject(ISolutionProperties solutionProperties)
        {
            SolutionProperties = solutionProperties;
        }
        public static EntityProject Create(ISolutionProperties solutionProperties)
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
                        var assembly = AssemblyLoadContext.Default
                                                           .LoadFromAssemblyPath(SolutionProperties.LogicAssemblyFilePath);
                        if (assembly != null)
                        {
                            assemblyTypes = assembly.GetTypes();
                        }
                    }
                }
                return assemblyTypes ?? Array.Empty<Type>();
            }
        }

        public IEnumerable<Type> EnumTypes => AssemblyTypes.Where(t => t.IsEnum);
        public IEnumerable<Type> InterfaceTypes => AssemblyTypes.Where(t => t.IsInterface);
        public IEnumerable<Type> EntityTypes => AssemblyTypes.Where(t => t.IsClass
                                                                      && t.IsNested == false
                                                                      && t.Namespace != null 
                                                                      && t.Namespace!.Contains($".{StaticLiterals.EntitiesFolder}")
                                                                      && t.Name.Equals(StaticLiterals.IdentityEntityName) == false
                                                                      && t.Name.Equals(StaticLiterals.VersionEntityName) == false);
    }
}
//MdEnd