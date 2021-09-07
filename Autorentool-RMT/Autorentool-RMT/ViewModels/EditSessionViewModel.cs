using Autorentool_RMT.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autorentool_RMT.ViewModels
{
    public class EditSessionViewModel : ViewModel
    {

        private List<MediaItem> sessionMediaItems;

        #region Constructor
        public EditSessionViewModel()
        {
            sessionMediaItems = new List<MediaItem>();
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
                    {new MediaItem(1, "Test", "png", "sdfkasjkfklösalöslöfd", "ImageOld.png", "Test", 0) },
                    {new MediaItem(2, "Hello", "png", "sdfkasjkfklösalöslöfd222","ImageOld.png", "Test2", 0) }
                };

        }
        #endregion
    }
}
