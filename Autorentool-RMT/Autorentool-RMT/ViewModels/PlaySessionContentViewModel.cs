using Autorentool_RMT.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Autorentool_RMT.ViewModels
{
    public class PlaySessionContentViewModel : INotifyPropertyChanged
    {

        private List<MediaItem> sessionMediaItems;
        private bool isImageVisible;
        private bool isMediaElementVisible;
        private ImageSource currentImageMediaItem;
        private int currentMediaItemIndex;
        private string sessionDuration;
        private string currentVideoOrAudioMediaItemPath;
        private Session session;
        private Stopwatch stopwatch;
        private bool isPreviousButtonVisible;
        private bool isNextButtonVisible;

        #region Constructor
        public PlaySessionContentViewModel()
        {
            session = new Session(1, "Test", new List<MediaItem>(), null);
            currentMediaItemIndex = 0;
            IsPreviousButtonVisible = false;
            IsNextButtonVisible = true;
            sessionMediaItems = new List<MediaItem>();
            CurrentVideoOrAudioMediaItemPath = "https://sec.ch9.ms/ch9/5d93/a1eab4bf-3288-4faf-81c4-294402a85d93/XamarinShow_mid.mp4";
            IsImageVisible = false;
            IsMediaElementVisible = true;
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

        #region IsMediaElementVisible
        public bool IsMediaElementVisible
        {
            get => isMediaElementVisible;
            set
            {
                isMediaElementVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsImageVisible
        public bool IsImageVisible
        {
            get => isImageVisible;
            set
            {
                isImageVisible = value;
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

        #region CurrentImageMediaItem
        public ImageSource CurrentImageMediaItem
        {
            get => currentImageMediaItem;
            set
            {
                currentImageMediaItem = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region CurrentVideoOrAudioMediaItemPath
        public string CurrentVideoOrAudioMediaItemPath
        {
            get => currentVideoOrAudioMediaItemPath;
            set
            {
                currentVideoOrAudioMediaItemPath = value;
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
            stopwatch = new Stopwatch();
            stopwatch.Start();


                session.DurationInSeconds = (int)stopwatch.ElapsedMilliseconds / 60;
                SessionDuration = session.ConvertMinutes + session.ConvertSeconds;
            
        }
        #endregion

        #region OnStartSession
        /// <summary>
        /// Stops the Stopwatch.
        /// </summary>
        public void StopSession()
        {
            stopwatch.Stop();
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

            CurrentImageMediaItem = SessionMediaItems[currentMediaItemIndex].GetFullPath;
        }
        #endregion
    }
}
