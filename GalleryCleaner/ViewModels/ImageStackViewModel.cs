using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GalleryCleaner.Models;
using GalleryCleaner.Services;
using Xamarin.Forms;

namespace GalleryCleaner.ViewModels
{
    public class ImageStackViewModel : INotifyPropertyChanged
    {
        private readonly IPhotoPickerService _photoPickerService;
        private ImageSource currentImage;
        private ImageSource nextImage;

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<ImageItem> _mediaItems { get; set; }


        public ImageSource CurrentImage
        {
            get => currentImage; set
            {
                currentImage = value;
                OnPropertyChanged();
            }
        }
        public ImageSource NextImage { get => nextImage; set { nextImage = value;
                OnPropertyChanged();
            } }

        public ImageStackViewModel()
        {
            _photoPickerService = DependencyService.Get<IPhotoPickerService>();

            _mediaItems = new ObservableCollection<ImageItem>();
            BindingBase.EnableCollectionSynchronization(_mediaItems, null, ObservableCollectionCallback);
            _photoPickerService.OnMediaAssetLoaded += OnMediaAssetLoaded;
        }

        public void LoadImages()
        {
            Task.Run(() => _photoPickerService.LoadImageAssetsAsync());
        }

        private void OnMediaAssetLoaded(object sender, MediaEventArgs e)
        {
            var imageSource = ImageSource.FromStream(() => e.Media.Stream);

            var item = new ImageItem
            {
                Name = "test name",
                Image = imageSource
            };
            _mediaItems.Add(item);

            if (CurrentImage == null)
                CurrentImage = imageSource;
            else if (NextImage == null)
                NextImage = imageSource;
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

    }
}
