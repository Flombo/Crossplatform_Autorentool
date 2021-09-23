using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.BackendCommunication
{
    class BackendPullSessions
    {
        /**
        private HttpRequestHelper _httpRequestHelper;
        private MessageWebSocket _webSocket;
        private SQLiteController _sQLiteController;
        private string _deviceIdentifier;
        private MediaItemBackendIdHelper _mediaItemBackendIdHelper;
        private SessionBackendIdHelper _sessionBackendIdHelper;
        private ResidentBackendIdHelper _residentBackendIdHelper;
        private SessionsView _sessionsView;
        private BackendContentHelper _backendContentHelper;

        /// <summary>
        /// constructor of BackendPullSessions-Class
        /// </summary>
        /// <param name="sessionsView">Class needs SessionsView instance for updating the UI with pulled sessions</param>
        public BackendPullSessions(SessionsView sessionsView)
        {
            _sessionsView = sessionsView;
            _sQLiteController = SQLiteController.GetInstance();
            _deviceIdentifier = GetDeviceIdentifier();
            _mediaItemBackendIdHelper = new MediaItemBackendIdHelper();
            _sessionBackendIdHelper = new SessionBackendIdHelper();
            _residentBackendIdHelper = new ResidentBackendIdHelper();
            _httpRequestHelper = new HttpRequestHelper();
        }

        ///<summary>
        ///gets unique applicationID
        ///</summary>
        private Guid GetApplicationId()
        {
            EasClientDeviceInformation deviceInformation = new EasClientDeviceInformation();
            Guid appId = deviceInformation.Id;

            return appId;
        }

        ///<summary>
        ///gets unique deviceID, if the device is able to generate the deviceID.
        ///</summary>
        private Guid GetDeviceId()
        {
            Guid deviceId = new Guid();
            SystemIdentificationInfo systemId = SystemIdentification.GetSystemIdForPublisher();

            if (systemId.Source != SystemIdentificationSource.None)
            {
                DataReader dataReader = DataReader.FromBuffer(systemId.Id);
                deviceId = dataReader.ReadGuid();
            }
            return deviceId;
        }

        ///<summary>
        ///builds deviceIdentifier => "serial number"
        ///</summary>
        private string GetDeviceIdentifier()
        {
            Guid appId = GetApplicationId();
            Guid deviceId = GetDeviceId();
            return Math.Abs(appId.GetHashCode() + deviceId.GetHashCode()).ToString(new CultureInfo("de-DE"));
        }

        /// <summary>
        /// Inits helper classes HTTPRequestHelper, MediaItemBackendHelper, SessionBackendIdHelper and ResidentBackendIdHelper.
        /// If the process fails, the ImportSessionsFromBackendButton will be disabled
        /// </summary>
        /// <returns></returns>
        public async Task InitHelper()
        {
            try
            {
                await _httpRequestHelper.GetCSRFToken(_deviceIdentifier).ConfigureAwait(true);
                await _mediaItemBackendIdHelper.Init(_deviceIdentifier).ConfigureAwait(true);
                await _sessionBackendIdHelper.Init(_deviceIdentifier).ConfigureAwait(true);
                await _residentBackendIdHelper.Init(_deviceIdentifier).ConfigureAwait(true);
                _backendContentHelper = new BackendContentHelper(_mediaItemBackendIdHelper, _deviceIdentifier);
            }
            catch (Exception exc)
            {
                _sessionsView.EnableOrDisableImportSessionsFromBackendButton(false);
            }
        }

        ///<summary>
        ///Creates a websocket connection to backend websocketserver.
        ///if connection is set, the websocket send first message with serialnumber and pulls new sessions
        ///</summary>
        public async Task InitWebSocket()
        {
            _webSocket = new MessageWebSocket();
            _webSocket.Control.MessageType = SocketMessageType.Utf8;

            try
            {
                //Before a connection to the backend websocket-server can be established,
                //the eventhandlers for message-recieving and websocket-closing must be set.
                _webSocket.MessageReceived += RecieveMessage;
                _webSocket.Closed += WebSocket_Closed;
                Task connectTask = _webSocket.ConnectAsync(new Uri("ws://141.28.44.195:8080")).AsTask();
                TaskScheduler taskScheduler = TaskScheduler.Default;

                String serialNumber = JsonConvert.SerializeObject(
                    new
                    {
                        command = "",
                        serialNumber = _deviceIdentifier
                    }
                );

                Task task = await connectTask.ContinueWith(_ => SendMessageUsingMessageWebSocket(serialNumber), taskScheduler).ConfigureAwait(false);
            }
            catch (Exception exc)
            {
                Windows.Web.WebErrorStatus webErrorStatus = WebSocketError.GetStatus(exc.GetBaseException().HResult);
            }
        }

        ///<summary>
        ///sends message over websocket.
        ///Therefore a new Datawriter must be created to write to outputstream
        ///</summary>
        ///<param string="message">Message that contains an empty command and the device serial-number</param>
        private async Task SendMessageUsingMessageWebSocket(string message)
        {
            using (var dataWriter = new DataWriter(_webSocket.OutputStream))
            {
                dataWriter.WriteString(message);
                await dataWriter.StoreAsync();
                dataWriter.DetachStream();
            }
        }

        /// <summary>
        /// Will be called, when MessageReceived event of websocket is triggered.
        /// Uses Datareader to read message string from MessageWebSocketMessageRecievedEventArgs
        /// If the message equals own serial number and shouldPullSessions is true, the device needs to pull sessions from backend.
        /// <summary>
        /// <param name="sender">Instance of the MessageWebSocket Class</param>
        /// <param name="args">Holder of the recieved message</param>
        private void RecieveMessage(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
            {
                try
                {
                    using (DataReader dataReader = args.GetDataReader())
                    {
                        dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                        string message = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                        if (message.Length > 0)
                        {
                            WebSocketMessage webSocketMessage = JsonConvert.DeserializeObject<WebSocketMessage>(message);

                            //check if this device should pull contents from backend 
                            if (webSocketMessage.SerialNumber == _deviceIdentifier && webSocketMessage.ShouldPullSessions)
                            {
                                _sessionsView.DisplayImportSessionsFromBackendByPushDialog();
                            }

                            //check if this device should delete mediaItems
                            else if (webSocketMessage.AppSessionIDs.Count > 0)
                            {
                                _sessionsView.DisplayDeleteSessionsViaDeleteCommand(webSocketMessage.AppSessionIDs);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //if an exception was thrown, the websockt will be closed.
                    Windows.Web.WebErrorStatus webErrorStatus = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                    CloseWebsocket(1000, "normal closure");
                }
            }


        /// <summary>
        /// closes websocket.
        /// </summary>
        /// <param name="code">Status code for websocket closing</param>
        /// <param name="reason">reasons for closing the websocket</param>
        public void CloseWebsocket(ushort code, string reason)
        {
            if (_webSocket != null)
            {
                _webSocket.Close(code, reason);
            }
        }

        /// <summary>
        /// if dispose method of websocket is called, the websocket Closed-Event will be triggered.
        /// this method acts as Handler and writes the reason for closing into Debug log.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void WebSocket_Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            Debug.WriteLine("WebSocket_Closed; Code: " + args.Code + ", Reason: \"" + args.Reason + "\"");
        }

        /// <summary>
        /// deletes requested sessions from WebSocket
        /// </summary>
        /// <param name="mediaItems"></param>
        /// <returns></returns>
        public async Task DeleteSessionsRetrievedByWebSocket(List<Session> sessions)
        {
            int sessionsDeleted = 0;
            await _sessionsView.SetVisibiltyOfProgressBarAndRing(true).ConfigureAwait(true);
            
            foreach (Session session in sessions)
            {
                await DeleteSession(session).ConfigureAwait(true);
                sessionsDeleted++;
                await _sessionsView.SetProgressBarStatusTxt(sessions.Count, sessionsDeleted).ConfigureAwait(true);
            }

            await _sessionsView.ResetSessionsAndReloadSessions().ConfigureAwait(true);
            await _sessionsView.SetVisibiltyOfProgressBarAndRing(false).ConfigureAwait(true);
        }

        /// <summary>
        /// deletes mediaItem by id.
        /// deletes relationship between lifetheme and mediaItem, if there is one.
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
                    _sQLiteController.DeleteOldSessionItems(sessionId);

                    int residentId = _sQLiteController.GetResidentIdBySessionId(sessionId);

                    if (residentId != -1)
                    {
                        _sQLiteController.UnbindResidentSession(sessionId, residentId);
                    }

                    _sQLiteController.DeleteSession(sessionId);

                    await _sessionsView.ResetSessionsAndReloadSessions().ConfigureAwait(true);
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        /// <summary>
        /// Pulls sessions, connected MediaItems and connected residents from Backend via HttpClient.
        /// The serial number is required and sent as JSON
        /// </summary>
        /// <returns>boolean that is used in the SessionView to decide whether the ImportMediaFromBackendButton should be diabled/enabled</returns>
        public async Task<bool> PullSessionsFromBackend()
        {
            try
            {
                String serialNumber = JsonConvert.SerializeObject(
                new
                {
                    serial_number = _deviceIdentifier
                }
                );

                HttpResponseMessage response = await _httpRequestHelper.SendRequestToBackend(serialNumber, "http://141.28.44.195/pullsessions").ConfigureAwait(true);
                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                List<Session> backendSessions = JsonConvert.DeserializeObject<List<Session>>(body);
                return await SavePulledSessionsFromBackend(backendSessions).ConfigureAwait(true);

            }
            catch (Exception exc)
            {
                //if request or SavePulledSessionsFromBackend method fails an error dialog will be displayed and the ImportSessionsFromBackendButton will be disabled.
                await _sessionsView.SetVisibiltyOfProgressBarAndRing(false).ConfigureAwait(true);
                _sessionsView.DisplayImportSessionsFromBackendConnectionErrorDialog();
                return false;
            }
        }

        /// <summary>
        /// Checks if there are contents that can be pulled into app.
        /// The serial number is required and sent as JSON.
        /// The response is sent in JSON format and must be read as String.
        /// If the request fails, false will be returned.
        /// </summary>
        /// <returns>boolean that is used in the ContentView to decide whether the ImportMediaFromBackendButton should be diabled/enabled</returns>
        public async Task<bool> ShouldDownloadSessionsFromBackend()
        {
            try
            {
                return await _backendContentHelper.ShouldDownloadPropertiesFromBackend(
                    _deviceIdentifier,
                    "http://141.28.44.195/getnewersessionsofbackend",
                    _httpRequestHelper
                ).ConfigureAwait(true);
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        /// <summary>
        /// Saves pulled sessions.
        /// foreach deserialized session a new session will be created or an existing session will be updated.
        /// </summary>
        /// <param name="backendSessions"></param>
        /// <returns></returns>
        private async Task<bool> SavePulledSessionsFromBackend(List<Session> backendSessions)
        {
            try 
            {
                await _sessionsView.SetVisibiltyOfProgressBarAndRing(true).ConfigureAwait(true);
                int sessionAmount = backendSessions.Count;
                int sessionsCreated = 0;

                foreach (Session backendSession in backendSessions)
                {
                    Session existingSession = _sQLiteController.GetSessionByBackendSessionId(backendSession.Id);

                    if(existingSession == null)
                    {
                        await CreatePulledSessionFromBackend(backendSession).ConfigureAwait(true);
                    }
                    else
                    {
                        await UpdateExistingSession(existingSession, backendSession).ConfigureAwait(true);
                    }
                    await _sessionsView.SetProgressBarStatusTxt(sessionsCreated, sessionAmount).ConfigureAwait(true);
                    sessionsCreated++;
                }

                await _sessionsView.SetVisibiltyOfProgressBarAndRing(false).ConfigureAwait(true);

                await _sessionsView.ResetSessionsAndReloadSessions().ConfigureAwait(true);

                //sets latest date of app database in backend
                bool latestDeviceSessionUpdateIsSet = await SetLatestDeviceSessionUpdate().ConfigureAwait(true);

                int setLastestDeviceVContentUpdateCounter = 0;
                //while setting is faulty it will be retried at max 3 times.
                while (!latestDeviceSessionUpdateIsSet && setLastestDeviceVContentUpdateCounter < 3)
                {
                    latestDeviceSessionUpdateIsSet = await SetLatestDeviceSessionUpdate().ConfigureAwait(true);
                    setLastestDeviceVContentUpdateCounter++;
                }

                return await ShouldDownloadSessionsFromBackend().ConfigureAwait(true);
            } 
            catch (Exception exc)
            {
                throw;
            }
        }

        /// <summary>
        /// creates pulled session from backend.
        /// also creates or updates connected resident and mediaItems
        /// </summary>
        /// <param name="backendSession"></param>
        /// <returns></returns>
        private async Task CreatePulledSessionFromBackend(Session backendSession) 
        {
            try
            {
                _sQLiteController.CreateSession(backendSession);
                int createdSessionId = _sQLiteController.GetSessionIdByName(backendSession.Name);
                _sQLiteController.UpdateSession(createdSessionId, backendSession.Name, backendSession.Id);
                await _sessionBackendIdHelper.SetAppSessionID(createdSessionId, backendSession.Id).ConfigureAwait(true);
                await SavePulledMediaItemsFromBackend(backendSession.MediaList, createdSessionId).ConfigureAwait(true);
                await SavePulledResidentFromBackend(backendSession.Resident, createdSessionId, backendSession).ConfigureAwait(true);
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        /// <summary>
        /// updates existing session with properties from backendSession
        /// </summary>
        /// <param name="existingSession"></param>
        /// <param name="backendSession"></param>
        /// <returns></returns>
        private async Task UpdateExistingSession(Session existingSession, Session backendSession)
        {
            try 
            {
                _sQLiteController.UpdateSession(existingSession.Id, backendSession.Name, backendSession.Id);
                await SavePulledMediaItemsFromBackend(backendSession.MediaList, existingSession.Id).ConfigureAwait(true);
                await SavePulledResidentFromBackend(backendSession.Resident, existingSession.Id, backendSession).ConfigureAwait(true);
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        /// <summary>
        /// saves pulled residents from backend.
        /// if a resident already exists, the resident will be updated.
        /// else a new resident will be created.
        /// in both cases the appResidentId will be set in backend and the resident will be bind to given session.
        /// </summary>
        /// <param name="backendResident"></param>
        /// <param name="createdSessionId"></param>
        /// <returns></returns>
        private async Task SavePulledResidentFromBackend(Resident backendResident, int createdSessionId, Session backendSession)
        {
            try
            {
                Resident existingResident = _sQLiteController.GetResident(backendResident.Id);

                if (backendResident.Id != 0)
                {
                    if (existingResident != null)
                    {
                        _sQLiteController.UpdateResident(backendResident);
                        await _residentBackendIdHelper.SetAppResidentID(existingResident.Id, backendResident.Id).ConfigureAwait(true);
                        _sQLiteController.BindResidentSession(createdSessionId, existingResident.Id);
                        _sQLiteController.CreateRating(existingResident.Id, createdSessionId, backendSession.Rating, backendSession.DurationInSeconds);
                        await SavePulledResidentLifethemesFromBackend(backendResident.Lifethemes, existingResident.Id).ConfigureAwait(true);
                    }
                    else
                    {
                        int createndResidentId = _sQLiteController.CreateResident(backendResident);
                        await _residentBackendIdHelper.SetAppResidentID(createndResidentId, backendResident.Id).ConfigureAwait(true);
                        _sQLiteController.CreateRating(createndResidentId, createdSessionId, backendSession.Rating, backendSession.DurationInSeconds);
                        _sQLiteController.BindResidentSession(createdSessionId, createndResidentId);
                        await SavePulledResidentLifethemesFromBackend(backendResident.Lifethemes, createndResidentId).ConfigureAwait(true);
                    }
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        /// <summary>
        /// creates pulled resident lifethemes from backend and binds them with given resident by id.
        /// </summary>
        /// <param name="lifethemes"></param>
        /// <param name="residentId"></param>
        /// <returns></returns>
        private async Task SavePulledResidentLifethemesFromBackend(List<Lifetheme> lifethemes, int residentId) 
        {
            try 
            {
                foreach(Lifetheme lifetheme in lifethemes) 
                {
                    _sQLiteController.CreateLifetheme(lifetheme.Name);
                    _sQLiteController.BindResidentLifetheme(residentId, lifetheme.Name);
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        /// <summary>
        /// Saves pulled MediaItems from session.
        /// foreach MediaItem a new MediaItem will be created.
        /// if MediaItem has Notes, the freshly created MediaItems needs to be updated.
        /// </summary>
        /// <param name="backendMediaItems"></param>
        /// <param name="createdSessionId"></param>
        /// <returns></returns>
        private async Task SavePulledMediaItemsFromBackend(List<MediaItem> backendMediaItems, int createdSessionId)
        {
            try
            {
                foreach (var backendMediaItem in backendMediaItems)
                {
                    MediaItem existingMediaItem = _sQLiteController.GetMediaItemByBackendMediaItemId(backendMediaItem.Id);
                    if (existingMediaItem == null)
                    {
                        StorageFile file = await _backendContentHelper.GetFileFromBackend(backendMediaItem.Path, backendMediaItem.Name).ConfigureAwait(true);

                        await CreatePulledMediaItemFromBackend(file, backendMediaItem, createdSessionId).ConfigureAwait(true);
                    }
                    else
                    {
                        await UpdateExistingMediaItem(existingMediaItem, backendMediaItem, createdSessionId).ConfigureAwait(true);
                    }
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates Existing MediaItem.
        /// File will be newly set and saved in LocalStorage.
        /// </summary>
        /// <param name="existingMediaItem">Instance of existing MediaItem of app</param>
        /// <param name="backendMediaItem">Instance of new backend MediaItem</param>
        private async Task UpdateExistingMediaItem(MediaItem existingMediaItem, MediaItem backendMediaItem, int createdSessionId)
        {
            try
            {
                StorageFile newFile = await _backendContentHelper.GetFileFromBackend(backendMediaItem.Path, backendMediaItem.Name).ConfigureAwait(true);

                if (newFile != null)
                {
                    await _backendContentHelper.UpdateExistingMediaItem(existingMediaItem, backendMediaItem).ConfigureAwait(true);
                    _sQLiteController.AppendSessionItem(existingMediaItem.Id, createdSessionId);
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        /// <summary>
        /// Creates single MediaItem from Backend
        /// </summary>
        /// <param name="mediaItemStorageFile">newly created StorageFile of backend MediaItem</param>
        /// <param name="backendMediaItem">newly created backend MediaItem</param>
        /// <returns></returns>
        private async Task CreatePulledMediaItemFromBackend(StorageFile mediaItemStorageFile, MediaItem backendMediaItem, int createdSessionId)
            {
                try
                {
                    await _backendContentHelper.CreatePulledMediaItemFromBackend(mediaItemStorageFile, backendMediaItem).ConfigureAwait(true);
                    MediaItem createdMediaItem = _sQLiteController.GetMediaItemByBackendMediaItemId(backendMediaItem.Id);
                    _sQLiteController.AppendSessionItem(createdMediaItem.Id, createdSessionId);
            }
                catch (Exception exc)
                {
                    throw;
                }
            }

        /// <summary>
        /// Sets latest device session update date in backend.
        /// </summary>
        /// <returns>returns boolean that is set by whether the request was successfull or not</returns>
        private async Task<bool> SetLatestDeviceSessionUpdate()
        {
            try
            {
                return await _backendContentHelper.SetLatestDevicePropertyUpdate(
                    _deviceIdentifier,
                    "http://141.28.44.195/setlatestdevicesessionupdate",
                    _httpRequestHelper
                ).ConfigureAwait(true);
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        **/
    }
}
