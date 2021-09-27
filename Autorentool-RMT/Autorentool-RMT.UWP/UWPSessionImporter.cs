using Autorentool_RMT.Models;
using Autorentool_RMT.Services;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using Autorentool_RMT.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(Autorentool_RMT.UWP.UWPSessionImporter))]
namespace Autorentool_RMT.UWP
{
    public class UWPSessionImporter : ISessionImporter
    {

        #region ImportSession
        /// <summary>
        /// Imports a session and the corresponding MediaItems from the picked folder.
        /// Returns a boolean depending if a folder was picked.
        /// Throws an exception if this process fails.
        /// </summary>
        /// <param name="sessionViewModel"></param>
        /// <returns></returns>
        public async Task<bool> ImportSession(SessionViewModel sessionViewModel)
        {
            try
            {
                string[] fileTypeFilters = new string[]
                {
                    ".mp3",
                    ".mp4",
                    ".jpeg",
                    ".jpg",
                    ".png",
                    ".txt",
                    ".html",
                    ".mediaItems",
                    ".session"
                };

                StorageFolder pickedFolder = await FolderPickerHelper.SelectFolderAsync(fileTypeFilters);

                if(pickedFolder == null)
                {
                    return false;
                }

                IReadOnlyList<StorageFile> sessionFiles = await pickedFolder.GetFilesAsync();

                if (sessionFiles == null || sessionFiles.Count == 0)
                {
                    //Just create an empty Session, if no files are available in the folder.
                    Session session = new Session()
                    {
                        Name = pickedFolder.Name,
                        BackendSessionId = 0
                    };

                    await AddNewSession(session);
                    return true;
                }
                else
                {
                    Session session = await CreateOrUpdateSessionFromSessionJSON(sessionFiles, pickedFolder.Name);
                    await LoadAndDeserializeMediaItemsJSONFile(sessionFiles, session, sessionViewModel);

                    return true;
                }

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region CreateSessionFromSessionJSON
        /// <summary>
        /// Loads and deserializes a Session from the .session-JSON.
        /// Creates a new Session with the given attributes of the deserialized Session or updates an existing Session with these attributes.
        /// Re-throws an exception if these processes failed.
        /// </summary>
        /// <param name="sessionFiles"></param>
        /// <returns></returns>
        public async Task<Session> CreateOrUpdateSessionFromSessionJSON(IReadOnlyList<StorageFile> sessionFiles, string folderName)
        {
            try
            {
                StorageFile sessionJSONFile = sessionFiles.FirstOrDefault(sessionFile => sessionFile.Name.Equals(".session"));
                Session session = null;

                if (sessionJSONFile != null)
                {

                    string sessionJSONString = await GetJSONStringFromJSONFile(sessionJSONFile);

                    session = JsonConvert.DeserializeObject<Session>(sessionJSONString);

                    Session queriedSession = await SessionDBHandler.GetSingleSession(session.Id);

                    if (queriedSession != null && queriedSession.Name.Equals(session.Name))
                    {
                        await SessionDBHandler.UpdateSession(session.Id, session.BackendSessionId, session.Name);
                    }
                    else
                    {
                        session = await AddNewSession(session);
                    }

                    return session;
                }
                else
                {
                    session = new Session()
                    {
                        Name = folderName,
                        BackendSessionId = 0
                    };

                    return await AddNewSession(session);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region AddNewSession
        /// <summary>
        /// Creates and returns a new Session by the parameters of the given Session.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private async Task<Session> AddNewSession(Session session)
        {
            try
            {
                int sessionID = await SessionDBHandler.AddSession(session.Name, session.BackendSessionId);
                return await SessionDBHandler.GetSingleSession(sessionID);

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region CreateOrUpdateMediaItems
        /// <summary>
        /// Extracts the JSON-string from the .mediaItems-file and deserializes it to a list of MediaItems.
        /// Persists them in db and sets the binding with the already built Session.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="sessionFiles"></param>
        /// <param name="session"></param>
        /// <param name="sessionViewModel"></param>
        /// <returns></returns>
        private async Task LoadAndDeserializeMediaItemsJSONFile(IReadOnlyList<StorageFile> sessionFiles, Session session, SessionViewModel sessionViewModel)
        {
            try
            {
                StorageFile mediaItemsJSON = sessionFiles.FirstOrDefault(sessionFile => sessionFile.Name.Equals(".mediaItems"));

                if (mediaItemsJSON != null)
                {
                    string mediaItemsJSONString = await GetJSONStringFromJSONFile(mediaItemsJSON);

                    List<MediaItem> mediaItems = JsonConvert.DeserializeObject<List<MediaItem>>(mediaItemsJSONString);

                    await SaveMediaItems(mediaItems, sessionViewModel, session, sessionFiles);
                }
                else
                {
                    //if the mediaItems-JSON is missing, create MediaItems from the existing media-files.
                    await SaveMediaItemsFromFile(sessionFiles, sessionViewModel, session);
                }

            } catch(Exception exc)
            {
                sessionViewModel.IsProgressBarVisible = false;

                throw exc;
            }
        }
        #endregion

        #region SaveMediaItemsFromFile
        /// <summary>
        /// Saves thumbnail and original medium if the .mediaItems-file doesn't exist.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="sessionFiles"></param>
        /// <param name="sessionViewModel"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        private async Task SaveMediaItemsFromFile(IReadOnlyList<StorageFile> sessionFiles, SessionViewModel sessionViewModel, Session session)
        {
            try
            {
                int currentProgress = 0;
                int maxProgress = sessionFiles.Count;
                sessionViewModel.IsProgressBarVisible = true;

                foreach (StorageFile mediaFile in sessionFiles)
                {
                    currentProgress++;
                    sessionViewModel.SetProgressAndStatus(currentProgress, maxProgress);
                    int mediaItemId = 0;

                    if (FileHandler.IsFiletypeValid(mediaFile.FileType))
                    {
                        using (Stream stream = await mediaFile.OpenStreamForReadAsync())
                        {
                            string filename = mediaFile.Name;
                            string hash = FileHandler.GetFileHashAsString(stream);
                            int duplicate = await MediaItemDBHandler.CountMediaItemDuplicates(hash);

                            MediaItem mediaItem = new MediaItem()
                            {
                                BackendMediaItemId = 0,
                                Notes = "",
                                Position = 0,
                                Name = filename
                            };

                            if (duplicate == 0)
                            {
                                mediaItemId = await AddMediaItem(mediaFile, hash, filename, mediaItem);
                                await BindLifethemesToMediaItem(mediaItem, mediaItemId);
                            }
                            else
                            {
                                mediaItemId = await MediaItemDBHandler.GetID(mediaItem.Name);
                            }

                            await BindMediaItemAndSession(mediaItemId, session.Id);
                        }
                    }
                }

                sessionViewModel.IsProgressBarVisible = false;

            } catch(Exception exc)
            {
                sessionViewModel.IsProgressBarVisible = false;

                throw exc;
            }
        }
        #endregion

        #region SaveMediaItems
        /// <summary>
        /// Enables/Disables the progressbar, sets the current progress and saves the given MediaItems.
        /// </summary>
        /// <param name="mediaItems"></param>
        /// <param name="sessionViewModel"></param>
        /// <param name="session"></param>
        /// <param name="sessionFiles"></param>
        /// <returns></returns>
        private async Task SaveMediaItems(
            List<MediaItem> mediaItems,
            SessionViewModel sessionViewModel,
            Session session,
            IReadOnlyList<StorageFile> sessionFiles
            )
        {
            try
            {
                int maxProgress = mediaItems.Count;
                int currentProgress = 0;
                sessionViewModel.Progress = 0;
                sessionViewModel.IsProgressBarVisible = true;

                foreach (MediaItem mediaItem in mediaItems)
                {
                    currentProgress++;
                    sessionViewModel.SetProgressAndStatus(currentProgress, maxProgress);
                    await CreateOrUpdateMediaItem(sessionFiles, mediaItem, session);
                }

                sessionViewModel.IsProgressBarVisible = false;

            } catch(Exception exc)
            {
                sessionViewModel.IsProgressBarVisible = false;

                throw exc;
            }
        }
        #endregion

        #region CreateOrUpdateMediaItem
        /// <summary>
        /// Creates or updates MediaItem depending if the file already exists in the MediaItems-folder.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="sessionFiles"></param>
        /// <param name="mediaItem"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        private async Task CreateOrUpdateMediaItem(IReadOnlyList<StorageFile> sessionFiles, MediaItem mediaItem, Session session)
        {
            try
            {

                StorageFile mediaFile = sessionFiles.FirstOrDefault(sessionFile => sessionFile.Name.Equals(mediaItem.Name));
                int mediaItemId = 0;

                if(FileHandler.IsFiletypeValid(mediaFile.FileType))
                {
                    using (Stream stream = await mediaFile.OpenStreamForReadAsync())
                    {
                        string filename = mediaFile.Name;
                        string hash = FileHandler.GetFileHashAsString(stream);
                        int duplicate = await MediaItemDBHandler.CountMediaItemDuplicates(hash);

                        if (duplicate == 0)
                        {
                            mediaItemId = await AddMediaItem(mediaFile, hash, filename, mediaItem);
                            await BindLifethemesToMediaItem(mediaItem, mediaItemId);
                        }
                        else
                        {
                            mediaItemId = await MediaItemDBHandler.GetID(mediaItem.Name);
                            await UpdateExistingMediaItem(mediaItem, mediaItemId);
                        }

                        await BindMediaItemAndSession(mediaItemId, session.Id);
                    }
                }

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region BindMediaItemAndSession
        /// <summary>
        /// Binds MediaItem and Session by given ID's if there isn't already a binding for those ID's.
        /// </summary>
        /// <param name="mediaItemId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        private async Task BindMediaItemAndSession(int mediaItemId, int sessionId)
        {
            try
            {
                int sessionMediaItemsId = await SessionMediaItemsDBHandler.GetID(mediaItemId, sessionId);

                if (sessionMediaItemsId == -1)
                {
                    await SessionMediaItemsDBHandler.BindSessionMediaItems(mediaItemId, sessionId);
                }

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region AddMediaItem
        /// <summary>
        /// Saves the given mediaFile into the MediaItems-folder and creates a MediaItem with the attributes of the given mediaItem.
        /// Creates a thumbnail.
        /// If this processes failed, an exception will be thrown.
        /// </summary>
        /// <param name="mediaFile"></param>
        /// <param name="hash"></param>
        /// <param name="filename"></param>
        /// <param name="mediaItem"></param>
        /// <returns></returns>
        private async Task<int> AddMediaItem(StorageFile mediaFile, string hash, string filename, MediaItem mediaItem)
        {
            try
            {
                string directoryPath = FileHandler.CreateDirectory("MediaItems");

                string uniqueFilename = FileHandler.GetUniqueFilename(filename, directoryPath);
                string filetype = FileHandler.ExtractFiletypeFromPath(filename);

                string filepath = Path.Combine(directoryPath, uniqueFilename);
                string thumbnailPath = "";

                using (Stream fileSaveStream = await mediaFile.OpenStreamForReadAsync())
                {
                    FileHandler.SaveFile(fileSaveStream, filepath);
                }

                if (filetype.Contains("jpg") || filetype.Contains("jpeg") || filetype.Contains("png"))
                {
                    thumbnailPath = FileHandler.CreateThumbnailAndReturnThumbnailPath(filename, filepath, 10);
                }

                return await MediaItemDBHandler.AddMediaItem(filename, filepath, thumbnailPath, filetype, hash, mediaItem.Notes, mediaItem.BackendMediaItemId);

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region UpdateExistingMediaItem
        /// <summary>
        /// Updates the notes and position fields of the existing MediaItem.
        /// Binds Lifethemes and the MediaItem together.
        /// Re-throws exception, if an error happens.
        /// </summary>
        /// <param name="mediaItem"></param>
        /// <param name="mediaItemId"></param>
        /// <returns></returns>
        private async Task UpdateExistingMediaItem(MediaItem mediaItem, int mediaItemId)
        {
            try
            {

                if (mediaItemId != -1)
                {
                    await MediaItemDBHandler.UpdateNotes(mediaItemId, mediaItem.Notes);
                    await MediaItemDBHandler.UpdatePosition(mediaItemId, mediaItem.Position);
                    await BindLifethemesToMediaItem(mediaItem, mediaItemId);
                }

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region BindLifethemesToMediaItem
        /// <summary>
        /// Binds Lifethemes to the given MediaItem, if the given MediaItem has Lifethemes.
        /// If these Lifethemes aren't already in the db, they need to be created.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="mediaItem"></param>
        /// <param name="mediaItemId"></param>
        /// <returns></returns>
        private async Task BindLifethemesToMediaItem(MediaItem mediaItem, int mediaItemId)
        {
            try
            {
                if (mediaItem.Lifethemes != null)
                {
                    foreach (Lifetheme lifetheme in mediaItem.Lifethemes)
                    {
                        int queriedLifethemeId = await LifethemeDBHandler.GetLifethemeIDByName(lifetheme.Name);

                        if (queriedLifethemeId == -1)
                        {
                            queriedLifethemeId = await LifethemeDBHandler.AddLifetheme(lifetheme.Name);
                        }

                        await MediaItemLifethemesDBHandler.BindMediaItemLifetheme(mediaItemId, queriedLifethemeId);
                    }
                }
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region GetJSONStringFromJSONFile
        /// <summary>
        /// Extracts the JSON-string from the given JSON-file.
        /// If an error occurs, an exception will be thrown.
        /// </summary>
        /// <param name="jsonFile"></param>
        /// <returns></returns>
        private async Task<string> GetJSONStringFromJSONFile(StorageFile jsonFile)
        {
            try
            {
                string jsonString = "";

                using (Stream stream = await jsonFile.OpenStreamForReadAsync())
                {
                    byte[] bytes = new byte[stream.Length];
                    await stream.ReadAsync(bytes, 0, bytes.Length);
                    jsonString = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                }

                return jsonString;

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

    }
}
