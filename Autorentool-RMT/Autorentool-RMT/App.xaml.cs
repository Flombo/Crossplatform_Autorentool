using System;
using Xamarin.Forms;
using Autorentool_RMT.ViewModels;

namespace Autorentool_RMT
{
    public partial class App : Application
    {
        /**
        static DatabaseMain database;

        // Create the database connection as a singleton.
        public static DatabaseMain Database
        {
            get
            {
                if (database == null)
                {
                    database = new DatabaseMain();
                }
                return database;
            }
        }**/

        public App()
        {
            InitializeComponent();

            //MainPage = new MainPage();

            //MainPage = new ResidentsPage();

            MainPage = new NavigationPage(new MainPage());

            BindingContext = new AppViewModel();
        }

        protected override void OnStart()
        {
            //ObservableCollection<View> views = NavbarStackLayout.VisibleViews;

            //StackLayout st = views[0] as StackLayout;
            //st.Children.Clear();

            ////or
            //StackLayout st1 = NavbarStackLayout.CurrentItem as StackLayout;
            //st1.Children.Clear();

            //foreach (var familyNames in UIFont.FamilyNames.OrderBy(c => c).ToList())
            //{
            //    Console.WriteLine(" * " + familyNames);
            //    foreach (var familyName in UIFont
            //           .FontNamesForFamilyName(familyNames)
            //           .OrderBy(c => c).ToList())
            //    {
            //        Console.WriteLine(" *-- " + familyName);
            //    }
            //}
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        //protected override void OnAppearing()
        //{
        //    Console.WriteLine("Nav stack size: " + Navigation.NavigationStack.Count);

        //    foreach (Page page in Navigation.NavigationStack)
        //    {
        //        Console.WriteLine(page.Title);
        //    }
        //}

        private async void OnHomeButton_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PopToRootAsync();
            //await MainPage.Navigation.PopToRootAsync();
            await ((NavigationPage)Application.Current.MainPage).PopToRootAsync();
        }

        private async void OnBackButton_Clicked(object sender, EventArgs e)
        {
            //await App.Current.MainPage.Navigation.PopAsync();
            //await Navigation.PopAsync();
            await ((NavigationPage)Application.Current.MainPage).PopAsync();
        }

        //private void GetNavStackPage()
        //{
        //    Console.WriteLine(((NavigationPage)Application.Current.MainPage).CurrentPage.Navigation.NavigationStack);
        //}

        void NavbarButton_Clicked(object sender, EventArgs e)
        {
            //Console.WriteLine(((NavigationPage)Application.Current.MainPage).Navigation.NavigationStack.Count);

            int currentPage = ((NavigationPage)Application.Current.MainPage).Navigation.NavigationStack.Count - 1;
            Console.WriteLine(((NavigationPage)Application.Current.MainPage).Navigation.NavigationStack[currentPage].Title);
        }

        private void OnTooltipButtonClicked(object sender, EventArgs e)
        {
            int currentPageIndex = Current.MainPage.Navigation.NavigationStack.Count - 1;

            Page currentPage = Current.MainPage.Navigation.NavigationStack[currentPageIndex];
            
            if(currentPage is ITooltipProvider iTooltipProvider)
            {
                iTooltipProvider.DisplayTooltip();
            }
        }
    }
}