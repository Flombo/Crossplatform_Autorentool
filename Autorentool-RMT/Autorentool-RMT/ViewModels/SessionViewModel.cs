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
            selectedSessionText = "";
            isSelectedSessionTextVisible = false;
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
    }
}
