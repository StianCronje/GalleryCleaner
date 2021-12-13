using System.IO;
using MvvmHelpers;
using Xamarin.Forms;

namespace GalleryCleaner.Models
{
    public class PhotoItem : ObservableObject
    {
        private string id;
        private string name;
        private ImageSource image;
        private Stream stream;

        public string Id { get => id; set => SetProperty(ref id, value); }
        public string Name { get => name; set => SetProperty(ref name, value); }
        public ImageSource Image { get => image; set => SetProperty(ref image, value); }
        public Stream Stream { get => stream; set => SetProperty(ref stream, value); }
    }
}
