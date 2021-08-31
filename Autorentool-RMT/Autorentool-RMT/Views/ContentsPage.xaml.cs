using Autorentool_RMT.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContentsPage : ContentPage
    {

        private ContentViewModel contentViewModel;
        public ContentsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            contentViewModel = new ContentViewModel();
            BindingContext = contentViewModel;
        }

        protected override async void OnAppearing()
        {
            await contentViewModel.OnLoadAllMediaItems();
        }
    }
}