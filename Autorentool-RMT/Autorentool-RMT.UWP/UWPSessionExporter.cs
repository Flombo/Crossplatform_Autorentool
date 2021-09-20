using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using Autorentool_RMT.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.IO;

[assembly: Dependency(typeof(Autorentool_RMT.UWP.UWPSessionExporter))]
namespace Autorentool_RMT.UWP
{
    public class UWPSessionExporter : ISessionExporter
    {
        #region ExportSession
        /// <summary>
        /// Exports selected Session into the selected folder.
        /// The Session-data will be saved in session.json, while the MediaItems will be saved in mediaitems.json.
        /// Returns a boolean depending if a folder was picked.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="sessionViewModel"></param>
        /// <param name="selectedSession"></param>
        /// <returns></returns>
        public async Task<bool> ExportSession(SessionViewModel sessionViewModel, Session selectedSession)
        {
            try
            {
                StorageFolder pickedFolder = await FolderPickerHelper.SelectFolderAsync(
                    new string[1]
                    {
                        "*"
                    }
                    );

                if (pickedFolder == null)
                {
                    return false;
                }

                IReadOnlyList<StorageFile> files = await pickedFolder.GetFilesAsync();

                if (files == null || files.Count == 0)
                {
                    List<MediaItem> sessionMediaItems = await SessionMediaItemsDBHandler.GetMediaItemsOfSession(selectedSession.Id);

                    await BuildSessionJSONFile(selectedSession, pickedFolder);
                    await BuildMediaItemsJSONFile(sessionMediaItems, pickedFolder);
                    await CopyMediaItemsToTargetFolder(sessionMediaItems, pickedFolder, sessionViewModel);

                    return true;
                }
                else
                {
                    throw new Exception("Folder not empty");
                }
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region BuildMediaItemsJSONFile
        /// <summary>
        /// Builds a json string of the MediaItems and writes that string in the '.mediaItems'-file within the picked folder.
        /// </summary>
        /// <param name="sessionMediaItems"></param>
        /// <param name="pickedFolder"></param>
        /// <returns></returns>
        private async Task BuildMediaItemsJSONFile(List<MediaItem> sessionMediaItems, StorageFolder pickedFolder)
        {
            try
            {

                string mediaItemsJSONString = JsonConvert.SerializeObject(sessionMediaItems);

                StorageFile mediaItemsJSONFile = await pickedFolder.CreateFileAsync(".mediaItems", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(mediaItemsJSONFile, mediaItemsJSONString);

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region BuildSessionJSONFile
        /// <summary>
        /// Converts the selected Session in a json string and saves this string in a '.session'-file within the picked folder
        /// </summary>
        /// <param name="selectedSession"></param>
        /// <param name="pickedFolder"></param>
        /// <returns></returns>
        private async Task BuildSessionJSONFile(Session selectedSession, StorageFolder pickedFolder)
        {
            try
            {

                string sessionJSONString = JsonConvert.SerializeObject(selectedSession);

                StorageFile sessionJSONFile = await pickedFolder.CreateFileAsync(".session", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sessionJSONFile, sessionJSONString);

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region CopyMediaItemsToTargetFolder
        /// <summary>
        /// Copies the media-files of the MediaItem into target folder.
        /// </summary>
        /// <param name="sessionMediaItems"></param>
        /// <param name="pickedFolder"></param>
        /// <param name="sessionViewModel"></param>
        /// <returns></returns>
        private async Task CopyMediaItemsToTargetFolder(List<MediaItem> sessionMediaItems, StorageFolder pickedFolder, SessionViewModel sessionViewModel)
        {
            try
            {
                int currentProgress = 0;
                int maxProgress = sessionMediaItems.Count;

                sessionViewModel.IsProgressBarVisible = true;
                sessionViewModel.SetProgressAndStatus(currentProgress, maxProgress);

                foreach (MediaItem sessionMediaItem in sessionMediaItems)
                {
                    currentProgress++;
                    sessionViewModel.SetProgressAndStatus(currentProgress, maxProgress);

                    await CreateFileInTargetFolder(sessionMediaItem, pickedFolder);
                }

                sessionViewModel.IsProgressBarVisible = false;

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region CreateFileInTargetFolder
        /// <summary>
        /// Writes media-file into the target folder.
        /// </summary>
        /// <param name="sessionMediaItem"></param>
        /// <param name="pickedFolder"></param>
        /// <returns></returns>
        private async Task CreateFileInTargetFolder(MediaItem sessionMediaItem, StorageFolder pickedFolder)
        {
            StorageFile medium = await StorageFile.GetFileFromPathAsync(sessionMediaItem.Path);
            StorageFile createdMediaFile = await pickedFolder.CreateFileAsync(sessionMediaItem.Name);

            using (Stream stream = await medium.OpenStreamForReadAsync())
            {
                byte[] bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes, 0, bytes.Length);
                await FileIO.WriteBytesAsync(createdMediaFile, bytes);
            }
        }
        #endregion

    }
}
