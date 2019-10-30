using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using AssetGenerator.IconsGeneration;
using Newtonsoft.Json;
using SkiaSharp;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace AssetGenerator
{
    public class IOSAssetGenerator : IAssetGenerator
    {

        private const string IphoneIdiom = "iphone";
        private const string IpadIdiom = "ipad";
        private const string IosMarketing = "ios-marketing";

        public async Task CreateAsset(string filepath, string filename, string destinationDirectory, int quality,
            string postfix = default)
        {
            for (var i = 1; i < 4; i++)
            {
                try
                {
                    var svg = GetSvg(filepath);

                    string newFilename;
                    if (i == 1)
                        newFilename = filename + ".png";
                    else
                        newFilename = filename + $"@{i}x.png";

                    var width = (int)(svg.CanvasSize.Width * i);
                    var height = (int)(svg.CanvasSize.Height * i);
                    await PngHelper.GeneratePng(width, height, filepath, Path.Combine(destinationDirectory, newFilename), quality);
                    Console.WriteLine($"Successfully created asset: {Path.GetFileName(newFilename)}");
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
        private List<GenerationIcon> IconSizes => new List<GenerationIcon>()
        {
            new GenerationIcon(new SKSize(20,20), new []{2,3}, IphoneIdiom),
            new GenerationIcon(new SKSize(29,29), new []{2,3}, IphoneIdiom),
            new GenerationIcon(new SKSize(40,40), new []{2,3}, IphoneIdiom),
            new GenerationIcon(new SKSize(60,60), new []{2,3}, IphoneIdiom),

            new GenerationIcon(new SKSize(20,20), new []{1,2}, IpadIdiom),
            new GenerationIcon(new SKSize(29,29), new []{1,2}, IpadIdiom),
            new GenerationIcon(new SKSize(40,40), new []{1,2}, IpadIdiom),
            new GenerationIcon(new SKSize(76,76), new []{1,2}, IpadIdiom),
            new GenerationIcon(new SKSize(83.5f,83.5f), new []{2}, IpadIdiom),
            new GenerationIcon(new SKSize(1024,1024), new []{1}, IosMarketing)
        };

        public async Task CreateIcon(string filePath, string fileName, string destinationDirectory, int quality)
        {
            Content contentJson = new Content();
            foreach (GenerationIcon icon in IconSizes)
            {
                foreach (int scale in icon.Scale)
                {
                    var newFilename = scale > 1 ? $"{fileName}-{icon.Idiom}-{icon.Size.Width}@{scale}x.png" : $"{fileName}-{icon.Idiom}-{icon.Size.Width}.png";
                    await PngHelper.GeneratePng(icon.Size.Width * scale, icon.Size.Height * scale, filePath, Path.Combine(destinationDirectory, newFilename), quality);
                    contentJson.Images.Add(new Image()
                    {
                        Scale = $"{scale}x",
                        Size = $"{icon.Size.Width}x{icon.Size.Height}",
                        Idiom = icon.Idiom,
                        Filename = newFilename
                    });

                    Console.WriteLine($"Successfully created asset: {Path.GetFileName(newFilename)}");
                }
            }

            var rawJson = JsonConvert.SerializeObject(contentJson, Formatting.Indented);
            var contentsJsonPath = Path.Combine(destinationDirectory, "Contents.json");
            await File.WriteAllTextAsync(contentsJsonPath, rawJson);
            Console.WriteLine($"Successfully created Contents.json file: {Path.GetFileName(contentsJsonPath)}");
            Console.WriteLine($"Contents.json content:\n {rawJson}");
        }


        private SKSvg GetSvg(string filePath)
        {
            var svg = new SKSvg();
            try
            {
                svg.Load(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error when parsing asset: {filePath}");
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine("Exiting with error 1");
                Environment.Exit(1);
            }

            return svg;
        }
    }
}