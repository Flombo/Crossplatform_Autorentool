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
    public class ContentViewModel : ViewModel
    {

        #region Attributes
        protected List<MediaItem> mediaItems;
        protected List<Lifetheme> currentMediaItemLifethemes;
        protected MediaItem selectedMediaItem;
        protected string selectedMediumImagePath;
        protected string selectedMediumMediaElementPath;
        protected string notes;
        protected bool isMediaItemImageVisible;
        protected bool isMediaItemMediaElementVisible;
        protected bool isFullscreenButtonVisible;
        protected bool isDeleteSelectedMediaItemButtonEnabled;
        protected string deleteSelectedMediaItemButtonBackgroundColour;
        protected bool isDeleteAllMediaItemsButtonEnabled;
        protected string deleteAllMediaItemsButtonBackgroundcolour;
        protected string lifethemesBackgroundColour;
        protected bool isLifethemesButtonEnabled;
        protected bool isLifethemesButtonVisible;
        protected bool isPhotosFilterChecked;
        protected bool isFilmsFilterChecked;
        protected bool isMusicFilterChecked;
        protected bool isDocumentsFilterChecked;
        protected bool isLinksFilterChecked;
        protected string searchText;
        protected bool isMediaItemTextVisible;
        protected string selectedMediumTextContent;
        protected bool isProgressBarVisible;
        protected float progress;
        protected string statusText;
        protected string progressText;
        protected bool isAddMediaItemButtonVisible;
        protected bool isAddMediaItemButtonEnabled;
        protected string addMediaItemButtonBackgroundColour;
        protected bool isContentPage;
        protected string title;
        #endregion

        #region Constructor
        public ContentViewModel()
        {
            mediaItems = new List<MediaItem>();
            notes = "";
            searchText = "";
            title = "";
            selectedMediumImagePath = "preview.png";
            selectedMediumMediaElementPath = null;
            isMediaItemImageVisible = true;
            isMediaItemMediaElementVisible = false;
            isFullscreenButtonVisible = false;
            isDeleteSelectedMediaItemButtonEnabled = false;
            isDeleteAllMediaItemsButtonEnabled = false;
            deleteSelectedMediaItemButtonBackgroundColour = "LightGray";
            deleteAllMediaItemsButtonBackgroundcolour = "LightGray";
            lifethemesBackgroundColour = "LightGray";
            addMediaItemButtonBackgroundColour = "LightGray";
            isLifethemesButtonEnabled = false;
            isLifethemesButtonVisible = true;
            isContentPage = true;
            selectedMediaItem = null;
            isFilmsFilterChecked = true;
            isMusicFilterChecked = true;
            isPhotosFilterChecked = true;
            isDocumentsFilterChecked = true;
            isLinksFilterChecked = true;
            isProgressBarVisible = false;
            progress = 0;
            statusText = "";
            progressText = "";
            isAddMediaItemButtonVisible = false;
            isAddMediaItemButtonEnabled = false;
            currentMediaItemLifethemes = new List<Lifetheme>();
        }
        #endregion

        #region AddMediaItemButtonBackgroundColour
        public string AddMediaItemButtonBackgroundColour
        {
            get => addMediaItemButtonBackgroundColour;
            set
            {
                addMediaItemButtonBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsAddMediaItemButtonEnabled
        public bool IsAddMediaItemButtonEnabled
        {
            get => isAddMediaItemButtonEnabled;
            set
            {
                isAddMediaItemButtonEnabled = value;
                AddMediaItemButtonBackgroundColour = GetBackgroundColour(isAddMediaItemButtonEnabled, "Green");
                OnPropertyChanged();
            }
        }
        #endregion

        #region Title
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsContentPage
        public bool IsContentPage
        {
            get => isContentPage;
            set
            {
                isContentPage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsLifethemesButtonVisible
        public bool IsLifethemesButtonVisible
        {
            get => isLifethemesButtonVisible;
            set
            {
                isLifethemesButtonVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsAddMediaItemButtonVisible
        public bool IsAddMediaItemButtonVisible
        {
            get => isAddMediaItemButtonVisible;
            set
            {
                isAddMediaItemButtonVisible = value;
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

        #region ProgressText
        public string ProgressText
        {
            get => progressText;
            set
            {
                progressText = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region StatusText
        public string StatusText
        {
            get => statusText;
            set
            {
                statusText = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Progress
        public float Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsProgressBarVisible
        public bool IsProgressBarVisible
        {
            get => isProgressBarVisible;
            set
            {
                isProgressBarVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SelectedMediumTextContent
        public string SelectedMediumTextContent
        {
            get => selectedMediumTextContent;
            set
            {
                selectedMediumTextContent = value;
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

        #region SearchText
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SetLifethemesBackgroundColour
        protected void SetLifethemesBackgroundColour()
        {
            LifethemesBackgroundColour = GetBackgroundColour(IsLifethemesButtonEnabled, "Green");
        }
        #endregion

        #region LifethemesBackgroundColour
        public string LifethemesBackgroundColour
        {
            get => lifethemesBackgroundColour;
            set
            {
                lifethemesBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsLifethemesButtonEnabled
        public bool IsLifethemesButtonEnabled
        {
            get => isLifethemesButtonEnabled;
            set
            {
                isLifethemesButtonEnabled = value;
                SetLifethemesBackgroundColour();
                OnPropertyChanged();
            }
        }
        #endregion

        #region SetDeleteAllMediaItemsButtonBackgroundColour
        protected void SetDeleteAllMediaItemsButtonBackgroundColour()
        {
            DeleteAllMediaItemsButtonBackgroundcolour = GetBackgroundColour(IsDeleteAllMediaItemsButtonEnabled, "Green");
        }
        #endregion

        #region DeleteAllMediaItemsButtonBackgroundcolour
        public string DeleteAllMediaItemsButtonBackgroundcolour
        {
            get => deleteAllMediaItemsButtonBackgroundcolour;
            set
            {
                deleteAllMediaItemsButtonBackgroundcolour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsDeleteAllMediaItemsButtonEnabled
        public bool IsDeleteAllMediaItemsButtonEnabled
        {
            get => isDeleteAllMediaItemsButtonEnabled;
            set
            {
                isDeleteAllMediaItemsButtonEnabled = value;
                SetDeleteAllMediaItemsButtonBackgroundColour();
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsDeleteSelectedMediaItemButtonEnabled
        public bool IsDeleteSelectedMediaItemButtonEnabled
        {
            get => isDeleteSelectedMediaItemButtonEnabled;
            set
            {
                isDeleteSelectedMediaItemButtonEnabled = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SetDeleteSelectedMediaItemBackgroundColour
        /// <summary>
        /// Sets the colour depending if the button is enabled or not.
        /// </summary>
        protected void SetDeleteSelectedMediaItemBackgroundColour()
        {
            DeleteSelectedMediaItemButtonBackgroundColour = GetBackgroundColour(IsDeleteSelectedMediaItemButtonEnabled, "Green");
        }
        #endregion

        #region DeleteSelectedMediaItemButtonBackgroundColour
        /// <summary>
        /// Setter and getter for the delete-selected-mediaitem-button.
        /// </summary>
        public string DeleteSelectedMediaItemButtonBackgroundColour
        {
            get => deleteSelectedMediaItemButtonBackgroundColour;
            set
            {
                deleteSelectedMediaItemButtonBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsFullscreenButtonVisible
        /// <summary>
        /// Getter and setter for the isFullscreenButtonVisible-property.
        /// IsFullscreenButtonVisible is used to decide, wether the fullscreen button should be visible or not.
        /// </summary>
        public bool IsFullscreenButtonVisible
        {
            get => isFullscreenButtonVisible;
            set
            {
                isFullscreenButtonVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsMediaItemImageVisible
        public bool IsMediaItemImageVisible
        {
            get => isMediaItemImageVisible;
            set
            {
                isMediaItemImageVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsMediaItemMediaElementVisible
        public bool IsMediaItemMediaElementVisible
        {
            get => isMediaItemMediaElementVisible;
            set
            {
                isMediaItemMediaElementVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SelectedMediumImagePath
        public string SelectedMediumImagePath
        {
            get => selectedMediumImagePath;
            set
            {
                selectedMediumImagePath = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SelectedMediumMediaElementPath
        public string SelectedMediumMediaElementPath
        {
            get => selectedMediumMediaElementPath;
            set
            {
                selectedMediumMediaElementPath = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SelectedMediaItem
        /// <summary>
        /// Getter and Setter for the SelectedMediaItem-property.
        /// If the value is not null while setting,
        /// the Notes-, IsDeleteSelectedMediaItemButtonEnabled-,
        /// the MediaPreviewProperties-,
        /// the CurrenMediaItemLifethemes- and the IsLifethemesButtonEnabled-properties will be set to true,
        /// else to false.
        /// </summary>
        public MediaItem SelectedMediaItem
        {
            get => selectedMediaItem;
            set
            {
                selectedMediaItem = value;

                if (value != null)
                {
                    Notes = selectedMediaItem.Notes;
                    IsDeleteSelectedMediaItemButtonEnabled = true;
                    SetMediaPreviewProperties();
                    LoadLifethemesOfSelectedMediaItem();
                    IsLifethemesButtonEnabled = true;
                }
                else
                {
                    Notes = "";
                    ResetMediaPreviewProperties();
                    IsDeleteSelectedMediaItemButtonEnabled = false;
                    IsLifethemesButtonEnabled = false;
                    CurrentMediaItemLifethemes = new List<Lifetheme>();
                }

                SetDeleteSelectedMediaItemBackgroundColour();

                OnPropertyChanged();
            }
        }
        #endregion

        #region SetMediaPreviewProperties
        /// <summary>
        /// Sets the media preview properties 
        /// (IsMediaItemImageVisible, IsMediaItemMediaElementVisible, SelectedMediumMediaELementPath and SelectedMediumImagePath) depending if the selected medium is an image or not.
        /// For the MediaElement it is necessary to reload the selected mediaitem, because the selected mediaitem contains the video icon as path and not the medium path.
        /// An uri instance must be built of the path, to retrieve local media on android.
        /// This is necessary, because the MediaElement doesn't support images.
        /// </summary>
        public void SetMediaPreviewProperties()
        {
            IsMediaItemImageVisible = selectedMediaItem.IsImage;
            IsMediaItemMediaElementVisible = selectedMediaItem.IsAudioOrVideo;
            IsMediaItemTextVisible = selectedMediaItem.IsTxt;
            SelectedMediumImagePath = selectedMediaItem.GetFullPath;
            SelectedMediumMediaElementPath = selectedMediaItem.GetAudioOrVideoSource;
            SelectedMediumTextContent = selectedMediaItem.GetTextContent;
        }
        #endregion

        #region Notes
        /// <summary>
        /// Setter and Getter for the notes-property.
        /// If a MediaItem is selected the change of the notes property will be persisted.
        /// </summary>
        public string Notes
        {
            get => notes;
            set
            {
                notes = value;

                if (selectedMediaItem != null)
                {
                    selectedMediaItem.Notes = notes;
                    OnNotesChanged();
                }

                OnPropertyChanged();
            }
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
                IsDeleteAllMediaItemsButtonEnabled = mediaItems.Count > 0;
                OnPropertyChanged();
            }
        }
        #endregion

        #region OnNotesChanged
        protected async void OnNotesChanged()
        {
            try
            {

                await MediaItemDBHandler.UpdateNotes(selectedMediaItem.Id, notes);

            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region ResetMediaPreviewProperties
        protected void ResetMediaPreviewProperties()
        {
            SelectedMediumImagePath = "preview.png";
            SelectedMediumMediaElementPath = null;
            IsFullscreenButtonVisible = false;
            IsMediaItemImageVisible = true;
            IsMediaItemMediaElementVisible = false;
        }
        #endregion

        #region OnLoadAllMediaItems
        /// <summary>
        /// Loads all existing MediaItems into MediaItems-property.
        /// Throws an exception if an error occured while loading.
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadAllMediaItems()
        {
            try
            {
                MediaItems = await MediaItemDBHandler.FilterMediaItems(true, true, true, true, true);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region LoadLifethemesOfSelectedMediaItem
        /// <summary>
        /// Loads all lifethemes of the selected mediaitem.
        /// Throws an exception if an error occured while loading.
        /// </summary>
        public async void LoadLifethemesOfSelectedMediaItem()
        {
            if (selectedMediaItem != null)
            {
                try
                {
                    CurrentMediaItemLifethemes = await MediaItemLifethemesDBHandler.GetLifethemesOfMediaItem(selectedMediaItem.Id);
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
        }
        #endregion

    }

}