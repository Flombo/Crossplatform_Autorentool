using Autorentool_RMT.Models;
using Autorentool_RMT.ViewModels;
using Autorentool_RMT.Views.Popups;
using System;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaySessionContentPage : ContentPage, ITooltipProvider
    {
        private PlaySessionContentViewModel playSessionContentViewModel;
        private Session selectedSession;
        private Resident selectedResident;
        private List<Tooltip> tooltips;
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
        /// Loads all mediaItems, generates the Tooltips and starts the session.
        /// </summary>
        protected override async void OnAppearing()
        {
            GenerateTooltips();

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

        #region OnTooltipButtonClicked
        /// <summary>
        /// Display TooltipPopup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTooltipButtonClicked(object sender, EventArgs e)
        {
            DisplayTooltip();
        }
        #endregion

        #region DisplayTooltip
        /// <summary>
        /// Displays TooltipPopup with the generated Tooltips.
        /// </summary>
        public void DisplayTooltip()
        {
            Navigation.ShowPopup(new TooltipPopup(tooltips));
        }
        #endregion

        #region GenerateTooltips
        /// <summary>
        /// Generates the Tooltips for the CloseSessionButton, the NextElementButton and the PreviousElementButton and adds them to the Tooltips-list
        /// </summary>
        public void GenerateTooltips()
        {
            ResourceDictionary resourceDictionary = Application.Current.Resources;

            Tooltip closeSessionTooltip = new Tooltip()
            {
                Title = "Sitzung beenden",
                Description = "Mit diesem Button können Sie die Sitzung jederzeit beenden. "
                              + "Die jeweils letzte Sitzungsdauer wird automatisch im Bewohnerprofil gespeichert.",
                Icon = resourceDictionary["CloseIcon"].ToString()
            };

            Tooltip nextElementTooltip = new Tooltip()
            {
                Title = "Nächter Inhaltsbaustein",
                Description = "Mit diesem Button können Sie in der Sitzung einen Inhaltsbaustein weitergehen. "
                + "Sind Sie am Ende der Sitzung angelangt, wird dieser Button ausgeblendet.",
                Icon = resourceDictionary["NextElementIcon"].ToString()
            };

            Tooltip previousElementTooltip = new Tooltip()
            {
                Title = "Vorheriger Inhaltsbaustein",
                Description = "Mit diesem Button gelangen Sie zum vorherigen Inhaltsbaustein. "
                + "Sind Sie am Anfang der Sitzung angelangt, wird dieser Button ausgeblendet.",
                Icon = resourceDictionary["PreviousElementIcon"].ToString()
            };

            tooltips = new List<Tooltip>()
            {
                closeSessionTooltip,
                previousElementTooltip,
                nextElementTooltip
            };
        }
        #endregion

    }
}