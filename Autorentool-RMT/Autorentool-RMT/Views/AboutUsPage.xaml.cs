using Autorentool_RMT.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutUsPage : ContentPage
    {

        private AboutUsViewModel aboutUsViewModel;

        public AboutUsPage()
        {
            aboutUsViewModel = new AboutUsViewModel();
            BindingContext = aboutUsViewModel;
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }

        #region OnAppearing
        /// <summary>
        /// Loads on appearing the MediaMetaDataList of 'Bildernachweis.csv'.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await aboutUsViewModel.LoadMediaMetaDataList();
        }
        #endregion
    }
}