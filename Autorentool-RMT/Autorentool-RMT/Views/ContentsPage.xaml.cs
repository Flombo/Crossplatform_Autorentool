using Autorentool_RMT.Models;
using Autorentool_RMT.ViewModels;
using System;
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
            try
            {
                await contentViewModel.OnLoadAllMediaItems();
            }
            catch (Exception)
            {
                await DisplayAlert("Fehler beim Laden der Medieninhalte", "Ein Fehler trat auf beim Laden der Medieninhalte", "Schließen");
            }
        }

        #region OnSelectionChanged
        /// <summary>
        /// Sets the SelectedMediaItem-property if triggered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MediaItem selectedMediaItem = e.CurrentSelection[0] as MediaItem;
            contentViewModel.SelectedMediaItem = selectedMediaItem;
        }
        #endregion
    }
}