using Autorentool_RMT.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Autorentool_RMT.ViewModels
{
    public class PlaySessionContentViewModel : INotifyPropertyChanged
    {

        private List<MediaItem> sessionMediaItems;
        private string sessionDuration;
        private Session session;
        private bool isSessionOngoing;
        private bool isPreviousButtonVisible;
        private bool isNextButtonVisible;
        private int duration;

        #region Constructor
        public PlaySessionContentViewModel()
        {
            session = new Session(1, "Test", new List<MediaItem>(), null);
            duration = 0;
            IsPreviousButtonVisible = false;
            isSessionOngoing = false;
            IsNextButtonVisible = true;
            SessionMediaItems = new List<MediaItem>()
            {
                {new MediaItem(1, "test.jpg", "jpg", "ImageOld.png", "Test", 0) },
                {new MediaItem(2, "test2.jpg", "jpg", "ImageOld.png", "Test2", 0) }
            };
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region OnPropertyChanged
        /// <summary>
        /// Calls the corresponding method for the OnPropertyChanged-event.
        /// </summary>
        /// <param name="name"></param>
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
                session.DurationInSeconds = duration;
                SessionDuration = session.ConvertMinutes + session.ConvertSeconds;

                return isSessionOngoing;
            });
        }
        #endregion

        #region OnStartSession
        /// <summary>
        /// Stops the Stopwatch.
        /// </summary>
        public void StopSession()
        {
            isSessionOngoing = false;
            duration = 0;
        }
        #endregion

        #region OnLoadAllMediaItems
        /// <summary>
        /// Loads all existing MediaItems into SessionMediaItems-property.
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadAllMediaItems()
        {
            //MediaItems = await MediaItemDBHandler.GetAllMediaItems();
            SessionMediaItems = new List<MediaItem>()
            {
                {new MediaItem(1, "test.jpg", "jpg", "ImageOld.png", "Test", 0) },
                {new MediaItem(2, "test2.jpg", "jpg", "ImageOld.png", "Test2", 0) }
            };
        }
        #endregion
    }
}
