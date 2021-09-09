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
    public partial class CreateOrEditResidentPage : ContentPage
    {

        private CreateOrEditResidentViewModel createOrEditResidentViewModel;
        private Resident selectedResident;
        private Session selectedSession;

        #region Constructor for creating a resident
        public CreateOrEditResidentPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            createOrEditResidentViewModel = new CreateOrEditResidentViewModel();
            BindingContext = createOrEditResidentViewModel;
        }
        #endregion

        #region Constructor for editing a resident
        /// <summary>
        ///Inits component, inits newResidentViewModel and sets binding context.
        /// </summary>
        /// <param name="resident"></param>
        public CreateOrEditResidentPage(Resident resident)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            selectedResident = resident;
            createOrEditResidentViewModel = new CreateOrEditResidentViewModel(selectedResident);
            createOrEditResidentViewModel.LoadResidentLifethemes(selectedResident.Id);
            createOrEditResidentViewModel.LoadResidentSessions(selectedResident.Id);
            BindingContext = createOrEditResidentViewModel;
        }
        #endregion

        #region OnAppearing
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (selectedResident != null)
            {
                createOrEditResidentViewModel.LoadResidentSessions(selectedResident.Id);
            }
        }
        #endregion

        #region OnCompleteButtonClicked
        /// <summary>
        /// Persists created resident in db and navigates to the ResidentsPage or updates resident with changed properties depending if there was a resident selected.
        /// Navigates to the resident overview if the processes were successfull.
        /// If an exception was thrown it will be catched and an error message will be prompted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCompleteButtonClicked(object sender, EventArgs e)
        {
            if (selectedResident == null)
            {

                try
                {
                    await createOrEditResidentViewModel.OnAddResident();
                    await Navigation.PopAsync();

                }
                catch (Exception)
                {
                    await DisplayAlert(
                            "Fehler beim Erstellen des Bewohners",
                            "Ein Fehler trat auf beim Erstellen des Bewohners " + selectedResident.ResidentOneLineSummary,
                            "OK"
                        );
                }

            }
            else
            {

                try
                {
                    await createOrEditResidentViewModel.OnEditResident();
                    await Navigation.PopAsync();

                }
                catch (Exception)
                {
                    await DisplayAlert(
                            "Fehler beim Bearbeiten des Bewohners",
                            "Ein Fehler trat auf beim Bearbeiten des Bewohners " + selectedResident.ResidentOneLineSummary,
                            "OK"
                        );
                }

            }
        }
        #endregion

        #region OnDeleteResidentButtonClicked
        /// <summary>
        /// Deletes a resident in db and navigates to the resident overview view, if the user accepts the action.
        /// If an error occurs while deleting a error prompt will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnDeleteResidentButtonClicked(object sender, EventArgs e)
        {
            bool shouldDeleteResident = await DisplayAlert(
                    "Bewohner löschen?",
                    "Wollen Sie den Bewohner " + selectedResident.ResidentOneLineSummary + " wirklich löschen?",
                    "Ja",
                    "Nein"
                );

            if (shouldDeleteResident)
            {
                try
                {
                    await createOrEditResidentViewModel.OnDeleteResident();
                    await Navigation.PopAsync();

                }
                catch (Exception exc)
                {
                    //await DisplayAlert("Fehler beim Löschen des Bewohners", "Ein Fehler trat auf beim Löschen des Bewohners " + selectedResident.ResidentOneLineSummary, "OK");
                    await DisplayAlert("Fehler", exc.Message, "Ok");
                }
            }
        }
        #endregion

        #region OnShowLifethemePopupButtonClicked
        /// <summary>
        /// Shows the LifethemePopup.
        /// After the popup was closed, the Result-containerclass will be returned.
        /// The selectedLifethemes-attribute will be finally used for setting the ResidentLifethemes-property of the NewResidentViewModel.
        /// If an error occurs, an error message will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnShowLifethemePopupButtonClicked(object sender, EventArgs e)
        {
            try
            {
                List<Lifetheme> selectedLifethemes = createOrEditResidentViewModel.ResidentLifethemes;

                LifethemePopup lifethemePopup = new LifethemePopup(selectedLifethemes);

                LifethemePopup.Result result = await Navigation.ShowPopupAsync(lifethemePopup);

                if (result != null)
                {
                    createOrEditResidentViewModel.ResidentLifethemes = result.selectedLifethemes;
                }

            }
            catch (Exception exc)
            {
                bool isDeleteException = exc.Message.StartsWith("Delete");

                if (isDeleteException)
                {
                    await DisplayAlert("Fehler beim Löschen eines Lebensthemas", "Ein Fehler trat auf beim Löschen eines Lebensthemas", "Ok");
                }
            }
        }
        #endregion

        #region OnSessionSelectionChanged
        /// <summary>
        /// Set the SelectedSession-property of the viewmodel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSessionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedSession = e.CurrentSelection[0] as Session;
            createOrEditResidentViewModel.SelectedSession = selectedSession;
        }
        #endregion

        #region OnStartSessionButtonClicked
        /// <summary>
        /// Starts selected session with the selected resident.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnStartSessionButtonClicked(object sender, EventArgs e)
        {
            createOrEditResidentViewModel.SelectedSession = null;
            await Navigation.PushAsync(new PlaySessionContentPage(selectedSession, selectedResident));
        }
        #endregion

        #region OnEditSessionButtonClicked
        /// <summary>
        /// Displays the EditSessionPage with the selected session.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnEditSessionButtonClicked(object sender, EventArgs e)
        {
            createOrEditResidentViewModel.SelectedSession = null;
            await Navigation.PushAsync(new EditSessionPage(selectedSession));
        }
        #endregion

        #region OnDeleteSessionButtonClicked
        /// <summary>
        /// Unbinds the selected session and the selected resident.
        /// Shows an error prompt if the process failed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnDeleteSessionButtonClicked(object sender, EventArgs e)
        {
            try
            {
                bool shouldUnbind = await DisplayAlert(
                    "Sitzungsverknüpfung löschen?",
                    $"Wollen Sie die Verknüpfung der Sitzung '{selectedSession.Name}' mit diesem Bewohner wirklich löschen? Die Sitzung selbst wird dabei nicht gelöscht.",
                    "Ja",
                    "Nein"
                    );

                if (shouldUnbind)
                {
                    await createOrEditResidentViewModel.UnbindResidentAndSession();
                    createOrEditResidentViewModel.LoadResidentSessions(selectedResident.Id);
                }
            }
            catch (Exception)
            {
                await DisplayAlert(
                    "Fehler beim Löschen der Verknüpfung",
                    $"Beim Löschen der Verknüpfung zwischen der Sitzung '{selectedSession.Name}' und dem Bewohner/der Bewohnerin '{selectedResident.ResidentOneLineSummary}'",
                    "Schließen"
                    );
            }
        }
        #endregion

    }
}