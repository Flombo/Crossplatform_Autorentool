using Autorentool_RMT.Models;
using Autorentool_RMT.Services.BackendCommunication;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using Autorentool_RMT.ViewModels;
using Autorentool_RMT.Views.Popups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionsPage : ContentPage, ITooltipProvider
    {

        private SessionViewModel sessionViewModel;
        private Session selectedSession;
        private List<Tooltip> tooltips;
        private BackendPullSessions backendPullSessions;

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
            GenerateTooltips();
            await LoadAllSessions();
            InitBackendCommunicationModules();
        }
        #endregion

        #region LoadAllSessions
        /// <summary>
        /// Loads all Sessions from db.
        /// Is used by the BackendPullSessions-class after downloading or deleting sessions.
        /// </summary>
        /// <returns></returns>
        public async Task LoadAllSessions()
        {
            await sessionViewModel.OnLoadAllSessions();
        }
        #endregion

        #region SetVisibilityOfProgressBarAndRing
        /// <summary>
        /// Sets the visibility for the progress elements.
        /// Is used by the BackendPullSessions-class during adding and deleting Sessions from backend
        /// </summary>
        /// <param name="isProgressBarAndRingVisible"></param>
        public void SetVisibilityOfProgressBarAndRing(bool isProgressBarAndRingVisible)
        {
            sessionViewModel.IsProgressBarVisible = isProgressBarAndRingVisible;
        }
        #endregion

        #region SetProgressBarStatusTxt
        /// <summary>
        /// Sets the progress of the progressbar and the status text.
        /// </summary>
        /// <param name="currentProgress"></param>
        /// <param name="maxProgress"></param>
        public void SetProgressBarStatusTxt(int currentProgress, int maxProgress)
        {
            sessionViewModel.SetProgressAndStatus(currentProgress, maxProgress);
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

        #region OnImportSessionButtonClicked
        /// <summary>
        /// Imports a session from folder.
        /// Displays an error message if an exception was thrown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        "Die Sitzung wurde erfolgreich importiert!",
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
        #endregion

        #region DisplayImportSessionErrorPrompt
        /// <summary>
        /// Displays an error prompt depending which errorMessage is given.
        /// </summary>
        /// <param name="errorMessage"></param>
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
        /// Generates the Tooltips for importing, exporting, creating, deleting, starting and editing sessions as well as a Tooltip for the CollectionView and adds them to the Tooltips-list
        /// </summary>
        public void GenerateTooltips()
        {
            ResourceDictionary resourceDictionary = Application.Current.Resources;


            Tooltip importSessionsFromBackendTooltip = new Tooltip()
            {
                Title = "Sitzungen aus unserem Online-Content-Pool importieren",
                Description = "Durch diesen Button können Sie Sitzungen aus unserem Online-Content-Pool importieren.",
                Icon = resourceDictionary["ImportFromBackendIcon"].ToString()
            };

            Tooltip importSessionTooltip = new Tooltip()
            {
                Title = "Sitzungen importieren",
                Description = "Durch diesen Button können Sie bereits exportierte Sitzungen aus einem Ordner importieren.",
                Icon = resourceDictionary["ImportIcon"].ToString()
            };

            Tooltip exportSessionTooltip = new Tooltip()
            {
                Title = "Sitzungen exportieren",
                Description = "Durch diesen Button können Sie die Sitzungen in einer Datei auf Ihrem Computer oder an einen externen Speicherort (z.B. USB-Stick) exportieren.",
                Icon = resourceDictionary["ExportIcon"].ToString()
            };

            Tooltip sessionListTooltip = new Tooltip()
            {
                Title = "Liste der Sitzungen",
                Description = "In dieser Liste sehen Sie alle bisher erstellten oder importierten Sitzungen. "
                              +"Um neue Inhaltsbausteine in die Sitzung hinzuzufügen oder zu löschen," 
                              +" wählen Sie eine Sitzung aus und klicken auf den Button SITZUNG BEARBEITEN.",
                Icon = ""
            };

            Tooltip editSessionTooltip = new Tooltip()
            {
                Title = "Sitzung bearbeiten",
                Description = "Durch diesen Button können Sie die ausgewählte Sitzung bearbeiten und die Reihenfolge der Inhalte verändern.",
                Icon = resourceDictionary["EditIcon"].ToString()
            };

            Tooltip startSessionTooltip = new Tooltip()
            {
                Title = "Sitzungen abspielen",
                Description = "Durch diesen Button können Sie die ausgewählte Sitzung abpielen (und am Ende bewerten, falls Sie die Sitzung einem Bewohner zuordnen).",
                Icon = resourceDictionary["PlayIcon"].ToString()
            };

            Tooltip deleteSessionTooltip = new Tooltip()
            {
                Title = "Sitzung löschen",
                Description = "Durch diesen Button können Sie die ausgewählte Sitzung löschen.",
                Icon = resourceDictionary["DeleteIcon"].ToString()
            };

            Tooltip addSessionTooltip = new Tooltip()
            {
                Title = "Sitzung hinzufügen",
                Description = "Durch diesen Button können Sie eine Sitzung hinzufügen.",
                Icon = resourceDictionary["AddIcon"].ToString()
            };

            tooltips = new List<Tooltip>()
            {
                importSessionsFromBackendTooltip,
                importSessionTooltip,
                exportSessionTooltip,
                sessionListTooltip,
                editSessionTooltip,
                startSessionTooltip,
                deleteSessionTooltip, 
                addSessionTooltip
            };
        }
        #endregion

        #region InitBackendCommunicationModules
        /// <summary>
        /// Inits the helper-classes and the websocket of the BackendPullSessions.
        /// After the initialization was successfull it will be checked if there are sessions available that can be downloaded.
        /// If these processes failed, the ImportFromBackendButton will be disabled.
        /// </summary>
        private async void InitBackendCommunicationModules()
        {
            try
            {
                backendPullSessions = new BackendPullSessions(this);
                await backendPullSessions.InitHelper();
                await backendPullSessions.InitWebSocket();
                sessionViewModel.IsImportFromBackendButtonEnabled = await backendPullSessions.ShouldDownloadSessionsFromBackend();
            }
            catch (Exception)
            {
                sessionViewModel.IsImportFromBackendButtonEnabled = false;
            }
        }
        #endregion

        #region DisplayImportSessionsFromBackendByPushDialog
        /// <summary>
        /// Displays a prompt, when an admin wants to update the Sessions via Push.
        /// If the user accepts this action, the Sessions will be pulled.
        /// If an error occurs, an error prompt will be displayed.
        /// </summary>
        public void DisplayImportSessionsFromBackendByPushDialog()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    bool shouldDownloadSessions = await DisplayAlert(
                        "Neue Sitzungen aus dem Online-Content-Pool verfügbar",
                        "Möchten Sie neue Sitzungen aus dem Online-Content-Pool herunterladen?",
                        "Herunterladen",
                        "Abbrechen"
                        );

                    if (shouldDownloadSessions)
                    {
                        sessionViewModel.IsImportFromBackendButtonEnabled = await backendPullSessions.PullSessionsFromBackend();
                    }

                } catch(Exception)
                {
                    DisplayImportSessionsFromBackendConnectionErrorDialog();
                }
            });
        }
        #endregion

        #region DisplayDeleteSessionsViaDeleteCommand
        public void DisplayDeleteSessionsViaDeleteCommand(List<int> appSessionIDs)
        {
            Device.BeginInvokeOnMainThread(async () => {
                try
                {
                    List<Session> deletableSessions = new List<Session>();
                    string deletableSessionNames = "";

                    foreach(int appSessionId in appSessionIDs)
                    {
                        Session deletableSession = await SessionDBHandler.GetSingleSession(appSessionId);

                        if(deletableSession != null)
                        {
                            deletableSessionNames += deletableSession.Name;
                            deletableSessions.Add(deletableSession);
                        }
                    }

                    bool shouldDelete = await DisplayAlert(
                        "Ein Admin möchte Sitzungen von Ihrem Gerät löschen",
                        $"Möchten Sie, dass folgende Sitzungen gelöscht werden: '{deletableSessionNames}' ?",
                        "Löschen",
                        "Abbrechen"
                        );

                    if (shouldDelete)
                    {
                        await backendPullSessions.DeleteSessionsRetrievedByWebSocket(deletableSessions);
                    }
                }
                catch (Exception)
                {
                    await DisplayAlert(
                        $"Fehler beim Löschen der Sitzung {selectedSession.Name}",
                        $"Ein Fehler trat auf beim Löschen der Sitzung {selectedSession.Name}",
                        "Schließen"
                        );
                }
            });
        }
        #endregion

        #region OnImportSessionsFromBackendButtonClicked
        /// <summary>
        /// Displays an reconfirmation prompt after the user clicked on the import-sessions-from-backend-button.
        /// If the user accepted the prompt, the PasswordPopup will be displayed.
        /// After the user entered a valid password, the sessions will be pulled.
        /// If an exception was thrown, an error prompt will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnImportSessionsFromBackendButtonClicked(object sender, EventArgs e)
        {
            try
            {
                bool shouldImportSessions = await DisplayAlert(
                    "Sitzungen durch Online-Content-Pool aktualisieren?",
                    "Möchten Sie die aktuellen Sitzungen aus unserem Content-Pool herunterladen?",
                    "Alles klar!",
                    "Abbrechen"
                    );

                while (shouldImportSessions)
                {
                    PasswordPopup.Result result = await Navigation.ShowPopupAsync(
                        new PasswordPopup("Passwort zum Importieren von Sitzungen aus unserem Online-Content-Pool")
                        );

                    shouldImportSessions = result != null && await CheckPasswordAndPullSessions(result);
                }

            } catch(Exception)
            {
                DisplayImportSessionsFromBackendConnectionErrorDialog();
            }
        }
        #endregion

        #region CheckPasswordAndPullSessions
        /// <summary>
        /// Checks if the entered password is valid and returns a boolean if the PasswordPopup should be displayed again.
        /// If thats the case, the sessions will be imported.
        /// Else an error prompt will be displayed and the user has the choice, if he want's to continue or not.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<bool> CheckPasswordAndPullSessions(PasswordPopup.Result result)
        {
            try
            {
                bool shouldImportSessions;

                if (result.IsPasswordValid)
                {
                    sessionViewModel.IsImportFromBackendButtonEnabled = await backendPullSessions.PullSessionsFromBackend();

                    shouldImportSessions = false;
                }
                else
                {
                    shouldImportSessions = await DisplayAlert(
                        "Falsches Passwort!",
                        $"Das eingegebenen Passwort {result.InsertedPassword} ist leider falsch.",
                        "Alles klar!",
                        "Abbrechen"
                        );
                }

                return shouldImportSessions;

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region DisplayImportSessionsFromBackendConnectionErrorDialog
        /// <summary>
        /// Displays an error prompt when the download of sessions from the backend fails.
        /// This prompt must run over the main thread or the app crashes.
        /// </summary>
        public void DisplayImportSessionsFromBackendConnectionErrorDialog()
        {
            Device.BeginInvokeOnMainThread(async () => {
                await DisplayAlert(
                    "Fehler beim Herunterladen von Sitzungen aus unserem Online-Content-Pool",
                    "Es kam zu einem Fehler beim Herunterladen der Sitzungsdaten aus unserem Online-Content-Pool",
                    "Schließen"
                    );
            });
        }
        #endregion
    }
}