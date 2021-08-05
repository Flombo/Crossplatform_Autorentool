using Autorentool_RMT.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler
{
    public class ResidentLifethemesDBHandler
    {

        #region BindResidentLifethemes
        /// <summary>
        /// Creates and inserts a new ResidentLifethemes-model to database asynchronously by given parameters.
        /// Returns the ID of the newly created ResidentLifethemes-model.
        /// If the combination of the ResidentId and the LifethemeId isn't unique, an exception will be thrown.
        /// </summary>
        /// <param name="residentId"></param>
        /// <param name="lifethemeId"></param>
        /// <returns></returns>
        public static async Task<int> BindResidentLifethemes(int residentId, int lifethemeId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            try
            {
                ResidentLifethemes duplicate = await sQLiteAsyncConnection.Table<ResidentLifethemes>().FirstOrDefaultAsync(
                        ResidentLifethemes => ResidentLifethemes.LifethemeId == lifethemeId && ResidentLifethemes.ResidentId == residentId
                    );

                if(duplicate != null)
                {
                    throw new Exception();
                }

                await sQLiteAsyncConnection.InsertAsync(
                    new ResidentLifethemes()
                    {
                        ResidentId = residentId,
                        LifethemeId = lifethemeId
                    });

                return await GetIdByResidentIdAndLifethemeId(residentId, lifethemeId);
            } catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region GetIdByResidentIdAndLifethemeId
        public static async Task<int> GetIdByResidentIdAndLifethemeId(int residentId, int lifethemeId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            try
            {
                ResidentLifethemes queriedResidentLifethemes = await sQLiteAsyncConnection.Table<ResidentLifethemes>()
                    .FirstOrDefaultAsync(
                        residentLifethemes => residentLifethemes.ResidentId == residentId 
                        && residentLifethemes.LifethemeId == lifethemeId
                    );

                return queriedResidentLifethemes.Id;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region UnbindCertainResidentLifethemes
        /// <summary>
        /// Unbinds Resident and Lifetheme by deleting entry in ResidentLifethemes table by given residentLifethemesId.
        /// </summary>
        /// <param name="residentLifethemesId"></param>
        /// <returns></returns>
        public static async Task UnbindCertainResidentLifethemes(int residentLifethemesId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAsync<ResidentLifethemes>(residentLifethemesId);
        }
        #endregion

        #region UnbindAllResidentLifethemesByResidentId
        /// <summary>
        /// Unbinds all ResidentLifethemes where the ResidentId equals the given residentId.
        /// </summary>
        /// <param name="residentId"></param>
        /// <returns></returns>
        public static async Task UnbindAllResidentLifethemesByResidentId(int residentId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<ResidentLifethemes> queriedResidentLifethemes = await sQLiteAsyncConnection.Table<ResidentLifethemes>()
                .Where(residentLifethemes => residentLifethemes.ResidentId == residentId)
                .ToListAsync();

            await DeleteGivenResidentLifethemes(queriedResidentLifethemes, sQLiteAsyncConnection);
        }
        #endregion

        #region UnbindAllResidentLifethemesByLifethemeId
        /// <summary>
        /// Unbinds all ResidentLifethemes where the LifethemeId equals the given lifethemeId.
        /// </summary>
        /// <param name="lifethemeId"></param>
        /// <returns></returns>
        public static async Task UnbindAllResidentLifethemesByLifethemeId(int lifethemeId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<ResidentLifethemes> queriedResidentLifethemes = await sQLiteAsyncConnection.Table<ResidentLifethemes>()
                .Where(residentLifethemes => residentLifethemes.LifethemeId == lifethemeId)
                .ToListAsync();

            await DeleteGivenResidentLifethemes(queriedResidentLifethemes, sQLiteAsyncConnection);
        }
        #endregion

        #region DeleteGivenResidentLifethemes
        /// <summary>
        /// Deletes all entries in given ResidentLifethemes list over given SQLiteAsyncConnection.
        /// </summary>
        /// <param name="residentLifethemes"></param>
        /// <param name="sQLiteAsyncConnection"></param>
        /// <returns></returns>
        private static async Task DeleteGivenResidentLifethemes(List<ResidentLifethemes> residentLifethemes, SQLiteAsyncConnection sQLiteAsyncConnection)
        {
            foreach(ResidentLifethemes residentLifetheme in residentLifethemes)
            {
                await sQLiteAsyncConnection.DeleteAsync<ResidentLifethemes>(residentLifetheme.Id);
            }
        }
        #endregion

        #region GetCertainResidentLifethemesEntry
        /// <summary>
        /// Returns certain ResidentLifethemes-entry for given residentLifethemesId.
        /// </summary>
        /// <param name="residentLifethemesId"></param>
        /// <returns></returns>
        public static async Task<ResidentLifethemes> GetCertainResidentLifethemesEntry(int residentLifethemesId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.GetAsync<ResidentLifethemes>(residentLifethemesId);
        }
        #endregion

        #region GetResidentLifethemesByLifethemeId
        /// <summary>
        /// Returns all ResidentLifethemes where the LifethemeId-property equals the given lifethemeId as a List.
        /// </summary>
        /// <param name="lifethemeId"></param>
        /// <returns></returns>
        public static async Task<List<ResidentLifethemes>> GetResidentLifethemesByLifethemeId(int lifethemeId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<ResidentLifethemes>()
                .Where(residentLifethemes => residentLifethemes.LifethemeId == lifethemeId)
                .ToListAsync();
        }
        #endregion

        #region GetResidentLifethemesByResidentId
        /// <summary>
        /// Returns all ResidentLifethemes where the ResidentId-property equals the given residentId as a List.
        /// </summary>
        /// <param name="residentId"></param>
        /// <returns></returns>
        public static async Task<List<ResidentLifethemes>> GetResidentLifethemesByResidentId(int residentId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<ResidentLifethemes>()
                .Where(residentLifethemes => residentLifethemes.ResidentId == residentId)
                .ToListAsync();
        }
        #endregion

        #region GetLifethemesOfResident
        /// <summary>
        /// Returns all Lifethemes of a Resident by given Resident-ID in a List.
        /// First all ResidentLifethemes-entries will be queried where the ResidentId equals the given ID.
        /// After that foreach entry the Lifetheme for the queried LifethemeId will be queried and added into the Lifetheme-List.
        /// In the end the List will be returned.
        /// If the query fails or there aren't any entries in the ResidentLifethemes- or the Lifethemes-Table with the given IDs,
        /// an empty List will be returned.
        /// </summary>
        /// <param name="residentId"></param>
        /// <returns></returns>
        public static async Task<List<Lifetheme>> GetLifethemesOfResident(int residentId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();
            List<ResidentLifethemes> queriedResidentLifethemes = await GetResidentLifethemesByResidentId(residentId);

            List<Lifetheme> lifethemesOfResident = new List<Lifetheme>();

            foreach(ResidentLifethemes queriedResidentLifetheme in queriedResidentLifethemes)
            {
                Lifetheme lifetheme = await LifethemeDBHandler.GetSingleLifetheme(queriedResidentLifetheme.LifethemeId);
                lifethemesOfResident.Add(lifetheme);
            }

            return lifethemesOfResident;
        }
        #endregion

        #region GetResidentsOfLifethemes
        /// <summary>
        /// Returns all Residents of a Lifetheme by given Lifetheme-ID in a List.
        /// First all ResidentLifethemes-entries will be queried where the LifethemeId equals the given ID.
        /// After that foreach entry the Resident for the queried ResidentId will be queried and added into the Resident-List.
        /// In the end the List will be returned.
        /// If the query fails or there aren't any entries in the ResidentLifethemes- or the Residents-Table with the given IDs,
        /// an empty List will be returned.
        /// </summary>
        /// <param name="lifethemeId"></param>
        /// <returns></returns>
        public static async Task<List<Resident>> GetResidentsOfLifethemes(int lifethemeId)
        {
            List<ResidentLifethemes> queriedResidentLifethemes = await GetResidentLifethemesByLifethemeId(lifethemeId);

            List<Resident> residentsOfLifetheme = new List<Resident>();

            foreach (ResidentLifethemes queriedResidentLifetheme in queriedResidentLifethemes)
            {
                Resident resident = await ResidentDBHandler.GetSingleResident(queriedResidentLifetheme.ResidentId);
                residentsOfLifetheme.Add(resident);
            }

            return residentsOfLifetheme;
        }
        #endregion

        #region GetAllResidentLifethemes
        /// <summary>
        /// Returns all ResidentLifethemes as a List.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ResidentLifethemes>> GetAllResidentLifethemes()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<ResidentLifethemes>().ToListAsync();
        }
        #endregion

        #region DeleteAllEntries
        /// <summary>
        /// Deletes all entries from ResidentLifethemes-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteAllEntries()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAllAsync<ResidentLifethemes>();
        }
        #endregion

        #region DropTable
        /// <summary>
        /// Drops the ResidentLifethemes-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DropTable()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DropTableAsync<ResidentLifethemes>();
        }
        #endregion

    }
}
