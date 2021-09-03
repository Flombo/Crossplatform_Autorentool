using Autorentool_RMT.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler
{
    public class MediaItemLifethemesDBHandler
    {
        #region BindMediaItemLifetheme
        /// <summary>
        /// Creates and inserts a new MediaItemLifethemes-model to database asynchronously by given parameters.
        /// Returns the ID of the newly created MediaItemLifethemes-model.
        /// If the insertion would cause duplicates, -1 will be returned.
        /// </summary>
        /// <param name="mediaItemId"></param>
        /// <param name="lifethemeId"></param>
        /// <returns></returns>
        public static async Task<int> BindMediaItemLifetheme(int mediaItemId, int lifethemeId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            MediaItemLifethemes mediaItemLifethemes = new MediaItemLifethemes()
            {
                MediaItemId = mediaItemId,
                LifethemeId = lifethemeId
            };

            try
            {
                MediaItemLifethemes mediaItemLifethemes1 = await sQLiteAsyncConnection.Table<MediaItemLifethemes>()
                    .FirstOrDefaultAsync(queriedMediaItemLifethemes =>
                            queriedMediaItemLifethemes.LifethemeId == lifethemeId
                            && queriedMediaItemLifethemes.MediaItemId == mediaItemId);

                if(mediaItemLifethemes1 != null)
                {
                    return -1;
                }

                await sQLiteAsyncConnection.InsertAsync(mediaItemLifethemes);
                return await GetID(mediaItemId, lifethemeId);
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region GetID
        /// <summary>
        /// Returns the ID by given parameters or returns -1 if no entry was found.
        /// </summary>
        /// <param name="mediaItemId"></param>
        /// <param name="lifethemeId"></param>
        /// <returns></returns>
        public static async Task<int> GetID(int mediaItemId, int lifethemeId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            MediaItemLifethemes queriedMediaItemLifethemes = await sQLiteAsyncConnection.Table<MediaItemLifethemes>()
                .FirstOrDefaultAsync(
                    mediaItemLifethemes => mediaItemLifethemes.MediaItemId == mediaItemId 
                    && mediaItemLifethemes.LifethemeId == lifethemeId
                );

            return queriedMediaItemLifethemes != null ? queriedMediaItemLifethemes.Id : -1;
        }
        #endregion


        #region UnbindCertainMediaItemLifethemes
        /// <summary>
        /// Unbinds MediaItem and Lifetheme by deleting entry in MediaItemLifethemes table.
        /// Therefore the given mediaItemLifethemesId-parameter will be used.
        /// </summary>
        /// <param name="mediaItemLifethemesId"></param>
        /// <returns></returns>
        public static async Task UnbindCertainMediaItemLifethemes(int mediaItemLifethemesId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAsync<MediaItemLifethemes>(mediaItemLifethemesId);
        }
        #endregion

        #region UnbindMediaItemLifethemesByMediaItemId
        /// <summary>
        /// Unbinds all MediaItemLifethemes-entries where the MediaItemId-property equals the given mediaItemId.
        /// Therefore all MediaItemLifethemes have to be queried where this condition is given.
        /// After that each entry will be deleted.
        /// </summary>
        /// <param name="mediaItemId"></param>
        /// <returns></returns>
        public static async Task UnbindMediaItemLifethemesByMediaItemId(int mediaItemId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<MediaItemLifethemes> queriedMediaItemLifethemes = await sQLiteAsyncConnection.Table<MediaItemLifethemes>()
                .Where(mediaItemLifethemes => mediaItemLifethemes.MediaItemId == mediaItemId)
                .ToListAsync();

            await DeleteGivenMediaItemLifethemes(queriedMediaItemLifethemes, sQLiteAsyncConnection);
        }
        #endregion

        #region UnbindMediaItemLifethemesByLifethemeId
        /// <summary>
        /// Unbinds all MediaItemLifethemes-entries where the LifethemeId-property equals the given lifethemeId.
        /// Therefore all MediaItemLifethemes have to be queried where this condition is given.
        /// After that each entry will be deleted.
        /// </summary>
        /// <param name="lifethemeId"></param>
        /// <returns></returns>
        public static async Task UnbindMediaItemLifethemesByLifethemeId(int lifethemeId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<MediaItemLifethemes> queriedMediaItemLifethemes = await sQLiteAsyncConnection.Table<MediaItemLifethemes>()
                .Where(mediaItemLifethemes => mediaItemLifethemes.LifethemeId == lifethemeId)
                .ToListAsync();

            await DeleteGivenMediaItemLifethemes(queriedMediaItemLifethemes, sQLiteAsyncConnection);
        }
        #endregion

        #region DeleteGivenMediaItemLifethemes
        /// <summary>
        /// Deletes all MediaItemLifethemes over the given SQLiteAsyncConnection
        /// </summary>
        /// <param name="mediaItemLifethemes"></param>
        /// <param name="sQLiteAsyncConnection"></param>
        /// <returns></returns>
        private static async Task DeleteGivenMediaItemLifethemes(List<MediaItemLifethemes> mediaItemLifethemes, SQLiteAsyncConnection sQLiteAsyncConnection)
        {
            foreach (MediaItemLifethemes mediaItemLifetheme in mediaItemLifethemes)
            {
                await sQLiteAsyncConnection.DeleteAsync<MediaItemLifethemes>(mediaItemLifetheme.Id);
            }
        }
        #endregion

        #region GetCertainMediaItemLifethemesEntry
        /// <summary>
        /// Returns a certain MediaLifethemes-entry where the MediaItemId- and LifethemeId-properties equals the given parameters.
        /// </summary>
        /// <param name="mediaItemId"></param>
        /// <param name="lifethemeId"></param>
        /// <returns></returns>
        public static async Task<MediaItemLifethemes> GetCertainMediaItemLifethemesEntry(int mediaItemId, int lifethemeId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<MediaItemLifethemes>()
                .Where(mediaItemLifethemes => mediaItemLifethemes.MediaItemId == mediaItemId && mediaItemLifethemes.LifethemeId == lifethemeId)
                .FirstAsync();
        }
        #endregion

        #region GetLifethemesOfMediaItem
        /// <summary>
        /// Returns all Lifethemes of a MediaItem by given MediaItem-ID in a List.
        /// First all MediaItemLifethemes-entries will be queried where the MediaItemId equals the given ID.
        /// After that foreach entry the Lifetheme for the queried LifethemeId will be queried and added into the Lifetheme-List.
        /// In the end the List will be returned.
        /// If the query fails or there aren't any entries in the MediaItemLifethemes- or the Lifethemes-Table with the given IDs,
        /// an empty List will be returned.
        /// </summary>
        /// <param name="mediaItemId"></param>
        /// <returns></returns>
        public static async Task<List<Lifetheme>> GetLifethemesOfMediaItem(int mediaItemId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<MediaItemLifethemes> queriedResidentLifethemes = await sQLiteAsyncConnection.Table<MediaItemLifethemes>()
                .Where(mediaItemLifethemes => mediaItemLifethemes.MediaItemId == mediaItemId)
                .ToListAsync();

            List<Lifetheme> lifethemesOfMediaItem = new List<Lifetheme>();

            foreach (MediaItemLifethemes queriedMediaItemLifetheme in queriedResidentLifethemes)
            {
                Lifetheme lifetheme = await LifethemeDBHandler.GetSingleLifetheme(queriedMediaItemLifetheme.LifethemeId);
                lifethemesOfMediaItem.Add(lifetheme);
            }

            return lifethemesOfMediaItem;
        }
        #endregion

        #region GetMediaItemsOfLifetheme
        /// <summary>
        /// Returns all MediaItems of a Lifetheme by given Lifetheme-ID in a List.
        /// First all MediaItemLifethemes-entries will be queried where the LifethemeId equals the given ID.
        /// After that foreach entry the MediaItem for the queried MediaItemId will be queried and added into the MediaItem-List.
        /// In the end the List will be returned.
        /// If the query fails or there aren't any entries in the MediaItemLifethemes- or the MediaItem-Table with the given IDs,
        /// an empty List will be returned.
        /// </summary>
        /// <param name="lifethemeId"></param>
        /// <returns></returns>
        public static async Task<List<MediaItem>> GetMediaItemsOfLifetheme(int lifethemeId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<MediaItemLifethemes> queriedMediaItemLifethemes = await sQLiteAsyncConnection.Table<MediaItemLifethemes>()
                .Where(mediaItemLifethemes => mediaItemLifethemes.LifethemeId == lifethemeId)
                .ToListAsync();

            List<MediaItem> mediaItemsOfLifetheme = new List<MediaItem>();

            foreach (MediaItemLifethemes queriedMediaItemLifetheme in queriedMediaItemLifethemes)
            {
                MediaItem mediaItem = await MediaItemDBHandler.GetSingleMediaItem(queriedMediaItemLifetheme.MediaItemId);
                mediaItemsOfLifetheme.Add(mediaItem);
            }

            return mediaItemsOfLifetheme;
        }
        #endregion

        #region GetAllMediaItemLifethemes
        /// <summary>
        /// Returns all MediaItemLifethemes as a List.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<MediaItemLifethemes>> GetAllMediaItemLifethemes()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<MediaItemLifethemes>().ToListAsync();
        }
        #endregion

        #region DeleteAllEntries
        /// <summary>
        /// Deletes all entries from MediaItemLifethemes-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteAllEntries()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAllAsync<MediaItemLifethemes>();
        }
        #endregion

        #region DropTable
        /// <summary>
        /// Drops the MediaItemLifethemes-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DropTable()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DropTableAsync<MediaItemLifethemes>();
        }
        #endregion
    }
}
