using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
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
        private int duration;
        private bool isNotesPanelVisible;
        private string selectedMediaItemNotes;
        private bool isPreviousItemButtonVisible;
        private bool isNextItemButtonVisible;
        public ICommand PreviousItemButtonClicked { get; }
        public ICommand NextItemButtonClicked { get; }

        #region Constructor
        public PlaySessionContentViewModel(Session selectedSession)
        {
            this.selectedSession = selectedSession;
            PreviousItemButtonClicked = new Command(SetPreviousItemAsCurrentMediaItem);
            NextItemButtonClicked = new Command(SetNextItemAsCurrentMediaItem);
            isNotesPanelVisible = false;
            duration = 0;
            isSessionOngoing = false;
            isNotesPanelVisible = false;
            isPreviousItemButtonVisible = false;
            isNextItemButtonVisible = true;
        }
        #endregion

        #region IsPreviousItemButtonVisible
        public bool IsPreviousItemButtonVisible
        {
            get => isPreviousItemButtonVisible;
            set
            {
                isPreviousItemButtonVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsNextItemButtonVisible
        public bool IsNextItemButtonVisible
        {
            get => isNextItemButtonVisible;
            set
            {
                isNextItemButtonVisible = value;
                OnPropertyChanged();
            }
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
                SetControlButtonsVisibility();
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
                if (isSessionOngoing)
                {
                    duration++;
                    selectedSession.DurationInSeconds = duration;
                    SessionDuration = selectedSession.ConvertMinutes + selectedSession.ConvertSeconds;
                }

                return isSessionOngoing;
            });
        }
        #endregion

        #region OnStartSession
        /// <summary>
        /// Stops the Stopwatch and persists the duration.
        /// </summary>
        public Session StopSession()
        {
            isSessionOngoing = false;
            return selectedSession;
        }
        #endregion

        #region OnLoadAllMediaItems
        /// <summary>
        /// Loads all existing MediaItems into SessionMediaItems-property.
        /// Throws an exception if an error occurs while loading.
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadAllMediaItems()
        {
            try
            {
                SessionMediaItems = await SessionMediaItemsDBHandler.GetMediaItemsOfSession(selectedSession.Id);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region SetControlButtonsVisibility
        /// <summary>
        /// Sets the properties IsPreviousItemButtonVisible and IsNextItemButtonVisible depending on the position of the current displayed MediaItem.
        /// If the currentMediaItem is null due to intializiation, the previous button must be invisible, because the first item is displayed.
        /// </summary>
        public void SetControlButtonsVisibility()
        {
            if(currentMediaItem != null)
            {
                int indexOfCurrentMediaItem = sessionMediaItems.IndexOf(currentMediaItem);

                IsPreviousItemButtonVisible = indexOfCurrentMediaItem != 0;
                IsNextItemButtonVisible = indexOfCurrentMediaItem != sessionMediaItems.Count - 1;
            }
            else
            {
                IsPreviousItemButtonVisible = false;
                IsNextItemButtonVisible = true;
            }
        }
        #endregion

        #region SetPreviousItemAsCurrentMediaItem
        /// <summary>
        /// Changes the currentMediaItem to the previous MediaItem.
        /// If the currentMediaItem isn't part of the list due to initialization, the first MediaItem will be choosed.
        /// </summary>
        private void SetPreviousItemAsCurrentMediaItem()
        {
            int currentMediaItemIndex = SessionMediaItems.IndexOf(CurrentMediaItem);

            if (currentMediaItemIndex != -1)
            {
                MediaItem previousMediaItem = SessionMediaItems[currentMediaItemIndex - 1];
                CurrentMediaItem = previousMediaItem;
            } else
            {
                CurrentMediaItem = SessionMediaItems[0];
            }
        }
        #endregion

        #region SetNextItemAsCurrentMediaItem
        /// <summary>
        /// Changes the currentMediaItem to the next MediaItem.
        /// If the currentMediaItem isn't part of the list due to initialization, the first MediaItem will be choosed.
        /// </summary>
        private void SetNextItemAsCurrentMediaItem()
        {
            int currentMediaItemIndex = SessionMediaItems.IndexOf(CurrentMediaItem);

            if (currentMediaItemIndex != -1)
            {
                MediaItem nextMediaItem = SessionMediaItems[currentMediaItemIndex + 1];
                CurrentMediaItem = nextMediaItem;
            } else
            {
                CurrentMediaItem = SessionMediaItems[0];
            }
        }
        #endregion
    }
}
