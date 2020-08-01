using System;
using System.Threading.Tasks;
using Svg.Skia;

namespace AssetGenerator
{
    public abstract class BaseAssetGenerator : IAssetGenerator
    {
        public abstract Task CreateAsset(string filepath, string filename, string destinationDirectory, int quality,
            string postfix = default);


        public abstract Task CreateIcon(string filePath, string fileName, string destinationDirectory, int quality);

        protected virtual SKSvg GetSvg(string filePath)
        {
            SKSvg svg = new SKSvg();
            svg.Load(filePath);
            if (svg?.Picture == null)
                throw new NullReferenceException(nameof(svg.Picture));
            return svg;
        }
    }
}
