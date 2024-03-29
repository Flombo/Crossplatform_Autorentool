﻿using Autorentool_RMT.Services.BackendCommunication.Helper;
using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using System.Text;
using System.Threading;
using Autorentool_RMT.Views;
using Xamarin.Essentials;

namespace Autorentool_RMT.Services.BackendCommunication
{
    class BackendPullMediaItems
    {
        #region Attributes
        private HttpRequestHelper httpRequestHelper;
        private ClientWebSocket clientWebSocket;
        private string deviceIdentifier;
        private MediaItemBackendIdHelper mediaItemBackendIdHelper;
        private ContentsPage contentsPage;
        private BackendContentHelper backendContentHelper;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of BackendPullMediaItems-Class
        /// </summary>
        /// <param name="contentManagementViewModel"></param>
        public BackendPullMediaItems(ContentsPage contentsPage)
        {
            this.contentsPage = contentsPage;
            deviceIdentifier = DeviceSerialNumberHelper.GetDeviceSerialNumber();
            mediaItemBackendIdHelper = new MediaItemBackendIdHelper();
            httpRequestHelper = new HttpRequestHelper();
        }
        #endregion

        #region InitHelper
        /// <summary>
        /// Inits helper classes HTTPRequestHelper and MediaItemBackendHelper.
        /// If the process fails, an exception will be thrown.
        /// </summary>
        /// <returns></returns>
        public async Task InitHelper()
        {
            try
            {
                await httpRequestHelper.GetCSRFToken(deviceIdentifier);
                await mediaItemBackendIdHelper.Init(deviceIdentifier);
                backendContentHelper = new BackendContentHelper(mediaItemBackendIdHelper, deviceIdentifier);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region InitWebSocket
        /// <summary>
        /// Creates a websocket connection to the backend websocketserver.
        /// If the connection is set, the websocket sends a first message with the  serialnumber and pulls new contents
        /// </summary>
        /// <returns></returns>
        public async Task InitWebSocket()
        {
            clientWebSocket = new ClientWebSocket();

            try
            {
                await clientWebSocket.ConnectAsync(new Uri("ws://141.28.44.195:8080"), CancellationToken.None);
                await SendFirstMessage();

                await Task.Factory.StartNew(async () =>
                {
                    while (!clientWebSocket.State.Equals(WebSocketState.Closed))
                    {
                        await RecieveMessage();
                    }
                }, 
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
                );
            }
            catch (Exception)
            {
                CloseWebSocket(WebSocketCloseStatus.InternalServerError);
            }
        }
        #endregion

        #region SendFirstMessage
        /// <summary>
        /// Sends the first message to the WebSocket-server if the connection is open.
        /// The first message must contain the serialNumber for further recognition of this device in the backend.
        /// Throws an exception when an error happens.
        /// </summary>
        /// <returns></returns>
        private async Task SendFirstMessage()
        {
            try
            {
                if (clientWebSocket.State.Equals(WebSocketState.Open))
                {

                    string serialNumber = JsonConvert.SerializeObject(
                        new
                        {
                            command = "",
                            serialNumber = deviceIdentifier
                        }
                        );

                    await SendMessageUsingMessageWebSocket(serialNumber);
                }
            } 
            catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region SendMessageUsingMessageWebSocket
        /// <summary>
        /// Sends message over websocket.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task SendMessageUsingMessageWebSocket(string message)
        {
            byte[] byteMessage = Encoding.UTF8.GetBytes(message);
            ArraySegment<byte> messageAsArraySegment = new ArraySegment<byte>(byteMessage);

            await clientWebSocket.SendAsync(messageAsArraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        #endregion

        #region RecieveMessage
        /// <summary>
        /// Will be called, when a message was recieved by the clientWebSocket.
        /// If the message equals own serial number and shouldPullContent is true, the device needs to pull contents from backend.
        /// If an error occurs, the ClientWebSocket will be closed.
        /// </summary>
        /// <returns></returns>
        private async Task RecieveMessage()
        {
            try
            {
                
                ArraySegment<byte> message = new ArraySegment<byte>(new byte[4096]);

                await clientWebSocket.ReceiveAsync(message, CancellationToken.None);
                string recievedMessage = Encoding.UTF8.GetString(message.Array);

                if (recievedMessage.Length > 0)
                {
                    WebSocketMessage webSocketMessage = JsonConvert.DeserializeObject<WebSocketMessage>(recievedMessage);

                    bool isOwnSerialNumber = webSocketMessage.SerialNumber == deviceIdentifier;
                    bool shouldPullContent = webSocketMessage.ShouldPullContent;

                    if (Preferences.Get("ShowPushDialog", false))
                    {
                        ProcessWebSocketMessage(webSocketMessage, isOwnSerialNumber, shouldPullContent);
                    } else
                    {
                        ShowDeletePromptOrSetImportFromBackendButton(webSocketMessage, isOwnSerialNumber, shouldPullContent);
                    }
                }
            }
            catch (Exception)
            {
                //if an exception was thrown, the websocket will be closed.
                CloseWebSocket(WebSocketCloseStatus.InternalServerError);
            }
        }
        #endregion

        #region ShowDeletePromptOrSetImportFromBackendButton
        private void ShowDeletePromptOrSetImportFromBackendButton(WebSocketMessage webSocketMessage, bool isOwnSerialNumber, bool shouldPullContent)
        {
            if (!ProcessDeleteCommands(webSocketMessage))
            {
                contentsPage.IsImportFromBackendButtonEnabled(isOwnSerialNumber && shouldPullContent);
            }
        }
        #endregion

        #region ProcessWebSocketMessage
        /// <summary>
        /// Checks if the WebSocketMessage commands to pull MediaItems or to delete MediaItems or Lifethemes.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="webSocketMessage"></param>
        /// <param name="isOwnSerialNumber"></param>
        /// <param name="shouldPullContent"></param>
        private void ProcessWebSocketMessage(WebSocketMessage webSocketMessage, bool isOwnSerialNumber, bool shouldPullContent)
        {
            try
            {

                if (isOwnSerialNumber && shouldPullContent)
                {
                    contentsPage.DisplayImportMediaFromBackendByPushDialog();
                } else {

                    //If deleting and showing the push-dailog are disabled, the ImportButton should be enabled.
                    if (!ProcessDeleteCommands(webSocketMessage))
                    {
                        contentsPage.IsImportFromBackendButtonEnabled(isOwnSerialNumber && shouldPullContent);
                    }

                }

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region ProcessDeleteCommands
        /// <summary>
        /// Displays a prompt for the deletion of MediaItems or Lifethemes, 
        /// depending on the contents of the AppMediaItemIDs- or LifethemeNames-attribute.
        /// If the user disabled the deletion nothing will happen.
        /// Throws an exception if an error happened.
        /// </summary>
        /// <param name="webSocketMessage"></param>
        private bool ProcessDeleteCommands(WebSocketMessage webSocketMessage)
        {
            try
            {
                bool shouldDelete = Preferences.Get("AllowAdminsDeletion", false);

                if (shouldDelete)
                {
                    shouldDelete = webSocketMessage.AppMediaItemIDs.Count > 0 || webSocketMessage.LifethemeNames.Count > 0;

                    //check if this device should delete mediaItems
                    if (webSocketMessage.AppMediaItemIDs.Count > 0)
                    {
                        contentsPage.DisplayDeleteMediaViaDeleteCommand(webSocketMessage.AppMediaItemIDs);
                    }

                    //check if this device shoudl delete liftheme
                    else if (webSocketMessage.LifethemeNames.Count > 0)
                    {
                        contentsPage.DisplayDeleteLifethemesViaDeleteCommandDialog(webSocketMessage.LifethemeNames);
                    }
                }

                return shouldDelete;

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region CloseWebSocket
        /// <summary>
        /// Closes the clientWebSocket.
        /// </summary>
        /// <param name="reason">reasons for closing the websocket</param>
        public async void CloseWebSocket(WebSocketCloseStatus reason)
        {
            try
            {
                if (clientWebSocket != null && clientWebSocket.State != WebSocketState.Aborted)
                {
                    await clientWebSocket.CloseAsync(reason, null, CancellationToken.None);
                    clientWebSocket.Dispose();
                } else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                clientWebSocket = null;
            }
        }
        #endregion

        #region DeleteMediaItemsRetrievedByWebSocket
        /// <summary>
        /// Deletes MediaItems that were requested for deletion by the backend.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="mediaItems"></param>
        /// <returns></returns>
        public async Task DeleteMediaItemsRetrievedByWebSocket(List<MediaItem> mediaItems)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();

                int filesDeleted = 0;
                contentsPage.SetProgressElementsVisibility(true);

                stopwatch.Start();

                foreach (MediaItem mediaItem in mediaItems)
                {
                    filesDeleted++;
                    await DeleteMediaItem(mediaItem);
                    contentsPage.SetProgressElements(filesDeleted, mediaItems.Count, stopwatch);
                }

                stopwatch.Stop();

                contentsPage.SetProgressElementsVisibility(false);

                await contentsPage.LoadAllMediaItems();

            } 
            catch (Exception exc)
            {
                contentsPage.SetProgressElementsVisibility(false);
                throw exc;
            }
        }
        #endregion

        #region DeleteMediaItem
        /// <summary>
        /// Deletes MediaItem by id.
        /// Unbinds all Lifethemes and Sessions from the given MediaItem.
        /// Throws an exception if the process fails.
        /// </summary>
        /// <param name="mediaItem"></param>
        /// <returns></returns>
        private async Task DeleteMediaItem(MediaItem mediaItem)
        {
            try
            {
                if (mediaItem != null)
                {
                    await MediaItemLifethemesDBHandler.UnbindMediaItemLifethemesByMediaItemId(mediaItem.Id);
                    await SessionMediaItemsDBHandler.UnbindSessionMediaItemsByMediaItemId(mediaItem.Id);

                    await MediaItemDBHandler.DeleteMediaItem(mediaItem.Id);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region PullContentsFromBackend
        /// <summary>
        /// Pulls both Lifethemes and MediaItems from Backend via HttpClient.
        /// The serial number is required and sent as JSON
        /// </summary>
        /// <returns>boolean that is used in the ContentView to decide whether the ImportMediaFromBackendButton should be diabled/enabled</returns>
        public async Task<bool> PullContentsFromBackend()
        {
            try
            {
                string serialNumber = JsonConvert.SerializeObject(
                    new
                    {
                        serial_number = deviceIdentifier
                    }
                    );

                HttpResponseMessage response = await httpRequestHelper.SendRequestToBackend(serialNumber, "http://141.28.44.195/pullmediaitemsofbackend");
                string body = await response.Content.ReadAsStringAsync();
                return await SavePulledMediaItemsFromBackend(body);

            }
            catch (Exception)
            {
                //if request or SavePulledMediaItemsFromBackend method fails an error dialog will be displayed and the ImportMediaFromBackendButton will be disabled.
                contentsPage.DisplayImportMediaFromBackendConnectionErrorDialog();
                return false;
            }
        }
        #endregion

        #region ShouldDownloadMediaItemsFromBackend
        /// <summary>
        /// Checks if there are contents that can be pulled into app.
        /// The serial number is required and sent as JSON.
        /// The response is sent in JSON format and must be read as String.
        /// If the request fails, an excetpion will be thrown.
        /// </summary>
        /// <returns>boolean that is used in the ContentsPage to decide whether the ImportMediaFromBackendButton should be diabled/enabled</returns>
        public async Task<bool> ShouldDownloadMediaItemsFromBackend()
        {
            try
            {
                return await backendContentHelper.ShouldDownloadPropertiesFromBackend(
                    deviceIdentifier,
                    "http://141.28.44.195/getnewermediaitemsofbackend",
                    httpRequestHelper
                    );
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region SavePulledMediaItemsFromBackend
        /// <summary>
        /// Saves pulled MediaItems.
        /// JSON response string must be deserialized to MediaItem Model.
        /// Foreach deserialized MediaItem a new MediaItem will be created or updated.
        /// Throws an exception if these processes failed.
        /// </summary>
        /// <param name="pulledMediaItemJSON">json string with pulled mediaItem data</param>
        /// <returns>boolean that is used in the ContentsPage to decide whether the ImportMediaFromBackendButton should be diabled/enabled</returns>
        private async Task<bool> SavePulledMediaItemsFromBackend(string pulledMediaItemJSON)
        {
            try
            {
                List<MediaItem> backendMediaItems = JsonConvert.DeserializeObject<List<MediaItem>>(pulledMediaItemJSON);
                int maxProgress = backendMediaItems.Count;
                int currentProgress = 0;

                contentsPage.SetProgressElementsVisibility(true);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                
                foreach (MediaItem backendMediaItem in backendMediaItems)
                {
                    currentProgress++;
                    contentsPage.SetProgressElements(currentProgress, maxProgress, stopwatch);

                    MediaItem existingMediaItem = await MediaItemDBHandler.GetMediaItemByBackendMediaItemId(backendMediaItem.Id);

                    if (existingMediaItem == null)
                    {
                        await backendContentHelper.DownloadFileFromBackend(null, backendMediaItem);
                    }
                    else
                    {
                        await backendContentHelper.DownloadFileFromBackend(existingMediaItem, backendMediaItem);
                    }
                }

                stopwatch.Stop();

                await contentsPage.LoadAllMediaItems();

                contentsPage.SetProgressElementsVisibility(false);

                //sets latest date of app database in backend
                bool latestDeviceContentUpdateIsSet = await SetLatestDeviceContentUpdate();

                return !latestDeviceContentUpdateIsSet
                    ? throw new Exception()
                    : await ShouldDownloadMediaItemsFromBackend();

            } 
            catch (Exception exc)
            {
                contentsPage.SetProgressElementsVisibility(false);
                await contentsPage.LoadAllMediaItems();

                throw exc;
            }
        }
        #endregion

        #region SetLatestDeviceContentUpdate
        /// <summary>
        /// Sets latest device content update date in backend.
        /// Throws an exception if the request failed.
        /// </summary>
        /// <returns>returns boolean that is set by whether the request was successfull or not</returns>
        private async Task<bool> SetLatestDeviceContentUpdate()
        {
            try
            {
                return await backendContentHelper.SetLatestDevicePropertyUpdate(
                    deviceIdentifier,
                    "http://141.28.44.195/setlatestdevicecontentupdate",
                    httpRequestHelper
                );
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

    }
}