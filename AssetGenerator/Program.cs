using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AssetGenerator.Enums;
using AssetGenerator.Extensions;
using AssetGenerator.Helpers;
using CommandLine;

namespace AssetGenerator
{
    public class XamarinAssetGenerator
    {
        private static async Task Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args);

            Options opts = result.MapResult(ParseHelper.ParseOptions, ParseHelper.ParseError);
            if (!opts.ShouldProceed)
                return;

            if (opts.MappedMode.IsAppIcon())
            {
                await GenerateIcons(opts.SourceFolderPath, opts.DestinationFolderPath, opts.Quality, opts.MappedMode);
                return;
            }

            var files = Directory.GetFiles(opts.SourceFolderPath, "*.svg");
            if (files.Length == 0)
            {
                Console.WriteLine("No .svg files found in directory");
                return;
            }

            await GenerateAssets(files, opts.MappedMode, opts.DestinationFolderPath, opts.Quality, opts.Postfix);
        }

        private static async Task GenerateIcons(string filePath, string destinationDirectory, int quality, GeneratorMode mode) =>
            await mode.CreateGenerator().CreateIcon(filePath, Path.GetFileNameWithoutExtension(filePath),
                destinationDirectory, quality);


        private static async Task GenerateAssets(string[] files, GeneratorMode mode, string destinationDirectory,
            int quality, string postfix)
        {
            var generator = mode.CreateGenerator();
            foreach (var filepath in files.OrderBy(s => s).ToList())
            {
                Console.WriteLine($"Creating assets from {filepath}");
                var filename = Path.GetFileNameWithoutExtension(filepath);
                await generator.CreateAsset(filepath, filename, destinationDirectory, quality, postfix);
            }
        }
    }
}