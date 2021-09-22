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

        #region ShowFilePicker
        /// <summary>
        /// Shows file picker for valid filetypes(jpg/jpeg, png, html, mp3, txt, mp4), saves them and builds MediaItems as representation.
        /// After that all new MediaItems will be displayed.
        /// If an exception occurs, it will be re-thrown to the code behind for displaying an error prompt.
        /// </summary>
        public async Task ShowFilePicker()
        {
            Stopwatch stopwatch = new Stopwatch();

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
                    DisableDeleteButtonsAndShowProgressIndicators();
                    float maxProgress = results.Count;
                    float currentProgress = 0;
                    Progress = currentProgress;
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
                                await AddMediaItem(fileResult, hash);
                            }
                            else
                            {
                                stopwatch.Stop();

                                MediaItems = await MediaItemDBHandler.FilterMediaItems(isPhotosFilterChecked, isMusicFilterChecked, isDocumentsFilterChecked, isFilmsFilterChecked, isLinksFilterChecked);
                                ResetDeleteButtonsAndProgressIndicators();
                                SelectedMediaItem = null;

                                throw new Exception("Duplicate");
                            }
                        }
                    }

                    stopwatch.Stop();

                    MediaItems = await MediaItemDBHandler.FilterMediaItems(isPhotosFilterChecked, isMusicFilterChecked, isDocumentsFilterChecked, isFilmsFilterChecked, isLinksFilterChecked);
                    ResetDeleteButtonsAndProgressIndicators();
                    SelectedMediaItem = null;
                }

            }
            catch (Exception exc)
            {
                stopwatch.Stop();

                MediaItems = await MediaItemDBHandler.FilterMediaItems(isPhotosFilterChecked, isMusicFilterChecked, isDocumentsFilterChecked, isFilmsFilterChecked, isLinksFilterChecked);
                ResetDeleteButtonsAndProgressIndicators();
                SelectedMediaItem = null;

                throw exc;
            }
        }
        #endregion

        #region AddMediaItems
        /// <summary>
        /// Saves the picked file, creates a thumbnail if the are picked file is an image and saves the resulting MediaItem.
        /// If any of these processes are failing, an exception will be thrown.
        /// </summary>
        /// <param name="fileResult"></param>
        /// <param name="hash"></param>
        private async Task AddMediaItem(FileResult fileResult, string hash)
        {
            try
            {
                string directoryPath = FileHandler.CreateDirectory("MediaItems");

                string filename = FileHandler.GetUniqueFilename(fileResult.FileName, directoryPath);
                string filetype = FileHandler.ExtractFiletypeFromPath(filename);

                string filepath = Path.Combine(directoryPath, filename);
                string thumbnailPath = "";

                using (Stream fileStream = await fileResult.OpenReadAsync())
                {
                    FileHandler.SaveFile(fileStream, filepath);
                }

                if (filetype.Contains("jpg") || filetype.Contains("jpeg") || filetype.Contains("png"))
                {
                    thumbnailPath = FileHandler.CreateThumbnailAndReturnThumbnailPath(filename, filepath, 10);
                }

                await MediaItemDBHandler.AddMediaItem(filename, filepath, thumbnailPath, filetype, hash, "", 0);

            } catch(Exception exc)
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
        public void SetProgressElements(float currentProgress, float maxProgress, Stopwatch stopwatch)
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
                    DisableDeleteButtonsAndShowProgressIndicators();
                    Progress = currentProgress;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    await DeleteMediaItem(selectedMediaItem, currentProgress, maxProgress, stopwatch);

                    stopwatch.Stop();

                    MediaItems.Remove(selectedMediaItem);
                    List<MediaItem> currentMediaItems = new List<MediaItem>();
                    currentMediaItems.AddRange(MediaItems);
                    MediaItems = currentMediaItems;

                    ResetDeleteButtonsAndProgressIndicators();
                    SelectedMediaItem = null;
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

                if(File.Exists(mediaItem.ThumbnailPath))
                {
                    File.Delete(mediaItem.ThumbnailPath);
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
                DisableDeleteButtonsAndShowProgressIndicators();
                Progress = currentProgress;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                foreach (MediaItem mediaItem in mediaItemsForDeletion)
                {
                    await DeleteMediaItem(mediaItem, currentProgress, maxProgress, stopwatch);
                }

                stopwatch.Stop();

                ResetDeleteButtonsAndProgressIndicators();
                SelectedMediaItem = null;

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
        public void ResetDeleteButtonsAndProgressIndicators()
        {
            IsProgressBarVisible = false;
            IsDeleteAllMediaItemsButtonEnabled = true;
            IsDeleteSelectedMediaItemButtonEnabled = true;
            IsActivityIndicatorRunning = false;
        }
        #endregion

        #region DisableDeleteButtonsAndShowProgressIndicators
        /// <summary>
        /// Disables the delete buttons and enables the ActivityIndicator and the Progressbar.
        /// </summary>
        public void DisableDeleteButtonsAndShowProgressIndicators()
        {
            IsProgressBarVisible = true;
            IsDeleteAllMediaItemsButtonEnabled = false;
            IsDeleteSelectedMediaItemButtonEnabled = false;
            IsActivityIndicatorRunning = true;
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
                    await BindMultipleLifethemesToOneMediaItem(selectedMediaItem.Id, selectedLifethemes);

                    CurrentMediaItemLifethemes = await MediaItemLifethemesDBHandler.GetLifethemesOfMediaItem(selectedMediaItem.Id);
                } catch(Exception exc)
                {
                    throw exc;
                }
            }
        }
        #endregion

        #region BindMultipleLifethemesToOneMediaItem
        /// <summary>
        /// Binds each of the given Lifethemes to the given MediaItem-ID.
        /// Throws an exception if an error occurs during this process.
        /// </summary>
        /// <param name="mediaItemId"></param>
        /// <param name="lifethemes"></param>
        /// <returns></returns>
        private async Task BindMultipleLifethemesToOneMediaItem(int mediaItemId, List<Lifetheme> lifethemes)
        {
            try
            {
                foreach (Lifetheme lifetheme in lifethemes)
                {
                    await MediaItemLifethemesDBHandler.BindMediaItemLifetheme(mediaItemId, lifetheme.Id);
                }

            } catch(Exception exc)
            {
                throw exc;
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

        #region ShowCSVPicker
        /// <summary>
        /// Shows csv file picker.
        /// The picked csv-files must be of the format (Dateiname, Beschreibung, Quelle, Tags).
        /// If there are MediaItems with a Name-attribute that matches the Dateiname-cell,
        /// it's Notes-attribute and Lifethemes will be updated with the file-contents.
        /// Throws an exception if the process fails.
        /// </summary>
        public async void ShowCSVPicker()
        {

            try
            {
                FilePickerFileType filePickerFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>> {
                        { DevicePlatform.iOS, new [] { "commaSeparatedText"} },
                        { DevicePlatform.Android, new [] { "text/csv" } },
                        { DevicePlatform.UWP, new []{ "*.csv"} }
                    });

                PickOptions pickOptions = new PickOptions
                {
                    PickerTitle = "Wählen Sie eine CSV-Datei aus",
                    FileTypes = filePickerFileType,
                };

                FileResult pickedFile = await FilePicker.PickAsync(pickOptions);

                if (pickedFile != null)
                {
                    DisableDeleteButtonsAndShowProgressIndicators();

                    await LoadCSVMediaItemMetaDataList(pickedFile);

                    MediaItems = await MediaItemDBHandler.FilterMediaItems(isPhotosFilterChecked, isMusicFilterChecked, isDocumentsFilterChecked, isFilmsFilterChecked, isLinksFilterChecked);
                    
                    ResetDeleteButtonsAndProgressIndicators();
                    SelectedMediaItem = null;
                }

            }
            catch (Exception exc)
            {

                MediaItems = await MediaItemDBHandler.FilterMediaItems(isPhotosFilterChecked, isMusicFilterChecked, isDocumentsFilterChecked, isFilmsFilterChecked, isLinksFilterChecked);
                ResetDeleteButtonsAndProgressIndicators();
                SelectedMediaItem = null;

                throw exc;
            }
        }
        #endregion

        #region LoadCSVMediaItemMetaDataList
        /// <summary>
        /// Loads CSVMediaItemMetaData-list from given fileResult.
        /// Throws an exception if an error happened.
        /// </summary>
        /// <param name="fileResult"></param>
        /// <returns></returns>
        private async Task LoadCSVMediaItemMetaDataList(FileResult fileResult)
        {
            Stopwatch stopwatch = new Stopwatch();

            try {
                
                using (Stream stream = await fileResult.OpenReadAsync())
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);

                    string csvAsString = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                    if (csvAsString.Length == 0)
                    {
                        return;
                    }

                    csvAsString = csvAsString.Trim();
                    string[] csvRows = csvAsString.Split('\n');

                    float maxProgress = csvRows.Length;
                    float currentProgress = 0;
                    Progress = currentProgress;
                    stopwatch.Start();

                    for(int i = 2; i < csvRows.Length; i++)
                    {
                        currentProgress++;
                        SetProgressElements(currentProgress, maxProgress, stopwatch);

                        CSVMediaItemMetaData csvMediaItemMetaData = await BuildCSVMediaItemMetaDataFromCSVRow(csvRows[i]);

                        MediaItem foundMediaItem = MediaItems.FirstOrDefault(mediaItem => mediaItem.Name == csvMediaItemMetaData.FileName);

                        if (foundMediaItem != null)
                        {
                            await MediaItemDBHandler.UpdateNotes(foundMediaItem.Id, csvMediaItemMetaData.Notes);
                            await BindMultipleLifethemesToOneMediaItem(foundMediaItem.Id, csvMediaItemMetaData.Lifethemes);
                        }
                    }

                    stopwatch.Stop();

                }
            } catch(Exception exc)
            {
                stopwatch.Stop();
                throw exc;
            }
        }
        #endregion

        #region BuildCSVMediaItemMetaDataFromCSVRow
        /// <summary>
        /// Builds CSVMediaItemMetaData-object by given csv-row.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private async Task<CSVMediaItemMetaData> BuildCSVMediaItemMetaDataFromCSVRow(string row)
        {
            //row contains: Dateiname, Beschreibung, Quelle, Tags (Tags are seperated by comma)
            string[] rowSplit = row.Split(',');
            CSVMediaItemMetaData csvMediaItemMetaData = new CSVMediaItemMetaData();

            for (int i = 0; i < rowSplit.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        csvMediaItemMetaData.FileName = rowSplit[i];
                        break;
                    case 1:
                        csvMediaItemMetaData.Notes = rowSplit[i];
                        break;
                    case 3:
                        csvMediaItemMetaData.Lifethemes = await BuildLifethemesFromCSVCell(rowSplit[i]);
                        break;
                    default:
                        break;
                }
            }
            return csvMediaItemMetaData;
        }
        #endregion

        #region BuildLifethemesFromCSVCell
        /// <summary>
        /// Returns the comma seperated lifethemes of the given csv-row.
        /// Splits row by comma and creates or loads lifethemes.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private async Task<List<Lifetheme>> BuildLifethemesFromCSVCell(string row)
        {
            List<Lifetheme> lifethemes = new List<Lifetheme>();

            if (row.Length > 0)
            {
                string[] lifethemeNames = row.Split(';');

                foreach (string lifethemeName in lifethemeNames)
                {
                    int lifethemeId = await LifethemeDBHandler.GetLifethemeIDByName(lifethemeName);

                    if (lifethemeId == -1)
                    {
                        lifethemeId = await LifethemeDBHandler.AddLifetheme(lifethemeName);
                    }

                    Lifetheme lifetheme = await LifethemeDBHandler.GetSingleLifetheme(lifethemeId);

                    lifethemes.Add(lifetheme);
                }
            }

            return lifethemes;
        }
        #endregion
    }
}
