using SQLite;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Autorentool_RMT.Models
{
    [Table("Mediaitems")]
    public class MediaItem : Model
    {
        #region MediaItem attributes
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        [NotNull, Unique]
        public string Name { get; set; }
        public string Path { get; set; }
        public string FileType { get; set; }
        public string Notes { get; set; }
        public string DisplayName { get; set; }

        [Ignore]
        public List<Lifetheme> Lifethemes { get; set; }

        public int BackendMediaItemId { get; set; }

        #region IconVisibility
        /// <summary>
        /// returns IconVisibility of MediaItem as boolean.
        /// if the FileType-property equals null, false will be returned.
        /// else the corresponding IconVisibility for the FileType-property will be returned.
        /// </summary>
        [Ignore]
        public bool IconVisibility
        {
            get
            {
                if (FileType == null) 
                    return false;

                switch (FileType.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                        return false;
                    default: return true;
                }
            }
        }
        #endregion

        #region ItemSymbolIcon
        /// <summary>
        /// returns ItemSymbolIcon as string.
        /// if the filetype property equals null, Important will be returned.
        /// else the ItemSymbolIcon of the medium will be returned as string
        /// </summary>
        [Ignore]
        public string ItemsSymbolIcon
        {
            get
            {
                if (FileType == null)
                {
                    return "Important";
                }

                switch (FileType.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                        return "Pictures";
                    case ".mp3":
                        return "MusicInfo";
                    case ".mp4":
                        return "Video";
                    case ".txt":
                        return "Font";
                    default: return "Important";
                }
            }
        }
        #endregion

        [Ignore]
        public Session Session { get; set; }
        [Ignore]
        public int Position { get; set; }
        [Ignore]
        public ImageSource StorageFile { get; set; }
        [Ignore]
        public Image PreviewImage { get; set; }
        #endregion

    }
}