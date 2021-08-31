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

        #region Equals method for objects
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Lifetheme lifetheme = obj as Lifetheme;

            return Id.Equals(lifetheme.Id);
        }
        #endregion

        #region Equals method for Lifethemes
        public bool Equals(Lifetheme lifetheme)
        {
            return lifetheme != null && Id.Equals(lifetheme.Id);
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
                hash = hash * 23 + Checked.GetHashCode();
                return hash;
            }
        }
        #endregion

    }
}
