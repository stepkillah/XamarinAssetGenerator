﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using SkiaSharp.Extended.Svg;

namespace AssetGenerator
{
    public class XamarinAssetGenerator
    {
        private const string Android = "Android";
        private const string iOS = "iOS";

        private static async Task<int> Main(string[] args)
        {
            var mode = "ios";
            var sourceDirectory = Directory.GetCurrentDirectory();
            var destinationDirectory = Directory.GetCurrentDirectory();
            var result = Parser.Default.ParseArguments<Options>(args);
            var postfix = string.Empty;
            var quality = 80;
            GeneratorMode currentMode = GeneratorMode.Android;
            result.WithParsed(options =>
                {
                    if (options.Version)
                    {
                        Console.WriteLine($"AssetGenerator Version - {Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");
                        Environment.Exit(1);
                    }
                    mode = options.Mode;
                    currentMode = Enum.Parse<GeneratorMode>(mode, true);
                    if (!string.IsNullOrEmpty(options.SourceFolderPath))
                    {
                        sourceDirectory = Path.Combine(sourceDirectory, options.SourceFolderPath);
                        if (currentMode == GeneratorMode.AppIcon)
                        {
                            if (!File.Exists(sourceDirectory))
                            {
                                Console.WriteLine("Source file does not exist");
                                Environment.Exit(1);
                            }
                        }
                        else
                        {
                            if (!Directory.Exists(sourceDirectory))
                            {
                                Console.WriteLine("Source directory does not exist");
                                Environment.Exit(1);
                            }
                        }

                    }

                    if (!string.IsNullOrEmpty(options.DestinationFolderPath))
                    {
                        destinationDirectory = options.DestinationFolderPath;
                        if (!Directory.Exists(destinationDirectory))
                        {
                            Console.WriteLine("Destination directory does not exist");
                            Environment.Exit(1);
                        }
                    }

                    if (quality < 1 || quality > 100)
                    {
                        Console.WriteLine("Quality must be between 1..100");
                        Environment.Exit(1);
                    }

                    if (!string.IsNullOrEmpty(options.Postfix))
                    {
                        postfix = options.Postfix;
                    }
                    quality = options.Quality;
                })
                .WithNotParsed(errors =>
                {
                    foreach (var error in errors) Console.WriteLine("Error: " + error.Tag);

                    Environment.Exit(1);
                });

            if (currentMode == GeneratorMode.AppIcon)
            {
                await GenerateIcons(sourceDirectory, destinationDirectory, quality);
            }
            else
            {
                var files = Directory.GetFiles(sourceDirectory, "*.svg");
                if (files.Length == 0)
                {
                    Console.WriteLine("No .svg files found in directory");
                    Environment.Exit(1);
                }

                await GenerateAssets(files, currentMode, destinationDirectory, quality, postfix);
            }

            return await Task.FromResult(0);
        }


        private static async Task GenerateIcons(string filePath, string destinationDirectory, int quality)
        {
            var generator = new IOSAssetGenerator();
            await generator.CreateIcon(filePath, Path.GetFileNameWithoutExtension(filePath), destinationDirectory, quality);
        }

        private static async Task GenerateAssets(string[] files, GeneratorMode mode, string destinationDirectory,
            int quality, string postfix)
        {
            // It fails to create the first file
            var firstItem = files[1];
            var initalLoad = new SKSvg();
            await using var stream = File.OpenRead(firstItem);
            initalLoad.Load(stream);
            await PngHelper.GeneratePng((int)initalLoad.CanvasSize.Width, (int)initalLoad.CanvasSize.Height,
                firstItem, "tempFile", 1);
            File.Delete("tempFile");

            foreach (var filepath in files.OrderBy(s => s).ToList())
            {
                Console.WriteLine($"Creating assets from {filepath}");

                var filename = Path.GetFileNameWithoutExtension(filepath);
                if (mode == GeneratorMode.iOS)
                {
                    var generator = new IOSAssetGenerator();
                    await generator.CreateAsset(filepath, filename, destinationDirectory, quality, string.Empty);
                }
                else
                {
                    var generator = new AndroidAssetGenerator();
                    await generator.CreateAsset(filepath, filename, destinationDirectory, quality, postfix);
                }
            }
        }
    }
}