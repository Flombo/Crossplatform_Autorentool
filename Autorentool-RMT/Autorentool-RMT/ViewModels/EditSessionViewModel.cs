using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Autorentool_RMT.ViewModels
{
    public class EditSessionViewModel : ViewModel
    {

        #region Attributes
        private List<MediaItem> sessionMediaItems;
        private bool isImageVisble;
        private bool isMediaElementVisible;
        private bool isMediaItemTextVisible;
        private string imagePath;
        private string mediaElementSource;
        private string mediaItemText;
        private Session selectedSession;
        private MediaItem selectedMediaItem;
        private bool isUnbindMediaItemButtonEnabled;
        private string unbindMediaItemButtonBackgroundColour;
        public MediaItem DraggedMediaItem { get; set; }
        #endregion

        #region Constructor
        public EditSessionViewModel(Session selectedSession)
        {
            sessionMediaItems = new List<MediaItem>();
            this.selectedSession = selectedSession;
            isImageVisble = true;
            imagePath = "preview.png";
            selectedMediaItem = null;
            isMediaElementVisible = false;
            mediaElementSource = null;
            mediaItemText = "";
            isMediaItemTextVisible = false;
            unbindMediaItemButtonBackgroundColour = "LightGray";
        }
        #endregion

        #region IsUnbindMediaItemButtonEnabled
        public bool IsUnbindMediaItemButtonEnabled
        {
            get => isUnbindMediaItemButtonEnabled;
            set
            {
                isUnbindMediaItemButtonEnabled = value;
                UnbindMediaItemButtonBackgroundColour = GetBackgroundColour(isUnbindMediaItemButtonEnabled, "#0091EA");
                OnPropertyChanged();
            }
        }
        #endregion

        #region UnbindMediaItemButtonBackgroundColour
        public string UnbindMediaItemButtonBackgroundColour
        {
            get => unbindMediaItemButtonBackgroundColour;
            set
            {
                unbindMediaItemButtonBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsMediaItemTextVisible
        public bool IsMediaItemTextVisible
        {
            get => isMediaItemTextVisible;
            set
            {
                isMediaItemTextVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region MediaItemText
        public string MediaItemText
        {
            get => mediaItemText;
            set
            {
                mediaItemText = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SelectedMediaItem
        public MediaItem SelectedMediaItem
        {
            get => selectedMediaItem;
            set
            {
                selectedMediaItem = value;
                SetPreviewProperties();
                IsUnbindMediaItemButtonEnabled = selectedMediaItem != null;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SetPreviewProperties
        /// <summary>
        /// Sets the media preview properties 
        /// (IsImageVisible, IsMediaElementVisible, IsMediaItemTextVisible, MediaElementSource, ImagePath and MediaItemText) depending if the selected medium is an image or not.
        /// For the MediaElement it is necessary to reload the selected mediaitem, because the selected mediaitem contains the video icon as path and not the medium path.
        /// An uri instance must be built of the path, to retrieve local media on android.
        /// This is necessary, because the MediaElement doesn't support images.
        /// </summary>
        private void SetPreviewProperties()
        {
            if(selectedMediaItem != null)
            {
                IsImageVisible = selectedMediaItem.IsImage;
                IsMediaElementVisible = selectedMediaItem.IsAudioOrVideo;
                IsMediaItemTextVisible = selectedMediaItem.IsTxt;
                ImagePath = selectedMediaItem.GetFullPath;
                MediaElementSource = selectedMediaItem.GetAudioOrVideoSource;
                MediaItemText = selectedMediaItem.GetTextContent;
            }
            else
            {
                IsImageVisible = true;
                IsMediaElementVisible = false;
                IsMediaItemTextVisible = false;
                MediaElementSource = null;
                MediaItemText = "";
                ImagePath = "preview.png";
            }
        }
        #endregion

        #region IsImageVisible
        public bool IsImageVisible
        {
            get => isImageVisble;
            set
            {
                isImageVisble = value;
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

        #region ImagePath
        public string ImagePath
        {
            get => imagePath;
            set
            {
                imagePath = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region MediaElementSource
        public string MediaElementSource
        {
            get => mediaElementSource;
            set
            {
                mediaElementSource = value;
                OnPropertyChanged();
            }
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
        public async Task OnLoadAllSessionMediaItems()
        {
            SessionMediaItems = await SessionMediaItemsDBHandler.GetMediaItemsOfSession(selectedSession.Id);

        }
        #endregion

        #region OnUnbindMediaItemFromSession
        /// <summary>
        /// Unbinds the selected mediaitem from the selected session
        /// </summary>
        /// <returns></returns>
        public async Task OnUnbindMediaItemFromSession()
        {
            try
            {
                int sessionMediaItemsID = await SessionMediaItemsDBHandler.GetID(selectedMediaItem.Id, selectedSession.Id);
                await SessionMediaItemsDBHandler.UnbindCertainSessionMediaItems(sessionMediaItemsID);
                SelectedMediaItem = null;

                await OnLoadAllSessionMediaItems();
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region ChangePosition
        /// <summary>
        /// Changes position of the dragged mediaitem and the mediaitem, where it dropped.
        /// </summary>
        /// <param name="targetMediaItem"></param>
        public async Task ChangePosition(MediaItem targetMediaItem)
        {
            if(targetMediaItem.Id != DraggedMediaItem.Id)
            {
                

                int targetIndex = sessionMediaItems.IndexOf(targetMediaItem);
                int draggedMediaItemIndex = sessionMediaItems.IndexOf(DraggedMediaItem);

                await MediaItemDBHandler.UpdatePosition(targetMediaItem.Id, draggedMediaItemIndex + 1);
                await MediaItemDBHandler.UpdatePosition(DraggedMediaItem.Id, targetIndex + 1);

                await OnLoadAllSessionMediaItems();
            }
        }
        #endregion
    }
}
