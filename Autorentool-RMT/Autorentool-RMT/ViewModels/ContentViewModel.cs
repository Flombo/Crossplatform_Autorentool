using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;
using System.IO;
using Autorentool_RMT.Services;

namespace Autorentool_RMT.ViewModels
{
    public class ContentViewModel : ViewModel
    {

        private List<MediaItem> mediaItems;
        private List<Lifetheme> currentMediaItemLifethemes;
        private MediaItem selectedMediaItem;
        private string selectedMediumImagePath;
        private string selectedMediumMediaElementPath;
        private string notes;
        private bool isMediaItemImageVisible;
        private bool isMediaItemMediaElementVisible;
        private bool isFullscreenButtonVisible;
        private bool isDeleteSelectedMediaItemButtonEnabled;
        private string deleteSelectedMediaItemButtonBackgroundColour;
        public ICommand ImportMediaItems { get; }

        #region Constructor
        public ContentViewModel()
        {
            mediaItems = new List<MediaItem>();
            ImportMediaItems = new Command(ShowFilePicker);
            notes = "";
            selectedMediumImagePath = "preview.png";
            selectedMediumMediaElementPath = "https://www.youtube.com/watch?v=2DVpys50LVE";
            isMediaItemImageVisible = true;
            isMediaItemMediaElementVisible = false;
            isFullscreenButtonVisible = false;
            isDeleteSelectedMediaItemButtonEnabled = false;
            deleteSelectedMediaItemButtonBackgroundColour = "LightGray";
            selectedMediaItem = null;
            currentMediaItemLifethemes = new List<Lifetheme>();
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
        private void SetDeleteSelectedMediaItemBackgroundColour()
        {
            DeleteSelectedMediaItemButtonBackgroundColour = IsDeleteSelectedMediaItemButtonEnabled ? "Green" : "LightGray";
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
                }
                else
                {
                    Notes = "";
                    ResetMediaPreviewProperties();
                    IsDeleteSelectedMediaItemButtonEnabled = false;
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
        /// This is necessary, because the MediaElement doesn't support images.
        /// </summary>
        public void SetMediaPreviewProperties()
        {
            if (selectedMediaItem.FileType.Equals("mp3") || selectedMediaItem.FileType.Equals("mp4"))
            {
                IsMediaItemImageVisible = false;
                IsMediaItemMediaElementVisible = true;
                SelectedMediumMediaElementPath = selectedMediaItem.GetFullPath;
                SelectedMediumImagePath = "preview.png";
                IsFullscreenButtonVisible = false;
            }
            else
            {
                IsMediaItemImageVisible = true;
                IsMediaItemMediaElementVisible = false;
                SelectedMediumMediaElementPath = "https://www.youtube.com/watch?v=pr03CYqhFr4&list=PLM75ZaNQS_FaEPpqVjfQdnFaSR1EWCeNZ";
                SelectedMediumImagePath = selectedMediaItem.GetFullPath;
                IsFullscreenButtonVisible = true;
            }
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
                OnPropertyChanged();
            }
        }
        #endregion

        #region ShowFilePicker
        public async void ShowFilePicker()
        {
            try
            {
                IEnumerable<FileResult> pickedFiles = await FilePicker.PickMultipleAsync();
                List<FileResult> results = pickedFiles.ToList();

                if (results.Count > 0)
                {
                    foreach (FileResult fileResult in results)
                    {
                        if (fileResult.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        fileResult.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                        {
                            Stream stream = await fileResult.OpenReadAsync();

                            string directoryPath = FileHandler.CreateDirectory("MediaItems");

                            string filename = fileResult.FileName;
                            string filetype = fileResult.ContentType;

                            int filetypeIndex = filename.LastIndexOf('.');

                            string mediaItemName = filename.Substring(0, filetypeIndex);

                            string filepath = directoryPath + filename;

                            filepath = FileHandler.GetUniqueFilenamePath(filepath);

                            FileHandler.SaveFile(stream, filepath);

                            await MediaItemDBHandler.AddMediaItem(mediaItemName, filepath, filetype, "", mediaItemName, 0);
                        }
                    }
                    MediaItems = await MediaItemDBHandler.GetAllMediaItems();
                }

            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region OnNotesChanged
        private async void OnNotesChanged()
        {
            try
            {

                await MediaItemDBHandler.UpdateMediaItem(
                    selectedMediaItem.Id,
                    selectedMediaItem.Name,
                    selectedMediaItem.GetFullPath,
                    selectedMediaItem.FileType,
                    selectedMediaItem.Notes,
                    selectedMediaItem.DisplayName,
                    selectedMediaItem.BackendMediaItemId
                    );

            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region ResetMediaPreviewProperties
        private void ResetMediaPreviewProperties()
        {
            SelectedMediumImagePath = "preview.png";
            SelectedMediumMediaElementPath = "https://www.youtube.com/watch?v=pr03CYqhFr4&list=PLM75ZaNQS_FaEPpqVjfQdnFaSR1EWCeNZ";
            IsFullscreenButtonVisible = false;
            IsMediaItemImageVisible = true;
            IsMediaItemMediaElementVisible = false;
        }
        #endregion

        #region OnDeleteMediaItem
        /// <summary>
        /// Deletes the selected mediaitem and reloads all existing mediaitems for the ui.
        /// If an exception was thrown, it will be re-thrown to the code-behind.
        /// </summary>
        /// <returns></returns>
        public async Task OnDeleteMediaItem()
        {
            if (selectedMediaItem != null)
            {
                try
                {
                    if (File.Exists(selectedMediaItem.GetFullPath))
                    {
                        File.Delete(selectedMediaItem.GetFullPath);
                    }

                    await MediaItemDBHandler.DeleteMediaItem(selectedMediaItem.Id);
                    SelectedMediaItem = null;
                    MediaItems = await MediaItemDBHandler.GetAllMediaItems();
                }
                catch (Exception exc)
                {
                    throw exc;
                }
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
        /// Throws an exception if an error occured while loading.
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadAllMediaItems()
        {
            try
            {
                MediaItems = await MediaItemDBHandler.GetAllMediaItems();
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
