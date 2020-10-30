using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AssetGenerator.Enums;
using CommandLine;

namespace AssetGenerator.Helpers
{
    public static class ParseHelper
    {
        public static Options ParseOptions(Options options)
        {
            if (options.Version)
            {
                Console.WriteLine($"AssetGenerator Version - {Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");
                options.ShouldProceed = false;
                return options;
            }

            if (!string.IsNullOrEmpty(options.SourceFolderPath))
            {
                options.SourceFolderPath = Path.Combine(Directory.GetCurrentDirectory(), options.SourceFolderPath);
                if (options.MappedMode == GeneratorMode.AppIconAndroid || options.MappedMode == GeneratorMode.AppIconIos)
                {
                    if (!File.Exists(options.SourceFolderPath))
                    {
                        Console.WriteLine("Source file does not exist");
                        options.ShouldProceed = false;
                        return options;
                    }
                }
                else
                {
                    if (!Directory.Exists(options.SourceFolderPath))
                    {
                        Console.WriteLine("Source directory does not exist");
                        options.ShouldProceed = false;
                        return options;
                    }
                }

            }

            if (!string.IsNullOrEmpty(options.DestinationFolderPath))
            {
                options.DestinationFolderPath = options.DestinationFolderPath;
                if (!Directory.Exists(options.DestinationFolderPath))
                {
                    Console.WriteLine("Destination directory does not exist");
                    options.ShouldProceed = false;
                    return options;
                }
            }

            if (options.Quality < 1 || options.Quality > 100)
            {
                Console.WriteLine("Quality must be between 1..100");
                options.ShouldProceed = false;
                return options;
            }

            return options;
        }

        public static Options ParseError(IEnumerable<Error> errors)
        {
            foreach (var error in errors) Console.WriteLine("Error: " + error.Tag);
            return new Options() { ShouldProceed = false };
        }
    }
}
