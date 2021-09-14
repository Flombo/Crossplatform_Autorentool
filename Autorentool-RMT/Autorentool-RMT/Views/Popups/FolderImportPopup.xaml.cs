using Autorentool_RMT.Models;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;
using System;
using Autorentool_RMT.ViewModels.PopupViewModels;
using Xamarin.Forms;

namespace Autorentool_RMT.Views.Popups
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FolderImportPopup : Popup<FolderImportPopup.Result>
    {

        private FolderImportPopupViewModel folderImportPopupViewModel;

        public FolderImportPopup(double height, double width)
        {
            folderImportPopupViewModel = new FolderImportPopupViewModel(height, width);
            BindingContext = folderImportPopupViewModel;
            InitializeComponent();
        }

        #region OnSearchButtonClicked
        /// <summary>
        /// Searches folders with inserted search text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearchButtonClicked(object sender, EventArgs e)
        {
            folderImportPopupViewModel.OnSearch();
        }
        #endregion

        #region OnAcceptButtonClicked
        /// <summary>
        /// Builds the result container object and sets the selectedFolder-attribut by the SelectedFolder of the viewmodel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAcceptButtonClicked(object sender, EventArgs e)
        {
            Result result = new Result()
            {
                selectedFolder = folderImportPopupViewModel.SelectedFolder
            };

            Dismiss(result);
        }
        #endregion

        #region OnAbortButtonClicked
        /// <summary>
        /// Closes popup and returns null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAbortButtonClicked(object sender, EventArgs e)
        {
            Dismiss(null);
        }
        #endregion

        #region OnFoldernameFilterChanged
        /// <summary>
        /// Filters folders by their name depending on the selected filter option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFoldernameFilterChanged(object sender, EventArgs e)
        {
            Picker folderNameFilterPicker = sender as Picker;
            int selectedIndex = folderNameFilterPicker.SelectedIndex;

            folderImportPopupViewModel.FilterFoldersByName(selectedIndex);
        }
        #endregion

        #region OnFolderDateTimeFilterChanged
        /// <summary>
        /// Filters folders by their date time depending on the selected filter option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFolderDateTimeFilterChanged(object sender, EventArgs e)
        {
            Picker folderDateTimeFilterPicker = sender as Picker;
            int selectedIndex = folderDateTimeFilterPicker.SelectedIndex;

            folderImportPopupViewModel.FilterFoldersByDateTime(selectedIndex);
        }
        #endregion

        #region OnExpandFolderButtonClicked
        /// <summary>
        /// Loads subfolders into UI of the expanded folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExpandFolderButtonClicked(object sender, EventArgs e)
        {
            Button imageButton = sender as Button;
            DirectoryStructure expandedFolder = imageButton.BindingContext as DirectoryStructure;

            folderImportPopupViewModel.LoadSubFolders(expandedFolder);
        }
        #endregion

        #region Result-container-class
        /// <summary>
        /// Container class which will be returned to the ContentPage-code-behind when the accept button was clicked.
        /// Contains the selected folder.
        /// </summary>
        public class Result
        {
            public DirectoryStructure selectedFolder;
        }
        #endregion
    }
}