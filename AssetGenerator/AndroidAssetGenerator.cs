﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SkiaSharp.Extended.Svg;

namespace AssetGenerator
{
    public class AndroidAssetGenerator : IAssetGenerator
    {
        public async Task CreateAsset(string filepath, string filename, string destinationDirectory, int quality)
        {
            var resourceTypes = new Dictionary<AndroidResourceType, float>
            {
                // Scale factors
                {AndroidResourceType.LDPI, 0.75f},
                {AndroidResourceType.MDPI, 1},
                {AndroidResourceType.HDPI, 1.5f},
                {AndroidResourceType.XHDPI, 2f},
                {AndroidResourceType.XXHDPI, 3f},
                {AndroidResourceType.XXXHDPI, 4f}
            };

            foreach (var resourceType in resourceTypes)
                try
                {
                    var name = Enum.GetName(typeof(AndroidResourceType), resourceType.Key);
                    var resourceDirectoryName = $"drawable-{name.ToLowerInvariant()}";
                    var svg = new SKSvg();
                    try
                    {
                        svg.Load(filepath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Unexpected error when parsing asset: {filepath}");
                        Console.WriteLine("Error: " + e.Message);
                        Console.WriteLine("Exiting with error 1");
                        Environment.Exit(1);
                    }

                    var width = (int) (svg.CanvasSize.Width * resourceType.Value);
                    var height = (int) (svg.CanvasSize.Height * resourceType.Value);
                    var filenameWithExtension = $"{filename}.png";
                    var resourceDir = Path.Combine(destinationDirectory, resourceDirectoryName);
                    if (!Directory.Exists(resourceDir))
                    {
                        Directory.CreateDirectory(resourceDir);
                    }
                    var finalPath = Path.Combine(resourceDir, filenameWithExtension);
                    await PngHelper.GeneratePng(width, height, filepath, finalPath, quality);
                    Console.WriteLine($"Successfully created asset: {finalPath}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to generate asset: {filename}");
                    Console.WriteLine("Error: " + e.Message);
                    Console.WriteLine("Exiting with error 1");
                    Environment.Exit(1);
                }
        }
    }
}