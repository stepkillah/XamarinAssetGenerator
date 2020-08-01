using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AssetGenerator.IconsGeneration;
using Newtonsoft.Json;
using SkiaSharp;
using Image = AssetGenerator.IconsGeneration.Image;

namespace AssetGenerator
{
    public class IosAssetGenerator : BaseAssetGenerator
    {
        private const string IphoneIdiom = "iphone";
        private const string IpadIdiom = "ipad";
        private const string IosMarketing = "ios-marketing";

        public override async Task CreateAsset(string filepath, string filename, string destinationDirectory, int quality,
            string postfix = default)
        {

            var svg = GetSvg(filepath);

            for (var i = 1; i < 4; i++)
            {
                try
                {
                    string newFilename;
                    if (i == 1)
                        newFilename = filename + ".png";
                    else
                        newFilename = filename + $"@{i}x.png";


                    var width = (int)(svg.Picture.CullRect.Width * i);
                    var height = (int)(svg.Picture.CullRect.Height * i);


                    await PngHelper.GeneratePng(svg, width, height, Path.Combine(destinationDirectory, newFilename), quality);
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

        public override async Task CreateIcon(string filePath, string fileName, string destinationDirectory, int quality)
        {
            Content contentJson = new Content();
            var iconSvg = GetSvg(filePath);
            foreach (GenerationIcon icon in IconSizes)
            {
                foreach (int scale in icon.Scale)
                {
                    var newFilename = scale > 1 ? $"{fileName}-{icon.Size.Width}@{scale}x~{icon.Idiom}.png" : $"{fileName}-{icon.Size.Width}~{icon.Idiom}.png";
                    await PngHelper.GeneratePng(iconSvg, icon.Size.Width * scale, icon.Size.Height * scale, Path.Combine(destinationDirectory, newFilename), quality, true);
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

    }
}