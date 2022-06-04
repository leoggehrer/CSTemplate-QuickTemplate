//@BaseCode
//MdStart
using System.Collections.Generic;

namespace TemplateCodeGenerator.ConApp.Contracts
{
    public interface ISolutionProperties
    {
        string SolutionPath { get; }
        string SolutionName { get; }
        string SolutionFilePath { get; }

        IEnumerable<string> ProjectNames { get; }

        string LogicCSProjectFilePath { get; }
        string LogicAssemblyFilePath { get; }
        string LogicProjectName { get; }
        string LogicSubPath { get; }
        string LogicControllersSubPath { get; }
        string LogicDataContextSubPath { get; }
        string LogicEntitiesSubPath { get; }

        string WebApiProjectName { get; }
        string WebApiSubPath { get; }
        string WebApiControllersSubPath { get; }

        string AspMvcAppProjectName { get; }
        string AspMvcAppSubPath { get; }
        string AspMvcControllersSubPath { get; }
    }
}
//MdEnd