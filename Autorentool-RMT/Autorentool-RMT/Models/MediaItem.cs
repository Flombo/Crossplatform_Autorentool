﻿using SQLite;
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
        public Image PreviewImage { get; set; }
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
        /// <param name="name"></param>
        /// <param name="filetype"></param>
        /// <param name="path"></param>
        /// <param name="notes"></param>
        /// <param name="backendMediaItemId"></param>
        public MediaItem(string name, string filetype, string path, string notes, int backendMediaItemId)
        {
            Name = name;
            FileType = filetype;
            Path = path;
            Notes = notes;
            BackendMediaItemId = backendMediaItemId;
        }
        #endregion

        #region GetFullPath
        /// <summary>
        /// Returns the Path property if a file exists under this path.
        /// Else the default image path will be returned.
        /// </summary>
        public string GetFullPath
        {
            get
            {
                if (File.Exists(Path))
                {
                    return Path;
                }
                else
                {
                    return "ImageOld.png";
                }

            }
        }
        #endregion

    }
}