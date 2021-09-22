using System;
using Xamarin.Forms;
using Autorentool_RMT.Views;
using Xamarin.CommunityToolkit.Extensions;
using Autorentool_RMT.Views.Popups;
using Autorentool_RMT.Models;
using System.Collections.Generic;

namespace Autorentool_RMT
{
    public partial class MainPage : ContentPage, ITooltipProvider
    {

        private List<Tooltip> tooltips;

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

        #region GenerateTooltips
        /// <summary>
        /// Generates tooltips for the 3 Menus BEWOHNER, INHALTE and SITZUNGEN.
        /// </summary>
        public void GenerateTooltips()
        {
            Tooltip residentsTooltip = new Tooltip()
            {
                Title = "Bewohnerprofile",
                Description = "Hier können Sie für die Bewohner*innen Profile anlegen und diese bearbeiten.",
                ImageIcon = "ImageOld.png"
            };

            Tooltip contentsTooltip = new Tooltip()
            {
                Title = "Multimediale Inhalte",
                Description = "Hier können Sie die in der App gespeicherten Inhalte (Bilder, Videos, Musik und Texte) ansehen und zu Lebensthemen zuordnen.",
                ImageIcon = "ImageSharingContent.png"
            };

            Tooltip sessionsTooltip = new Tooltip()
            {
                Title = "Sitzungen",
                Description = "In diesem Bereich können Sie Sitzungen mit verschiedenen multimedialen Inhalten für die Bewohner*innen zusammenstellen und abspielen.",
                ImageIcon = "ImageChecklist.png"
            };

            tooltips = new List<Tooltip>()
            {
                residentsTooltip,
                contentsTooltip,
                sessionsTooltip
            };
        }
        #endregion

        protected override void OnAppearing()
        {
            //// ab: Show NavStack on Console
            //Console.WriteLine("Nav stack size: " + Navigation.NavigationStack.Count);

            //foreach (Page page in Navigation.NavigationStack)
            //{
            //    Console.WriteLine(page.Title);
            //}
            GenerateTooltips();
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

        private async void OnAboutUsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutUsPage());
        }

        #region DisplayTooltip
        /// <summary>
        /// Displays TooltipPopup with previous generated tooltips.
        /// </summary>
        public void DisplayTooltip()
        {
            Navigation.ShowPopup(new TooltipPopup(tooltips));
        }
        #endregion

    }
}