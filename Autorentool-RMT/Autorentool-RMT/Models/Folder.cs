using System;

namespace Autorentool_RMT.Models
{
    /// <summary>
    /// Folder-model used as data representation of folders and for displaying folders in FolderImportPopup.
    /// </summary>
    public class Folder : Model
    {

        public string Name { get; set; }
        public string FolderPath { get; set; }
        public DateTime CreationDateTime { get; set; }

        public Folder(string name, string folderPath, DateTime creationDateTime)
        {
            Name = name;
            FolderPath = folderPath;
            CreationDateTime = creationDateTime;
        }

    }
}
