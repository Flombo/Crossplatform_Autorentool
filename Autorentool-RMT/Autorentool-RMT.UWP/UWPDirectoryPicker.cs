using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Pickers;
using System.Collections.Generic;
using Xamarin.Forms;
using Autorentool_RMT.Services;
using System.IO;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.ViewModels;
using System.Diagnostics;

[assembly: Dependency(typeof(Autorentool_RMT.UWP.UWPDirectoryPicker))]
namespace Autorentool_RMT.UWP
{
    class UWPDirectoryPicker : IDirectoryPicker
    {

        #region GetDirectoryFilePaths
        /// <summary>
        /// Shows the folder-picker, saves the contained files and builds MediaItems.
        /// Throws an exception if an error occured.
        /// </summary>
        /// <param name="contentManagementViewModel"></param>
        /// <returns></returns>
        public async Task ShowFolderPicker(ContentManagementViewModel contentManagementViewModel)
        {
            try
            {
                StorageFolder pickedFolder = await SelectFolderAsync();

                if (pickedFolder == null)
                {
                    return;
                }

                QueryOptions queryOptions = new QueryOptions
                {
                    FolderDepth = FolderDepth.Deep,
                    IndexerOption = IndexerOption.UseIndexerWhenAvailable
                };

                StorageFileQueryResult query = pickedFolder.CreateFileQueryWithOptions(queryOptions);
                List<StorageFile> filesFromFolder = new List<StorageFile>(await query.GetFilesAsync());

                await SaveDirectoryFiles(filesFromFolder, contentManagementViewModel);

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region SelectFolderAsync
        /// <summary>
        /// Shows the folder-picker and returns the picked StorageFolder.
        /// The start of the picker is the desktop.
        /// Throws an exception if the process fails.
        /// </summary>
        /// <returns></returns>
        private static async Task<StorageFolder> SelectFolderAsync()
        {
            try
            {
                FolderPicker folderPicker = new FolderPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.Desktop
                };

                folderPicker.FileTypeFilter.Add(".mp3");
                folderPicker.FileTypeFilter.Add(".mp4");
                folderPicker.FileTypeFilter.Add(".jpeg");
                folderPicker.FileTypeFilter.Add(".jpg");
                folderPicker.FileTypeFilter.Add(".png");
                folderPicker.FileTypeFilter.Add(".txt");
                folderPicker.FileTypeFilter.Add(".html");

                StorageFolder folder = await folderPicker.PickSingleFolderAsync();

                return folder;

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region SaveDirectoryFiles
        /// <summary>
        /// Saves the StorageFiles of the picked directory if their filetype is valid.
        /// Creates and saves a thumbnail if the current file is an image.
        /// Sets the progressbar and progresstext.
        /// Throws an error if the process fails.
        /// </summary>
        /// <param name="directoryFiles"></param>
        /// <param name="contentManagementViewModel"></param>
        /// <returns></returns>
        private async Task SaveDirectoryFiles(List<StorageFile> directoryFiles, ContentManagementViewModel contentManagementViewModel)
        {
            try
            {
                float maxProgress = directoryFiles.Count;
                float currentProgress = 0;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                contentManagementViewModel.SetProgressElements(currentProgress, maxProgress, stopwatch);
                contentManagementViewModel.DisableDeleteButtonsAndShowProgressIndicators();

                foreach (StorageFile directoryFile in directoryFiles)
                {
                    currentProgress++;
                    contentManagementViewModel.SetProgressElements(currentProgress, maxProgress, stopwatch);
                    string filename = directoryFile.Name;
                    var fileType = directoryFile.FileType.ToLower();

                    if (FileHandler.IsFiletypeValid(fileType))
                    {
                        await SaveFileAndCreateMediaItem(directoryFile, filename);
                    }
                }

                stopwatch.Stop();
                contentManagementViewModel.SetProgressElements(currentProgress, maxProgress, stopwatch);
                contentManagementViewModel.ResetDeleteButtonsAndProgressIndicators();

            } catch(Exception exc)
            {

                contentManagementViewModel.ResetDeleteButtonsAndProgressIndicators();

                throw exc;
            }
        }
        #endregion

        #region SaveFileAndCreateMediaItem
        /// <summary>
        /// Saves the StorageFile and creates a MediaItem in db.
        /// If the file would cause a duplicate, it won't be saved.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="directoryFile"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private async Task SaveFileAndCreateMediaItem(StorageFile directoryFile, string filename)
        {
            try
            {
                using (Stream stream = await directoryFile.OpenStreamForReadAsync())
                {
                    string hash = FileHandler.GetFileHashAsString(stream);
                    int duplicate = await MediaItemDBHandler.SearchMediaItemWithGivenHash(hash);

                    if (duplicate == 0)
                    {
                        string directoryPath = FileHandler.CreateDirectory("MediaItems");

                        string uniqueFilename = FileHandler.GetUniqueFilename(filename, directoryPath);
                        string filetype = FileHandler.ExtractFiletypeFromPath(filename);

                        string filepath = Path.Combine(directoryPath, uniqueFilename);
                        string thumbnailPath = "";

                        using (Stream fileSaveStream = await directoryFile.OpenStreamForReadAsync())
                        {
                            FileHandler.SaveFile(fileSaveStream, filepath);
                        }

                        if (filetype.Contains("jpg") || filetype.Contains("jpeg") || filetype.Contains("png"))
                        {
                            thumbnailPath = FileHandler.CreateThumbnailAndReturnThumbnailPath(filename, filepath, 10);
                        }

                        await MediaItemDBHandler.AddMediaItem(filename, filepath, thumbnailPath, filetype, hash, "", 0);
                    }
                }
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

    }
}
