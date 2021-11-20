﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using GalleryCleaner.iOS.Services;
using GalleryCleaner.Services;
using Photos;
using PhotosUI;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(PhotoPickerService))]
namespace GalleryCleaner.iOS.Services
{
    public class PhotoPickerService : IPhotoPickerService
    {
        public event EventHandler<MediaEventArgs> OnMediaAssetLoaded;

        public async Task<IEnumerable<MediaItem>> LoadImageAssetsAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<IEnumerable<MediaItem>>();

            var task = LoadImagesAsync();

            var completedTask = await Task.WhenAny(task, taskCompletionSource.Task);

            return await completedTask;
        }

        private async Task<IEnumerable<MediaItem>> LoadImagesAsync()
        {
            var mediaItems = new List<MediaItem>();

            var imageManager = new PHCachingImageManager();

            await Task.Run(async () =>
            {
                var requestOptions = new PHImageRequestOptions
                {
                    ResizeMode = PHImageRequestOptionsResizeMode.None,
                    DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat,
                    NetworkAccessAllowed = true,
                    Synchronous = true,
                };

                var fetchResults = PHAsset.FetchAssets(PHAssetMediaType.Image, null);
                var allAssets = fetchResults.Select(a => a as PHAsset).ToArray();

                imageManager.StartCaching(allAssets, PHImageManager.MaximumSize, PHImageContentMode.AspectFit, requestOptions);

                foreach (var result in fetchResults)
                {
                    var phAsset = result as PHAsset;
                    var mediaItem = new MediaItem
                    {
                        Id = phAsset.LocalIdentifier,
                        Name = PHAssetResource.GetAssetResources(phAsset)?.FirstOrDefault().OriginalFilename
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

                            if(imageData != null)
                            {
                                var imageStream = imageData.AsStream();
                                mediaItem.Stream = imageStream;
                            }
                        }

                        UIApplication.SharedApplication.InvokeOnMainThread(delegate {
                            OnMediaAssetLoaded?.Invoke(this, new MediaEventArgs(mediaItem));
                        });
                        mediaItems.Add(mediaItem);
                    });
                }

                imageManager.StopCaching();
            });

            return mediaItems;
        }
    }
}