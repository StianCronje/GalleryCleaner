using System.ComponentModel;
using Xamarin.Forms;
using GalleryCleaner.ViewModels;

namespace GalleryCleaner.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}
