using Autorentool_RMT.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.DBHandling
{
    /// <summary>
    /// This class provides methods for persisting Residents into the Residents-table.
    /// Therefore the DBHandler-class is used to retrieve an open SQLiteAsyncConnection.
    /// </summary>
    public class ResidentDBHandler
    {

        #region AddResident
        /// <summary>
        /// Creates a new Resident by the given parameters and insert the Resident into the residents-table async.
        /// Returns the ID of the created Resident.
        /// If no Resident was found an exception will be thrown.
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="gender"></param>
        /// <param name="age"></param>
        /// <param name="profilePicPath"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static async Task<int> AddResident(string firstname, string lastname, Gender gender, int age, string profilePicPath, string notes)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            Resident resident = new Resident
            {
                Firstname = firstname,
                Lastname = lastname,
                Gender = gender,
                Age = age,
                ProfilePicPath = profilePicPath,
                Notes = notes.Length > 0 ? notes : ""
            };

            try
            {
                await sQLiteAsyncConnection.InsertAsync(resident);
                return await GetID(firstname, lastname, age, notes);
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion
        
        #region GetID
        /// <summary>
        /// Returns the ID of a Resident by given parameter.
        /// If no Resident was found an exception will be thrown.
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="age"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        private static async Task<int> GetID(string firstname, string lastname, int age, string notes)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            try
            {
                Resident queriedResident = await sQLiteAsyncConnection.Table<Resident>()
                    .FirstOrDefaultAsync(
                        resident => resident.Firstname == firstname
                            && resident.Lastname == lastname
                            && resident.Age == age
                            && resident.Notes == notes
                            );

                return queriedResident.Id;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region GetAllResidents
        /// <summary>
        /// Returns all Residents asynchronously as a List.
        /// Calls on every Resident the SetProfilePicImageSource-method.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Resident>> GetAllResidents()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();
            List<Resident> residents = await sQLiteAsyncConnection.Table<Resident>().ToListAsync();

            residents.ForEach(resident => resident.SetProfilePicImageSource());

            return residents;
        }
        #endregion

        #region DeleteResident
        /// <summary>
        /// Deletes a Resident by given ID asynchronously.
        /// </summary>
        /// <param name="residentID"></param>
        /// <returns></returns>
        public static async Task DeleteResident(int residentID)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();
            await sQLiteAsyncConnection.DeleteAsync<Resident>(residentID);
        }
        #endregion

        #region UpdateResident
        /// <summary>
        /// Updates Resident by given parameters asynchronously.
        /// </summary>
        /// <param name="residentID"></param>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="age"></param>
        /// <param name="gender"></param>
        /// <param name="profilePicPath"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static async Task UpdateResident(int residentID, string firstname, string lastname, int age, Gender gender, string profilePicPath, string notes)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            Resident resident = new Resident()
            {
                Id = residentID,
                Gender = gender,
                ProfilePicPath = profilePicPath,
                Notes = notes
            };

            if (firstname.Length > 0)
            {
                resident.Firstname = firstname;
            }

            if (lastname.Length > 0)
            {
                resident.Lastname = lastname;
            }

            if (age > 0)
            {
                resident.Age = age;
            }

            await sQLiteAsyncConnection.UpdateAsync(resident);
        }
        #endregion

        #region GetSingleResident
        /// <summary>
        /// Returns a single Resident by given ID.
        /// </summary>
        /// <param name="residentID"></param>
        /// <returns></returns>
        public static async Task<Resident> GetSingleResident(int residentID)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();
            return await sQLiteAsyncConnection.GetAsync<Resident>(residentID);
        }
        #endregion

        #region DeleteAllEntries
        /// <summary>
        /// Deletes all entries from Resident-table.
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
        /// Drops the Residents-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DropTable()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DropTableAsync<Resident>();
            await sQLiteAsyncConnection.CreateTableAsync<Resident>();
        }
        #endregion

    }
}
