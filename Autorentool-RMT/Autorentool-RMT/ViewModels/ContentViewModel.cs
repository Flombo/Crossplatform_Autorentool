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
            selectedMediaItem = null;
            currentMediaItemLifethemes = new List<Lifetheme>();
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
                IsFullscreenButtonVisible = isMediaItemImageVisible;
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
                IsFullscreenButtonVisible = !isMediaItemMediaElementVisible;
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
                Notes = selectedMediaItem.Notes;
                CheckFiletype();
                LoadLifethemesOfSelectedMediaItem();
                OnPropertyChanged();
            }
        }
        #endregion

        #region CheckFiletype
        public void CheckFiletype()
        {
            if(selectedMediaItem.FileType.Equals("mp3") || selectedMediaItem.FileType.Equals("mp4"))
            {
                IsMediaItemImageVisible = false;
                IsMediaItemMediaElementVisible = true;
                SelectedMediumMediaElementPath = selectedMediaItem.GetFullPath;
                SelectedMediumImagePath = "preview.png";
            }
            else
            {
                IsMediaItemImageVisible = true;
                IsMediaItemMediaElementVisible = false;
                SelectedMediumMediaElementPath = "https://www.youtube.com/watch?v=2DVpys50LVE";
                SelectedMediumImagePath = selectedMediaItem.GetFullPath;
            }
        }
        #endregion

        #region Notes
        public string Notes
        {
            get => notes;
            set
            {
                notes = value;
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
            } catch(Exception exc)
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
            if(selectedMediaItem != null)
            {
                try
                {
                    CurrentMediaItemLifethemes = await MediaItemLifethemesDBHandler.GetLifethemesOfMediaItem(selectedMediaItem.Id);
                } catch(Exception exc)
                {
                    throw exc;
                }
            }
        }
        #endregion
    }
}
