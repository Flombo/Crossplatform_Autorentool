using SQLite;
using System.Collections.Generic;

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
        [Unique, NotNull]
        public string Hash { get; set; }
        public string Notes { get; set; }
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
        #endregion

        #region Empty Constructor
        public MediaItem()
        {

        }
        #endregion

        #region Constructor with all parameters
        /// <summary>
        /// Constructor with all parameters.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="filetype"></param>
        /// <param name="path"></param>
        /// <param name="notes"></param>
        /// <param name="backendMediaItemId"></param>
        public MediaItem(int id, string name, string filetype, string hash, string path, string notes, int backendMediaItemId)
        {
            Id = id;
            Name = name;
            FileType = filetype;
            Path = path;
            Notes = notes;
            Hash = hash;
            BackendMediaItemId = backendMediaItemId;
        }
        #endregion

        #region IsImage
        /// <summary>
        /// Determines if the MediaItem is an image and returns the corresponding boolean.
        /// Important for filtering and the ContentsPage
        /// </summary>
        public bool IsImage => FileType.Equals("jpeg") || FileType.Equals("png") || FileType.Equals("jpg");
        #endregion

        #region IsVideo
        public bool IsVideo => FileType.Equals("mp4");
        #endregion

        #region IsAudio
        public bool IsAudio => FileType.Equals("mp3");
        #endregion

        #region IsTxt
        public bool IsTxt => FileType.Equals("txt");
        #endregion

        #region IsHTML
        public bool IsHTML => FileType.Equals("html");
        #endregion

        #region GetFullPath
        /// <summary>
        /// Returns the Path property if a file exists under this path.
        /// </summary>
        public string GetFullPath
        {
            get
            {
                return Path;
            }

        }
        #endregion

        #region GetPreviewPath
        public string GetPreviewPath
        {
            get
            {
                switch (FileType)
                {
                    case "mp3":
                        return "MusikIcon.png";
                    case "mp4":
                        return "FilmIcon.png";
                    case "txt":
                        return "TextIcon.png";
                    default:
                        return Path;
                }
            }
        }
        #endregion

    }
}