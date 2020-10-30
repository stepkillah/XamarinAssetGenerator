using System.Collections.Generic;
using Newtonsoft.Json;

namespace AssetGenerator.AppIconsGeneration
{
    internal class Content
    {
        public Content()
        {
            Images = new List<IosImage>();
            Properties = new Properties();
            Info = new Info();
        }

        [JsonProperty("images")]
        public List<IosImage> Images { get; set; }

        [JsonProperty("info")]
        public Info Info { get; set; }
        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }

    internal class IosImage
    {
        public IosImage()
        {
        }



        [JsonProperty("scale")] public string Scale { get; set; }

        [JsonProperty("size")] public string Size { get; set; }

        [JsonProperty("idiom")]
        public string Idiom { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }
    }

    internal class Info
    {
        [JsonProperty("version")] public long Version { get; set; } = 1;

        [JsonProperty("author")] public string Author { get; set; } = "xcode";
    }

    internal class Properties
    {
    }
}
