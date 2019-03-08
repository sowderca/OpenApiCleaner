using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

using static OpenApiCleaner.Helpers;

#pragma warning disable RECS0002

namespace OpenApiCleaner {
    /// <summary>
    /// Main program
    /// </summary>
    internal static class Program {
        /// <summary>
        /// The program name.
        /// </summary>
        private const string Name = "OpenApiCleaner";
        /// <summary>
        /// The help option.
        /// </summary>
        private const string HelpOption = "-h|--help";
        /// <summary>
        /// Executes the clean command.
        /// </summary>
        private const string CleanCommand = "-c|--clean";
        /// <summary>
        /// Path to the folder containing the OpenAPI specifications files.
        /// </summary>
        private const string SpecificationOption = "-s|--specs";
        /// <summary>
        /// Path to the directory in which to write the cleaned specification files to.
        /// </summary>
        private const string OutputOption = "-o|--output";
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args) => Run(args);
        /// <summary>
        /// Executes the clean command.
        /// </summary>
        /// <returns>The clean.</returns>
        /// <param name="command">Command.</param>
        private static void Clean(CommandLineApplication command) {
            command.Name = "Clean";
            command.Description = "Cleans the OpenAPI specification files of any non-standard enteries";
            var specificationOption = command.Option(SpecificationOption, "Path to the directory in which to write the cleaned specification files too.", CommandOptionType.SingleValue);
            var outputOption = command.Option(OutputOption, "Path to the directory in which the cleaned specification files are written to", CommandOptionType.SingleValue);
            command.OnExecute(() => {
                if (specificationOption.HasValue() && outputOption.HasValue()) {
                    var specFilePath = specificationOption.Value();
                    var outputFilePath = outputOption.Value();
                    try {
                        if (!Directory.Exists(specFilePath)) {
                            throw new DirectoryNotFoundException();
                        }
                        if (!Directory.Exists(outputFilePath)) {
                            Directory.CreateDirectory(outputFilePath);
                        }
                        Parallel.ForEach(Directory.GetDirectories(specFilePath), (string folder) => {
                            var container = GetHighestVersionFolder(folder);
                            foreach (var file in Directory.GetFiles(container, "*.json")) {
                                ProcessSpecificationFile(file, outputFilePath).Wait();
                            }
                        });
                    } catch (Exception error) {
                        Console.Error.WriteLine(error.Message);
                    }
                } else if (!specificationOption.HasValue() && !outputOption.HasValue()) {
                    command.ShowHelp();
                }
                return 0;
            });
        }
        /// <summary>
        /// Creates a console application to handle user input.
        /// </summary>
        /// <returns>The console app.</returns>
        private static void Run(string[] args) {
            var app = new CommandLineApplication {
                Name = Name,
                Description = "Cleans dirty OpenApi specification files"
            };
            app.HelpOption(HelpOption);
            app.Command(CleanCommand, command => Clean(command));
            app.Execute(args);
        }
    }
}
