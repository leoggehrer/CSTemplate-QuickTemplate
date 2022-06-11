//MdStart
using System.Collections.Concurrent;
using System.Text;
using TemplateCodeGenerator.ConApp.Contracts;

namespace TemplateCodeGenerator.ConApp
{
    public static partial class Generator
    {
        public static IEnumerable<IGeneratedItem> Generate(string solutionPath)
        {
            ISolutionProperties solutionProperties = Generation.SolutionProperties.Create(solutionPath);

            return Generate(solutionProperties);
        }
        public static IEnumerable<IGeneratedItem> Generate(ISolutionProperties solutionProperties)
        {
            var result = new ConcurrentBag<IGeneratedItem>();
            var logicGenerator = new Generation.LogicGenerator(solutionProperties);
            var webApiGenerator = new Generation.WebApiGenerator(solutionProperties);
            var aspMvcGenerator = new Generation.AspMvcGenerator(solutionProperties);
            var tasks = new List<Task>();

            #region Logic
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var generatedItems = new List<IGeneratedItem>();

                Console.WriteLine("Create Logic-Modles...");
                generatedItems.AddRange(logicGenerator.GenerateAll());
                result.AddRangeSafe(generatedItems);
            }));
            #endregion Logic

            #region WebApi
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var generatedItems = new List<IGeneratedItem>();

                Console.WriteLine("Create WebApi-Modles...");
                generatedItems.AddRange(webApiGenerator.GenerateAll());
                result.AddRangeSafe(generatedItems);
            }));
            #endregion WebApi

            #region AspMvc
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var generatedItems = new List<IGeneratedItem>();

                Console.WriteLine("Create AspMvc-Modles...");
                generatedItems.AddRange(aspMvcGenerator.GenerateAll());
                result.AddRangeSafe(generatedItems);
            }));
            #endregion AspMvc

            Task.WaitAll(tasks.ToArray());
            return result;
        }

        public static void DeleteGenerationFiles(string path)
        {
            Console.WriteLine("Delete all generation files...");
            foreach (var searchPattern in StaticLiterals.SourceFileExtensions.Split("|"))
            {
                var deleteFiles = GetGenerationFiles(path, searchPattern, new[] { StaticLiterals.GeneratedCodeLabel });

                foreach (var item in deleteFiles)
                {
                    File.Delete(item);
                }
            }
        }
        private static IEnumerable<string> GetGenerationFiles(string path, string searchPattern, string[] labels)
        {
            var result = new List<string>();

            foreach (var file in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
            {
                var lines = File.ReadAllLines(file, Encoding.Default);

                if (lines.Any() && labels.Any(l => lines.First().Contains(l)))
                {

                    result.Add(file);
                }
            }
            return result;
        }
    }
}
//MdEnd