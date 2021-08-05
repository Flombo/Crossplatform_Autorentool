using Autorentool_RMT.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler
{
    public class MediaItemFileHashesDBHandler
    {

        #region AddMediaItemFileHashes
        /// <summary>
        /// Creates and inserts a new MediaItemFileHash-instance by the given hash into the database.
        /// Returns the ID or throws an exception if no entry was found.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static async Task<int> AddMediaItemFileHashes(string hash) 
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            MediaItemFileHashes mediaItemFileHashes = new MediaItemFileHashes()
            {
                Hash = hash
            };

            try
            {
                await sQLiteAsyncConnection.InsertAsync(mediaItemFileHashes);
                return await GetID(hash);
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region GetID
        /// <summary>
        /// Returns the ID by given hash or throws an exception if no entry was found.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static async Task<int> GetID(string hash)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            try
            {
                MediaItemFileHashes queriedMediaItemFileHashes = await sQLiteAsyncConnection.Table<MediaItemFileHashes>()
                    .FirstOrDefaultAsync(mediaItemFileHashes => mediaItemFileHashes.Hash == hash);

                return queriedMediaItemFileHashes.Id;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region DeleteCertainMediaItemFileHashes
        /// <summary>
        /// Deletes certain MediaItemFileHashes-entry by given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task DeleteCertainMediaItemFileHashes(int id)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAsync<MediaItemFileHashes>(id);
        }
        #endregion

        #region UpdateMediaItemFileHashesEntry
        /// <summary>
        /// Updates Hash-property of MediaItemFileHashes-entry by given id and hash.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static async Task UpdateMediaItemFileHashesEntry(int id, string hash)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            MediaItemFileHashes mediaItemFileHashes = new MediaItemFileHashes()
            {
                Id = id,
                Hash = hash
            };

            await sQLiteAsyncConnection.UpdateAsync(mediaItemFileHashes);
        }
        #endregion

        #region GetCertainMediaItemFileHashesEntry
        /// <summary>
        /// Returns MediaItemFileHash by given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<MediaItemFileHashes> GetCertainMediaItemFileHashesEntry(int id)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.GetAsync<MediaItemFileHashes>(id);
        }
        #endregion

        #region GetAllMediaItemFileHashes
        /// <summary>
        /// Returns all MediaItemFileHashes-entries as a List.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<MediaItemFileHashes>> GetAllMediaItemFileHashes()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<MediaItemFileHashes>().ToListAsync();
        }
        #endregion

        #region DeleteAllEntries
        /// <summary>
        /// Deletes all entries in MediaItemFileHashes-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteAllEntries()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAllAsync<MediaItemFileHashes>();
        }
        #endregion

        #region DropTable
        /// <summary>
        /// Drops the whole MediaItemFileHashes-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DropTable()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DropTableAsync<MediaItemFileHashes>();
        }
        #endregion
    }
}
