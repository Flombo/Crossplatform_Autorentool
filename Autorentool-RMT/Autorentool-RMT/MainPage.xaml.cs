using System;
using Xamarin.Forms;
using Autorentool_RMT.Views;

namespace Autorentool_RMT
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // ab: Disable navigation bar
            NavigationPage.SetHasNavigationBar(this, false);

            //System.Console.WriteLine("NAVIGATION STACK: " + Navigation.NavigationStack.ToList().ToString());
            //System.Console.WriteLine("NAVIGATION STACK: " + Navigation.NavigationStack.ToList());

            //foreach (var item in Navigation.NavigationStack.ToArray())
            //{
            //    Console.WriteLine(item.ToString());
            //}

            //Console.WriteLine("Blubb");

            //System.Threading.Thread.Sleep(100);

            //this.Title = "Hauptseite Autorentool";
            //Console.WriteLine(this.Title);

            //Console.WriteLine("Nav stack size: " + Navigation.NavigationStack.Count);

            //foreach (Page page in Navigation.NavigationStack)
            //{
            //    Console.WriteLine(page.Title);
            //}


        }

        protected override void OnAppearing()
        {
            //// ab: Show NavStack on Console
            //Console.WriteLine("Nav stack size: " + Navigation.NavigationStack.Count);

            //foreach (Page page in Navigation.NavigationStack)
            //{
            //    Console.WriteLine(page.Title);
            //}
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
            await Navigation.PushAsync(new ResidentsPage());
        }

        private async void OnContentButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ContentsPage());
        }

        private async void OnSessionButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SessionsPage());
        }

        private async void OnContactButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ContactPage());
        }
    }
}