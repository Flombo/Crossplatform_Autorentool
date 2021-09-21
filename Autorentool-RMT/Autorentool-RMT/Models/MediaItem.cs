using Autorentool_RMT.Services;
using Newtonsoft.Json;
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
        [Ignore, JsonIgnore]
        public ImageSource Source { get; set; }
        [Ignore, JsonIgnore]
        public ImageSource ThumbnailSource { get; set; }
        public string FileType { get; set; }
        [Unique, NotNull, JsonIgnore]
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

        #region SetSource
        /// <summary>
        /// Sets the Source-property if it is null and if the MediaItem is an image.
        /// </summary>
        public void SetSource()
        {
            if (Source == null && IsImage)
            {
                Source = FileHandler.GetImageSource(Path);
            }
        }
        #endregion

        #region SetThumbnailSource
        /// <summary>
        /// Sets the ThumbnailSource-property if it is null and if the MediaItem is an image.
        /// </summary>
        public void SetThumbnailSource()
        {
            if (ThumbnailSource == null && IsImage)
            {
                ThumbnailSource = FileHandler.GetImageSource(ThumbnailPath);
            }
        }
        #endregion

        #region IsImage
        /// <summary>
        /// Determines if the MediaItem is an image and returns the corresponding boolean.
        /// Important for filtering and the ContentsPage
        /// </summary>
        [Ignore, JsonIgnore]
        public bool IsImage => FileType.Equals("jpeg") || FileType.Equals("png") || FileType.Equals("jpg");
        #endregion

        #region IsVideo
        [Ignore, JsonIgnore]
        public bool IsVideo => FileType.Equals("mp4");
        #endregion

        #region IsAudio
        [Ignore, JsonIgnore]
        public bool IsAudio => FileType.Equals("mp3");
        #endregion

        #region IsAudioOrVideo
        [Ignore, JsonIgnore]
        public bool IsAudioOrVideo => IsAudio || IsVideo;
        #endregion

        #region IsTxt
        [Ignore, JsonIgnore]
        public bool IsTxt => FileType.Equals("txt");
        #endregion

        #region IsHTML
        [Ignore, JsonIgnore]
        public bool IsHTML => FileType.Equals("html");
        #endregion

        #region GetTextContent
        /// <summary>
        /// Returns the text content if the medium is a txt file.
        /// Else an empty string will be returned.
        /// </summary>
        /// <returns></returns>
        [Ignore, JsonIgnore]
        public string GetTextContent => IsTxt ? File.ReadAllText(GetFullPath) : "";
        #endregion

        #region GetAudioOrVideoSource
        /// <summary>
        /// Returns the source for the MediaElement if the medium is an audio or video file.
        /// Else null will be returned.
        /// </summary>
        /// <returns></returns>
        [Ignore, JsonIgnore]
        public string GetAudioOrVideoSource => IsAudioOrVideo ? new Uri(GetFullPath).LocalPath : null;
        #endregion

        #region GetFullPath
        /// <summary>
        /// Returns the Path property if a file exists under this path.
        /// </summary>
        [Ignore, JsonIgnore]
        public string GetFullPath => Path;
        #endregion

        #region GetPreviewPath
        [Ignore, JsonIgnore]
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
                    case "html":
                        return "HyperlinkIcon.png";
                    default:
                        return ThumbnailSource;
                }
            }
        }
        #endregion

        #region Equals method for objects
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MediaItem))
            {
                return false;
            }

            MediaItem mediaItem = obj as MediaItem;

            return Id.Equals(mediaItem.Id);
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