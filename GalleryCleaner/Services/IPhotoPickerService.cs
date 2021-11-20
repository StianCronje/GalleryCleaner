using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GalleryCleaner.Services
{
    public interface IPhotoPickerService
    {
        Task<IEnumerable<MediaItem>> LoadImageAssetsAsync();
        event EventHandler<MediaEventArgs> OnMediaAssetLoaded;
    }

    public class MediaItem
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Stream Stream { get; set; }
    }

    public class MediaEventArgs : EventArgs
    {
        public MediaItem Media { get; }
        public MediaEventArgs(MediaItem media)
        {
            Media = media;
        }
    }
}
