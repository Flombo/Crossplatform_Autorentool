using Autorentool_RMT.Models;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;

namespace Autorentool_RMT.ViewModels.PopupViewModels
{
    public class FolderImportPopupViewModel : ViewModel
    {

        private List<Folder> folders;
        private Folder selectedFolder;
        private string folderImportButtonText;
        private string folderImportButtonBackgroundColour;
        private bool isFolderImportButtonEnabled;
        private string searchText;
        private double height;
        private double width;

        public FolderImportPopupViewModel(double height, double width)
        {
            this.height = height;
            this.width = width;
            folders = new List<Folder>() 
            {
                {new Folder("Test", "12334", DateTime.Today) },
                {new Folder("Test2", "12334", DateTime.Today) }
            };
            selectedFolder = null;
            folderImportButtonText = "Kein Ordner ausgewählt";
            searchText = "";
            folderImportButtonBackgroundColour = "LightGray";
            isFolderImportButtonEnabled = false;
        }


        #region PopupHeight
        public double PopupHeight => height * 0.85;
        #endregion

        #region PopupWidth

        public double PopupWidth => width * 0.75;
        #endregion

        #region Size
        public Size Size => new Size(PopupWidth, PopupHeight);
        #endregion

        #region ContentHeight
        public double ContentHeight => PopupHeight * 0.1;
        #endregion

        #region ContentWidth
        public double ContentWidth => PopupWidth * 0.85;
        #endregion

        #region CollectionViewHeight
        public double CollectionViewHeight => PopupHeight * 0.8;
        #endregion

        #region IsFolderImportButtonEnabled
        public bool IsFolderImportButtonEnabled
        {
            get => isFolderImportButtonEnabled;
            set
            {
                isFolderImportButtonEnabled = value;
                FolderImportButtonBackgroundColour = GetBackgroundColour(isFolderImportButtonEnabled, "Green");
                OnPropertyChanged();
            }
        }
        #endregion

        #region FolderImportButtonBackgroundColour
        public string FolderImportButtonBackgroundColour
        {
            get => folderImportButtonBackgroundColour;
            set
            {
                folderImportButtonBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SearchText
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region FolderImportButtonText
        public string FolderImportButtonText
        {
            get => folderImportButtonText;
            set
            {
                folderImportButtonText = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SelectedFolder
        public Folder SelectedFolder
        {
            get => selectedFolder;
            set
            {
                selectedFolder = value;

                FolderImportButtonText = selectedFolder != null ? $"'{selectedFolder.Name}' importieren" : "Kein Ordner ausgewählt";
                IsFolderImportButtonEnabled = selectedFolder != null;

                OnPropertyChanged();
            }
        }
        #endregion

        #region Folders
        public List<Folder> Folders
        {
            get => folders;
            set
            {
                folders = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region OnSearch
        /// <summary>
        /// Searches folders that contain the given search string.
        /// If none were found the folders will be reloaded.
        /// </summary>
        public void OnSearch()
        {
            if(searchText.Length > 0)
            {
                List<Folder> foundFolders = new List<Folder>();

                foreach(Folder folder in folders)
                {
                    if (folder.Name.Contains(searchText))
                    {
                        foundFolders.Add(folder);
                    }
                }

                Folders = foundFolders;

            } else
            {
                Folders = new List<Folder>()
                {
                    {new Folder("Test", "12334", DateTime.Today) },
                    {new Folder("Test2", "12334", DateTime.Today) }
                };
            }
        }
        #endregion

        #region FilterFoldersByName
        /// <summary>
        /// Filters folders by their name.
        /// Depending on the selectedIndex, asc or desc.
        /// </summary>
        /// <param name="selectedIndex"></param>
        public void FilterFoldersByName(int selectedIndex)
        {
            List<Folder> filteredFolders;

            if(selectedIndex == 1)
            {
                filteredFolders = folders.OrderBy(Folder => Folder.Name).ToList();
            } else
            {
                filteredFolders = folders.OrderByDescending(Folder => Folder.Name).ToList();
            }

            Folders = filteredFolders;
        }
        #endregion

        #region FilterFoldersByDateTime
        /// <summary>
        /// Filters folders by their datetime depending on the selected filter index.
        /// </summary>
        /// <param name="selectedIndex"></param>
        public void FilterFoldersByDateTime(int selectedIndex)
        {
            List<Folder> filteredFolders;

            if (selectedIndex == 1)
            {
                filteredFolders = folders.OrderBy(Folder => Folder.CreationDateTime).ToList();
            }
            else
            {
                filteredFolders = folders.OrderByDescending(Folder => Folder.CreationDateTime).ToList();
            }

            Folders = filteredFolders;
        }
        #endregion

    }
}
