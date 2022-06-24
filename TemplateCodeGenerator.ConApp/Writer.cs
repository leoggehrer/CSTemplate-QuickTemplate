//MdStart
using System.Text;
using TemplateCodeGenerator.ConApp.Common;
using TemplateCodeGenerator.ConApp.Contracts;

namespace TemplateCodeGenerator.ConApp
{
    internal partial class Writer
    {
        #region Class-Constructors
        static Writer()
        {
            ClassConstructing();
            ClassConstructed();
        }
        static partial void ClassConstructing();
        static partial void ClassConstructed();
        #endregion Class-Constructors
        public static bool WriteToGroupFile { get; set; } = false;
        public static void WriteAll(string solutionPath, ISolutionProperties solutionProperties, IEnumerable<IGeneratedItem> generatedItems)
        {
            var tasks = new List<Task>();

            #region WriteLogicItems
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.DbContext));

                Console.WriteLine("Write Logic-DataContext...");
                WriteItems(projectPath, writeItems);
            })));
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.Model));

                Console.WriteLine("Write Logic-Models...");
                WriteItems(projectPath, writeItems);
            })));
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.AccessContract));

                Console.WriteLine("Write Logic-Access-Contracts...");
                WriteItems(projectPath, writeItems);
            })));
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.Controller));

                Console.WriteLine("Write Logic-Controllers...");
                WriteItems(projectPath, writeItems);
            })));
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.FacadeContract));

                Console.WriteLine("Write Logic-Facade-Contracts...");
                WriteItems(projectPath, writeItems);
            })));
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.Facade));

                Console.WriteLine("Write Logic-Facades...");
                WriteItems(projectPath, writeItems);
            })));
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.Factory));

                Console.WriteLine("Write Logic-Factory...");
                WriteItems(projectPath, writeItems);
            })));
            #endregion WriteLogicItems

            #region WriteWebApiModels
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.WebApiProjectName);
                var writeItems = generatedItems.Where(e => e.UnitType == UnitType.WebApi && e.ItemType == ItemType.Model);

                Console.WriteLine("Write WebApi-Models...");
                WriteItems(projectPath, writeItems);
            }));
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.WebApiProjectName);
                var writeItems = generatedItems.Where(e => e.UnitType == UnitType.WebApi && e.ItemType == ItemType.EditModel);

                Console.WriteLine("Write WebApi-EditModels...");
                WriteItems(projectPath, writeItems);
            }));
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.WebApiProjectName);
                var writeItems = generatedItems.Where(e => e.UnitType == UnitType.WebApi && e.ItemType == ItemType.Controller);

                Console.WriteLine("Write WebApi-Controllers...");
                WriteItems(projectPath, writeItems);
            }));
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.WebApiProjectName);
                var writeItems = generatedItems.Where(e => e.UnitType == UnitType.WebApi && e.ItemType == ItemType.AddServices);

                Console.WriteLine("Write WebApi-AddServices...");
                WriteItems(projectPath, writeItems);
            }));
            #endregion WriteWebApiModels

            #region WriteAspMvcModels
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.AspMvcAppProjectName);
                var writeItems = generatedItems.Where(e => e.UnitType == UnitType.AspMvc && e.ItemType == ItemType.Model);

                Console.WriteLine("Write AspMvc-Models...");
                WriteItems(projectPath, writeItems);
            }));
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.AspMvcAppProjectName);
                var writeItems = generatedItems.Where(e => e.UnitType == UnitType.AspMvc && e.ItemType == ItemType.FilterModel);

                Console.WriteLine("Write AspMvc-FilterModels...");
                WriteItems(projectPath, writeItems);
            }));
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.AspMvcAppProjectName);
                var writeItems = generatedItems.Where(e => e.UnitType == UnitType.AspMvc && e.ItemType == ItemType.Controller);

                Console.WriteLine("Write AspMvc-Controllers...");
                WriteItems(projectPath, writeItems);
            }));
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.AspMvcAppProjectName);
                var writeItems = generatedItems.Where(e => e.UnitType == UnitType.AspMvc && e.ItemType == ItemType.AddServices);

                Console.WriteLine("Write AspMvc-AddServices...");
                WriteItems(projectPath, writeItems);
            }));
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.AspMvcAppProjectName);
                var writeItems = generatedItems.Where(e => e.UnitType == UnitType.AspMvc && e.ItemType == ItemType.View);

                Console.WriteLine("Write AspMvc-Views...");
                WriteItems(projectPath, writeItems);
            }));
            #endregion WriteAspMvcModels

            Task.WaitAll(tasks.ToArray());
        }

        #region Write methods
        public static void WriteItems(string projectPath, IEnumerable<IGeneratedItem> generatedItems)
        {
            if (WriteToGroupFile)
            {
                WriteGeneratedCodeFile(projectPath, StaticLiterals.GeneratedCodeFileName, generatedItems);
            }
            else
            {
                WriteCodeFiles(projectPath, generatedItems);
            }
        }
        public static void WriteGeneratedCodeFile(string projectPath, string fileName, IEnumerable<IGeneratedItem> generatedItems)
        {
            if (generatedItems.Any())
            {
                var idx = 0;
                var count = generatedItems.Count();
                var subPath = new StringBuilder();
                var subPaths = generatedItems.Select(e => Path.GetDirectoryName(e.SubFilePath));
                var minSubPath = subPaths.MinBy(e => e?.Length);

                minSubPath ??= String.Empty;

                while (idx < minSubPath.Length
                       && count == generatedItems.Where(e => idx < e.SubFilePath.Length && e.SubFilePath[idx] == minSubPath[idx]).Count())
                {
                    subPath.Append(minSubPath[idx++]);
                }

                var fullFilePath = default(string);

                if (subPath.Length == 0)
                    fullFilePath = Path.Combine(projectPath, fileName);
                else
                    fullFilePath = Path.Combine(projectPath, subPath.ToString(), fileName);

                WriteGeneratedCodeFile(fullFilePath, generatedItems);
            }
        }
        public static void WriteGeneratedCodeFile(string fullFilePath, IEnumerable<IGeneratedItem> generatedItems)
        {
            var lines = new List<string>();
            var directory = Path.GetDirectoryName(fullFilePath);

            foreach (var item in generatedItems)
            {
                lines.AddRange(item.SourceCode);
            }

            if (lines.Any())
            {
                var sourceLines = new List<string>(lines);

                if (string.IsNullOrEmpty(directory) == false && Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }

                //sourceLines.Insert(0, $"{StaticLiterals.NullableDisableLabel}");
                sourceLines.Insert(0, $"//{StaticLiterals.GeneratedCodeLabel}");
                File.WriteAllLines(fullFilePath, sourceLines);
            }
        }
        public static void WriteCodeFiles(string projectPath, IEnumerable<IGeneratedItem> generatedItems)
        {
            foreach (var item in generatedItems)
            {
                var sourceLines = new List<string>(item.SourceCode);
                var filePath = Path.Combine(projectPath, item.SubFilePath);

                //sourceLines.Insert(0, $"{StaticLiterals.NullableDisableLabel}");
                if (item.FileExtension == StaticLiterals.CSharpHtmlFileExtension)
                {
                    sourceLines.Insert(0, $"@*{StaticLiterals.GeneratedCodeLabel}*@");
                }
                else
                {
                    sourceLines.Insert(0, $"//{StaticLiterals.GeneratedCodeLabel}");
                }
                WriteCodeFile(filePath, sourceLines);
            }
        }
        public static void WriteCodeFile(string filePath, IEnumerable<string> source)
        {
            var canCreate = true;
            var path = Path.GetDirectoryName(filePath);
            var generatedCode = StaticLiterals.GeneratedCodeLabel;

            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                var header = lines.FirstOrDefault(l => l.Contains(StaticLiterals.GeneratedCodeLabel)
                                  || l.Contains(StaticLiterals.CustomizedAndGeneratedCodeLabel));

                if (header != null)
                {
                    File.Delete(filePath);
                }
                else
                {
                    canCreate = false;
                }
            }
            else if (string.IsNullOrEmpty(path) == false && Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            if (canCreate && source.Any())
            {
                File.WriteAllLines(filePath, source);
            }
        }
        #endregion Write methods
    }
}
//MdEnd