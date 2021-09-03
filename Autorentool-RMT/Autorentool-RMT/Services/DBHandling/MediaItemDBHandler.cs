using Autorentool_RMT.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.DBHandling
{
    /// <summary>
    /// This class provides methods for persisting MediaItems into the MediaItems-table.
    /// Therefore the DBHandler-class is used to retrieve an open SQLiteAsyncConnection.
    /// </summary>
    public class MediaItemDBHandler
    {

        #region AddMediaItem
        /// <summary>
        /// Creates and inserts a new MediaItem to database asynchronously by given parameters.
        /// Returns the ID of the created MediaItem.
        /// If the process failed, an exception will be thrown.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="filetype"></param>
        /// <param name="notes"></param>
        /// <param name="displayname"></param>
        /// <param name="backendMediaItemID"></param>
        /// <returns></returns>
        public static async Task<int> AddMediaItem(string name, string path, string filetype, string notes, string displayname, int backendMediaItemID)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            MediaItem mediaItem = new MediaItem()
            {
                Name = name,
                Path = path,
                FileType = filetype,
                Notes = notes,
                DisplayName = displayname,
                BackendMediaItemId = backendMediaItemID
            };

            try
            {
                await sQLiteAsyncConnection.InsertAsync(mediaItem);
                return await GetID(name);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region GetID
        /// <summary>
        /// Returns the ID of a MediaItem by the given name.
        /// If no entry with that name was found, an exception will be thrown.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<int> GetID(string name)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            try
            {
                MediaItem queriedMediaItem = await sQLiteAsyncConnection.Table<MediaItem>()
                    .FirstOrDefaultAsync(mediaItem => mediaItem.Name == name);

                return queriedMediaItem.Id;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region DeleteMediaItem
        /// <summary>
        /// Deletes MediaItem by given ID asynchronously.
        /// </summary>
        /// <param name="mediaItemID"></param>
        /// <returns></returns>
        public static async Task DeleteMediaItem(int mediaItemID)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAsync<MediaItem>(mediaItemID);
        }
        #endregion

        #region UpdateMediaItem
        /// <summary>
        /// Updates MediaItem by given ID and parameters.
        /// </summary>
        /// <param name="mediaItemID"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="filetype"></param>
        /// <param name="notes"></param>
        /// <param name="displayname"></param>
        /// <param name="backendMediaItemID"></param>
        /// <returns></returns>
        public static async Task UpdateMediaItem(int mediaItemID, string name, string path, string filetype, string notes, string displayname, int backendMediaItemID)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            MediaItem mediaItem = new MediaItem()
            {
                Id = mediaItemID,
                Name = name,
                Path = path,
                FileType = filetype,
                Notes = notes,
                DisplayName = displayname,
                BackendMediaItemId = backendMediaItemID
            };

            await sQLiteAsyncConnection.UpdateAsync(mediaItem);
        }
        #endregion

        #region GetAllMediaItems
        /// <summary>
        /// Returns all MediaItems as a List asynchronously.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<MediaItem>> GetAllMediaItems()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<MediaItem>().ToListAsync();
        }
        #endregion

        #region GetSingleMediaItem
        /// <summary>
        /// Returns a single MediaItem by given ID async.
        /// </summary>
        /// <param name="mediaItemID"></param>
        /// <returns></returns>
        public static async Task<MediaItem> GetSingleMediaItem(int mediaItemID)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.GetAsync<MediaItem>(mediaItemID);
        }
        #endregion

        #region DeleteAllEntries
        /// <summary>
        /// Deletes all entries from MediaItem-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteAllEntries()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAllAsync<MediaItem>();
        }
        #endregion

        #region DropTable
        /// <summary>
        /// Drops the MediaItems-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DropTable()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DropTableAsync<MediaItem>();
        }
        #endregion

        #region SearchMediaItems
        /// <summary>
        /// Searches MediaItems that contains the searchText.
        /// First of all filters all MediaItems depending on the given filters.
        /// Returns all MediaItems that are matching the filters and the search string.
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="photoFilter"></param>
        /// <param name="musicFilter"></param>
        /// <param name="documentFilter"></param>
        /// <param name="filmFilter"></param>
        /// <param name="linkFilter"></param>
        /// <returns></returns>
        public static async Task<List<MediaItem>> SearchMediaItems(string searchText, bool photoFilter, bool musicFilter, bool documentFilter, bool filmFilter, bool linkFilter)
        {
            List<MediaItem> filteredMediaItems = await FilterMediaItems(photoFilter, musicFilter, documentFilter, filmFilter, linkFilter);

            List<MediaItem> foundMediaItems = filteredMediaItems.FindAll(filteredMediaItem => filteredMediaItem.Name.Contains(searchText));
            
            return foundMediaItems;
        }
        #endregion

        #region FilterMediaItems
        /// <summary>
        /// Filters MediaItems depending on the given filters.
        /// </summary>
        /// <param name="photoFilter"></param>
        /// <param name="musicFilter"></param>
        /// <param name="documentFilter"></param>
        /// <param name="filmFilter"></param>
        /// <param name="linkFilter"></param>
        /// <returns></returns>
        public static async Task<List<MediaItem>> FilterMediaItems(bool photoFilter, bool musicFilter, bool documentFilter, bool filmFilter, bool linkFilter)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<MediaItem> filteredMediaItems = new List<MediaItem>();

            if (photoFilter)
            {
                List<MediaItem> imageMediaItems = await sQLiteAsyncConnection.Table<MediaItem>().Where(
                        mediaItem => 
                        mediaItem.FileType.Equals("jpeg") 
                        || mediaItem.FileType.Equals("jpg") 
                        || mediaItem.FileType.Equals("png")
                    )
                    .ToListAsync();

                filteredMediaItems.AddRange(imageMediaItems);
            }

            if(musicFilter)
            {
                filteredMediaItems.AddRange(await sQLiteAsyncConnection.Table<MediaItem>().Where(mediaItem => mediaItem.FileType.Equals("mp3")).ToListAsync());
            }

            if(documentFilter)
            {
                filteredMediaItems.AddRange(await sQLiteAsyncConnection.Table<MediaItem>().Where(mediaItem => mediaItem.FileType.Equals("txt")).ToListAsync());
            }

            if(filmFilter)
            {
                filteredMediaItems.AddRange(await sQLiteAsyncConnection.Table<MediaItem>().Where(mediaItem => mediaItem.FileType.Equals("mp4")).ToListAsync());
            }

            if(linkFilter)
            {
                filteredMediaItems.AddRange(await sQLiteAsyncConnection.Table<MediaItem>().Where(mediaItem => mediaItem.FileType.Equals("html")).ToListAsync());
            }

            return filteredMediaItems;
        }
        #endregion

    }
}
