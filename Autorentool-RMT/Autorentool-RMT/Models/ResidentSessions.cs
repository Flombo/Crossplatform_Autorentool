using SQLite;

namespace Autorentool_RMT.Models
{
    [Table("ResidentSessions")]
    public class ResidentSessions
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("resident_id")]
        public int ResidentId { get; set; }

        [Column("session_id")]
        public int SessionId { get; set; }
    }
}
