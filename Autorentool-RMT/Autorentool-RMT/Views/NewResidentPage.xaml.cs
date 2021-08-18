using Autorentool_RMT.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewResidentPage : ContentPage
    {

        private NewResidentViewModel newResidentViewModel;

        public NewResidentPage()
        {
            InitializeComponent();
            newResidentViewModel = new NewResidentViewModel();
            BindingContext = newResidentViewModel;
        }

        private async void OnCompleteButtonClicked(object sender, EventArgs e)
        {
            await newResidentViewModel.OnAddResident();
            await Navigation.PopAsync();
        }
    }
}