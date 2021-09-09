using Autorentool_RMT.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler
{
    public class RatingDBHandler
    {
        #region AddRating
        /// <summary>
        /// Creates and inserts a new Rating to database asynchronously by given parameters.
        /// Returns the ID of the newly created Rating or -1 if no entry exists.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="residentId"></param>
        /// <param name="durationInSeconds"></param>
        /// <param name="ratingValue"></param>
        /// <returns></returns>
        public static async Task<int> AddRating(int sessionId, int residentId, int durationInSeconds, int ratingValue)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            Rating rating = new Rating()
            {
                SessionId = sessionId,
                ResidentId = residentId,
                DurationInSeconds = durationInSeconds,
                RatingValue = ratingValue
            };

            try
            {
                await sQLiteAsyncConnection.InsertAsync(rating);
                return await GetID(sessionId, residentId);
            } catch(Exception exc)
            {
                return -1;
            }
        }
        #endregion

        #region UpdateRatingValue
        /// <summary>
        /// Updates ratingValue by given ratingValue.
        /// </summary>
        /// <param name="ratingId"></param>
        /// <param name="ratingValue"></param>
        /// <returns></returns>
        public static async Task UpdateRatingValue(int ratingId, int ratingValue)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.QueryAsync<Rating>("UPDATE Ratings SET rating_value = ? WHERE id == ?", ratingValue, ratingId);
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
                Rating queriedRating = await sQLiteAsyncConnection.Table<Rating>()
                    .FirstOrDefaultAsync(rating => rating.SessionId == sessionId && rating.ResidentId == residentId);

                return queriedRating.Id;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region DeleteCertainRating
        /// <summary>
        /// Deletes Rating by given ratingId asynchronously.
        /// </summary>
        /// <param name="ratingId"></param>
        /// <returns></returns>
        public static async Task DeleteCertainRating(int ratingId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAsync<Rating>(ratingId);
        }
        #endregion

        #region DeleteAllRatingsOfSession
        /// <summary>
        /// Deletes all ratings where the SessionId-property equals the given sessionId.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task DeleteAllRatingsOfSession(int sessionId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<Rating> ratingsOfSession = await sQLiteAsyncConnection.Table<Rating>()
                .Where(ratings => ratings.SessionId == sessionId).ToListAsync();

            await DeleteGivenRatings(ratingsOfSession, sQLiteAsyncConnection);
        }
        #endregion

        #region DeleteAllRatingsOfResident
        /// <summary>
        /// Deletes all ratings where the ResidentId-property equals the given residentId.
        /// </summary>
        /// <param name="residentId"></param>
        /// <returns></returns>
        public static async Task DeleteAllRatingsOfResident(int residentId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            List<Rating> ratingsOfResident = await sQLiteAsyncConnection.Table<Rating>()
                .Where(ratings => ratings.ResidentId == residentId)
                .ToListAsync();

            await DeleteGivenRatings(ratingsOfResident, sQLiteAsyncConnection);
        }
        #endregion

        #region DeleteGivenRatings
        /// <summary>
        /// Deletes all entries from the given Ratings-List over the given SQLiteAsyncConnection.
        /// </summary>
        /// <param name="ratings"></param>
        /// <param name="sQLiteAsyncConnection"></param>
        /// <returns></returns>
        private static async Task DeleteGivenRatings(List<Rating> ratings, SQLiteAsyncConnection sQLiteAsyncConnection)
        {
            foreach(Rating rating in ratings)
            {
                await sQLiteAsyncConnection.DeleteAsync<Rating>(rating.Id);
            }
        }
        #endregion

        #region UpdateRating
        /// <summary>
        /// Updates Rating by given ID and parameters.
        /// </summary>
        /// <param name="ratingId"></param>
        /// <param name="sessionId"></param>
        /// <param name="residentId"></param>
        /// <param name="durationInSeconds"></param>
        /// <param name="ratingValue"></param>
        /// <returns></returns>
        public static async Task UpdateRating(int ratingId, int sessionId, int residentId, int durationInSeconds, int ratingValue)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            Rating rating = new Rating()
            {
                Id = ratingId,
                SessionId = sessionId,
                ResidentId = residentId,
                DurationInSeconds = durationInSeconds,
                RatingValue = ratingValue
            };

            await sQLiteAsyncConnection.UpdateAsync(rating);
        }
        #endregion

        #region GetRatingOfSessionAndResident
        /// <summary>
        /// Returns Rating for corresponding SessionId and ResidentId.
        /// If no Rating exists null will be returned.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="residentId"></param>
        /// <returns></returns>
        public static async Task<Rating> GetRatingOfSessionAndResident(int sessionId, int residentId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<Rating>()
                .Where(rating => rating.SessionId == sessionId && rating.ResidentId == residentId)
                .FirstOrDefaultAsync();
        }
        #endregion

        #region GetAllRatingsOfSession
        /// <summary>
        /// Returns all Ratings where the SessionId-property equals the sessionId-parameter as a List.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task<List<Rating>> GetAllRatingsOfSession(int sessionId) 
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<Rating>().Where(ratings => ratings.SessionId == sessionId).ToListAsync();
        }
        #endregion

        #region GetAllRatingsOfResident
        /// <summary>
        /// Returns all Ratings where the SessionId-property equals the sessionId-parameter as a List.
        /// </summary>
        /// <param name="residentId"></param>
        /// <returns></returns>
        public static async Task<List<Rating>> GetAllRatingsOfResident(int residentId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<Rating>().Where(ratings => ratings.ResidentId == residentId).ToListAsync();
        }
        #endregion

        #region GetCertainRating
        /// <summary>
        /// Returns certain Rating by given ratingId.
        /// </summary>
        /// <param name="ratingId"></param>
        /// <returns></returns>
        public static async Task<Rating> GetCertainRating(int ratingId)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.GetAsync<Rating>(ratingId);
        }
        # endregion

        #region GetAllRatings
        /// <summary>
        /// Returns all Ratings as a List.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Rating>> GetAllRatings()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<Rating>().ToListAsync();
        }
        #endregion

        #region DeleteAllEntries
        /// <summary>
        /// Deletes all entries from Ratings-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteAllEntries()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAllAsync<Rating>();
        }
        #endregion

        #region DropTable
        /// <summary>
        /// Drops the Ratings-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DropTable()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DropTableAsync<Rating>();
        }
        #endregion
    }
}
