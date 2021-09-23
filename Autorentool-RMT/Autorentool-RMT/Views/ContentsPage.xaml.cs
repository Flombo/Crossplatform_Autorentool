using Autorentool_RMT.Models;
using Autorentool_RMT.Services.BackendCommunication;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using Autorentool_RMT.ViewModels;
using Autorentool_RMT.Views.Popups;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContentsPage : ContentPage, ITooltipProvider
    {

        #region Attributes
        private ContentViewModel viewModel;
        private Session selectedSession;
        private List<Tooltip> tooltips;
        private BackendPullMediaItems backendPullMediaItems;
        #endregion

        #region Empty Constructor
        /// <summary>
        /// Constructor for the management of mediaitems.
        /// </summary>
        public ContentsPage()
        {
            InitializeComponent();
            selectedSession = null;
            NavigationPage.SetHasNavigationBar(this, false);
            viewModel = new ContentManagementViewModel();
            BindingContext = viewModel;
            InitBackendCommunicationModules();
        }
        #endregion

        #region Constructor with the selected session parameter
        /// <summary>
        /// Constructor for the selection of mediaitems.
        /// Is used by EditSessionPage and takes the selected session for binding the selected mediaitems.
        /// </summary>
        /// <param name="selectedSession"></param>
        public ContentsPage(Session selectedSession)
        {
            InitializeComponent();
            this.selectedSession = selectedSession;
            NavigationPage.SetHasNavigationBar(this, false);
            viewModel = new SelectContentViewModel(selectedSession);
            BindingContext = viewModel;
        }
        #endregion

        #region OnAppearing
        /// <summary>
        /// Inits the ContentManagementViewModel or the SelectContentViewModel depending on the selectedSession-attribute.
        /// If it is null, the user was on the main menu before, else the user was on the EditSessionPage.
        /// </summary>
        protected override async void OnAppearing()
        {
            try
            {
                GenerateTooltips();

                if (selectedSession == null)
                {
                    await viewModel.OnLoadAllMediaItems();
                } else
                {
                    SelectContentViewModel selectContentViewModel = viewModel as SelectContentViewModel;
                    selectContentViewModel.OnFilter();
                }

            }
            catch (Exception)
            {
                await DisplayAlert("Fehler beim Laden der Medieninhalte", "Ein Fehler trat auf beim Laden der Medieninhalte", "Schließen");
            }
        }
        #endregion

        #region LoadAllMediaItems
        /// <summary>
        /// Loads all MediaItems in ContentViewModel.
        /// Is used by the BackendPullMediaItems-class after pulling or deleting MediaItems and Lifethemes.
        /// </summary>
        /// <returns></returns>
        public async Task LoadAllMediaItems()
        {
            await viewModel.OnLoadAllMediaItems();
        }
        #endregion

        #region SetProgressElements
        /// <summary>
        /// Sets the progressbar progress and the status text.
        /// Is used by the BackendPullMediaItems-class while deleting and pulling MediaItems and Lifethemes.
        /// </summary>
        /// <param name="currentProgress"></param>
        /// <param name="maxProgress"></param>
        /// <param name="stopwatch"></param>
        public void SetProgressElements(int currentProgress, int maxProgress, Stopwatch stopwatch)
        {
            ContentManagementViewModel contentManagementViewModel = viewModel as ContentManagementViewModel;
            contentManagementViewModel.SetProgressElements(currentProgress, maxProgress, stopwatch);
        }
        #endregion

        #region SetProgressElementsVisibility
        /// <summary>
        /// Sets the visibility of the progressbar and the progressring.
        /// Is used by the BackendPullMediaItems-class while deleting and pulling MediaItems and Lifethemes.
        /// </summary>
        /// <param name="isProgressBarVisible"></param>
        public void SetProgressElementsVisibility(bool isProgressBarVisible)
        {
            viewModel.IsActivityIndicatorRunning = isProgressBarVisible;
            viewModel.IsProgressBarVisible = isProgressBarVisible;
        }
        #endregion

        #region OnSelectionChanged
        /// <summary>
        /// Sets the SelectedMediaItem-property if triggered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MediaItem selectedMediaItem = e.CurrentSelection[0] as MediaItem;
            viewModel.SelectedMediaItem = selectedMediaItem;
        }
        #endregion

        #region OnFullscreenButtonClicked
        private async void OnFullscreenButtonClicked(object sender, EventArgs e)
        {
            MediaItem selectedMediaItem = viewModel.SelectedMediaItem;
            MediaItemFullscreenPopup mediaItemFullscreenPopup = new MediaItemFullscreenPopup(selectedMediaItem, Height, Width);

            await Navigation.ShowPopupAsync(mediaItemFullscreenPopup);
        }
        #endregion

        #region OnCompleteButtonClicked
        private async void OnCompleteButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        #endregion

        #region OnDeleteSelectedMediaItemButtonClicked
        /// <summary>
        /// Deletes selected mediaitem.
        /// If an error occurs, an error message will be prompted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnDeleteSelectedMediaItemButtonClicked(object sender, EventArgs e)
        {
            try
            {
                ContentManagementViewModel contentViewModel = viewModel as ContentManagementViewModel;
                await contentViewModel.OnDeleteMediaItem();
            } catch(Exception)
            {
                string selectedMediaItemName = viewModel.SelectedMediaItem.Name;

                await DisplayAlert("Fehler beim Löschen des Mediums: " + selectedMediaItemName, "Ein Fehler trat auf beim Löschen des Mediums " + selectedMediaItemName, "Schließen");
            }
        }
        #endregion

        #region OnLifethemesButtonClicked
        /// <summary>
        /// Opens LifethemePopup and binds the selected lifethemes with the selected mediaitem.
        /// If an error occurs an error prompt will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnLifethemesButtonClicked(object sender, EventArgs e)
        {
            try
            {
                LifethemePopup.Result result = await Navigation.ShowPopupAsync(new LifethemePopup(viewModel.CurrentMediaItemLifethemes));

                ContentManagementViewModel contentViewModel = viewModel as ContentManagementViewModel;

                await contentViewModel.SetCurrentMediaItemLifethemes(result.selectedLifethemes);
            }
            catch (Exception)
            {
                await DisplayAlert("Fehler beim Auswählen von Lebensthemen", "Beim Auswählen der Lebensthemen kam es zu einem Fehler", "Schließen");
            }
        }
        #endregion

        #region OnDeleteAllMediaItemsButtonClicked
        /// <summary>
        /// Deletes all MediaItems.
        /// First an dialog will be prompted, where the user can choose if he want to really delete all MediaItems.
        /// After that he has to enter the right password.
        /// If an exception happens, an error prompt will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnDeleteAllMediaItemsButtonClicked(object sender, EventArgs e)
        {
            try
            {
                bool shouldDeleteAllMediaItems = await DisplayAlert(
                    "Alle Bausteine löschen?",
                    "Sind Sie sicher, dass Sie alle Inhaltsbausteine endgültig löschen wollen? (Dabei werden auch alle mit diesen Bausteinen verknüpften Sitzungen geleert.)",
                    "Alles klar!",
                    "Abbrechen"
                    );

                while (shouldDeleteAllMediaItems)
                {
                    PasswordPopup.Result result = await Navigation.ShowPopupAsync(new PasswordPopup("Passwort fürs Löschen aller Inhalte"));

                    if(result != null)
                    {
                        if (result.IsPasswordValid)
                        {
                            ContentManagementViewModel contentViewModel = viewModel as ContentManagementViewModel;

                            await contentViewModel.OnDeleteAllMediaItems();
                            shouldDeleteAllMediaItems = false;
                        } else
                        {
                            shouldDeleteAllMediaItems = await DisplayAlert("Falsches Passwort!", "Das eingegebenen Passwort " + result.InsertedPassword + " ist leider falsch.", "Alles klar!", "Abbrechen");
                        }
                    } else
                    {
                        shouldDeleteAllMediaItems = false;
                    }
                    
                }
            } catch(Exception)
            {
                await DisplayAlert("Fehler beim Löschen der Medien", "Ein Fehler trat auf beim Löschen der Medien", "Schließen");
            }
        }
        #endregion

        #region OnImportButtonClicked
        /// <summary>
        /// Shows file picker (if the user selected this option), saves the selected files and displays them.
        /// Shows the directory picker (if the user selected this option), saves the files in the selected folder and displays them.
        /// If an error occured an error prompt will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnImportButtonClicked(object sender, EventArgs e)
        {
            try
            {
                ContentManagementViewModel contentManagementViewModel = viewModel as ContentManagementViewModel;

                string action = await DisplayActionSheet("Import von Medieninhalten: Art des Imports auswählen", "Abbrechen", null, "Ordnerauswahl", "Einzelne Dateien");

                if (action.Equals("Einzelne Dateien"))
                {
                    await contentManagementViewModel.ShowFilePicker();
                } else
                {
                    //Get the implementation of the IDirectoryPicker interface by dependency injection depending on the platform (UWP = UWPDirectoryPicker, Android = ToDo, iOS = ToDo)
                    IDirectoryPicker directortyPickerImplementation = DependencyService.Get<IDirectoryPicker>();

                    /**Shows the folder-picker and saves the contained files.
                     * The ContentManagementViewModel is needed for setting / resetting the progress-ui-elements.
                    **/
                    await directortyPickerImplementation.ShowFolderPicker(contentManagementViewModel);
                    await contentManagementViewModel.OnLoadAllMediaItems();
                }

            } catch (Exception exc)
            {
                if (exc.Message.Contains("Duplicate"))
                {
                    await DisplayAlert("Fehler beim Hinzufügen neuer Inhalte", "Es dürfen keine bereits existierenden Dateien hinzugefügt werden", "Schließen");
                } else if (exc.Message.Contains("Folder")) 
                {
                    await DisplayAlert("Fehler beim Hinzufügen der Ordnerinhalte", "Es kam zu einem Fehler beim Hinzufügen der Dateien aus dem ausgewählten Ordner", "Schließen");
                } else
                {
                    await DisplayAlert("Fehler beim Hinzufügen neuer Inhalte", "Ein Fehler trat auf beim Hinzufügen neuer Inhalte", "Schließen");
                }
            }
        }
        #endregion

        #region OnAddMediaItemButtonClicked
        /// <summary>
        /// Binds all checked mediaitems to the selected session and Navigates back to the EditSessionPage.
        /// If an error occured, an error message will be prompted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnAddMediaItemButtonClicked(object sender, EventArgs e)
        {
            SelectContentViewModel selectContentViewModel = viewModel as SelectContentViewModel;

            try
            {
                await selectContentViewModel.BindCheckedMediaItemsToSession();
                await Navigation.PopAsync();
            } catch(Exception)
            {
                await DisplayAlert("Fehler beim Hinzufügen der Bausteine","Beim Hinzufügen der Bausteine kam es zu einem Fehler", "Schließen");
            }
        }
        #endregion

        #region OnPickCSVFileButtonClicked
        private async void OnPickCSVFileButtonClicked(object sender, EventArgs e)
        {
            try
            {
                
                ContentManagementViewModel contentManagementViewModel = viewModel as ContentManagementViewModel;
                contentManagementViewModel.ShowCSVPicker();
            
            } catch(Exception)
            {
                await DisplayAlert("Fehler beim Laden der CSV-Datei", "Beim Laden der CSV-Datei kam es zu einem Fehler", "Schließen");
            }
        }
        #endregion

        #region OnMediaItemCheckboxChanged
        /// <summary>
        /// Adds or removes a mediaItem from the CheckedMediaItems-list if the checkbox is checked or unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMediaItemCheckboxChanged(object sender, CheckedChangedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            Grid grid = checkBox.Parent as Grid;
            MediaItem mediaItem = grid.BindingContext as MediaItem;

            SelectContentViewModel selectContentViewModel = viewModel as SelectContentViewModel;

            if (checkBox.IsChecked)
            {
                selectContentViewModel.AddMediaItemToCheckedMediaItems(mediaItem);
            } else
            {
                selectContentViewModel.RemoveMediaItemFromCheckedMediaItems(mediaItem);
            }
        }
        #endregion

        #region OnHyperlinkTextTapped
        /// <summary>
        /// Opens the hyperlink in the default browser, if it isn't empty or the user canceled the process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnHyperlinkTextTapped(object sender, EventArgs e)
        {
            string hyperlink = viewModel.GetHyperlink;

            if(hyperlink.Length > 0)
            {
                bool shouldOpenURL = await DisplayAlert(
                    "Web-Browser zum Link-Ziel öffnen?",
                    "Klicken Sie auf OK, um den Web-Browser zum ausgewählten Link-Ziel zu öffnen, oder brechen Sie den Vorgang ab.",
                    "Ok",
                    "Abbrechen"
                    );

                if (shouldOpenURL)
                {
                    await Launcher.OpenAsync(hyperlink);
                }
            }
        }
        #endregion

        #region DisplayTooltip
        /// <summary>
        /// Displays the previous generated Tooltips in the TooltipPopup.
        /// </summary>
        public void DisplayTooltip()
        {
            Navigation.ShowPopup(new TooltipPopup(tooltips));
        }
        #endregion

        #region GenerateTooltips
        /// <summary>
        /// Generates tooltips and adds them to the Tooltip-list.
        /// </summary>
        public void GenerateTooltips()
        {
            ResourceDictionary resourceDictionary = Application.Current.Resources;

            Tooltip searchMediaItemTooltip = new Tooltip()
            {
                Title = "Inhalte suchen",
                Description = "In diesem Suchfeld können Sie alle der App hinzugefügten multimedialen Inhalte durchsuchen. "
                              + "Mit den Auswahlboxen rechts daneben können Sie auf Wunsch bei der Suche einen Filter setzen.",
                Icon = resourceDictionary["FullScreenIcon"].ToString()
            };

            Tooltip deleteAllMediaItemTooltip = new Tooltip()
            {
                Title = "Alle multimediale Inhalte löschen",
                Description = "Durch diesen Button können Sie alle der App hinzugefügten multimediale Inhalte löschen," 
                              + " sofern Sie über das entsprechende Passwort verfügen.",
                Icon = resourceDictionary["DeleteIcon"].ToString()
            };

            Tooltip pickCSVFileTooltip = new Tooltip()
            {
                Title = "Lebensthemen und Metainformationen über CSV-Datei hinzufügen",
                Description = "Durch diesen Button können Sie Lebensthemen und Metainformation den Medienbausteinen hinzufügen," 
                              +" wenn deren Dateinamen in der CSV-Datei vorhanden sind."
                              + "Hierzu können Sie diese Datei von Ihrem Computer auswählen und der App hinzufügen.",
                Icon = resourceDictionary["ExportIcon"].ToString()
            };

            Tooltip addMediaItemsTooltip = new Tooltip()
            {
                Title = "Multimediale Inhalte hinzufügen",
                Description = "Durch diesen Button können Sie multimediale Inhalte von Ihrem Computer auswählen und der App hinzufügen.",
                Icon = resourceDictionary["ImportIcon"].ToString()
            };

            Tooltip pullMediaItemsFromBackendTooltip = new Tooltip()
            {
                Title = "Multimediale Inhalte aus dem Online-Content-Pool hinzufügen",
                Description = "Durch diesen Button können Sie multimediale Inhalte aus unserem Online-Content-Pool hinzufügen, "
                              +"sofern Sie über das entsprechende Passwort verfügen.",
                Icon = resourceDictionary["ImportFromBackendIcon"].ToString()
            };

            Tooltip assignLifethemesTooltip = new Tooltip()
            {
                Title = "Lebensthemen zuweisen",
                Description = "Durch diesen Button können Sie den Inhalten neue oder bestehende Lebensthemen zuweisen.",
                Icon = resourceDictionary["EditIcon"].ToString()
            };

            tooltips = new List<Tooltip>()
            {
                searchMediaItemTooltip,
                deleteAllMediaItemTooltip,
                pullMediaItemsFromBackendTooltip,
                pickCSVFileTooltip,
                addMediaItemsTooltip,
                assignLifethemesTooltip
            };
        }
        #endregion

        #region InitBackendCommunicationModules
        /// <summary>
        /// Inits the helper-classes of the BackendPullMediaItems-class and it's clientWebSocket.
        /// After that a request will be sent to the backend, that tells the app, if there are MediaItems for pulling.
        /// If an exception was thrown, the ImportFromBackendButton will be disabled.
        /// </summary>
        private async void InitBackendCommunicationModules()
        {
            /*
            * If previous page was the HomeView, the ContentView should check if new contents are available in online-content-pool.
            * If this isn't the case, it shouldn't check for new contents, because the previous page was the SessionEditView.
            * This eliminates waiting time in SessionEditView for checking if there are new contents available in ContentView.
            */
            //if (sQLiteController.GetAreBackendModulesEnabledField())
                //Check if there are new mediaitems in backend to download and enable/disable ImportMediaFromBackendButton depending on result.
                try
                {
                    backendPullMediaItems = new BackendPullMediaItems(this);
                    await backendPullMediaItems.InitHelper();
                    await backendPullMediaItems.InitWebSocket();
                    viewModel.IsImportFromBackendButtonVisible = await backendPullMediaItems.ShouldDownloadMediaItemsFromBackend();
                }
                catch (Exception)
                {
                    viewModel.IsImportFromBackendButtonVisible = false;
                }
            /**}
            else
            {
                ImportMediaFromBackendButton.IsEnabled = false;
            }**/
        }
        #endregion

        #region DisplayImportMediaFromBackendByPushDialog
        /// <summary>
        /// Displays an prompt for pulling MediaItems from backend and pulls them if the user accepts it.
        /// Displays an error message if an error happens.
        /// </summary>
        public async void DisplayImportMediaFromBackendByPushDialog()
        {
            try
            {
                bool shouldDownloadMedia = await DisplayAlert(
                    "Neue Medieninhalte aus dem Online-Content-Pool verfügbar",
                    "Möchten Sie neue Medieninhalte aus dem Online-Content-Pool herunterladen?",
                    "Herunterladen",
                    "Abbrechen"
                    );

                if (shouldDownloadMedia)
                {
                    viewModel.IsImportFromBackendButtonVisible = await backendPullMediaItems.PullContentsFromBackend();
                }

            } catch(Exception)
            {
                DisplayImportBackendMediaItemsErrorPrompt();
            }
        }
        #endregion

        #region DisplayDeleteMediaViaDeleteCommand
        /// <summary>
        /// Displays an prompt for deleting MediaItems and deletes them, when the user accepts this action.
        /// Displays an error prompt, when an exception occurs.
        /// </summary>
        /// <param name="appMediaItemIDs"></param>
        public async void DisplayDeleteMediaViaDeleteCommand(List<int> appMediaItemIDs)
        {
            try
            {
                List<MediaItem> mediaItems = new List<MediaItem>();
                string mediaItemsNames = "";

                foreach (int appMediaItemID in appMediaItemIDs)
                {
                    MediaItem mediaItem = await MediaItemDBHandler.GetSingleMediaItem(appMediaItemID);

                    if (mediaItem != null)
                    {
                        mediaItemsNames += mediaItem.Name + ", ";
                        mediaItems.Add(mediaItem);
                    }

                }

                bool shouldDeleteMediaItems = await DisplayAlert(
                    "Ein Admin möchte Inhalte von Ihrem Gerät löschen",
                    "Möchten Sie, dass folgende Inhalte gelöscht werden: '" + mediaItemsNames + "' ?",
                    "Löschen",
                    "Abbrechen"
                    );

                if (shouldDeleteMediaItems)
                {
                    await backendPullMediaItems.DeleteMediaItemsRetrievedByWebSocket(mediaItems);
                }

            } catch (Exception)
            {
                await DisplayAlert("Fehler beim Löschen der Inhalte", "Beim Löschen der Inhalte kam es zu einem Fehler", "Schließen");
            }
        }
        #endregion

        #region DisplayDeleteLifethemesViaDeleteCommandDialog
        /// <summary>
        /// Displays an prompt for deleting Lifethemes and deletes them, when the user confirms the action.
        /// If an error occurs an error dialog will be prompted.
        /// </summary>
        /// <param name="lifethemeNames"></param>
        public async void DisplayDeleteLifethemesViaDeleteCommandDialog(List<string> lifethemeNames)
        {
            try
            {
                string lifethemeNamesString = "";

                foreach (string lifethemeName in lifethemeNames)
                {
                    lifethemeNamesString += lifethemeName + ", ";
                }

                bool shouldDeleteLifethemes = await DisplayAlert(
                    "Ein Admin möchte Lebensthemen von Ihrem Gerät löschen",
                    "Möchten Sie, dass folgende Lebensthemen gelöscht werden: '" + lifethemeNamesString + "' ?",
                    "Löschen",
                    "Abbrechen"
                    );

                if (shouldDeleteLifethemes)
                {
                    foreach (string lifethemeName in lifethemeNames)
                    {
                        int lifethemeId = await LifethemeDBHandler.GetLifethemeIDByName(lifethemeName);

                        if (lifethemeId != -1)
                        {
                            await MediaItemLifethemesDBHandler.UnbindMediaItemLifethemesByLifethemeId(lifethemeId);
                            await LifethemeDBHandler.DeleteLifetheme(lifethemeId);
                        }
                    }
                }
            } catch (Exception)
            {
                await DisplayAlert("Fehler beim Löschen der Lebensthemen", "Es trat ein Fehler auf beim Löschen der Lebensthemen", "Schließen");
            }
        }
        #endregion

        #region DisplayImportMediaFromBackendConnectionErrorDialog
        /// <summary>
        /// Displays an error prompt when the backend isn't reachable.
        /// </summary>
        public async void DisplayImportMediaFromBackendConnectionErrorDialog()
        {
            await DisplayAlert(
                "Fehler beim Verbindungsaufbau zum Online-Content-Pool", 
                "Es konnte keine Verbindung zum Online-Content-Pool hergestellt werden.", 
                "Schließen"
                );
        }
        #endregion

        #region OnImportMediaItemsFromBackendButtonClicked
        /// <summary>
        /// Imports MediaItems from backend.
        /// The user has to confirm the pulling process twice, by clicking on accept and by entering the password.
        /// If an error happens, an error prompt will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnImportMediaItemsFromBackendButtonClicked(object sender, EventArgs e)
        {
            try
            {
                bool shouldImportMediaItems = await DisplayAlert(
                    "Medieninhalte durch Online-Content-Pool aktualisieren?",
                    "Möchten Sie die aktuellen Inhalte aus unserem Content-Pool herunterladen?",
                    "Alles klar!",
                    "Abbrechen"
                    );

                while (shouldImportMediaItems)
                {
                    PasswordPopup.Result result = await Navigation.ShowPopupAsync(new PasswordPopup("Passwort zum Importieren von Medien aus unserem Online-Content-Pool"));

                    if (result != null)
                    {
                        if (result.IsPasswordValid)
                        {
                            viewModel.IsImportFromBackendButtonVisible = await backendPullMediaItems.PullContentsFromBackend();
                            
                            shouldImportMediaItems = false;
                        }
                        else
                        {
                            shouldImportMediaItems = await DisplayAlert("Falsches Passwort!", "Das eingegebenen Passwort " + result.InsertedPassword + " ist leider falsch.", "Alles klar!", "Abbrechen");
                        }
                    }
                    else
                    {
                        shouldImportMediaItems = false;
                    }
                }
            } catch (Exception)
            {
                DisplayImportBackendMediaItemsErrorPrompt();
            }
        }
        #endregion

        #region DisplayImportBackendMediaItemsErrorPrompt
        /// <summary>
        /// Displays an error prompt, when the pulling of MediaItems from backend fails.
        /// </summary>
        private async void DisplayImportBackendMediaItemsErrorPrompt()
        {
            await DisplayAlert(
                "Fehler beim Herunterladen von Medien aus dem Online-Content-Pool", 
                "Ein Fehler trat auf beim Herunterladen der Medien aus dem Online-Content-Pool", 
                "Schließen"
                );
        }
        #endregion

    }
}