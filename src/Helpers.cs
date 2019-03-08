using NSwag;
using System.IO;
using NJsonSchema;
using System.Linq;
using System.Threading.Tasks;

namespace OpenApiCleaner {
    /// <summary>
    /// Helper functions
    /// </summary>
    public static class Helpers {
        /// <summary>
        /// Determines the most recent version of the API using the repositories folder structure.
        /// </summary>
        /// <returns>The highest version folder.</returns>
        /// <param name="path">Path.</param>
        public static string GetHighestVersionFolder(string path) {
            var childDirectories = Directory.GetDirectories(path);
            var folders = childDirectories.Select(folder => new DirectoryInfo(folder)).Where(folder => folder.Name != "tfs-4.1").OrderByDescending(folder => folder.Name);
            return folders.FirstOrDefault()?.FullName;
        }
        /// <summary>
        /// Processes the specification file.
        /// </summary>
        /// <returns>The specification file.</returns>
        /// <param name="file">File.</param>
        /// <param name="output">Output.</param>
        public static async Task ProcessSpecificationFile(string file, string output) {
            var spec = await SwaggerDocument.FromFileAsync(file);
            spec.ExtensionData?.Clear();
            foreach (var definition in spec.Definitions) {
                definition.Value.ExtensionData?.Clear();
            }
            foreach (var path in spec.Paths) {
                foreach (var operation in path.Value) {
                    foreach (var param in operation.Value.Parameters) {
                        param.ExtensionData?.Clear();
                    }
                    operation.Value.ExtensionData.TryGetValue("x-ms-vss-method", out object descriptor);
                    operation.Value.ExtensionData?.Clear();
                    operation.Value.OperationId = descriptor.ToString();
                }
            }
            var content = spec.ToJson(SchemaType.Swagger2);
            var outFile = Path.Join(output, new DirectoryInfo(file).Name);
            File.WriteAllText(outFile, content);
        }
    }
}