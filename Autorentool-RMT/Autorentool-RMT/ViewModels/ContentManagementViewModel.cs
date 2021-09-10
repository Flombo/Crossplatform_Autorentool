using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Linq;
using System.IO;
using Autorentool_RMT.Services;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace Autorentool_RMT.ViewModels
{
    public class ContentManagementViewModel : ContentViewModel
    {

        public ICommand Search { get; }

        #region Constructor
        public ContentManagementViewModel()
        {
            Title = "INHALTE";
            IsContentPage = true;
            Search = new Command(OnSearch);
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

        #region SaveFilesOfSelectedFolder
        /// <summary>
        /// Saves valid files of the selected folder by the given folder path.
        /// If the proces failed an exception will be thrown.
        /// </summary>
        /// <param name="selectedFolderPath"></param>
        /// <returns></returns>
        public async Task SaveFilesOfSelectedFolder(string selectedFolderPath)
        {
            try
            {
                List<string> folderFilePaths = FileHandler.GetFolderFilePaths(selectedFolderPath);

                IsProgressBarVisible = true;
                IsActivityIndicatorRunning = true;
                float maxProgress = folderFilePaths.Count;
                float currentProgress = 0;
                Progress = currentProgress;
                IsDeleteAllMediaItemsButtonEnabled = false;
                IsDeleteSelectedMediaItemButtonEnabled = false;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                foreach (string folderFilePath in folderFilePaths)
                {
                    currentProgress++;
                    SetProgressElements(currentProgress, maxProgress, stopwatch);

                    using (FileStream fileStream = File.OpenRead(folderFilePath))
                    {
                        string filenameFromFileStream = Path.GetFileName(folderFilePath);
                        string hash = FileHandler.GetFileHashAsString(fileStream);

                        int duplicate = await MediaItemDBHandler.SearchMediaItemWithGivenHash(hash);

                        if (duplicate == 0)
                        {

                            string directoryPath = FileHandler.CreateDirectory("MediaItems");

                            string filename = FileHandler.GetUniqueFilename(filenameFromFileStream, directoryPath);
                            string filetype = FileHandler.ExtractFiletypeFromPath(filename);

                            string filepath = Path.Combine(directoryPath, filename);

                            using (Stream fileSaveStream = File.OpenRead(folderFilePath))
                            {
                                FileHandler.SaveFile(fileSaveStream, filepath);
                            }

                            await MediaItemDBHandler.AddMediaItem(filename, filepath, filetype, hash, "", 0);
                        }
                        else
                        {
                            stopwatch.Stop();

                            MediaItems = await MediaItemDBHandler.FilterMediaItems(isPhotosFilterChecked, isMusicFilterChecked, isDocumentsFilterChecked, isFilmsFilterChecked, isLinksFilterChecked);
                            ResetDeleteButtonsAndProgressIndicators();

                            throw new Exception("Duplicate");
                        }
                    }
                }

                stopwatch.Stop();

                MediaItems = await MediaItemDBHandler.FilterMediaItems(isPhotosFilterChecked, isMusicFilterChecked, isDocumentsFilterChecked, isFilmsFilterChecked, isLinksFilterChecked);
                ResetDeleteButtonsAndProgressIndicators();
            }
            catch (Exception){
                throw new Exception("Folder");
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
                    IsActivityIndicatorRunning = true;
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
                                ResetDeleteButtonsAndProgressIndicators();

                                throw new Exception("Duplicate");
                            }
                        }
                    }

                    stopwatch.Stop();

                    MediaItems = await MediaItemDBHandler.FilterMediaItems(isPhotosFilterChecked, isMusicFilterChecked, isDocumentsFilterChecked, isFilmsFilterChecked, isLinksFilterChecked);
                    ResetDeleteButtonsAndProgressIndicators();
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
                    IsActivityIndicatorRunning = true;

                    await DeleteMediaItem(selectedMediaItem, currentProgress, maxProgress, stopwatch);

                    stopwatch.Stop();

                    MediaItems = await MediaItemDBHandler.FilterMediaItems(isPhotosFilterChecked, isMusicFilterChecked, isDocumentsFilterChecked, isFilmsFilterChecked, isLinksFilterChecked);

                    ResetDeleteButtonsAndProgressIndicators();
                }
                catch (Exception exc)
                {
                    ResetDeleteButtonsAndProgressIndicators();

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
                IsActivityIndicatorRunning = true;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                foreach (MediaItem mediaItem in mediaItemsForDeletion)
                {
                    await DeleteMediaItem(mediaItem, currentProgress, maxProgress, stopwatch);
                }

                stopwatch.Stop();

                ResetDeleteButtonsAndProgressIndicators();

                MediaItems = new List<MediaItem>();

            } catch(Exception exc)
            {
                ResetDeleteButtonsAndProgressIndicators();

                throw exc;
            }
        }
        #endregion

        #region ResetDeleteButtonsAndProgressIndicators
        /// <summary>
        /// Resets delete buttons and progress indicator to their default state.
        /// </summary>
        private void ResetDeleteButtonsAndProgressIndicators()
        {
            SelectedMediaItem = null;
            IsProgressBarVisible = false;
            IsDeleteAllMediaItemsButtonEnabled = true;
            IsDeleteSelectedMediaItemButtonEnabled = true;
            IsActivityIndicatorRunning = false;
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
        /// <summary>
        /// Checks if a lifetheme was freshly created and delivers the lifethemes which can be unbound.
        /// </summary>
        /// <param name="unboundLifethemes"></param>
        /// <returns></returns>
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
        protected async void OnSearch()
        {
            SelectedMediaItem = null;
            CurrentMediaItemLifethemes = new List<Lifetheme>();

            if (searchText.Length > 0)
            {
                MediaItems = await MediaItemDBHandler.SearchMediaItems(
                        searchText,
                        IsPhotosFilterChecked,
                        IsMusicFilterChecked,
                        IsDocumentsFilterChecked,
                        IsFilmsFilterChecked,
                        IsLinksFilterChecked
                    );
            }
            else
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
        protected async void OnFilter()
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
