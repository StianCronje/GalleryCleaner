using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GalleryCleaner.Models;
using GalleryCleaner.Services;
using Xamarin.Forms;
using Dasync.Collections;
using System.Collections.Generic;

namespace GalleryCleaner.ViewModels
{
    public class ImageStackViewModel : INotifyPropertyChanged
    {
        private readonly IPhotoService _photoPickerService;
        private PhotoItem currentImage;
        private PhotoItem nextImage;

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<PhotoItem> _mediaItems { get; set; }
        private IAsyncEnumerable<MediaItem> Images;

        private int _skip = 0;
        private int _take = 1;


        public PhotoItem CurrentImage
        {
            get => currentImage; set
            {
                currentImage = value;
                OnPropertyChanged();
            }
        }
        public PhotoItem NextImage { get => nextImage; set { nextImage = value;
                OnPropertyChanged();
            } }

        public ImageStackViewModel()
        {
            _photoPickerService = DependencyService.Get<IPhotoService>();

            _mediaItems = new ObservableCollection<PhotoItem>();
            BindingBase.EnableCollectionSynchronization(_mediaItems, null, ObservableCollectionCallback);
        }

        public void LoadImages()
        {
            Images = _photoPickerService.LoadImageAssetsAsync();
            
            Task.Run(async () =>
            {
                CurrentImage = await GetNextPhoto();
                NextImage = await GetNextPhoto();
            });
        }


        private void ObservableCollectionCallback(System.Collections.IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            lock (collection)
            {
                accessMethod?.Invoke();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task HandleNext()
        {
            NextImage = await GetNextPhoto();
        }

        public async Task<PhotoItem> GetNextPhoto()
        {
            if (Images == null)
                return null;

            var image = await Images.Skip(_skip++).Take(_take).FirstOrDefaultAsync();

            var imageSource = ImageSource.FromStream(() => image.Stream);

            var photo = new PhotoItem
            {
                Id = image.Id,
                Name = image.Name,
                Image = imageSource
            };

            return photo;
        }
    }
}
