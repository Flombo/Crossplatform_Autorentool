using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autorentool_RMT.ViewModels
{
    public class SelectContentViewModel : ContentViewModel
    {

        public List<MediaItem> checkedMediaItems { get; set; }
        private Session selectedSession;

        public SelectContentViewModel(Session selectedSession)
        {
            Title = "BAUSTEINE HINZUFÜGEN";
            checkedMediaItems = new List<MediaItem>();
            IsContentPage = false;
            IsLifethemesButtonVisible = false;
            this.selectedSession = selectedSession;
            IsAddMediaItemButtonVisible = true;
        }

        #region AddMediaItemToCheckedMediaItems
        public void AddMediaItemToCheckedMediaItems(MediaItem mediaItem)
        {
            checkedMediaItems.Add(mediaItem);
            IsAddMediaItemButtonEnabled = true;
        }
        #endregion

        #region RemoveMediaItemFromCheckedMediaItems
        public void RemoveMediaItemFromCheckedMediaItems(MediaItem mediaItem)
        {
            checkedMediaItems.Remove(mediaItem);

            IsAddMediaItemButtonEnabled = checkedMediaItems.Count > 0;
        }
        #endregion

        #region BindCheckedMediaItemsToSession
        /// <summary>
        /// Binds every checked mediaitem to the selected session.
        /// If an error occurs an exception will be thrown to the code behind.
        /// </summary>
        public async Task BindCheckedMediaItemsToSession()
        {
            try
            {
                int position = 1;

                foreach (MediaItem checkedMediaItem in checkedMediaItems)
                {
                    await SessionMediaItemsDBHandler.BindSessionMediaItems(checkedMediaItem.Id, selectedSession.Id);
                    await MediaItemDBHandler.UpdatePosition(checkedMediaItem.Id, position);
                    position++;
                }
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region OnLoadAllMediaItems
        /// <summary>
        /// Loads all existing MediaItems into MediaItems-property.
        /// Throws an exception if an error occured while loading.
        /// </summary>
        /// <returns></returns>
        public async new Task OnLoadAllMediaItems()
        {
            try
            {
                MediaItems = await MediaItemDBHandler.FilterMediaItems(true, true, true, true, true);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region OnLoadRemainingMediaItems
        /// <summary>
        /// Loads all mediaItems which weren't already bound to the selected session
        /// </summary>
        public async Task OnLoadRemainingMediaItems()
        {
            List<MediaItem> sessionMediaItems = await SessionMediaItemsDBHandler.GetMediaItemsOfSession(selectedSession.Id);
            List<MediaItem> allExistingMediaItems = await MediaItemDBHandler.GetAllMediaItems();
            
            foreach(MediaItem sessionMediaItem in sessionMediaItems)
            {
                allExistingMediaItems.Remove(sessionMediaItem);
            }

            MediaItems = allExistingMediaItems;
        }
        #endregion

    }
}
