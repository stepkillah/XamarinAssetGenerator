using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace AssetGenerator.IconsGeneration
{
    internal class GenerationIcon
    {
        public GenerationIcon(SKSize size, int[] scale, string idiom)
        {
            Size = size;
            Scale = scale;
            Idiom = idiom;
        }

        public string Idiom { get; set; }
        public SKSize Size { get; set; }
        public int[] Scale { get; set; }
    }
}
