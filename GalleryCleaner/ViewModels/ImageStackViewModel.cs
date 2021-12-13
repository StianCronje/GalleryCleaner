using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalleryCleaner.Models;
using GalleryCleaner.Services;
using Xamarin.Forms;
using Dasync.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using MvvmHelpers.Commands;
using MvvmHelpers;
using System.Diagnostics;

namespace GalleryCleaner.ViewModels
{
    public class ImageStackViewModel : ObservableObject
    {
        private readonly IPhotoService _photoPickerService;

        private IAsyncEnumerable<MediaItem> Images;
        private ObservableCollection<PhotoItem> photoList = new ObservableCollection<PhotoItem>();

        private int _skip = 0;
        private int _take = 1;
        private int _stackSize = 1;

        public ICommand LoadImagesCommand;
        public ICommand HandleNextCommand;

        public ObservableCollection<PhotoItem> PhotoList { get => photoList; set => SetProperty(ref photoList, value); }

        public ImageStackViewModel()
        {
            _photoPickerService = DependencyService.Get<IPhotoService>();

            LoadImagesCommand = new AsyncCommand(LoadImages);
            HandleNextCommand = new AsyncCommand(HandleNext);
        }

        private async Task LoadImages()
        {
            Images = _photoPickerService.LoadImageAssets();

            for (int i = 0; i < _stackSize; i++)
            {
                AddItem(await GetNextPhoto());
            }
        }

        private async Task HandleNext()
        {
            RemoveItem();
            AddItem(await GetNextPhoto());
        }

        public async Task AddNext()
        {
            AddItem(await GetNextPhoto());
        }

        public async Task<PhotoItem> GetNextPhoto()
        {
            if (Images == null)
                return null;

            var photo = await Task.Run(async () =>
            {
                try
                {
                    var image = await Images.Skip(_skip++).Take(_take).FirstOrDefaultAsync();

                    if (image == null)
                        return null;

                    var imageSource = ImageSource.FromStream(() => image.Stream);

                    var photoItem = new PhotoItem
                    {
                        Id = image.Id,
                        Name = image.Name,
                        Image = imageSource,
                        Stream = image.Stream
                    };
                    return photoItem;
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex);
                    return null;
                }
                
            });

            return photo;
        }

        private void AddItem(PhotoItem item)
        {
            if (item == null)
                return;

            PhotoList.Insert(0, item);
        }

        private void RemoveItem()
        {
            var lastIndex = PhotoList.Count - 1;
            PhotoList.RemoveAt(lastIndex);
        }
    }
}
