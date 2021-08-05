using SQLite;

namespace Autorentool_RMT.Models
{
    [Table("Lifethemes")]
    public class Lifetheme : Model
    {
        #region Lifetheme attributes
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }
        [NotNull, Unique]
        public string Name { get; set; }
        public bool Checked { get; set; }
        #endregion

        #region Lifetheme empty constructor
        public Lifetheme()
        {

        }
        #endregion

        #region Lifetheme constructor
        /// <summary>
        /// constructor which sets the name property of the Lifetheme
        /// </summary>
        /// <param name="name"></param>
        public Lifetheme(string name)
        {
            Name = name;
        }
        #endregion
    }
}
