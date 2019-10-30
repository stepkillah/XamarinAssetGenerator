using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SkiaSharp;

namespace AssetGenerator.IconsGeneration
{
    internal class Content
    {
        public Content()
        {
            Images = new List<Image>();
            Properties = new Properties();
            Info = new Info();
        }

        [JsonProperty("images")]
        public List<Image> Images { get; set; }

        [JsonProperty("info")]
        public Info Info { get; set; }
        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }

    internal class Image
    {
        public Image()
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
