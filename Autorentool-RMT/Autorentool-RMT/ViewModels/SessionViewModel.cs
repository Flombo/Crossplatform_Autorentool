using Autorentool_RMT.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Autorentool_RMT.ViewModels
{
    public class SessionViewModel : INotifyPropertyChanged
    {

        private List<Session> sessions;
        private List<MediaItem> selectedSessionMediaItems;

        #region Constructor
        public SessionViewModel()
        {
            selectedSessionMediaItems = new List<MediaItem>();
            sessions = new List<Session>();
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
        #endregion

        #region OnLoadAllSessions
        /// <summary>
        /// Loads all existing Sessions into Sessions-property.
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadAllSessions()
        {
            SelectedSessionMediaItems = new List<MediaItem>()
                {
                    {new MediaItem(1, "Test", "png", "ImageOld.png", "Test", 0) }
                };

            Sessions = new List<Session>()
                {
                    {new Session(1, "Test", SelectedSessionMediaItems, null) },
                    {new Session(2, "Test1", SelectedSessionMediaItems, null) }
                };

        }
        #endregion
    }
}
