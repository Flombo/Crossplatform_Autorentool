using Autorentool_RMT.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
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
        public string ThumbnailPath { get; set; }
        [Ignore]
        public ImageSource Source 
        {
            get; set;
        }

        [Ignore]
        public ImageSource ThumbnailSource
        {
            get; set;
        }

        #region SetSource
        /// <summary>
        /// Sets the Source-property if it is null.
        /// </summary>
        public void SetSources()
        {
            if (Source == null && IsImage)
            {
                Source = FileHandler.GetImageSource(Path);
                ThumbnailSource = FileHandler.GetImageSource(ThumbnailPath);
            }
        }
        #endregion

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

        #region IsAudioOrVideo
        public bool IsAudioOrVideo => IsAudio || IsVideo;
        #endregion

        #region IsTxt
        public bool IsTxt => FileType.Equals("txt");
        #endregion

        #region IsHTML
        public bool IsHTML => FileType.Equals("html");
        #endregion

        #region GetTextContent
        /// <summary>
        /// Returns the text content if the medium is a txt file.
        /// Else an empty string will be returned.
        /// </summary>
        /// <returns></returns>
        public string GetTextContent => IsTxt ? File.ReadAllText(GetFullPath) : "";
        #endregion

        #region GetAudioOrVideoSource
        /// <summary>
        /// Returns the source for the MediaElement if the medium is an audio or video file.
        /// Else null will be returned.
        /// </summary>
        /// <returns></returns>
        public string GetAudioOrVideoSource => IsAudioOrVideo ? new Uri(GetFullPath).LocalPath : null;
        #endregion

        #region GetFullPath
        /// <summary>
        /// Returns the Path property if a file exists under this path.
        /// </summary>
        public string GetFullPath => Path;
        #endregion

        #region GetPreviewPath
        public ImageSource GetPreviewPath
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
                        return ThumbnailSource;
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