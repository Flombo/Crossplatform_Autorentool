using Autorentool_RMT.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Autorentool_RMT.ViewModels
{
    public class EditSessionViewModel : INotifyPropertyChanged
    {

        private List<MediaItem> sessionMediaItems;

        #region Constructor
        public EditSessionViewModel()
        {
            sessionMediaItems = new List<MediaItem>();
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
        /// Getter and Setter for the sessionMediaItems-List.
        /// UI retrieves over this method the MediaItems and sets new MediaItems.
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

        #region OnLoadAllSessionMediaItems
        /// <summary>
        /// Loads all existing MediaItems into SessionMediaItems-property.
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadAllSessions()
        {
            SessionMediaItems = new List<MediaItem>()
                {
                    {new MediaItem(1, "Test", "png", "ImageOld.png", "Test", 0) },
                    {new MediaItem(2, "Hello", "png", "ImageOld.png", "Test2", 0) }
                };

        }
        #endregion
    }
}
