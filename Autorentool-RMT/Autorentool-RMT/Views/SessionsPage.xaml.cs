﻿using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using Autorentool_RMT.ViewModels;
using Autorentool_RMT.Views.Popups;
using System;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionsPage : ContentPage
    {

        private SessionViewModel sessionViewModel;
        private Session selectedSession;

        public SessionsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            sessionViewModel = new SessionViewModel();
            BindingContext = sessionViewModel;
        }

        #region OnAppearing
        protected override async void OnAppearing()
        {
            await sessionViewModel.OnLoadAllSessions();
        }
        #endregion

        #region OnEditSessionButtonClicked
        private async void OnEditSessionButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditSessionPage(selectedSession));
        }
        #endregion

        #region OnDeleteSessionButtonClicked
        /// <summary>
        /// Deletes session and reloads all remaining sessions.
        /// If an error occurs an error prompt will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnDeleteSessionButtonClicked(object sender, EventArgs e)
        {
            if(selectedSession != null)
            {
                bool shouldDelete = await DisplayAlert("Sitzung löschen?", $"Wollen Sie die Sitzung {selectedSession.Name} wirklich löschen?", "Ja", "Nein");

                if (shouldDelete)
                {
                    try
                    {
                        await sessionViewModel.OnDeleteSession();
                        await sessionViewModel.OnLoadAllSessions();
                        selectedSession = null;
                    }
                    catch (Exception)
                    {
                        await DisplayAlert($"Fehler beim Löschen der Sitzung {selectedSession.Name}", $"Ein Fehler trat auf beim Löschen der Sitzung {selectedSession.Name}", "Schließen");
                    }
                }
            }
        }
        #endregion

        #region OnEditSessionNameButtonClicked
        /// <summary>
        /// Opens SessionEditSessionNamePopup and changes the session name, depending on the user input.
        /// If there exists a session under the inserted name, an error prompt will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnEditSessionNameButtonClicked(object sender, EventArgs e)
        {
            if (selectedSession != null)
            {
                SessionEditSessionNamePopup.Result result = await Navigation.ShowPopupAsync(new SessionEditSessionNamePopup(selectedSession));

                if (result != null)
                {
                    if (result.WasEditingSuccessful)
                    {
                        await sessionViewModel.OnLoadAllSessions();
                    }
                    else
                    {
                        await DisplayAlert("Fehler beim Ändern des Sitzungsnamens", $"Eine Sitzung mit dem Namen '{result.SessionName}' existiert bereits", "Schließen");
                    }
                }
            }
        }
        #endregion

        #region OnStartSessionButtonClicked
        private async void OnStartSessionButtonClicked(object sender, EventArgs e)
        {
            SessionResidentSelectionPopup.Result result = await Navigation.ShowPopupAsync(new SessionResidentSelectionPopup());

            if (result != null)
            {
                if (result.SelectedResident != null)
                {
                    await ResidentSessionsDBHandler.BindResidentSession(result.SelectedResident.Id, selectedSession.Id);
                }

                await Navigation.PushAsync(new PlaySessionContentPage(selectedSession, result.SelectedResident));
            }
        }
        #endregion

        #region OnCreateSessionButtonClicked
        /// <summary>
        /// Diplays the SessionCreationPopup and inserts a new session in db depending on the user input.
        /// If the process failed, an error prompt will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCreateSessionButtonClicked(object sender, EventArgs e)
        {
            SessionCreationPopup.Result result = await Navigation.ShowPopupAsync(new SessionCreationPopup());

            if (result != null)
            {
                if (!result.WasCreationSuccessful)
                {
                    await DisplayAlert($"Fehler beim Erstellen der Sitzung '{result.SessionName}'", "Ein Fehler trat beim Erstellen der Sitzung " + result.SessionName, "Schließen");
                }
                else
                {
                    await sessionViewModel.OnLoadAllSessions();
                }
            }
        }
        #endregion

        #region OnSelectionChanged
        /// <summary>
        /// Sets the selectedSession-property for further actions and sets also the SelectedSession-property of the SessionViewModel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedSession = e.CurrentSelection[0] as Session;
            sessionViewModel.SelectedSession = selectedSession;
        }
        #endregion

        #region OnExportSessionButtonClicked
        /// <summary>
        /// Exports the selected session.
        /// If an exception was thrown, an error prompt will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnExportSessionButtonClicked(object sender, EventArgs e)
        {
            try
            {
                sessionViewModel.DisableSessionButtons();

                ISessionExporter sessionExporter = DependencyService.Get<ISessionExporter>();
                bool folderPicked = await sessionExporter.ExportSession(sessionViewModel, selectedSession);

                if (folderPicked)
                {
                    await DisplayAlert(
                        "Sitzung exportiert",
                        $"Die Sitzung wurde mit {sessionViewModel.SelectedSessionMediaItems.Count} Baustein(en) erfolgreich exportiert!",
                        "Alles klar!"
                        );
                }

                sessionViewModel.EnableSessionButtons();

            } catch(Exception exc)
            {
                DisplayExportSessionErrorPrompt(exc.Message);

                sessionViewModel.EnableSessionButtons();
            }
        }
        #endregion

        private async void OnImportSessionButtonClicked(object sender, EventArgs e)
        {
            try
            {
                sessionViewModel.DisableSessionButtons();

                ISessionImporter sessionImporter = DependencyService.Get<ISessionImporter>();
                bool folderPicked = await sessionImporter.ImportSession(sessionViewModel);
                
                await sessionViewModel.OnLoadAllSessions();

                if (folderPicked)
                {
                    await DisplayAlert(
                        "Sitzung importiert",
                        $"Die Sitzung wurde erfolgreich importiert!",
                        "Alles klar!"
                        );
                }

                sessionViewModel.EnableSessionButtons();

            } catch(Exception exc)
            {
                DisplayImportSessionErrorPrompt(exc.Message);
                sessionViewModel.EnableSessionButtons();
            }
        }

        #region DisplayImportSessionErrorPrompt
        private async void DisplayImportSessionErrorPrompt(string errorMessage)
        {
            switch (errorMessage)
            {
                case "Folder empty":
                    await DisplayAlert("Fehler beim Importieren der Sitzung", "Der ausgewählte Ordner ist leer", "Schließen");
                    break;
                case "Session already existing":
                    await DisplayAlert("Fehler beim Importieren der Sitzung", "Die Sitzung ist bereits vorhanden", "Schließen");
                    break;
                default:
                    await DisplayAlert("Fehler beim Importieren der Sitzung", "Ein Fehler trat auf beim Importieren der ausgewählten Sitzung", "Schließen");
                    break;
            }
        }
        #endregion

        #region DisplayExportSessionErrorPrompt
        /// <summary>
        /// Displays an error prompt depending on the content of the error-message.
        /// </summary>
        /// <param name="errorMessage"></param>
        private async void DisplayExportSessionErrorPrompt(string errorMessage)
        {
            if (errorMessage.Contains("Folder not empty"))
            {
                await DisplayAlert(
                    $"Fehler beim Exportieren der Sitzung '{selectedSession.Name}'",
                    "Der Zielordner ist nicht leer. Zum Exportieren von Sitzungen muss ein leerer Ordner gewählt werden",
                    "Schließen"
                    );
            }
            else
            {
                await DisplayAlert(
                    $"Fehler beim Exportieren der Sitzung '{selectedSession.Name}'",
                    $"Es kam zu einem Fehler beim Exportieren der Sitzung '{selectedSession.Name}'",
                    "Schließen"
                    );
            }
        }
        #endregion

    }
}