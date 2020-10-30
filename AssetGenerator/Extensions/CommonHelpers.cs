using AssetGenerator.Enums;
using AssetGenerator.IconsGeneration;

namespace AssetGenerator.Extensions
{
    public static class CommonHelpers
    {
        public static bool IsAppIcon(this GeneratorMode mode) =>
            mode == GeneratorMode.AppIconAndroid || mode == GeneratorMode.AppIconIos;

        public static IAssetGenerator CreateGenerator(this GeneratorMode mode) =>
            mode == GeneratorMode.AppIconIos || mode == GeneratorMode.iOS
                ? (IAssetGenerator)new IosAssetGenerator()
                : new AndroidAssetGenerator();
    }
}
