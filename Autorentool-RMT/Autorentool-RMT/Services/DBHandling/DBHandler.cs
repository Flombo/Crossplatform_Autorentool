using Autorentool_RMT.Models;
using SQLite;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Autorentool_RMT.Services.DBHandling
{

    /// <summary>
    /// The DBHandler-class creates the database and all tables in the Init-method.
    /// This method is also used by the other DBHandler-classes to retrieve a single SQLiteAsyncConnection.
    /// This class also provides methods for deleting all entries in all tables and a method for dropping all tables.
    /// </summary>
    public class DBHandler
    {
        #region attributes
        private static SQLiteAsyncConnection sQLiteAsyncConnection;
        #endregion

        #region Init
        /// <summary>
        /// Inits the autorentool database file and creates a SQLiteAsyncConnection as a singleton.
        /// Returns the created SQLiteAsyncConnection
        /// </summary>
        /// <returns></returns>
        public static async Task<SQLiteAsyncConnection> Init()
        {
            if(sQLiteAsyncConnection == null)
            {
                string databasePath = Path.Combine(FileSystem.AppDataDirectory, "autorentool.db");
                sQLiteAsyncConnection = new SQLiteAsyncConnection(databasePath);

                await sQLiteAsyncConnection.CreateTableAsync<MediaItem>();

                await sQLiteAsyncConnection.CreateTableAsync<Resident>();
                await sQLiteAsyncConnection.CreateTableAsync<MediaItem>();
                await sQLiteAsyncConnection.CreateTableAsync<Lifetheme>();
                await sQLiteAsyncConnection.CreateTableAsync<Session>();

                await sQLiteAsyncConnection.CreateTableAsync<MediaItemLifethemes>();
                await sQLiteAsyncConnection.CreateTableAsync<ResidentLifethemes>();
                await sQLiteAsyncConnection.CreateTableAsync<SessionMediaItems>();
                await sQLiteAsyncConnection.CreateTableAsync<ResidentSessions>();
                await sQLiteAsyncConnection.CreateTableAsync<Rating>();

            }
            return sQLiteAsyncConnection;
        }
        #endregion

        #region DeleteAllEntriesInTables
        /// <summary>
        /// Deletes all entries in all tables.
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteAllEntriesInTables()
        {
            await Init();
            await sQLiteAsyncConnection.DeleteAllAsync<Resident>();
            await sQLiteAsyncConnection.DeleteAllAsync<MediaItem>();
            await sQLiteAsyncConnection.DeleteAllAsync<Session>();
            await sQLiteAsyncConnection.DeleteAllAsync<Lifetheme>();
            await sQLiteAsyncConnection.DeleteAllAsync<Rating>();
            await sQLiteAsyncConnection.DeleteAllAsync<ResidentSessions>();
            await sQLiteAsyncConnection.DeleteAllAsync<SessionMediaItems>();
            await sQLiteAsyncConnection.DeleteAllAsync<ResidentLifethemes>();
            await sQLiteAsyncConnection.DeleteAllAsync<MediaItemLifethemes>();
        }
        #endregion

        #region DropTables
        /// <summary>
        /// Drops all tables asynchronously.
        /// </summary>
        /// <returns></returns>
        public static async Task DropTables()
        {
            string databasePath = Path.Combine(FileSystem.AppDataDirectory, "autorentool.db");
            SQLiteAsyncConnection sQLiteAsyncConnection = new SQLiteAsyncConnection(databasePath);
            
            await sQLiteAsyncConnection.DropTableAsync<Resident>();
            await sQLiteAsyncConnection.DropTableAsync<MediaItem>();
            await sQLiteAsyncConnection.DropTableAsync<Session>();
            await sQLiteAsyncConnection.DropTableAsync<Lifetheme>();
            await sQLiteAsyncConnection.DropTableAsync<Rating>();
            await sQLiteAsyncConnection.DropTableAsync<ResidentSessions>();
            await sQLiteAsyncConnection.DropTableAsync<SessionMediaItems>();
            await sQLiteAsyncConnection.DropTableAsync<ResidentLifethemes>();
            await sQLiteAsyncConnection.DropTableAsync<MediaItemLifethemes>();

            sQLiteAsyncConnection = null;
        }
        #endregion

    }
}
