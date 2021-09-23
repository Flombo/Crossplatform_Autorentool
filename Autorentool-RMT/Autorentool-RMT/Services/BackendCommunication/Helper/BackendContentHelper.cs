using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.BackendCommunication.Helper
{
    public class BackendContentHelper
    {

        #region Attributes
        private MediaItemBackendIdHelper mediaItemBackendIdHelper;
        private string deviceIdentifier;
        private readonly WebClient webClient;
        #endregion

        #region Constructor
        /// <summary>
        /// constructor of BackendContentHelper.
        /// </summary>
        /// <param name="mediaItemBackendIdHelper"></param>
        /// <param name="deviceIdentifier"></param>
        public BackendContentHelper(MediaItemBackendIdHelper mediaItemBackendIdHelper,string deviceIdentifier)
        {
            this.mediaItemBackendIdHelper = mediaItemBackendIdHelper;
            this.deviceIdentifier = deviceIdentifier;
            webClient = new WebClient();
        }
        #endregion

        #region DownloadFileFromBackend
        /// <summary>
        /// Downloads file from backend.
        /// If the pulled file wouldn't cause a duplication, only the BackendMediaItemData must be persisted.
        /// Else the new file must be saved.
        /// Throws an exception if the process failed.
        /// </summary>
        /// <param name="backendFilePath">path to backend MediaItem file</param>
        /// <returns>returns newly created StorageFile from Backend</returns>
        public async Task DownloadFileFromBackend(MediaItem existingMediaItem, MediaItem backendMediaItem)
        {
            try
            {
                Uri backendFileURI = new Uri("http://127.0.0.1:8000/getmedium?backendmediaitempath=" + backendMediaItem.Path + "&serialnumber=" + deviceIdentifier);

                byte[] fileData = webClient.DownloadData(backendFileURI);

                string hash = FileHandler.GetFileHashAsString(new MemoryStream(fileData));
                int duplicate = await MediaItemDBHandler.SearchMediaItemWithGivenHash(hash);

                if (duplicate == 0)
                {
                    await SaveFilesAndPersistBackendMediaItemData(existingMediaItem, backendMediaItem, new MemoryStream(fileData), hash);

                } else
                {
                    await MediaItemDBHandler.UpdateMediaItem(existingMediaItem.Id, backendMediaItem.Name, backendMediaItem.Id, backendMediaItem.Notes);
                    await BindLifethemesAndMediaItemAndSetAppMediaItemId(backendMediaItem.Lifethemes, existingMediaItem.Id, backendMediaItem.Id);

                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region SaveFilesAndPersistBackendMediaItemData
        /// <summary>
        /// Saves pulled file and creates a thumbnail (if the file is an image)
        /// Updates or Creates a MediaItem with the data from the BackendMediaItem.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="existingMediaItem"></param>
        /// <param name="backendMediaItem"></param>
        /// <param name="stream"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        private async Task SaveFilesAndPersistBackendMediaItemData(MediaItem existingMediaItem, MediaItem backendMediaItem, Stream stream, string hash)
        {
            try
            {
                string directoryPath = FileHandler.CreateDirectory("MediaItems");

                string filename = GetFilename(existingMediaItem, backendMediaItem, directoryPath);

                string filetype = FileHandler.ExtractFiletypeFromPath(filename);

                string filepath = Path.Combine(directoryPath, filename);
                string thumbnailPath = "";

                FileHandler.SaveFile(stream, filepath);

                if (filetype.Contains("jpg") || filetype.Contains("jpeg") || filetype.Contains("png"))
                {
                    thumbnailPath = FileHandler.CreateThumbnailAndReturnThumbnailPath(filename, filepath, 10);
                }

                int mediaItemId = await UpdateOrAddMediaItem(existingMediaItem, hash, filepath, thumbnailPath, filetype, filename, backendMediaItem.Notes, backendMediaItem.Id);
                await BindLifethemesAndMediaItemAndSetAppMediaItemId(backendMediaItem.Lifethemes, mediaItemId, backendMediaItem.Id);

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region BindLifethemesAndMediaItemAndSetAppMediaItemId
        /// <summary>
        /// Binds the given Lifethemes to the given MediaItem-Id and sets the AppMediaItemId in the backend.
        /// Throws an exception if an error happend.
        /// </summary>
        /// <param name="lifethemes"></param>
        /// <param name="mediaItemId"></param>
        /// <param name="backendMediaItemId"></param>
        /// <returns></returns>
        private async Task BindLifethemesAndMediaItemAndSetAppMediaItemId(List<Lifetheme> lifethemes, int mediaItemId, int backendMediaItemId)
        {
            try
            {
                await SavePulledLifethemesFromBackend(lifethemes, mediaItemId);
                await mediaItemBackendIdHelper.SetAppMediaItemID(mediaItemId, backendMediaItemId);
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region GetFilename
        /// <summary>
        /// Returns the filename, depending on the existingMediaItem.
        /// If it isn't null, the filename is just its name.
        /// Else an unique filename has to be generated, because a new file will be created.
        /// </summary>
        /// <param name="existingMediaItem"></param>
        /// <param name="backendMediaItem"></param>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        private string GetFilename(MediaItem existingMediaItem, MediaItem backendMediaItem, string directoryPath)
        {
            string filename;

            if (existingMediaItem != null)
            {
                filename = existingMediaItem.Name;
            } else
            {
                filename = FileHandler.GetUniqueFilename(backendMediaItem.Name, directoryPath);
            }

            return filename;
        }
        #endregion

        #region UpdateOrAddMediaItem
        /// <summary>
        /// Adds new MediaItem or updates the fields hash, path and thumbnailPath, depending on the existingMediaItem.
        /// If it is null, a new MediaItem should be persisted.
        /// Else it should be updated.
        /// If these processes fail, then an exception will be thrown.
        /// </summary>
        /// <param name="existingMediaItem"></param>
        /// <param name="hash"></param>
        /// <param name="filepath"></param>
        /// <param name="thumbnailPath"></param>
        /// <param name="filetype"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private async Task<int> UpdateOrAddMediaItem(
            MediaItem existingMediaItem,
            string hash,
            string filepath,
            string thumbnailPath,
            string filetype,
            string filename,
            string notes,
            int backendMediaItemId
            )
        {
            try
            {
                int mediaItemId;

                if (existingMediaItem != null)
                {
                    mediaItemId = existingMediaItem.Id;
                    await MediaItemDBHandler.UpdateHashAndPathFields(mediaItemId, hash, filepath, thumbnailPath, notes);
                }
                else
                {
                    mediaItemId = await MediaItemDBHandler.AddMediaItem(filename, filepath, thumbnailPath, filetype, hash, notes, backendMediaItemId);
                }

                return mediaItemId;

            } catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region SavePulledLifethemesFromBackend
        /// <summary>
        /// Saves pulled Lifethemes.
        /// JSON response String must be deserialized to Lifetheme Model.
        /// foreach deserialized lifetheme a new Lifetheme will be created.
        /// if lifethemes contain mediaItems list, a new binding will be created between them.
        /// Throws an exception if the process failed.
        /// </summary>
        /// <param name="lifethemes">List of pulled LifeThemes from backend</param>
        /// <param name="mediaItem">created mediaItem from backend</param>
        private async Task SavePulledLifethemesFromBackend(List<Lifetheme> lifethemes, int mediaItemId)
        {
            try
            {
                foreach (Lifetheme lifetheme in lifethemes)
                {
                    int lifethemeId = await LifethemeDBHandler.GetLifethemeIDByName(lifetheme.Name);

                    if (lifethemeId == -1)
                    {
                        lifethemeId = await LifethemeDBHandler.AddLifetheme(lifetheme.Name);
                        await MediaItemLifethemesDBHandler.BindMediaItemLifetheme(mediaItemId, lifethemeId);
                    } else
                    {
                        await MediaItemLifethemesDBHandler.BindMediaItemLifetheme(mediaItemId, lifethemeId);
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region ShouldDownloadPropertiesFromBackend
        /// <summary>
        /// Checks if there are properties that can be pulled into app.
        /// The serial number is required and sent as JSON.
        /// The response is sent in JSON format and must be read as String.
        /// If the request fails, false will be returned.
        /// Throws an exception if an error happened.
        /// </summary>
        /// <returns>boolean that is used in the ContentView/SessionView to decide whether the ImportMediaFromBackendButton/ImportSessionsFromBackendButton should be diabled/enabled</returns>
        public async Task<bool> ShouldDownloadPropertiesFromBackend(string deviceIdentifier, string targetURL, HttpRequestHelper httpRequestHelper)
        {
            try
            {
                string serialNumber = JsonConvert.SerializeObject(
                   new
                   {
                       serial_number = deviceIdentifier
                   }
                );

                HttpResponseMessage response = await httpRequestHelper.SendRequestToBackend(serialNumber, targetURL);
                string body = await response.Content.ReadAsStringAsync();
                HttpMessage httpMessage = JsonConvert.DeserializeObject<HttpMessage>(body);

                return httpMessage.ShouldDownload;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region SetLatestDevicePropertyUpdate
        /// <summary>
        /// Sets latest device property update date in backend.
        /// Throws an exception if the process failed.
        /// </summary>
        /// <returns>returns boolean that is set by whether the request was successfull or not</returns>
        public async Task<bool> SetLatestDevicePropertyUpdate(string deviceIdentifier, string targetUrl, HttpRequestHelper httpRequestHelper)
        {
            try
            {
                string serialNumber = JsonConvert.SerializeObject(
                    new
                    {
                        serial_number = deviceIdentifier
                    }
                );

                HttpResponseMessage httpResponseMessage = await httpRequestHelper.SendRequestToBackend(serialNumber, targetUrl);
                return httpResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

    }
}