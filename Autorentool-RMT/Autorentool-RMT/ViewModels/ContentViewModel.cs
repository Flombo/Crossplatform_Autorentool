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
using System.Diagnostics;

namespace Autorentool_RMT.ViewModels
{
    public class ContentViewModel : ViewModel
    {

        #region Attributes
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
        private string lifethemesBackgroundColour;
        private bool isLifethemesButtonEnabled;
        private bool isPhotosFilterChecked;
        private bool isFilmsFilterChecked;
        private bool isMusicFilterChecked;
        private bool isDocumentsFilterChecked;
        private bool isLinksFilterChecked;
        private string searchText;
        private bool isMediaItemTextVisible;
        private string selectedMediumTextContent;
        private bool isProgressBarVisible;
        private float progress;
        private string statusText;
        private string progressText;
        public ICommand Search { get; }
        #endregion

        #region Constructor
        public ContentViewModel()
        {
            mediaItems = new List<MediaItem>();
            Search = new Command(OnSearch);
            notes = "";
            searchText = "";
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
            isLifethemesButtonEnabled = false;
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
            currentMediaItemLifethemes = new List<Lifetheme>();
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

        #region IsPhotosFilterChecked
        public bool IsPhotosFilterChecked
        {
            get => isPhotosFilterChecked;
            set
            {
                isPhotosFilterChecked = value;

                if (searchText.Length > 0)
                {
                    OnSearch();
                }
                else
                {
                    OnFilter();
                }

                OnPropertyChanged();
            }
        }
        #endregion

        #region IsLinksFilterChecked
        public bool IsLinksFilterChecked
        {
            get => isLinksFilterChecked;
            set
            {
                isLinksFilterChecked = value;

                if (searchText.Length > 0)
                {
                    OnSearch();
                }
                else
                {
                    OnFilter();
                }

                OnPropertyChanged();
            }
        }
        #endregion

        #region IsDocumentsFilterChecked
        public bool IsDocumentsFilterChecked
        {
            get => isDocumentsFilterChecked;
            set
            {
                isDocumentsFilterChecked = value;

                if (searchText.Length > 0)
                {
                    OnSearch();
                }
                else
                {
                    OnFilter();
                }

                OnPropertyChanged();
            }
        }
        #endregion

        #region IsMusicFilterChecked
        public bool IsMusicFilterChecked
        {
            get => isMusicFilterChecked;
            set
            {
                isMusicFilterChecked = value;

                if (searchText.Length > 0)
                {
                    OnSearch();
                }
                else
                {
                    OnFilter();
                }

                OnPropertyChanged();
            }
        }
        #endregion

        #region IsFilmsFilterChecked
        public bool IsFilmsFilterChecked
        {
            get => isFilmsFilterChecked;
            set
            {
                isFilmsFilterChecked = value;

                if (searchText.Length > 0)
                {
                    OnSearch();
                }
                else
                {
                    OnFilter();
                }

                OnPropertyChanged();
            }
        }
        #endregion

        #region SetLifethemesBackgroundColour
        private void SetLifethemesBackgroundColour()
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
        public async void SetMediaPreviewProperties()
        {
            if (selectedMediaItem.IsAudio || selectedMediaItem.IsVideo)
            {
                IsMediaItemImageVisible = false;
                IsMediaItemMediaElementVisible = true;
                IsMediaItemTextVisible = false;

                try
                {
                    MediaItem mediaItem = await MediaItemDBHandler.GetSingleMediaItem(selectedMediaItem.Id);

                    SelectedMediumMediaElementPath = new Uri(mediaItem.GetFullPath).LocalPath;
                } catch(Exception)
                {
                    SelectedMediumMediaElementPath = null;
                }

                SelectedMediumImagePath = "preview.png";
                SelectedMediumTextContent = "";
                IsFullscreenButtonVisible = false;
            }

            else if(selectedMediaItem.IsImage)
            {
                IsMediaItemImageVisible = true;
                IsMediaItemMediaElementVisible = false;
                IsMediaItemTextVisible = false;
                SelectedMediumMediaElementPath = null;
                SelectedMediumTextContent = "";
                SelectedMediumImagePath = selectedMediaItem.GetFullPath;
                IsFullscreenButtonVisible = true;

            } else if(selectedMediaItem.IsTxt)
            {
                IsMediaItemImageVisible = false;
                IsMediaItemMediaElementVisible = false;
                IsMediaItemTextVisible = true;
                SelectedMediumMediaElementPath = null;
                SelectedMediumImagePath = selectedMediaItem.GetFullPath;
                IsFullscreenButtonVisible = false;
                SelectedMediumTextContent = File.ReadAllText(selectedMediaItem.GetFullPath);
            } else
            {
                SelectedMediaItem = null;
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
        /// <summary>
        /// Shows file picker for valid filetypes(jpg/jpeg, png, html, mp3, txt, mp4), saves them and builds MediaItems as representation.
        /// After that all new MediaItems will be displayed.
        /// If an exception occurs, it will be re-thrown to the code behind for displaying an error prompt.
        /// </summary>
        public async Task ShowFilePicker()
        {
            try
            {
                FilePickerFileType filePickerFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>> {
                        { DevicePlatform.iOS, new [] { "jpeg", "png", "mp3", "mpeg4Movie", "plaintext", "utf8PlainText", "html" } },
                        { DevicePlatform.Android, new [] { "image/jpeg", "image/png", "audio/mp3", "audio/mpeg", "video/mp4", "text/*", "text/html" } },
                        { DevicePlatform.UWP, new []{ "*.jpg", "*.jpeg", "*.png", "*.mp3", "*.mp4", "*.txt", "*.html" } }
                    });

                PickOptions pickOptions = new PickOptions
                {
                    PickerTitle = "Wählen Sie eine oder mehrere Dateien aus",
                    FileTypes = filePickerFileType,
                };

                IEnumerable<FileResult> pickedFiles = await FilePicker.PickMultipleAsync(pickOptions);
                List<FileResult> results = pickedFiles.ToList();

                if (results != null && results.Count > 0)
                {
                    IsProgressBarVisible = true;
                    float maxProgress = results.Count;
                    float currentProgress = 0;
                    Progress = currentProgress;
                    IsDeleteAllMediaItemsButtonEnabled = false;
                    IsDeleteSelectedMediaItemButtonEnabled = false;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    foreach (FileResult fileResult in results)
                    {
                        currentProgress++;
                        SetProgressElements(currentProgress, maxProgress, stopwatch);

                        using (Stream stream = await fileResult.OpenReadAsync())
                        {
                            string hash = FileHandler.GetFileHashAsString(stream);

                            int duplicate = await MediaItemDBHandler.SearchMediaItemWithGivenHash(hash);

                            if (duplicate == 0)
                            {

                                string directoryPath = FileHandler.CreateDirectory("MediaItems");

                                string filename = FileHandler.GetUniqueFilename(fileResult.FileName, directoryPath);
                                string filetype = FileHandler.ExtractFiletypeFromPath(filename);

                                string filepath = Path.Combine(directoryPath, filename);

                                using (Stream fileSaveStream = await fileResult.OpenReadAsync())
                                {
                                    FileHandler.SaveFile(fileSaveStream, filepath);
                                }

                                await MediaItemDBHandler.AddMediaItem(filename, filepath, filetype, hash, "", 0);
                            }
                            else
                            {
                                stopwatch.Stop();

                                MediaItems = await MediaItemDBHandler.FilterMediaItems(isPhotosFilterChecked, isMusicFilterChecked, isDocumentsFilterChecked, isFilmsFilterChecked, isLinksFilterChecked);
                                IsProgressBarVisible = false;
                                IsDeleteAllMediaItemsButtonEnabled = true;
                                IsDeleteSelectedMediaItemButtonEnabled = true;

                                throw new Exception("Es dürfen keine bereits existierenden Dateien hinzugefügt werden");
                            }
                        }
                    }

                    stopwatch.Stop();

                    MediaItems = await MediaItemDBHandler.FilterMediaItems(isPhotosFilterChecked, isMusicFilterChecked, isDocumentsFilterChecked, isFilmsFilterChecked, isLinksFilterChecked);
                    IsProgressBarVisible = false;
                    IsDeleteAllMediaItemsButtonEnabled = true;
                    IsDeleteSelectedMediaItemButtonEnabled = true;
                }

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region SetProgressElements
        /// <summary>
        /// Sets the progress elements ProgressBar-Progress, StatusText and ProgressText.
        /// </summary>
        /// <param name="currentProgress"></param>
        /// <param name="maxProgress"></param>
        /// <param name="stopwatch"></param>
        private void SetProgressElements(float currentProgress, float maxProgress, Stopwatch stopwatch)
        {
            Progress = currentProgress / maxProgress;
            StatusText = currentProgress + " / " + maxProgress;

            TimeSpan t = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);
            string formattedTime = $"{t.Minutes:D2}m:{t.Seconds:D2}s";

            ProgressText = $"{currentProgress} Dateien von {maxProgress} in {formattedTime} hinzugefügt.";
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
                    float currentProgress = 0;
                    float maxProgress = 1;
                    IsProgressBarVisible = true;
                    Progress = currentProgress;
                    IsDeleteAllMediaItemsButtonEnabled = false;
                    IsDeleteSelectedMediaItemButtonEnabled = false;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    await DeleteMediaItem(selectedMediaItem, currentProgress, maxProgress, stopwatch);

                    stopwatch.Stop();

                    SelectedMediaItem = null;
                    MediaItems = await MediaItemDBHandler.FilterMediaItems(isPhotosFilterChecked, isMusicFilterChecked, isDocumentsFilterChecked, isFilmsFilterChecked, isLinksFilterChecked);

                    IsProgressBarVisible = false;
                    IsDeleteAllMediaItemsButtonEnabled = true;
                    IsDeleteSelectedMediaItemButtonEnabled = true;
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
        /// <param name="currentProgress"></param>
        /// <param name="maxProgress"></param>
        /// <param name="stopwatch"></param>
        /// <returns></returns>
        private async Task DeleteMediaItem(MediaItem mediaItem, float currentProgress, float maxProgress, Stopwatch stopwatch)
        {
            try
            {

                currentProgress++;
                SetProgressElements(currentProgress, maxProgress, stopwatch);

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
            try
            {
                List<MediaItem> mediaItemsForDeletion = await MediaItemDBHandler.GetAllMediaItems();

                float currentProgress = 0;
                float maxProgress = mediaItemsForDeletion.Count;
                IsProgressBarVisible = true;
                Progress = currentProgress;
                IsDeleteAllMediaItemsButtonEnabled = false;
                IsDeleteSelectedMediaItemButtonEnabled = false;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                foreach (MediaItem mediaItem in mediaItemsForDeletion)
                {
                    await DeleteMediaItem(mediaItem, currentProgress, maxProgress, stopwatch);
                }

                stopwatch.Stop();

                IsProgressBarVisible = false;
                IsDeleteAllMediaItemsButtonEnabled = true;
                IsDeleteSelectedMediaItemButtonEnabled = true;

                SelectedMediaItem = null;
                MediaItems = new List<MediaItem>();

            } catch(Exception exc)
            {
                throw exc;
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

        #region SetCurrentMediaItemLifethemes
        /// <summary>
        /// Sets the CurrentMediaItemLifethemes-property with the given selectedLifethemes.
        /// Unbinds lifethemes that aren't part of the selectedLifethemes-List.
        /// Binds the selected lifethemes to the selected mediaitem.
        /// While this process is ongoing, the two delete buttons will be disabled. 
        /// </summary>
        /// <param name="selectedLifethemes"></param>
        /// <returns></returns>
        public async Task SetCurrentMediaItemLifethemes(List<Lifetheme> selectedLifethemes)
        {
            IsDeleteAllMediaItemsButtonEnabled = false;
            IsDeleteSelectedMediaItemButtonEnabled = false;

            try
            {
                List<Lifetheme> unboundLifethemes = GetUnboundLifethemes(selectedLifethemes);

                unboundLifethemes = await CheckIfLifethemesWereFreshlyCreated(unboundLifethemes);

                await UnbindSelectedLifethemes(unboundLifethemes);
                await BindSelectedLifethemes(selectedLifethemes);

                IsDeleteAllMediaItemsButtonEnabled = true;
                IsDeleteSelectedMediaItemButtonEnabled = true;
            } catch(Exception exc)
            {
                IsDeleteAllMediaItemsButtonEnabled = true;
                IsDeleteSelectedMediaItemButtonEnabled = true;

                throw exc;
            }
        }
        #endregion

        #region CheckIfLifethemesWereFreshlyCreated
        private async Task<List<Lifetheme>> CheckIfLifethemesWereFreshlyCreated(List<Lifetheme> unboundLifethemes)
        {
            List<Lifetheme> deleteableLifethemes = new List<Lifetheme>();

            foreach(Lifetheme unboundLifetheme in unboundLifethemes)
            {
                int mediaItemLifethemesId = await MediaItemLifethemesDBHandler.GetID(selectedMediaItem.Id, unboundLifetheme.Id);

                if(mediaItemLifethemesId == -1)
                {
                    await MediaItemLifethemesDBHandler.BindMediaItemLifetheme(selectedMediaItem.Id, unboundLifetheme.Id);
                } else
                {
                    deleteableLifethemes.Add(unboundLifetheme);
                }
            }

            return deleteableLifethemes;

        }
        #endregion

        #region GetUnboundLifethemes
        /// <summary>
        /// Returns the lifethemes for unbinding.
        /// If the selectedLifethemes-list is empty, then all lifethemes should be unbound.
        /// Else the name-property of each CurrentMediaItemLifetheme and selectedLifetheme has to be compared;
        /// </summary>
        /// <param name="selectedLifethemes"></param>
        /// <returns></returns>
        private List<Lifetheme> GetUnboundLifethemes(List<Lifetheme> selectedLifethemes)
        {
            List<Lifetheme> unboundLifethemes = new List<Lifetheme>();

            if (selectedLifethemes.Count > 0)
            {
                foreach (Lifetheme currentMediaItemLifetheme in CurrentMediaItemLifethemes)
                {
                    foreach (Lifetheme selectedLifetheme in selectedLifethemes)
                    {
                        if (!currentMediaItemLifetheme.Name.Equals(selectedLifetheme.Name))
                        {
                            unboundLifethemes.Add(currentMediaItemLifetheme);
                        }
                    }
                }
            }
            else
            {
                unboundLifethemes = CurrentMediaItemLifethemes;
            }

            return unboundLifethemes;
        }
        #endregion

        #region UnbindSelectedLifethemes
        /// <summary>
        /// Unbinds the given lifethemes from the selected mediaitem if it isn't null.
        /// Throws an exception if the process failed.
        /// </summary>
        /// <param name="unboundLifethemes"></param>
        /// <returns></returns>
        private async Task UnbindSelectedLifethemes(List<Lifetheme> unboundLifethemes)
        {
            if(unboundLifethemes.Count > 0 && selectedMediaItem != null)
            {
                try
                {
                    foreach(Lifetheme unboundLifetheme in unboundLifethemes)
                    {
                        int mediaItemLifethemesId = await MediaItemLifethemesDBHandler.GetID(selectedMediaItem.Id, unboundLifetheme.Id);
                        await MediaItemLifethemesDBHandler.UnbindCertainMediaItemLifethemes(mediaItemLifethemesId);
                    }

                    CurrentMediaItemLifethemes = new List<Lifetheme>();
                } catch(Exception exc)
                {
                    throw exc;
                }
            }
        }
        #endregion

        #region BindSelectedLifethemes
        /// <summary>
        /// Binds selected lifethemes to the selected mediaitem if it isn't null.
        /// Throws an exception if the process failed.
        /// </summary>
        /// <param name="selectedLifethemes"></param>
        /// <returns></returns>
        private async Task BindSelectedLifethemes(List<Lifetheme> selectedLifethemes)
        {
            if (selectedMediaItem != null)
            {
                try
                {
                    foreach (Lifetheme selectedLifetheme in selectedLifethemes)
                    {
                        await MediaItemLifethemesDBHandler.BindMediaItemLifetheme(selectedMediaItem.Id, selectedLifetheme.Id);
                    }

                    CurrentMediaItemLifethemes = await MediaItemLifethemesDBHandler.GetLifethemesOfMediaItem(selectedMediaItem.Id);
                } catch(Exception exc)
                {
                    throw exc;
                }
            }
        }
        #endregion

        #region OnSearch
        /// <summary>
        /// Searches MediaItems which contain the search string.
        /// If no search string was given, the MediaItems will be reloaded.
        /// </summary>
        private async void OnSearch()
        {
            SelectedMediaItem = null;
            CurrentMediaItemLifethemes = new List<Lifetheme>();

            if(searchText.Length > 0)
            {
                MediaItems = await MediaItemDBHandler.SearchMediaItems(
                        searchText,
                        IsPhotosFilterChecked,
                        IsMusicFilterChecked,
                        IsDocumentsFilterChecked,
                        IsFilmsFilterChecked,
                        IsLinksFilterChecked
                    );
            } else
            {
                MediaItems = await MediaItemDBHandler.FilterMediaItems(
                        IsPhotosFilterChecked,
                        IsMusicFilterChecked,
                        IsDocumentsFilterChecked,
                        IsFilmsFilterChecked,
                        IsLinksFilterChecked
                    );
            }
        }
        #endregion

        #region OnFilter
        /// <summary>
        /// Filters MediaItems depending on, which filter is disabled/enabled.
        /// </summary>
        private async void OnFilter()
        {
            SelectedMediaItem = null;
            CurrentMediaItemLifethemes = new List<Lifetheme>();

            MediaItems = await MediaItemDBHandler.FilterMediaItems(
                    IsPhotosFilterChecked,
                    IsMusicFilterChecked,
                    IsDocumentsFilterChecked,
                    IsFilmsFilterChecked,
                    IsLinksFilterChecked
                );
        }
        #endregion

    }
}
