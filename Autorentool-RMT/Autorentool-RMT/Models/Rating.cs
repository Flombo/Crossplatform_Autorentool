using SQLite;

namespace Autorentool_RMT.Models
{
    [Table("Ratings")]
    public class Rating : Model
    {
        #region Rating attributes
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        
        [Unique, NotNull]
        [Column("session_id")]
        public int SessionId { get; set; }
        
        [Unique, NotNull]
        [Column("resident_id")]
        public int ResidentId { get; set; }
        
        [NotNull]
        [Column("rating_value")]
        public int RatingValue { get; set; }
        
        [NotNull]
        [Column("duration_in_seconds")]
        public int DurationInSeconds { get; set; }
        #endregion

    }
}
