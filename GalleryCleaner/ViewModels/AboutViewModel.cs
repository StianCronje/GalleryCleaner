using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalleryCleaner.Models;
using GalleryCleaner.Services;
using GalleryCleaner.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GalleryCleaner.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public ObservableCollection<PhotoItem> MediaItems { get; set; }

        public AboutViewModel()
        {
            Title = "About";
            OpenImagePageCommand = new Command(async () => await Shell.Current.GoToAsync($"/{nameof(ImageStackPage)}"));

            MediaItems = new ObservableCollection<PhotoItem>();
            Xamarin.Forms.BindingBase.EnableCollectionSynchronization(MediaItems, null, ObservableCollectionCallback);
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

        public ICommand OpenImagePageCommand { get; }
    }
}
