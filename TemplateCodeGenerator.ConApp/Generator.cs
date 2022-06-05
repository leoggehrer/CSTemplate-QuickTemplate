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
            var result = new ConcurrentBag<IGeneratedItem>();
            ISolutionProperties solutionProperties = Generation.SolutionProperties.Create(solutionPath);
            var modelGenerator = new Generation.ModelGenerator(solutionProperties);
            var tasks = new List<Task>();

            #region Models
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var generatedItems = new List<IGeneratedItem>();

                Console.WriteLine("Create Logic-Entities...");
                generatedItems.AddRange(modelGenerator.GenerateAll());
                result.AddRangeSafe(generatedItems);
            }));
            #endregion Models


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