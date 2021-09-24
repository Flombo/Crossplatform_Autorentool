using Autorentool_RMT.Models;
using Autorentool_RMT.Services.BackendCommunication.Helper;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using Autorentool_RMT.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.BackendCommunication
{
    public class BackendPullSessions
    {

        #region Attributes
        private HttpRequestHelper httpRequestHelper;
        private ClientWebSocket clientWebSocket;
        private string deviceIdentifier;
        private MediaItemBackendIdHelper mediaItemBackendIdHelper;
        private SessionBackendIdHelper sessionBackendIdHelper;
        private SessionsPage sessionsPage;
        private BackendContentHelper backendContentHelper;
        #endregion

        #region Constructor
        /// <summary>
        /// constructor of BackendPullSessions-Class
        /// </summary>
        /// <param name="sessionsPage">Class needs SessionsView instance for updating the UI with pulled sessions</param>
        public BackendPullSessions(SessionsPage sessionsPage)
        {
            this.sessionsPage = sessionsPage;
            deviceIdentifier = DeviceSerialNumberHelper.GetDeviceSerialNumber();
            mediaItemBackendIdHelper = new MediaItemBackendIdHelper();
            sessionBackendIdHelper = new SessionBackendIdHelper();
            httpRequestHelper = new HttpRequestHelper();
        }
        #endregion

        #region InitHelper
        /// <summary>
        /// Inits helper classes HTTPRequestHelper, MediaItemBackendHelper, SessionBackendIdHelper and ResidentBackendIdHelper.
        /// If the process fails, the ImportSessionsFromBackendButton will be disabled
        /// </summary>
        /// <returns></returns>
        public async Task InitHelper()
        {
            try
            {
                await httpRequestHelper.GetCSRFToken(deviceIdentifier).ConfigureAwait(true);
                await mediaItemBackendIdHelper.Init(deviceIdentifier).ConfigureAwait(true);
                await sessionBackendIdHelper.Init(deviceIdentifier).ConfigureAwait(true);
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
                await clientWebSocket.ConnectAsync(new Uri("ws://127.0.0.1:8080"), CancellationToken.None);
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

        #region RecieveMessage
        /// <summary>
        /// Will be called, when a message was recieved by the clientWebSocket.
        /// If the message equals own serial number and shouldPullSessions is true, the device needs to pull sessions from backend.
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

                    //check if this device should pull contents from backend 
                    if (webSocketMessage.SerialNumber == deviceIdentifier && webSocketMessage.ShouldPullSessions)
                    {
                        sessionsPage.DisplayImportSessionsFromBackendByPushDialog();
                    }

                    //check if this device should delete mediaItems
                    else if (webSocketMessage.AppSessionIDs.Count > 0)
                    {
                        sessionsPage.DisplayDeleteSessionsViaDeleteCommand(webSocketMessage.AppSessionIDs);
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
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                clientWebSocket.Dispose();
                clientWebSocket = null;
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
            catch (Exception exc)
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

        #region DeleteSessionsRetrievedByWebSocket
        /// <summary>
        /// Deletes requested sessions from WebSocket
        /// </summary>
        /// <param name="mediaItems"></param>
        /// <returns></returns>
        public async Task DeleteSessionsRetrievedByWebSocket(List<Session> sessions)
        {
            try
            {
                int sessionsDeleted = 0;
                sessionsPage.SetVisibilityOfProgressBarAndRing(true);

                foreach (Session session in sessions)
                {
                    await DeleteSession(session);
                    sessionsDeleted++;
                    sessionsPage.SetProgressBarStatusTxt(sessions.Count, sessionsDeleted);
                }

                await sessionsPage.LoadAllSessions();
                sessionsPage.SetVisibilityOfProgressBarAndRing(false);

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region DeleteSession
        /// <summary>
        /// Deletes Session by given Session.
        /// Deletes relationship between Lifethemes and MediaItems, if there is one.
        /// </summary>
        /// <param name="mediaItem"></param>
        /// <returns></returns>
        private async Task DeleteSession(Session session)
        {
            try
            {
                if (session != null)
                {
                    int sessionId = session.Id;

                    await SessionMediaItemsDBHandler.UnbindSessionMediaItemsBySessionId(sessionId);
                    await ResidentSessionsDBHandler.UnbindAllResidentSessionsBySessionId(sessionId);
                    await SessionDBHandler.DeleteSession(sessionId);

                    await sessionsPage.LoadAllSessions();
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region PullSessionsFromBackend
        /// <summary>
        /// Pulls sessions, connected MediaItems and connected residents from Backend via HttpClient.
        /// The serial number is required and sent as JSON.
        /// If an exception was thrown, the progress elements will be closed, an error message will be displayed and the ImportSessionsFromBackendButton will be disabled.
        /// </summary>
        /// <returns>boolean that is used in the SessionView to decide whether the ImportMediaFromBackendButton should be diabled/enabled</returns>
        public async Task<bool> PullSessionsFromBackend()
        {
            try
            {
                string serialNumber = JsonConvert.SerializeObject(
                new
                {
                    serial_number = deviceIdentifier
                }
                );

                HttpResponseMessage response = await httpRequestHelper.SendRequestToBackend(serialNumber, "http://127.0.0.1:8000/pullsessions");
                string body = await response.Content.ReadAsStringAsync();

                List<Session> backendSessions = JsonConvert.DeserializeObject<List<Session>>(body);

                return await SavePulledSessionsFromBackend(backendSessions);

            }
            catch (Exception)
            {
                sessionsPage.SetVisibilityOfProgressBarAndRing(false);
                sessionsPage.DisplayImportSessionsFromBackendConnectionErrorDialog();
                return false;
            }
        }
        #endregion

        #region ShouldDownloadSessionsFromBackend
        /// <summary>
        /// Checks if there are contents that can be pulled into app.
        /// The serial number is required and sent as JSON.
        /// The response is sent in JSON format and must be read as String.
        /// If the request fails, an exception will be thrown.
        /// </summary>
        /// <returns>boolean that is used in the SessionsPage to decide whether the ImportSessionsFromBackendButton should be diabled/enabled</returns>
        public async Task<bool> ShouldDownloadSessionsFromBackend()
        {
            try
            {
                return await backendContentHelper.ShouldDownloadPropertiesFromBackend(
                    deviceIdentifier,
                    "http://127.0.0.1:8000/getnewersessionsofbackend",
                    httpRequestHelper
                );
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region SavePulledSessionsFromBackend
        /// <summary>
        /// Saves pulled sessions.
        /// Foreach deserialized session a new session will be created or an existing session will be updated.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="backendSessions"></param>
        /// <returns></returns>
        private async Task<bool> SavePulledSessionsFromBackend(List<Session> backendSessions)
        {
            try
            {
                sessionsPage.SetVisibilityOfProgressBarAndRing(true);
                int sessionAmount = backendSessions.Count;
                int sessionsCreated = 0;

                foreach (Session backendSession in backendSessions)
                {
                    sessionsPage.SetProgressBarStatusTxt(sessionsCreated, sessionAmount);
                    sessionsCreated++;

                    Session existingSession = await SessionDBHandler.GetSessionByBackendSessionId(backendSession.Id);

                    if (existingSession == null)
                    {
                        await CreatePulledSessionFromBackend(backendSession);
                    }
                    else
                    {
                        await UpdateExistingSession(existingSession, backendSession);
                    }
                }

                sessionsPage.SetVisibilityOfProgressBarAndRing(false);

                await sessionsPage.LoadAllSessions();

                //sets latest date of app database in backend
                await SetLatestDeviceSessionUpdate();

                return await ShouldDownloadSessionsFromBackend();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region CreatePulledSessionFromBackend
        /// <summary>
        /// Creates pulled session from backend.
        /// Also creates or updates connected resident and mediaItems
        /// </summary>
        /// <param name="backendSession"></param>
        /// <returns></returns>
        private async Task CreatePulledSessionFromBackend(Session backendSession)
        {
            try
            {
                string sessionName = await GetUniqueSessionName(backendSession.Name);
                int sessionId = await SessionDBHandler.AddSession(sessionName, backendSession.Id);
                await sessionBackendIdHelper.SetAppSessionID(sessionId, backendSession.Id);
                await SavePulledMediaItemsFromBackend(backendSession.MediaList, sessionId);

            } catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region UpdateExistingSession
        /// <summary>
        /// Updates existing session with properties from Backend-Session.
        /// Throws an exception if the update fails.
        /// </summary>
        /// <param name="existingSession"></param>
        /// <param name="backendSession"></param>
        /// <returns></returns>
        private async Task UpdateExistingSession(Session existingSession, Session backendSession)
        {
            try
            {
                string sessionName = await GetUniqueSessionName(backendSession.Name);
                await SessionDBHandler.UpdateSession(existingSession.Id, backendSession.Id, sessionName);
                await SavePulledMediaItemsFromBackend(backendSession.MediaList, existingSession.Id);

            } catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region SavePulledMediaItemsFromBackend
        /// <summary>
        /// Saves pulled MediaItems from session.
        /// Throws an error if an exception occurs.
        /// </summary>
        /// <param name="backendMediaItems"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        private async Task SavePulledMediaItemsFromBackend(List<MediaItem> backendMediaItems, int sessionId)
        {
            try
            {

                await SessionMediaItemsDBHandler.UnbindSessionMediaItemsBySessionId(sessionId);

                foreach (MediaItem backendMediaItem in backendMediaItems)
                {
                    MediaItem existingMediaItem = await MediaItemDBHandler.GetMediaItemByBackendMediaItemId(backendMediaItem.Id);

                    int mediaItemId = 0;
                    
                    if (existingMediaItem == null)
                    {
                        await backendContentHelper.DownloadFileFromBackend(null, backendMediaItem);
                        MediaItem foundMediaItem = await MediaItemDBHandler.GetMediaItemByBackendMediaItemId(backendMediaItem.Id);
                        mediaItemId = foundMediaItem.Id;

                    } else
                    {
                        await backendContentHelper.DownloadFileFromBackend(existingMediaItem, backendMediaItem);
                        mediaItemId = existingMediaItem.Id;
                    }

                    await SessionMediaItemsDBHandler.BindSessionMediaItems(mediaItemId, sessionId);
                }
            } catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region SetLatestDeviceSessionUpdate
        /// <summary>
        /// Sets latest device session update date in backend.
        /// If the request fails, then an exception will be thrown.
        /// </summary>
        /// <returns>returns boolean that is set by whether the request was successfull or not</returns>
        private async Task<bool> SetLatestDeviceSessionUpdate()
        {
            try
            {
                return await backendContentHelper.SetLatestDevicePropertyUpdate(
                    deviceIdentifier,
                    "http://127.0.0.1:8000/setlatestdevicesessionupdate",
                    httpRequestHelper
                );
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region GetUniqueSessionName
        /// <summary>
        /// Returns an unique session name.
        /// </summary>
        /// <param name="whishedSessionName"></param>
        /// <returns></returns>
        private async Task<string> GetUniqueSessionName(string whishedSessionName)
        {
            int increment = 1;
            string uniqueSessionName = whishedSessionName;

            while(await SessionDBHandler.GetSessionByName(uniqueSessionName) != null)
            {
                uniqueSessionName = whishedSessionName;
                uniqueSessionName += increment;
                increment++;
            }

            return uniqueSessionName;
        }
        #endregion
    }
}