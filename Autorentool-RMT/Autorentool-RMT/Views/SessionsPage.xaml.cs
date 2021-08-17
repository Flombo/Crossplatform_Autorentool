using Autorentool_RMT.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionsPage : ContentPage
    {

        private SessionViewModel sessionViewModel;
    
        public SessionsPage()
        {
            InitializeComponent();
            sessionViewModel = new SessionViewModel();
            BindingContext = sessionViewModel;
        }

        protected override async void OnAppearing()
        {
            await sessionViewModel.OnLoadAllSessions();
        }

        private async void OnEditSessionButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditSessionPage());
        }

    }
}