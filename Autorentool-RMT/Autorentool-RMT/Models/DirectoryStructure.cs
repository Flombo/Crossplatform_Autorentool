using System;

namespace Autorentool_RMT.Models
{
    /// <summary>
    /// Folder-model used as data representation of folders and for displaying folders in FolderImportPopup.
    /// </summary>
    public class DirectoryStructure : Model
    {

        #region Attributes
        public string Name { get; set; }
        public DirectoryStructureTypes DirectoryStructureType { get; set; }
        public string FolderPath { get; set; }
        public bool HasChildren { get; set; }
        public bool IsExpanded { get; set; }
        public DateTime CreationDateTime { get; set; }
        #endregion

        #region Full-constructor
        public DirectoryStructure(string name, string folderPath, DateTime creationDateTime, DirectoryStructureTypes directoryStructureType)
        {
            Name = name;
            FolderPath = folderPath;
            CreationDateTime = creationDateTime;
            DirectoryStructureType = directoryStructureType;
        }
        #endregion

        #region Empty-constructor
        public DirectoryStructure()
        {

        }
        #endregion

        #region GetIcon
        /// <summary>
        /// Returns depending on the DirectoryStructureTypes the corresponding icon.
        /// </summary>
        public string GetIcon
        {
            get => DirectoryStructureType.Equals(DirectoryStructureTypes.FOLDER) ? "📁" : "💾";
        }
        #endregion

        #region DirectoryStructureTypes
        public enum DirectoryStructureTypes
        {
            DISK, FOLDER
        }
        #endregion

    }
}
