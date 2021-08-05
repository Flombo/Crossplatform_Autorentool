using SQLite;

namespace Autorentool_RMT.Models
{
    [Table("SessionMediaItems")]
    public class SessionMediaItems
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [NotNull]
        [Column("session_id")]
        public int SessionId { get; set; }
        
        [NotNull]
        [Column("media_item_id")]
        public int MediaItemId { get; set; }

        [NotNull]
        public int Position { get; set; }
    }
}
