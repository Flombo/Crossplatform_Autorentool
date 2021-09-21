using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Autorentool_RMT.UWP
{
    public class FolderPickerHelper
    {

        #region SelectFolderAsync
        /// <summary>
        /// Shows the folder-picker and returns the picked StorageFolder.
        /// The start of the picker is the desktop.
        /// Throws an exception if the process fails.
        /// </summary>
        /// <param name="fileTypeFilters"></param>
        /// <returns></returns>
        public static async Task<StorageFolder> SelectFolderAsync(string[] fileTypeFilters)
        {
            try
            {
                FolderPicker folderPicker = new FolderPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.Desktop
                };

                foreach(string fileTypeFilter in fileTypeFilters)
                {
                    folderPicker.FileTypeFilter.Add("*");
                }

                StorageFolder folder = await folderPicker.PickSingleFolderAsync();

                return folder;

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

    }
}
