// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessing.Core.Tools
{
    public static class ImageTool
    {

        /// <summary>
        /// Rotate image
        /// </summary>
        /// <param name="blobContent">Image file as stream</param>
        /// <param name="rotate">How to rotate image, by default - 180 degree</param>
        /// <returns>Rotated image and image encoder</returns>
        public static async Task<(Image, IImageEncoder)> RotateImageAsync(Stream blobContent, RotateMode rotate = RotateMode.Rotate180)
        {
            // Load blob content as image
            (Image content, IImageFormat format) metadata = await Image.LoadWithFormatAsync(blobContent);

            // Resolve image encoder
            var formatManager = Configuration.Default.ImageFormatsManager;
            var format = formatManager.FindFormatByMimeType(metadata.format.DefaultMimeType);
            var encoder = formatManager.FindEncoder(format);

            // Rotate image
            metadata.content.Mutate(image => image.RotateFlip(rotateMode: rotate, flipMode: FlipMode.None));

            return (metadata.content, encoder);
        }

        /// <summary>
        /// Check that file format is supported
        /// </summary>
        /// <param name="fileName">File name to be uploaded</param>
        /// <returns></returns>
        public static bool IsSupportedImageFormat(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            switch (ext.ToLower())
            {
                case ".jpg":
                    return true;
                case ".jpeg":
                    return true;
                case ".png":
                    return true;
                default:
                    return false;
            }
        }
    }
}
