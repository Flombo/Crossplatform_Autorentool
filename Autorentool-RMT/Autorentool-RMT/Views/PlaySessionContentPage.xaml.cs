using Autorentool_RMT.Models;
using Autorentool_RMT.ViewModels;
using Autorentool_RMT.Views.Popups;
using System;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaySessionContentPage : ContentPage
    {
        private PlaySessionContentViewModel playSessionContentViewModel;
        private Session selectedSession;
        private Resident selectedResident;
        public PlaySessionContentPage(Session selectedSession, Resident selectedResident)
        {
            InitializeComponent();
            this.selectedResident = selectedResident;
            this.selectedSession = selectedSession;
            NavigationPage.SetHasNavigationBar(this, false);
            playSessionContentViewModel = new PlaySessionContentViewModel(selectedSession);
            BindingContext = playSessionContentViewModel;
        }

        #region OnAppearing
        /// <summary>
        /// Loads all mediaItems and starts the session.
        /// </summary>
        protected override async void OnAppearing()
        {
            try
            {
                await playSessionContentViewModel.OnLoadAllMediaItems();
                playSessionContentViewModel.StartSession();

            } catch(Exception)
            {
                await DisplayAlert("Fehler beim Laden der Sitzung", "Es kam zu einem Fehler beim Laden der Sitzung", "Schließen");
                await Navigation.PopAsync();
            }
        }
        #endregion

        #region OnCloseSessionClicked
        /// <summary>
        /// Closes the PlaySessionContentPage.
        /// First displays a validation prompt.
        /// Second shows the rating popup or closes the Page, depending on the selectedResident-attribute.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCloseSessionClicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Sitzung beenden?", "Wollen Sie die Sitzung wirklich beenden?", "Ja", "Nein");
            
            if (result)
            {
                selectedSession = playSessionContentViewModel.StopSession();

                if (selectedResident != null)
                {
                    await Navigation.ShowPopupAsync(new SessionRatingPopup(selectedSession, selectedResident));
                    await Navigation.PopAsync();
                }
                else
                {
                    await Navigation.PopAsync();
                }
            }
        }
        #endregion

        #region OnNotesButtonClicked
        /// <summary>
        /// Shows the NotesPopup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNotesButtonClicked(object sender, EventArgs e)
        {
            Navigation.ShowPopup(new NotesPopup(playSessionContentViewModel.CurrentMediaItem.Notes));
        }
        #endregion
    }
}