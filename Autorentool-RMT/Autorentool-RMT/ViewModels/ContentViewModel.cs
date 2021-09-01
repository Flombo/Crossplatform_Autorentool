using Autorentool_RMT.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autorentool_RMT.ViewModels
{
    public class ContentViewModel : ViewModel
    {

        private List<MediaItem> mediaItems;
        private List<Lifetheme> currentMediaItemLifethemes;

        #region Constructor
        public ContentViewModel()
        {
            mediaItems = new List<MediaItem>();
            currentMediaItemLifethemes = new List<Lifetheme>();
        }
        #endregion

        #region MediaItems
        /// <summary>
        /// Getter and Setter for the mediaItems-List.
        /// UI retrieves over this method the MediaItems and sets new MediaItemss.
        /// Setter calls OnPropertyChanged for updating the UI.
        /// </summary>
        public List<MediaItem> MediaItems
        {
            get => mediaItems;
            set
            {
                mediaItems = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region CurrentMediaLifethemes
        /// <summary>
        /// Getter and Setter for the currentMediaItemLifethemes-List.
        /// UI retrieves over this method the Lifethemes and sets new Lifethemes.
        /// Setter calls OnPropertyChanged for updating the UI.
        /// </summary>
        public List<Lifetheme> CurrentMediaItemLifethemes
        {
            get => currentMediaItemLifethemes;
            set
            {
                currentMediaItemLifethemes = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region OnLoadAllMediaItems
        /// <summary>
        /// Loads all existing MediaItems into MediaItems-property.
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadAllMediaItems()
        {
            //MediaItems = await MediaItemDBHandler.GetAllMediaItems();
            MediaItems = new List<MediaItem>()
            {
                {new MediaItem(1, "test.jpg", "jpg", "ImageOld.png", "Test", 0) },
                {new MediaItem(2, "test2.jpg", "jpg", "ImageOld.png", "Test2", 0) }
            };

            CurrentMediaItemLifethemes = new List<Lifetheme>()
            {
                {new Lifetheme("Test") }
            };
        }
        #endregion
    }
}
