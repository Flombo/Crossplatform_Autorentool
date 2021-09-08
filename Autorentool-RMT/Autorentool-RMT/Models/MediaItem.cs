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
        [Ignore]
        public int BackendMediaItemId { get; set; }
        public int Position { get; set; }
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

        #region Equals method for objects
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            MediaItem lifetheme = obj as MediaItem;

            return Id.Equals(lifetheme.Id);
        }
        #endregion

        #region Equals method for MediaItems
        public bool Equals(MediaItem mediaItem)
        {
            return mediaItem != null && Id.Equals(mediaItem.Id);
        }
        #endregion

        #region GetHashCode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + Name.GetHashCode();
                hash = hash * 23 + FileType.GetHashCode();
                hash = hash * 23 + BackendMediaItemId.GetHashCode();
                hash = hash * 23 + Notes.GetHashCode();
                hash = hash * 23 + Path.GetHashCode();
                hash = hash * 23 + Hash.GetHashCode();
                return hash;
            }
        }
        #endregion

    }
}