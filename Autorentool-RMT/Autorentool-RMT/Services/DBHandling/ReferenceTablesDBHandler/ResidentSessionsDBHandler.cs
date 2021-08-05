using Autorentool_RMT.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler
{
    public class ResidentSessionsDBHandler
    {
        #region BindResidentSessions
        /// <summary>
        /// Creates and inserts a new ResidentSessions-model to database asynchronously by given parameters.
        /// Returns the ID of the newly created ResidentSessions-model if there aren't any entries with the same ResidentId and SessionId combination.
        /// If no ID was found an exception will be thrown
        /// If this combination isn't uníque, an exception will be thrown.
        /// </summary>
        /// <param name="residentId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task<int> BindResidentLifethemes(int residentId, int sessionId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            ResidentSessions residentSessions = new ResidentSessions()
            {
                ResidentId = residentId,
                SessionId = sessionId
            };

            try
            {
                ResidentSessions residentSession1 = await sQLiteAsyncConnection.Table<ResidentSessions>()
                    .FirstOrDefaultAsync(
                        queriedResidentSession =>
                            queriedResidentSession.ResidentId == residentId
                            && queriedResidentSession.SessionId == sessionId
                            );

                await sQLiteAsyncConnection.InsertAsync(residentSessions);
                return await GetID(residentId, sessionId);
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
        /// <param name="sessionId"></param>
        /// <param name="residentId"></param>
        /// <returns></returns>
        public static async Task<int> GetID(int sessionId, int residentId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            try
            {
                ResidentSessions queriedResidentSessions = await sQLiteAsyncConnection.Table<ResidentSessions>()
                    .FirstOrDefaultAsync(
                        residentSessions => residentSessions.SessionId == sessionId
                        && residentSessions.ResidentId == residentId
                    );

                return queriedResidentSessions.Id;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region UnbindCertainResidentSessions
        /// <summary>
        /// Unbinds Resident and Session by deleting entry in ResidentSessions table by given residentSessionsId.
        /// </summary>
        /// <param name="residentSessionsId"></param>
        /// <returns></returns>
        public static async Task UnbindCertainResidentLifethemes(int residentSessionsId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAsync<ResidentSessions>(residentSessionsId);
        }
        #endregion

        #region UnbindAllResidentSessionsByResidentId
        /// <summary>
        /// Unbinds all ResidentSessions where the ResidentId equals the given residentId.
        /// </summary>
        /// <param name="residentId"></param>
        /// <returns></returns>
        public static async Task UnbindAllResidentSessionsByResidentId(int residentId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<ResidentSessions> queriedResidentSessions = await sQLiteAsyncConnection.Table<ResidentSessions>()
                .Where(residentSessions => residentSessions.ResidentId == residentId)
                .ToListAsync();

            await DeleteGivenResidentSessions(queriedResidentSessions, sQLiteAsyncConnection);
        }
        #endregion

        #region UnbindAllResidentSessionsByLifethemeId
        /// <summary>
        /// Unbinds all ResidentSessions where the SessionId equals the given sessionId.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task UnbindAllResidentSessionsByLifethemeId(int sessionId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<ResidentSessions> queriedResidentSessions = await sQLiteAsyncConnection.Table<ResidentSessions>()
                .Where(residentSessions => residentSessions.SessionId == sessionId)
                .ToListAsync();

            await DeleteGivenResidentSessions(queriedResidentSessions, sQLiteAsyncConnection);
        }
        #endregion

        #region DeleteGivenResidentSessions
        /// <summary>
        /// Deletes all entries in given ResidentSessions list over given SQLiteAsyncConnection.
        /// </summary>
        /// <param name="residentSessions"></param>
        /// <param name="sQLiteAsyncConnection"></param>
        /// <returns></returns>
        private static async Task DeleteGivenResidentSessions(List<ResidentSessions> residentSessions, SQLiteAsyncConnection sQLiteAsyncConnection)
        {
            foreach (ResidentSessions residentSession in residentSessions)
            {
                await sQLiteAsyncConnection.DeleteAsync<ResidentSessions>(residentSession.Id);
            }
        }
        #endregion

        #region GetCertainResidentSessionsEntry
        /// <summary>
        /// Returns certain ResidentSessions-entry for given residentSessionId.
        /// </summary>
        /// <param name="residentSessionId"></param>
        /// <returns></returns>
        public static async Task<ResidentSessions> GetCertainResidentSessionsEntry(int residentSessionId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.GetAsync<ResidentSessions>(residentSessionId);
        }
        #endregion

        #region GetResidentSessionsBySessionId
        /// <summary>
        /// Returns all ResidentSessions where the SessionId-property equals the given sessionId as a List.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task<List<ResidentSessions>> GetResidentSessionsBySessionId(int sessionId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<ResidentSessions>()
                .Where(residentSessions => residentSessions.SessionId == sessionId)
                .ToListAsync();
        }
        #endregion

        #region GetResidentSessionsByResidentId
        /// <summary>
        /// Returns all ResidentSessions where the ResidentId-property equals the given residentId as a List.
        /// </summary>
        /// <param name="residentId"></param>
        /// <returns></returns>
        public static async Task<List<ResidentSessions>> GetResidentSessionsByResidentId(int residentId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<ResidentSessions>()
                .Where(residentSessions => residentSessions.ResidentId == residentId)
                .ToListAsync();
        }
        #endregion

        #region GetSessionsOfResident
        /// <summary>
        /// Returns all Sessions of a Resident by given Resident-ID in a List.
        /// First all ResidentSessions-entries will be queried where the ResidentId equals the given ID.
        /// After that foreach entry the Session for the queried SessionId will be queried and added into the Session-List.
        /// In the end the List will be returned.
        /// If the query fails or there aren't any entries in the ResidentSessions- or the Sessions-Table with the given IDs,
        /// an empty List will be returned.
        /// </summary>
        /// <param name="residentId"></param>
        /// <returns></returns>
        public static async Task<List<Session>> GetSessionsOfResident(int residentId)
        {
            List<ResidentSessions> queriedResidentSessions = await GetResidentSessionsByResidentId(residentId);

            List<Session> sessionsOfResident = new List<Session>();

            foreach (ResidentSessions queriedResidentSession in queriedResidentSessions)
            {
                Session session = await SessionDBHandler.GetSingleSession(queriedResidentSession.SessionId);
                sessionsOfResident.Add(session);
            }

            return sessionsOfResident;
        }
        #endregion

        #region GetResidentsOfSession
        /// <summary>
        /// Returns all Residents of a Session by given Session-ID in a List.
        /// First all ResidentSessions-entries will be queried where the SessionId equals the given ID.
        /// After that foreach entry the Resident for the queried ResidentId will be queried and added into the Resident-List.
        /// In the end the List will be returned.
        /// If the query fails or there aren't any entries in the ResidentSessions- or the Residents-Table with the given IDs,
        /// an empty List will be returned.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task<List<Resident>> GetResidentsOfLifethemes(int sessionId)
        {
            List<ResidentSessions> queriedResidentSessions = await GetResidentSessionsBySessionId(sessionId);

            List<Resident> residentsOfSession = new List<Resident>();

            foreach (ResidentSessions queriedResidentSession in queriedResidentSessions)
            {
                Resident resident = await ResidentDBHandler.GetSingleResident(queriedResidentSession.ResidentId);
                residentsOfSession.Add(resident);
            }

            return residentsOfSession;
        }
        #endregion

        #region GetAllResidentSessions
        /// <summary>
        /// Returns all ResidentSessions as a List.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ResidentSessions>> GetAllResidentLifethemes()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<ResidentSessions>().ToListAsync();
        }
        #endregion

        #region DeleteAllEntries
        /// <summary>
        /// Deletes all entries from ResidentSessions-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteAllEntries()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAllAsync<ResidentSessions>();
        }
        #endregion

        #region DropTable
        /// <summary>
        /// Drops the ResidentSessions-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DropTable()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DropTableAsync<ResidentSessions>();
        }
        #endregion
    }
}
