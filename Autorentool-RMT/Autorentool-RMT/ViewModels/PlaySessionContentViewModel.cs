using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Autorentool_RMT.ViewModels
{
    public class PlaySessionContentViewModel : ViewModel
    {

        private List<MediaItem> sessionMediaItems;
        private MediaItem currentMediaItem;
        private string sessionDuration;
        private Session selectedSession;
        private bool isSessionOngoing;
        private bool isPreviousButtonVisible;
        private bool isNextButtonVisible;
        private int duration;
        private bool isNotesPanelVisible;
        private string selectedMediaItemNotes;

        #region Constructor
        public PlaySessionContentViewModel(Session selectedSession)
        {
            this.selectedSession = selectedSession;
            isNotesPanelVisible = false;
            duration = 0;
            IsPreviousButtonVisible = false;
            isSessionOngoing = false;
            IsNextButtonVisible = true;
            isNotesPanelVisible = false;
        }
        #endregion

        #region SelectedMediaItemNotes
        public string SelectedMediaItemNotes
        {
            get => selectedMediaItemNotes;
            set
            {
                selectedMediaItemNotes = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsNotesPanelVisible
        public bool IsNotesPanelVisible
        {
            get => isNotesPanelVisible;
            set
            {
                isNotesPanelVisible = value;
                SelectedMediaItemNotes = CurrentMediaItem.Notes;
                OnPropertyChanged();
            }
        }
        #endregion

        #region CurrentMediaItem
        public MediaItem CurrentMediaItem
        {
            get => currentMediaItem;
            set
            {
                currentMediaItem = value;
                IsNotesPanelVisible = currentMediaItem.Notes.Length > 0;
                OnPropertyChanged();
            }
        }
        #endregion

        #region MediaItems
        /// <summary>
        /// Getter and Setter for the sessionMediaItems-List.
        /// UI retrieves over this method the MediaItems and sets new MediaItemss.
        /// Setter calls OnPropertyChanged for updating the UI.
        /// </summary>
        public List<MediaItem> SessionMediaItems
        {
            get => sessionMediaItems;
            set
            {
                sessionMediaItems = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsNextButtonVisible
        public bool IsNextButtonVisible
        {
            get => isNextButtonVisible;
            set
            {
                isNextButtonVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsPreviousButtonVisible
        public bool IsPreviousButtonVisible
        {
            get => isPreviousButtonVisible;
            set
            {
                isPreviousButtonVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SessionDuration
        public string SessionDuration
        {
            get => sessionDuration;
            set
            {
                sessionDuration = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region OnStartSession
        /// <summary>
        /// Starts the Stopwatch and sets the SessionDuration-property.
        /// </summary>
        public void StartSession()
        {
            duration = 0;
            isSessionOngoing = true;

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                duration++;
                selectedSession.DurationInSeconds = duration;
                SessionDuration = selectedSession.ConvertMinutes + selectedSession.ConvertSeconds;

                return isSessionOngoing;
            });
        }
        #endregion

        #region OnStartSession
        /// <summary>
        /// Stops the Stopwatch and persists the duration.
        /// </summary>
        public async void StopSession()
        {
            try
            {
                isSessionOngoing = false;
                duration = 0;
                await SessionDBHandler.UpdateDurationInSeconds(selectedSession.Id, selectedSession.DurationInSeconds);
            }
            catch (Exception exc)
            {

            }
        }
        #endregion

        #region OnLoadAllMediaItems
        /// <summary>
        /// Loads all existing MediaItems into SessionMediaItems-property.
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadAllMediaItems()
        {
            try
            {
                SessionMediaItems = await SessionMediaItemsDBHandler.GetMediaItemsOfSession(selectedSession.Id);
            }
            catch (Exception)
            {

            }
        }
        #endregion
    }
}
