using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autorentool_RMT.ViewModels
{
    public class SessionViewModel : ViewModel
    {

        #region Attributes
        private List<Session> sessions;
        private List<MediaItem> selectedSessionMediaItems;
        private Session selectedSession;
        private bool isStartSessionButtonEnabled;
        private string startSessionButtonBackgroundColour;
        private string editSessionButtonBackgroundColour;
        private bool isEditSessionButtonEnabled;
        private string deleteSessionButtonBackgroundColour;
        private bool isDeleteSessionButtonEnabled;
        private string changeNameSessionButtonBackgroundColour;
        private bool isChangeNameSessionButtonEnabled;
        private string enabledButtonBackgroundColour = "#0091EA";
        private string selectedSessionText;
        private bool isSelectedSessionTextVisible;
        private string exportSessionButtonBackgroundColour;
        private bool isExportSessionButtonEnabled;
        private bool isProgressBarVisible;
        private int progress;
        private string statusText;
        #endregion

        #region Constructor
        public SessionViewModel()
        {
            selectedSessionMediaItems = new List<MediaItem>();
            sessions = new List<Session>();
            selectedSession = null;
            isStartSessionButtonEnabled = false;
            startSessionButtonBackgroundColour = "LightGray";
            isEditSessionButtonEnabled = false;
            editSessionButtonBackgroundColour = "LightGray";
            isDeleteSessionButtonEnabled = false;
            deleteSessionButtonBackgroundColour = "LightGray";
            isChangeNameSessionButtonEnabled = false;
            changeNameSessionButtonBackgroundColour = "LightGray";
            exportSessionButtonBackgroundColour = "LightGray";
            selectedSessionText = "";
            isSelectedSessionTextVisible = false;
            isExportSessionButtonEnabled = false;
            progress = 0;
            statusText = "";
            isProgressBarVisible = false;
        }
        #endregion

        #region IsProgressBarVisible
        public bool IsProgressBarVisible
        {
            get => isProgressBarVisible;
            set
            {
                isProgressBarVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Progress
        public int Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region StatusText
        public string StatusText
        {
            get => statusText;
            set
            {
                statusText = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsExportSessionButtonEnabled
        public bool IsExportSessionButtonEnabled
        {
            get => isExportSessionButtonEnabled;
            set
            {
                isExportSessionButtonEnabled = value;
                ExportSessionButtonBackgroundColour = GetBackgroundColour(isExportSessionButtonEnabled, "Green");
                OnPropertyChanged();
            }
        }
        #endregion

        #region ExportSessionButtonBackgroundColour
        public string ExportSessionButtonBackgroundColour
        {
            get => exportSessionButtonBackgroundColour;
            set
            {
                exportSessionButtonBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SelectedSessionText
        public string SelectedSessionText
        {
            get => selectedSessionText;
            set
            {
                selectedSessionText = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsSelectedSessionTextVisible
        public bool IsSelectedSessionTextVisible
        {
            get => isSelectedSessionTextVisible;
            set
            {
                isSelectedSessionTextVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsSelectedSessionTextVisible
        #endregion

        #region DeleteSessionButtonBackgroundColour
        public string DeleteSessionButtonBackgroundColour
        {
            get => deleteSessionButtonBackgroundColour;
            set
            {
                deleteSessionButtonBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsDeleteSessionButtonEnabled
        public bool IsDeleteSessionButtonEnabled
        {
            get => isDeleteSessionButtonEnabled;
            set
            {
                isDeleteSessionButtonEnabled = value;
                DeleteSessionButtonBackgroundColour = GetBackgroundColour(isDeleteSessionButtonEnabled, enabledButtonBackgroundColour);
                OnPropertyChanged();
            }
        }
        #endregion

        #region ChangeNameSessionButtonBackgroundColour
        public string ChangeNameSessionButtonBackgroundColour
        {
            get => changeNameSessionButtonBackgroundColour;
            set
            {
                changeNameSessionButtonBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsChangeNameSessionButtonEnabled
        public bool IsChangeNameSessionButtonEnabled
        {
            get => isChangeNameSessionButtonEnabled;
            set
            {
                isChangeNameSessionButtonEnabled = value;
                ChangeNameSessionButtonBackgroundColour = GetBackgroundColour(isChangeNameSessionButtonEnabled, enabledButtonBackgroundColour);
                OnPropertyChanged();
            }
        }
        #endregion

        #region EditSessionButtonBackgroundColour
        public string EditSessionButtonBackgroundColour
        {
            get => editSessionButtonBackgroundColour;
            set
            {
                editSessionButtonBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsEditSessionButtonEnabled
        public bool IsEditSessionButtonEnabled
        {
            get => isEditSessionButtonEnabled;
            set
            {
                isEditSessionButtonEnabled = value;
                EditSessionButtonBackgroundColour = GetBackgroundColour(isEditSessionButtonEnabled, enabledButtonBackgroundColour);
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsStartSessionButtonEnabled
        public bool IsStartSessionButtonEnabled
        {
            get => isStartSessionButtonEnabled;
            set
            {
                isStartSessionButtonEnabled = value;
                StartSessionButtonBackgroundColour = GetBackgroundColour(isStartSessionButtonEnabled, enabledButtonBackgroundColour);
                OnPropertyChanged();
            }
        }
        #endregion

        #region StartSessionButtonBackgroundColour
        public string StartSessionButtonBackgroundColour
        {
            get => startSessionButtonBackgroundColour;
            set
            {
                startSessionButtonBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SelectedSession
        public Session SelectedSession
        {
            get => selectedSession;
            set
            {
                selectedSession = value;
                LoadSelectedSessionMediaItems();
                EnableOrDisableButtons();
                IsStartSessionButtonEnabled = selectedSession != null;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SetSelectedSessionText
        /// <summary>
        /// Sets the visibility and the text property of the SelectedSessionText-property.
        /// </summary>
        private void SetSelectedSessionText()
        {
            IsSelectedSessionTextVisible = selectedSession != null;
            
            if (selectedSession != null)
            {
                SelectedSessionText = $"Die Sitzung '{selectedSession.Name}' hat {SelectedSessionMediaItems.Count} Baustein(e)";
            }
        }
        #endregion

        #region EnableOrDisableButtons
        /// <summary>
        /// Enables or disables buttons depending on the selectedSession-property.
        /// </summary>
        private void EnableOrDisableButtons()
        {
            bool shouldBeEnabled = selectedSession != null;
            IsChangeNameSessionButtonEnabled = shouldBeEnabled;
            IsDeleteSessionButtonEnabled = shouldBeEnabled;
            IsEditSessionButtonEnabled = shouldBeEnabled;
            IsExportSessionButtonEnabled = selectedSession != null;
        }
        #endregion

        #region LoadSelectedSessionMediaItems
        /// <summary>
        /// Loads the mediaitems of the selected session if it isn't null.
        /// Sets the SelectedSessionMediaItems with the retrieved mediaitems.
        /// Else an empty list will be set.
        /// </summary>
        private async void LoadSelectedSessionMediaItems()
        {
            try
            {
                if (selectedSession != null)
                {
                    SelectedSessionMediaItems = await SessionMediaItemsDBHandler.GetMediaItemsOfSession(selectedSession.Id);
                    IsStartSessionButtonEnabled = SelectedSessionMediaItems.Count > 0;
                }
                else
                {
                    SelectedSessionMediaItems = new List<MediaItem>();
                    IsStartSessionButtonEnabled = false;
                }

                SetSelectedSessionText();
            } catch(Exception)
            {

            }
        }
        #endregion

        #region SelectedSessionMediaItems
        /// <summary>
        /// Getter and Setter for the selectedSessionMediaItems-List.
        /// UI retrieves over this method the MediaItems and sets new MediaItems.
        /// Setter calls OnPropertyChanged for updating the UI.
        /// </summary>
        public List<MediaItem> SelectedSessionMediaItems
        {
            get => selectedSessionMediaItems;
            set
            {
                selectedSessionMediaItems = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Sessions
        /// <summary>
        /// Getter and Setter for the sessions-List.
        /// UI retrieves over this method the Sessions and sets new Sessions.
        /// Setter calls OnPropertyChanged for updating the UI.
        /// </summary>
        public List<Session> Sessions
        {
            get => sessions;
            set
            {
                sessions = value;
                OnPropertyChanged();
            }
        }

        public Task SessionLifethemesDBHanlder { get; private set; }
        #endregion

        #region OnLoadAllSessions
        /// <summary>
        /// Loads all existing Sessions into Sessions-property.
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadAllSessions()
        {

            SelectedSession = null;
            Sessions = await SessionDBHandler.GetAllSessions();

        }
        #endregion

        #region OnDeleteSession
        /// <summary>
        /// Unbinds selected session from all mediaitems/residents and deletes the session.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <returns></returns>
        public async Task OnDeleteSession()
        {
            if(selectedSession != null)
            {
                try
                {

                    await SessionMediaItemsDBHandler.UnbindSessionMediaItemsBySessionId(selectedSession.Id);
                    await ResidentSessionsDBHandler.UnbindAllResidentSessionsBySessionId(selectedSession.Id);
                    await SessionDBHandler.DeleteSession(selectedSession.Id);

                    selectedSession = null;
                    SelectedSessionMediaItems = new List<MediaItem>();

                    EnableOrDisableButtons();
                    IsSelectedSessionTextVisible = false;
                    SelectedSessionText = "";

                } catch(Exception exc)
                {
                    throw exc;
                }
            }
        }
        #endregion

        #region SetProgressAndStatus
        /// <summary>
        /// Calculates the currentProgress and sets the StatusText
        /// </summary>
        /// <param name="currentProgress"></param>
        /// <param name="maxProgress"></param>
        public void SetProgressAndStatus(int currentProgress, int maxProgress)
        {
            Progress = currentProgress / maxProgress;
            StatusText = (Progress * 100) + "%";
        }
        #endregion

        #region DisableSessionButtons
        /// <summary>
        /// Disables SessionButtons after exporting/importing a session.
        /// </summary>
        public void DisableSessionButtons()
        {
            IsDeleteSessionButtonEnabled = false;
            IsChangeNameSessionButtonEnabled = false;
            IsEditSessionButtonEnabled = false;
            IsSelectedSessionTextVisible = false;
            IsStartSessionButtonEnabled = false;
        }
        #endregion

        #region EnableSessionButtons
        /// <summary>
        /// Enables SessionButtons after exporting/importing a session.
        /// </summary>
        public void EnableSessionButtons()
        {
            IsDeleteSessionButtonEnabled = true;
            IsChangeNameSessionButtonEnabled = true;
            IsEditSessionButtonEnabled = true;
            IsSelectedSessionTextVisible = true;
            IsStartSessionButtonEnabled = true;
        }
        #endregion

    }
}
