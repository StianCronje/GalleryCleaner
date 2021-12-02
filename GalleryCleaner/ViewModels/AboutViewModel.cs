using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalleryCleaner.Models;
using GalleryCleaner.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GalleryCleaner.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public ObservableCollection<PhotoItem> MediaItems { get; set; }
        //private readonly IPhotoPickerService _photoPickerService;

        public AboutViewModel()
        {
            //_photoPickerService = DependencyService.Get<IPhotoPickerService>();

            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));

            MediaItems = new ObservableCollection<PhotoItem>();
            Xamarin.Forms.BindingBase.EnableCollectionSynchronization(MediaItems, null, ObservableCollectionCallback);
            //_photoPickerService.OnMediaAssetLoaded += OnMediaAssetLoaded;
        }

        private void OnMediaAssetLoaded(object sender, MediaEventArgs e)
        {
            var imageSource = ImageSource.FromStream(() => e.Media.Stream);

            var item = new PhotoItem
            {
                Name = "test name",
                Image = imageSource
            };
            MediaItems.Add(item);
        }

        private void ObservableCollectionCallback(System.Collections.IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            lock (collection)
            {
                accessMethod?.Invoke();
            }
        }

        public ICommand OpenWebCommand { get; }
        public ICommand PickImageCommand => new Command(async () => await PickImage());

        private async Task PickImage()
        {
            var canUsePhotos = await Permissions.CheckStatusAsync<Permissions.Photos>();

            if(canUsePhotos == PermissionStatus.Granted)
            {
                try
                {
                    //await _photoPickerService.LoadImageAssetsAsync();
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
