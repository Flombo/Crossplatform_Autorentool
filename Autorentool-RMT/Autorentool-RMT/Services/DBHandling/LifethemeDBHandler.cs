﻿using Autorentool_RMT.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.DBHandling
{
    public class LifethemeDBHandler
    {

        #region AddLifetheme
        /// <summary>
        /// Creates and inserts a new Lifetheme to database asynchronously by given parameters.
        /// Returns the Id of the created Lifetheme.
        /// If there isn't any Id, an exception will be thrown.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<int> AddLifetheme(string name)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            Lifetheme lifetheme = new Lifetheme()
            {
                Name = name
            };

            try 
            {
                await sQLiteAsyncConnection.InsertAsync(lifetheme);
                return await GetLifethemeIDByName(name);
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region GetLifethemeIDByName
        /// <summary>
        /// Returns the ID of the Lifetheme by name.
        /// If there isn't any Lifetheme under that name, an exception will be thrown.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<int> GetLifethemeIDByName(string name)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();
            
            try 
            {
                Lifetheme queriedLifetheme = await sQLiteAsyncConnection.Table<Lifetheme>()
                    .FirstOrDefaultAsync(lifetheme => lifetheme.Name == name);

                return queriedLifetheme.Id;
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region DeleteLifetheme
        /// <summary>
        /// Deletes Lifetheme by given ID asynchronously.
        /// </summary>
        /// <param name="lifethemeID"></param>
        /// <returns></returns>
        public static async Task DeleteLifetheme(int lifethemeID)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAsync<Session>(lifethemeID);
        }
        #endregion

        #region UpdateLifetheme
        /// <summary>
        /// Updates Lifetheme by given ID and parameters.
        /// </summary>
        /// <param name="lifethemeID"></param>
        /// <param name="backendSessionID"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task UpdateLifetheme(int lifethemeID, string name)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            Lifetheme lifetheme = new Lifetheme()
            {
                Id = lifethemeID,
                Name = name
            };

            await sQLiteAsyncConnection.UpdateAsync(lifetheme);
        }
        #endregion

        #region GetAllLifethemes
        /// <summary>
        /// Returns all Lifethemes as a List asynchronously.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Lifetheme>> GetAllLifethemes()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            return await sQLiteAsyncConnection.Table<Lifetheme>().ToListAsync();
        }
        #endregion

        #region GetSingleLifetheme
        /// <summary>
        /// Returns a single Lifetheme by given ID async.
        /// </summary>
        /// <param name="lifethemeID"></param>
        /// <returns></returns>
        public static async Task<Lifetheme> GetSingleLifetheme(int lifethemeID)
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            int count =  await sQLiteAsyncConnection.Table<Lifetheme>().CountAsync();
            var lifetheme = await sQLiteAsyncConnection.Table<Lifetheme>().FirstOrDefaultAsync(lifetheme1 => lifetheme1.Id == lifethemeID);
            return await sQLiteAsyncConnection.GetAsync<Lifetheme>(lifethemeID);
        }
        #endregion

        #region DeleteAllEntries
        /// <summary>
        /// Deletes all entries from Lifethemes-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteAllEntries()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DeleteAllAsync<Lifetheme>();
        }
        #endregion

        #region DropTable
        /// <summary>
        /// Drops the Lifethemes-table.
        /// </summary>
        /// <returns></returns>
        public static async Task DropTable()
        {
            SQLiteAsyncConnection sQLiteAsyncConnection = await DBHandler.Init();

            await sQLiteAsyncConnection.DropTableAsync<Lifetheme>();
        }
        #endregion

    }
}
