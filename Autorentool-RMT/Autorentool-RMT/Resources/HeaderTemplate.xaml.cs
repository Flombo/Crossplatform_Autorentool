using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Autorentool_RMT.Resources
{
    public partial class HeaderTemplate : ContentPage
    {
        public HeaderTemplate()
        {
            InitializeComponent();
        }

        private async void OnHomeButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnResidentsButtonClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new ResidentsPage());
        }
    }
}
