using Autorentool_RMT.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler
{
    public class SessionMediaItemsDBHandler
    {
        #region BindSessionMediaItems
        /// <summary>
        /// Creates and inserts a new SessionMediaItems-model to database asynchronously by given parameters.
        /// Returns the ID of the newly created SessionMediaItems-model or throws an exception when no SessionMediaItems entry was found.
        /// </summary>
        /// <param name="mediaItemId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task<int> BindSessionMediaItems(int mediaItemId, int sessionId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            SessionMediaItems mediaItemLifethemes = new SessionMediaItems()
            {
                MediaItemId = mediaItemId,
                SessionId = sessionId
            };

            try
            {
                await sQLiteAsyncConnection.InsertAsync(mediaItemLifethemes);
                return await GetID(mediaItemId, sessionId);
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region GetID
        /// <summary>
        /// Returns the ID by given parameters or throws an exception if no entry was found.
        /// </summary>
        /// <param name="mediaItemId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task<int> GetID(int mediaItemId, int sessionId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            try
            {
                SessionMediaItems queriedSessionMediaItems = await sQLiteAsyncConnection.Table<SessionMediaItems>()
                .Where(sessionMediaItems => sessionMediaItems.MediaItemId == mediaItemId && sessionMediaItems.SessionId == sessionId)
                .FirstOrDefaultAsync();

                return queriedSessionMediaItems.Id;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region UnbindCertainSessionMediaItems
        /// <summary>
        /// Unbinds Session and MediaItems by deleting entry in SessionMediaItems table.
        /// Therefore the given sessionMediaItemsId-parameter will be used<.
        /// </summary>
        /// <param name="sessionMediaItemsId"></param>
        /// <returns></returns>
        public static async Task UnbindCertainSessionMediaItems(int sessionMediaItemsId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAsync<SessionMediaItems>(sessionMediaItemsId);
        }
        #endregion

        #region UnbindSessionMediaItemsBySessionId
        /// <summary>
        /// Unbinds Session and MediaItem where the SessionId-property equals the given sessionId.
        /// First all SessionMediaItems will be queried where the SessionId equals the parameter.
        /// Then each entry will be deleted.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task UnbindSessionMediaItemsBySessionId(int sessionId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<SessionMediaItems> queriedSessionMediaItems = await sQLiteAsyncConnection.Table<SessionMediaItems>()
                .Where(sessionMediaItems => sessionMediaItems.SessionId == sessionId)
                .ToListAsync();

            await DeleteGivenSessionMediaItems(queriedSessionMediaItems, sQLiteAsyncConnection);
        }
        #endregion

        #region UnbindSessionMediaItemsByMediaItemId
        /// <summary>
        /// Unbinds Session and MediaItem where the MediaItemId-property equals the given mediaItemId.
        /// First all SessionMediaItems will be queried where the MediaItemId equals the parameter.
        /// Then each entry will be deleted.
        /// </summary>
        /// <param name="mediaItemId"></param>
        /// <returns></returns>
        public static async Task UnbindSessionMediaItemsByMediaItemId(int mediaItemId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<SessionMediaItems> queriedSessionMediaItems = await sQLiteAsyncConnection.Table<SessionMediaItems>()
                .Where(sessionMediaItems => sessionMediaItems.MediaItemId == mediaItemId)
                .ToListAsync();

            await DeleteGivenSessionMediaItems(queriedSessionMediaItems, sQLiteAsyncConnection);
        }
        #endregion

        #region DeleteGivenSessionMediaItems
        /// <summary>
        /// Deletes all entries in given SessionMediaItems list over the given SQLiteAsync Connection.
        /// </summary>
        /// <param name="sessionMediaItems"></param>
        /// <param name="sQLiteAsyncConnection"></param>
        /// <returns></returns>
        private static async Task DeleteGivenSessionMediaItems(List<SessionMediaItems> sessionMediaItems, SQLiteAsyncConnection sQLiteAsyncConnection)
        {
            foreach (SessionMediaItems sessionMediaItem in sessionMediaItems) 
            {
                await sQLiteAsyncConnection.DeleteAsync<SessionMediaItems>(sessionMediaItem.Id);
            }
        }
        #endregion

        #region GetSessionsOfMediaItem
        /// <summary>
        /// Returns all Sessions of a MediaItem by given MediaItem-ID in a List.
        /// First all SessionMediaItems-entries will be queried where the MediaItemId equals the given ID.
        /// After that foreach entry the Session for the queried SessionId will be queried and added into the Session-List.
        /// In the end the List will be returned.
        /// If the query fails or there aren't any entries in the SessionMediaItems- or the Session-Table with the given IDs,
        /// an empty List will be returned.
        /// </summary>
        /// <param name="mediaItemId"></param>
        /// <returns></returns>
        public static async Task<List<Session>> GetSessionsOfMediaItem(int mediaItemId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<SessionMediaItems> queriedSessionMediaItems = await sQLiteAsyncConnection.Table<SessionMediaItems>()
                .Where(sessionMediaItems => sessionMediaItems.MediaItemId == mediaItemId)
                .ToListAsync();

            List<Session> sessionsOfMediaItem = new List<Session>();

            foreach (SessionMediaItems queriedSessionMediaItem in queriedSessionMediaItems)
            {
                Session session = await SessionDBHandler.GetSingleSession(queriedSessionMediaItem.SessionId);
                sessionsOfMediaItem.Add(session);
            }

            return sessionsOfMediaItem;
        }
        #endregion

        #region GetMediaItemsOfSession
        /// <summary>
        /// Returns all MediaItems of a Session by given Session-ID in a List.
        /// First all SessionMediaItems-entries will be queried where the SessionId equals the given ID.
        /// After that foreach entry the MediaItem for the queried MediaItemId will be queried and added into the MediaItem-List.
        /// In the end the List will be returned.
        /// If the query fails or there aren't any entries in the SessionMediaItems- or the MediaItem-Table with the given IDs,
        /// an empty List will be returned.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task<List<MediaItem>> GetMediaItemsOfSession(int sessionId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<SessionMediaItems> queriedMediaItemLifethemes = await sQLiteAsyncConnection.Table<SessionMediaItems>()
                .Where(sessionMediaItems => sessionMediaItems.SessionId == sessionId)
                .ToListAsync();

            List<MediaItem> mediaItemsOfSession = new List<MediaItem>();

            foreach (SessionMediaItems queriedSessionMediaItem in queriedMediaItemLifethemes)
            {
                MediaItem mediaItem = await MediaItemDBHandler.GetSingleMediaItem(queriedSessionMediaItem.MediaItemId);
                mediaItemsOfSession.Add(mediaItem);
            }

            mediaItemsOfSession = mediaItemsOfSession.OrderBy(mediaItem => mediaItem.Position).ToList();

            mediaItemsOfSession.ForEach(mediaItem => mediaItem.SetThumbnailSource());

            return mediaItemsOfSession;
        }
        #endregion

        #region GetSessionMediaItemBySessionIdAndMediaItemId
        /// <summary>
        /// Returns an SessionMediaItems entry where the MediaItemId- and SessionId-properties equals the given id-parameters.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="mediaItemId"></param>
        /// <returns></returns>
        public static async Task<SessionMediaItems> GetSessionMediaItemBySessionIdAndMediaItemId(int sessionId, int mediaItemId) 
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<SessionMediaItems>()
                .Where(sessionMediaItems => sessionMediaItems.MediaItemId == mediaItemId && sessionMediaItems.SessionId == sessionId)
                .FirstAsync();
        }
        #endregion

        #region GetCertainSessionMediaItem
        /// <summary>
        /// Returns an SessionMediaItems entry by given sessionMediaItemId.
        /// </summary>
        /// <param name="sessionMediaItemId"></param>
        /// <returns></returns>
        public static async Task<SessionMediaItems> GetCertainSessionMediaItem(int sessionMediaItemId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.GetAsync<SessionMediaItems>(sessionMediaItemId);
        }
        #endregion

        #region DeleteAllEntries
        /// <summary>
        /// Deletes all entries from SessionMediaItems-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteAllEntries()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAllAsync<SessionMediaItems>();
        }
        #endregion

        #region DropTable
        /// <summary>
        /// Drops the SessionMediaItems-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DropTable()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DropTableAsync<SessionMediaItems>();
        }
        #endregion
    }
}
