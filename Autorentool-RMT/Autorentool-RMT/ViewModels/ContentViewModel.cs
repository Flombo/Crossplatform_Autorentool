﻿using Autorentool_RMT.Models;
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
        private bool isDeleteAllMediaItemsButtonEnabled;
        private string deleteAllMediaItemsButtonBackgroundcolour;
        public ICommand ImportMediaItems { get; }

        #region Constructor
        public ContentViewModel()
        {
            mediaItems = new List<MediaItem>();
            ImportMediaItems = new Command(ShowFilePicker);
            notes = "";
            selectedMediumImagePath = "preview.png";
            selectedMediumMediaElementPath = null;
            isMediaItemImageVisible = true;
            isMediaItemMediaElementVisible = false;
            isFullscreenButtonVisible = false;
            isDeleteSelectedMediaItemButtonEnabled = false;
            isDeleteAllMediaItemsButtonEnabled = false;
            deleteSelectedMediaItemButtonBackgroundColour = "LightGray";
            deleteAllMediaItemsButtonBackgroundcolour = "LightGray";
            selectedMediaItem = null;
            currentMediaItemLifethemes = new List<Lifetheme>();
        }
        #endregion

        #region SetDeleteAllMediaItemsButtonBackgroundColour
        private void SetDeleteAllMediaItemsButtonBackgroundColour()
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
        private void SetDeleteSelectedMediaItemBackgroundColour()
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
        /// For the MediaElement it is necessary to reload the selected mediaitem, because the selected mediaitem contains the video icon as path and not the medium path.
        /// An uri instance must be built of the path, to retrieve local media on android.
        /// This is necessary, because the MediaElement doesn't support images.
        /// </summary>
        public async void SetMediaPreviewProperties()
        {
            if (selectedMediaItem.FileType.Equals("mp3") || selectedMediaItem.FileType.Equals("mp4"))
            {
                IsMediaItemImageVisible = false;
                IsMediaItemMediaElementVisible = true;

                MediaItem mediaItem = await MediaItemDBHandler.GetSingleMediaItem(selectedMediaItem.Id);

                SelectedMediumMediaElementPath = new Uri(mediaItem.GetFullPath).LocalPath;

                SelectedMediumImagePath = "preview.png";
                IsFullscreenButtonVisible = false;
            }
            else
            {
                IsMediaItemImageVisible = true;
                IsMediaItemMediaElementVisible = false;
                SelectedMediumMediaElementPath = null;
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
                IsDeleteAllMediaItemsButtonEnabled = mediaItems.Count > 0;
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
                        Stream stream = await fileResult.OpenReadAsync();

                        string directoryPath = FileHandler.CreateDirectory("MediaItems");

                        string filename = fileResult.FileName;
                        string filetype = FileHandler.ExtractFiletypeFromPath(filename);

                        string filepath = directoryPath + filename;

                        filepath = FileHandler.GetUniqueFilenamePath(filepath);

                        FileHandler.SaveFile(stream, filepath);

                        await MediaItemDBHandler.AddMediaItem(filename, filepath, filetype, "", filename, 0);
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
            SelectedMediumMediaElementPath = null;
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
                    await DeleteMediaItem(selectedMediaItem);

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

        #region DeleteMediaItem
        /// <summary>
        /// Deletes given mediaItem and corresponding file.
        /// If an error occured an exception will be thrown.
        /// </summary>
        /// <param name="mediaItem"></param>
        /// <returns></returns>
        private async Task DeleteMediaItem(MediaItem mediaItem)
        {
            try
            {
                if (File.Exists(mediaItem.GetFullPath))
                {
                    File.Delete(mediaItem.GetFullPath);
                }
                await SessionMediaItemsDBHandler.UnbindSessionMediaItemsByMediaItemId(mediaItem.Id);
                await MediaItemLifethemesDBHandler.UnbindMediaItemLifethemesByMediaItemId(mediaItem.Id);
                await MediaItemDBHandler.DeleteMediaItem(mediaItem.Id);
            
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region OnDeleteAllMediaItems
        /// <summary>
        /// Deletes all MediaItems if the MediaItems-list isn't empty.
        /// Throws an Exception if an error occurs.
        /// </summary>
        /// <returns></returns>
        public async Task OnDeleteAllMediaItems()
        {
            if(mediaItems.Count > 0)
            {
                try
                {
                    SelectedMediaItem = null;

                    foreach(MediaItem mediaItem in mediaItems)
                    {
                        await DeleteMediaItem(mediaItem);
                    }

                    MediaItems = new List<MediaItem>();

                } catch(Exception exc)
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
