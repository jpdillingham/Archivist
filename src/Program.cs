using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivist
{
    class Program
    {
        [Argument('d', "directory", "the directory to analyze")]
        private static string Root { get; set; }

        [Argument('n', "name", "the name of the output file")]
        private static string Name { get; set; } = "files";

        [Argument('r', "repo", "the path to the output repository")]
        private static string Repository { get; set; } = @"C:\Users\Desktop\JP.WHATNET\Desktop\archivist";

        [Argument('c', "created", "show creation time")]
        private static bool Created { get; set; }

        [Argument('m', "modified", "show modified time")]
        private static bool Modified { get; set; }

        [Argument('k', "checksum", "compute md5 checksum for each file")]
        private static bool Checksum { get; set; }

        [Argument('a', "accessed", "show accessed time")]
        private static bool Accessed { get; set; }

        static async Task Main(string[] args)
        {
            Arguments.Populate();

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;

            using (var stream = new StreamWriter(Path.Combine(Repository, Name))) 
            { 
                await Analyze(new DirectoryInfo(Root), stream);
            }

            Console.WriteLine($"Done.");
        }

        static async Task Analyze(DirectoryInfo directory, StreamWriter output)
        {
            var subdirectories = directory
                .GetDirectories()
                .OrderBy(d => d.Name);

            var files = directory
                .GetFiles()
                .OrderBy(f => f.Name).ToList();

            var dirCount = subdirectories.Count();
            var fileCount = files.Count;
            var size = files.Sum(f => f.Length);

            output.WriteLine("==================================================================================");
            output.WriteLine($"{{\n  \"directory\": \"{directory.FullName.Replace(@"\", "\\")}\",\n  \"subdirectoryCount\": {dirCount},\n  \"fileCount\": {fileCount},\n  \"size\": {size},\n  \"entries\": [");
            
            foreach (var subdirectory in subdirectories)
            {
                output.WriteLine($"    {{ \"directory\": \"{subdirectory.Name}\" }},");
            }

            
            for (int i = 0; i < fileCount; i++)
            {
                output.WriteLine($"    {GetFileJson(files[i])}{(i == fileCount - 1 ? string.Empty : ",")}");
            }

            output.WriteLine("  ]\n}");

            foreach (var subdirectory in subdirectories)
            {
                await Analyze(subdirectory, output);
            }
        }

        static string GetFileJson(FileInfo file)
        {
            var sb = new StringBuilder();
            sb.Append($"{{ \"file\": \"{file.Name}\", ");
            sb.Append($"\"size\": \"{file.Length}\", ");

            return sb.ToString();
        }
    }
}
