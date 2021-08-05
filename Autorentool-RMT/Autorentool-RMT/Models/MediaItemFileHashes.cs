using SQLite;

namespace Autorentool_RMT.Models
{
    [Table("MediaItemFileHashes")]
    public class MediaItemFileHashes
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        
        [Unique, NotNull]
        public string Hash { get; set; }

    }
}
