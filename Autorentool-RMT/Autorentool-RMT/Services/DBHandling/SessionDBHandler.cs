using Autorentool_RMT.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.DBHandling
{
    public class SessionDBHandler
    {

        #region AddSession
        /// <summary>
        /// Creates and inserts a new Session to database asynchronously by given parameters.
        /// Returns the created ID of the Session or throws an exception if no Session was found.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="backendSessionID"></param>
        /// <returns></returns>
        public static async Task<int> AddSession(string name, int backendSessionID)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            Session session = new Session()
            {
                Name = name,
                BackendSessionId = backendSessionID
            };

            try
            {
                await sQLiteAsyncConnection.InsertAsync(session);
                return await GetID(name, backendSessionID);
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region GetID
        /// <summary>
        /// Returns the ID of a Session by given parameters or throws an exception if no Session was found.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="backendSessionId"></param>
        /// <returns></returns>
        public static async Task<int> GetID(string name, int backendSessionId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            try
            {
                Session queriedSession = await sQLiteAsyncConnection.Table<Session>()
                    .FirstOrDefaultAsync(session => session.Name == name && session.BackendSessionId == backendSessionId);

                return queriedSession.Id;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region DeleteSession
        /// <summary>
        /// Deletes Session by given ID asynchronously.
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public static async Task DeleteSession(int sessionID)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAsync<Session>(sessionID);
        }
        #endregion

        #region UpdateSession
        /// <summary>
        /// Updates Session by given ID and parameters.
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="backendSessionID"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task UpdateSession(int sessionID, int backendSessionID, string name)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            Session session = new Session()
            {
                Id = sessionID,
                BackendSessionId = backendSessionID,
                Name = name
            };

            await sQLiteAsyncConnection.UpdateAsync(session);
        }
        #endregion

        #region UpdateDurationInSeconds
        /// <summary>
        /// Updates duration_in_seconds-field by given duration.
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="durationInSeconds"></param>
        /// <returns></returns>
        public static async Task UpdateDurationInSeconds(int sessionID, int durationInSeconds)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.QueryAsync<Session>("UPDATE Sessions SET duration_in_seconds = ? WHERE id = ?", durationInSeconds, sessionID);
        }
        #endregion

        #region GetAllSessions
        /// <summary>
        /// Returns all Sessions as a List asynchronously.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Session>> GetAllSessions()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<Session>().ToListAsync();
        }
        #endregion

        #region GetSingleSession
        /// <summary>
        /// Returns a single Session by given ID async.
        /// If no Session was found, null will be returned.
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public static async Task<Session> GetSingleSession(int sessionID)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<Session>().FirstOrDefaultAsync();
        }
        #endregion

        #region GetSessionByName
        /// <summary>
        /// Returns a single Session or null by given session name.
        /// </summary>
        /// <param name="sessionName"></param>
        /// <returns></returns>
        public static async Task<Session> GetSessionByName(string sessionName)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<Session>().Where(Session => Session.Name.Equals(sessionName)).FirstOrDefaultAsync();
        }
        #endregion

        #region DeleteAllEntries
        /// <summary>
        /// Deletes all entries from Session-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteAllEntries()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAllAsync<Session>();
        }
        #endregion

        #region DropTable
        /// <summary>
        /// Drops the Sessions-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DropTable()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DropTableAsync<Session>();
        }
        #endregion

    }
}
