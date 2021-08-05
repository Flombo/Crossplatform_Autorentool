using SQLite;

namespace Autorentool_RMT.Models
{
    [Table("MediaItemLifethemes")]
    public class MediaItemLifethemes
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        
        [NotNull]
        [Column("media_item_id")]
        public int MediaItemId { get; set; }

        [NotNull]
        [Column("lifetheme_id")]
        public int LifethemeId { get; set; }
    }
}
