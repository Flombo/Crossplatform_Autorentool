using SQLite;

namespace Autorentool_RMT.Models
{
    [Table("ResidentLifethemes")]
    public class ResidentLifethemes
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        
        [NotNull]
        [Column("resident_id")]
        public int ResidentId { get; set; }
        [NotNull]
        [Column("lifetheme_id")]
        public int LifethemeId { get; set; }
    }
}
