using Autorentool_RMT.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaySessionContentPage : ContentPage
    {
        private PlaySessionContentViewModel playSessionContentViewModel;
        public PlaySessionContentPage()
        {
            InitializeComponent();
            playSessionContentViewModel = new PlaySessionContentViewModel();
            BindingContext = playSessionContentViewModel;
        }

        protected override async void OnAppearing()
        {
            await playSessionContentViewModel.OnLoadAllMediaItems();
            playSessionContentViewModel.StartSession();
        }

        private async void OnCloseSessionClicked(object sender, EventArgs e)
        {
            playSessionContentViewModel.StopSession();
            await Navigation.PopAsync();
        }
    }
}