using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AssetGenerator.Enums;

namespace AssetGenerator.IconsGeneration
{
    public class AndroidAssetGenerator : BaseAssetGenerator
    {

        public override async Task CreateAsset(string filepath, string filename, string destinationDirectory, int quality,
            string postfix = default)
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


            await GenerateAsset(new AndroidGeneratorOptions(resourceTypes, filepath, filename, destinationDirectory, quality,
                false, postfix));
        }

        public override async Task CreateIcon(string filePath, string fileName, string destinationDirectory, int quality)
        {
            var resourceTypes = new Dictionary<AndroidResourceType, float>
            {
                // Scale factors
                {AndroidResourceType.LDPI, 48},
                {AndroidResourceType.MDPI, 72},
                {AndroidResourceType.HDPI, 96},
                {AndroidResourceType.XHDPI, 144},
                {AndroidResourceType.XXHDPI, 192},
                {AndroidResourceType.XXXHDPI, 512}
            };

            await GenerateAsset(new AndroidGeneratorOptions(resourceTypes, filePath, fileName, destinationDirectory, quality,
                true, prefix: "mipmap"));
        }



        private async Task GenerateAsset(AndroidGeneratorOptions options)
        {
            var svg = GetSvg(options.FilePath);

            foreach (var resourceType in options.ResourcesTypes)
                try
                {
                    var name = Enum.GetName(typeof(AndroidResourceType), resourceType.Key);
                    if (string.IsNullOrEmpty(name))
                        throw new ArgumentException("Unknown android resource type");

                    var resourceDirectoryName = string.IsNullOrEmpty(options.Postfix)
                        ? $"{options.Prefix}-{name.ToLowerInvariant()}"
                        : $"{options.Prefix}-{name.ToLowerInvariant()}-{options.Postfix}";

                    var width = options.Icon ? resourceType.Value : svg.Picture.CullRect.Width * resourceType.Value;
                    var height = options.Icon ? resourceType.Value : svg.Picture.CullRect.Height * resourceType.Value;

                    // Cheap clamp
                    if (width < 1f)
                        width = 1;
                    if (height < 1f)
                        height = 1;

                    var filenameWithExtension = $"{options.FileName}.png";
                    var resourceDir = Path.Combine(options.DestinationDirectory, resourceDirectoryName);
                    if (!Directory.Exists(resourceDir))
                    {
                        Directory.CreateDirectory(resourceDir);
                    }
                    var finalPath = Path.Combine(resourceDir, filenameWithExtension);
                    await GeneratePng(svg, width, height, finalPath, options.Quality);

                    Console.WriteLine($"Successfully created asset: {finalPath}");

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to generate asset: {options.FileName}");
                    Console.WriteLine("Error: " + e.Message);
                    Console.WriteLine("Exiting with error 1");
                    Environment.Exit(1);
                }
        }



        internal class AndroidGeneratorOptions
        {

            public AndroidGeneratorOptions(Dictionary<AndroidResourceType, float> resourcesTypes, string filePath, string fileName, string destinationDirectory, int quality, bool icon,
                string postfix = default, string prefix = "drawable")
            {
                ResourcesTypes = resourcesTypes;
                FilePath = filePath;
                FileName = fileName;
                DestinationDirectory = destinationDirectory;
                Quality = quality;
                Icon = icon;
                Postfix = postfix;
                Prefix = prefix;
            }

            public Dictionary<AndroidResourceType, float> ResourcesTypes { get; }
            public string FilePath { get; }
            public string FileName { get; }
            public string DestinationDirectory { get; }
            public int Quality { get; }
            public bool Icon { get; }
            public string Postfix { get; }
            public string Prefix { get; }

        }
    }
}