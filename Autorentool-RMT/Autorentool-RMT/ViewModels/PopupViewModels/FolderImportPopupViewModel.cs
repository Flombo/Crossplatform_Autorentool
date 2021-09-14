using Autorentool_RMT.Models;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using System.IO;
using System.Windows.Input;

namespace Autorentool_RMT.ViewModels.PopupViewModels
{
    public class FolderImportPopupViewModel : ViewModel
    {

        #region Attributes
        private List<DirectoryStructure> rootFolders;
        private List<DirectoryStructure> subFolders;
        private DirectoryStructure selectedFolder;
        private string folderImportButtonText;
        private string folderImportButtonBackgroundColour;
        private bool isFolderImportButtonEnabled;
        private bool isExpanded;
        private string searchText;
        private double height;
        private double width;
        private bool isOneStepOutButtonVisible;
        string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        public ICommand OneStepOut { get; }
        #endregion

        public FolderImportPopupViewModel(double height, double width)
        {
            this.height = height;
            this.width = width;
            OneStepOut = new Command(OnOneStepOut);
            isOneStepOutButtonVisible = false;
            rootFolders = new List<DirectoryStructure>();
            subFolders = new List<DirectoryStructure>();
            LoadAllRootFolders(rootPath);
            selectedFolder = null;
            folderImportButtonText = "Kein Ordner ausgewählt";
            isExpanded = false;
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

        #region IsOneStepOutButtonVisible
        public bool IsOneStepOutButtonVisible
        {
            get => isOneStepOutButtonVisible;
            set
            {
                isOneStepOutButtonVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsExpanded
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                isExpanded = value;
                OnPropertyChanged();
            }
        }
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
        public DirectoryStructure SelectedFolder
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

        #region RootFolders
        public List<DirectoryStructure> RootFolders
        {
            get => rootFolders;
            set
            {
                rootFolders = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SubFolders
        public List<DirectoryStructure> SubFolders
        {
            get => subFolders;
            set
            {
                subFolders = value;
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
                List<DirectoryStructure> foundFolders = new List<DirectoryStructure>();

                foreach(DirectoryStructure folder in rootFolders)
                {
                    if (folder.Name.Contains(searchText))
                    {
                        foundFolders.Add(folder);
                    }
                }

                RootFolders = foundFolders;

            } else
            {
                RootFolders = new List<DirectoryStructure>()
                {
                    {new DirectoryStructure("Test", "12334", DateTime.Today, DirectoryStructure.DirectoryStructureTypes.FOLDER) },
                    {new DirectoryStructure("Test2", "12334", DateTime.Today, DirectoryStructure.DirectoryStructureTypes.FOLDER) }
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
            List<DirectoryStructure> filteredFolders;

            if(selectedIndex == 1)
            {
                filteredFolders = rootFolders.OrderBy(Folder => Folder.Name).ToList();
            } else
            {
                filteredFolders = rootFolders.OrderByDescending(Folder => Folder.Name).ToList();
            }

            RootFolders = filteredFolders;
        }
        #endregion

        #region FilterFoldersByDateTime
        /// <summary>
        /// Filters folders by their datetime depending on the selected filter index.
        /// </summary>
        /// <param name="selectedIndex"></param>
        public void FilterFoldersByDateTime(int selectedIndex)
        {
            List<DirectoryStructure> filteredFolders;

            if (selectedIndex == 1)
            {
                filteredFolders = rootFolders.OrderBy(Folder => Folder.CreationDateTime).ToList();
            }
            else
            {
                filteredFolders = rootFolders.OrderByDescending(Folder => Folder.CreationDateTime).ToList();
            }

            RootFolders = filteredFolders;
        }
        #endregion

        #region LoadAllRootFolders
        public void LoadAllRootFolders(string rootPath)
        {
            string[] disks = Directory.GetDirectories(rootPath);

            List<DirectoryStructure> rootFolders = new List<DirectoryStructure>();

            foreach (string disk in disks)
            {
                string[] subfolders = Directory.GetDirectories(disk);

                DirectoryStructure directoryStructure = new DirectoryStructure(Path.GetDirectoryName(disk), disk, Directory.GetCreationTime(disk), DirectoryStructure.DirectoryStructureTypes.DISK)
                {
                    HasChildren = subfolders.Length > 0
                };

                rootFolders.Add(directoryStructure);

                List<DirectoryStructure> childFolders = new List<DirectoryStructure>();


                foreach (string subfolder in subfolders)
                {
                    string subfolderName = Path.GetFileName(subfolder);
                    DirectoryStructure folder = new DirectoryStructure(subfolderName, subfolder, Directory.GetCreationTime(subfolder), DirectoryStructure.DirectoryStructureTypes.FOLDER);

                    string[] subfoldersOfSubfolder = Directory.GetDirectories(subfolder);
                    folder.HasChildren = subfoldersOfSubfolder.Length > 0;

                    childFolders.Add(folder);
                }

                SubFolders = childFolders;

            }
            RootFolders = rootFolders;
        }
        #endregion

        #region LoadSubFolders
        public void LoadSubFolders(DirectoryStructure expandedFolder)
        {
            if (!IsExpanded)
            {
                IsExpanded = true;
                expandedFolder.IsExpanded = true;
                LoadAllRootFolders(expandedFolder.FolderPath);
                IsOneStepOutButtonVisible = true;
                
            } else
            {
                IsExpanded = false;
                expandedFolder.IsExpanded = false;
                IsOneStepOutButtonVisible = false;
                LoadAllRootFolders(rootPath);
            }
        }
        #endregion

        #region OnOneStepOut
        public void OnOneStepOut()
        {
            LoadAllRootFolders(rootPath);
            IsExpanded = false;
        }
        #endregion

    }
}
