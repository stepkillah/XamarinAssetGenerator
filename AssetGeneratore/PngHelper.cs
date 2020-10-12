using System;
using System.IO;
using System.Threading.Tasks;
using SkiaSharp;
using Svg.Skia;

namespace AssetGenerator
{
    public class PngHelper
    {
        public static async Task GeneratePng(SKSvg svg2, float width, float height, string filename, int quality, bool icon = false)
        {
            if (svg2?.Picture == null) return;

            // calculate the scaling need to fit
            var matrix = SKMatrix.CreateScale(width / svg2.Picture.CullRect.Width, height / svg2.Picture.CullRect.Height);
            using var image = icon
                ? ConvertProfile(SKImage.FromPicture(svg2.Picture, new SKSizeI((int)width, (int)height), matrix), width,
                    height)
                : SKImage.FromPicture(svg2.Picture, new SKSizeI((int)width, (int)height), matrix);
            using var data = image.Encode(SKEncodedImageFormat.Png, quality);
            // save the data to a stream
            await using var stream = File.OpenWrite(filename);
            data.SaveTo(stream);
            await stream.FlushAsync();
        }
        private static SKImage ConvertProfile(SKImage data, float width, float height)
        {
            using SKImage srcImg = data;
            SKImageInfo info = new SKImageInfo((int)width, (int)height,
                SKImageInfo.PlatformColorType, SKAlphaType.Opaque, SKColorSpace.CreateSrgb());
            // this is the important part. set the destination ColorSpace as
            // `SKColorSpace.CreateSrgb()`. Skia will then be able to automatically convert
            // the original CMYK colorspace, to this new sRGB colorspace.

            SKImage newImg = SKImage.Create(info);
            srcImg.ScalePixels(newImg.PeekPixels(), SKFilterQuality.None);

            // Remove transparency
            var bitmap = RemoveTransparency(srcImg);
            newImg = SKImage.FromBitmap(bitmap);
            // now when doing this resize, Skia knows the original ColorSpace, and the
            // destination ColorSpace, and converts the colors from CMYK to sRGB.
            return newImg;
        }

        private static SKBitmap RemoveTransparency(SKImage orig)
        {
            var original = SKBitmap.FromImage(orig);

            // create a new bitmap with the same dimensions
            // also avoids the first copy if the color type is index8
            var copy = new SKBitmap(original.Width, original.Height);

            using var canvas = new SKCanvas(copy);
            // clear the bitmap with the desired color for transparency
            canvas.Clear(SKColors.White);

            // draw the bitmap on top
            canvas.DrawBitmap(original, 0, 0);

            return copy;
        }
    }
}