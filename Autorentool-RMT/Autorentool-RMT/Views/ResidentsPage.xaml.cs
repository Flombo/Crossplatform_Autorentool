using Autorentool_RMT.ViewModels;
using Autorentool_RMT.Views;
using System;
using Xamarin.Forms;

namespace Autorentool_RMT.Views
{
    public partial class ResidentsPage : ContentPage
    {
        private ResidentViewModel residentViewModel;
        public ResidentsPage()
        {
            InitializeComponent();
            // ab: Disable navigation bar
            NavigationPage.SetHasNavigationBar(this, false);

            //System.Console.WriteLine("NAVIGATION STACK: " + Navigation.ToString());

            //foreach (ContentPage cp in Navigation.NavigationStack)
            //{
            //    Console.WriteLine(cp.ToString());
            //}

            //this.Title = "Bewohner-Seite";

            //Console.WriteLine("Blubb2");
            //Console.WriteLine(this.Title);
            //Console.WriteLine("Nav stack size: " + Navigation.NavigationStack.Count);

            //foreach (Page page in Navigation.NavigationStack)
            //{
            //    Console.WriteLine(page.Title);
            //}

            //Navigation.PopAsync();

            // Check if page is on top of stack
            //public bool PageTypeIsAlreadyAtTopOfStack(
            //ContentPage parentPage,
            //Type typeofPageAppearing)
            //{
            //    var stack = parentPage.Navigation.NavigationStack;
            //    return (stack[stack.Count - 1].GetType() == typeofPageAppearing);
            residentViewModel = new ResidentViewModel();
            BindingContext = residentViewModel;
        }

        protected override void OnAppearing()
        {
            //// ab: Show NavStack on Console
            //Console.WriteLine("Nav stack size: " + Navigation.NavigationStack.Count);

            //foreach (Page page in Navigation.NavigationStack)
            //{
            //    Console.WriteLine(page.Title);
            //}
            residentViewModel.OnLoadAllResidents();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnHomeButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private async void OnAddNewResidentButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewResidentPage());
        }
    }
}