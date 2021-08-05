using System.Collections.Generic;
using SQLite;

namespace Autorentool_RMT.Models
{
    [Table("Sessions")]
    public class Session : Model
    {
        #region Session attributes
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        public int BackendSessionId { get; set; }
        [Unique]
        public string Name { get; set; }
        [Ignore]
        public List<MediaItem> MediaList { get; set; }
        [Ignore]
        public Resident Resident { get; set; }
        [Column("rating_id")]
        public int RatingId { get; set; }
        [Ignore]
        public int DurationInSeconds { get; set; }
        //public Visibility RatingVisibility = Visibility.Visible;
        #endregion

        #region Empty constructor
        /// <summary>
        /// empty constructor
        /// </summary>
        public Session()
        {

        }
        #endregion

        #region Session Constructor
        /// <summary>
        /// Session constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="medialist"></param>
        /// <param name="resident"></param>
        public Session(int id, string name, List<MediaItem> medialist, Resident resident)
        {
            Id = id;
            Name = name;
            MediaList = medialist;
            Resident = resident;
        }
        #endregion

        #region Convert-closures for DurationInSeconds-property
        /// <summary>
        /// Converts DurationInSeconds to minutes and saves it as string.
        /// </summary>
        public string ConvertMinutes => $"{" " + DurationInSeconds % 3600 / 60 + "m"}";

        /// <summary>
        /// Converts DurationInSeconds to seconds and saves it as a string.
        /// </summary>
        public string ConvertSeconds => $"{" " + DurationInSeconds % 60 + "s"}";
        #endregion

    }
}
