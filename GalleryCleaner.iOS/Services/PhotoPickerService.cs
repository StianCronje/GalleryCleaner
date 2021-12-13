using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using GalleryCleaner.iOS.Services;
using GalleryCleaner.Services;
using Photos;
using Xamarin.Forms;

[assembly: Dependency(typeof(PhotoService))]
namespace GalleryCleaner.iOS.Services
{
    public class PhotoService : IPhotoService
    {
        public async IAsyncEnumerable<MediaItem> LoadImageAssets()
        {
            var mediaItems = new List<MediaItem>();

            var imageManager = new PHImageManager();

            var requestOptions = new PHImageRequestOptions
            {
                ResizeMode = PHImageRequestOptionsResizeMode.Fast,
                DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat,
                NetworkAccessAllowed = false,
                Synchronous = false,
            };

            var fetchResults = PHAsset.FetchAssets(PHAssetMediaType.Image, null);
            var allAssets = fetchResults.Select(a => a as PHAsset).ToArray();

            foreach (var result in fetchResults)
            {
                var task = new TaskCompletionSource<MediaItem>();
                var phAsset = result as PHAsset;
                var mediaItem = new MediaItem
                {
                    Id = phAsset.LocalIdentifier,
                    Name = PHAssetResource.GetAssetResources(phAsset)?.FirstOrDefault()?.OriginalFilename
                };

                imageManager.RequestImageForAsset(phAsset, PHImageManager.MaximumSize, PHImageContentMode.AspectFit, requestOptions, (image, info) =>
                {
                    if (image != null)
                    {
                        NSData imageData = null;
                        if (image.CGImage.RenderingIntent == CoreGraphics.CGColorRenderingIntent.Default)
                        {
                            imageData = image.AsJPEG();
                        }
                        else
                        {
                            imageData = image.AsPNG();
                        }

                        if (imageData != null)
                        {
                            var imageStream = imageData.AsStream();
                            mediaItem.Stream = imageStream;
                        }
                    }

                    task.SetResult(mediaItem);
                });
                yield return await task.Task;
            }
        }
    }
}
