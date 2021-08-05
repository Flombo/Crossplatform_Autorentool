using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace Autorentool_RMT.Models
{
    [Table("Residents")]
    public class Resident : Model
    {
        #region Resident attributes
        [PrimaryKey, AutoIncrement,Column("id")]
        public int Id { get; set; }
        
        [NotNull, MaxLength(256)]
        public string Firstname { get; set; }

        [NotNull, MaxLength(256)]
        public string Lastname { get; set; }
        public string Notes { get; set; }
        [NotNull]
        public int Age { get; set; }
        [NotNull]
        public Gender Gender { get; set; }
        public string ProfilePicPath { get; set; }
        [Ignore]
        public ImageSource ProfilePicImageSource { get; set; }
        
        [Ignore]
        public List<Lifetheme> Lifethemes { get; set; }
        #endregion

        #region Resident constructor with parameters for all attributes
        /// <summary>
        /// Resident constructor that takes all single parameters.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="age"></param>
        /// <param name="gender"></param>
        /// <param name="notes"></param>
        public Resident(int id, string firstname, string lastname, int age, Gender gender, string notes, string profilePicPath)
        {
            Id = id;
            Firstname = firstname;
            Lastname = lastname;
            Age = age;
            Gender = gender;
            Notes = notes;
            ProfilePicPath = profilePicPath;
        }
        #endregion

        #region Resident constructor with another Resident as parameter
        /// <summary>
        /// Resident constructor that takes another Resident for setting the properties of this class.
        /// </summary>
        /// <param name="resident"></param>
        public Resident(Resident resident)
        {
            Id = resident.Id;
            Firstname = resident.Firstname;
            Lastname = resident.Lastname;
            Age = resident.Age;
            Gender = resident.Gender;
            Notes = resident.Notes;
            ProfilePicPath = resident.ProfilePicPath;
            //ProfileImage = resident.ProfileImage;
        }
        #endregion

        #region Empty constructor
        /// <summary>
        /// Empty constructor.
        /// Only the Notes and ProfilePicPath will be initialised with an empty string.
        /// </summary>
        public Resident()
        {
            Notes = "";
            ProfilePicPath = "";
        }
        #endregion

        #region ResidentOneLineSummary
        /// <summary>
        /// Sets ResidentOneLineSummary stringproperty with a concatenation of the firstname and lastname.
        /// </summary>
        public string ResidentOneLineSummary => $"{Firstname} {Lastname}";
        #endregion

        #region GetFullProfilePicPath
        private string GetFullProfilePicPath
        {
            get
            {
                if (File.Exists(ProfilePicPath))
                {
                    return ProfilePicPath;
                } else
                {
                    return "ImageOld.png";
                }
               
            }
        }
        #endregion

        #region GetImageSource
        /// <summary>
        /// Returns ImageSource of Resident by ProfilePicPath property
        /// </summary>
        public ImageSource ResidentImageSource
        {
            get
            {
                try
                {
                    return GetFullProfilePicPath;
                }
                catch (Exception)
                {
                    return ImageSource.FromFile("ImageOld.png");
                }
            }
        }
        #endregion

        public override bool Equals(object obj)
        {
            return obj is Resident resident &&
                   Id == resident.Id &&
                   Firstname == resident.Firstname &&
                   Lastname == resident.Lastname &&
                   Notes == resident.Notes &&
                   Age == resident.Age &&
                   Gender == resident.Gender &&
                   ProfilePicPath == resident.ProfilePicPath;
                   //EqualityComparer<BitmapImage>.Default.Equals(ProfileImage, resident.ProfileImage);
        }

        #region GetHashCode
        /// <summary>
        /// returns HashCode for the properties of the Resident properties.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = -861800702;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Firstname);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Lastname);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Notes);
            hashCode = hashCode * -1521134295 + Age.GetHashCode();
            hashCode = hashCode * -1521134295 + Gender.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProfilePicPath);
            //hashCode = hashCode * -1521134295 + EqualityComparer<BitmapImage>.Default.GetHashCode(ProfileImage);
            return hashCode;
        }
        #endregion
    }

    #region Gender enum
    /// <summary>
    /// Gender enum class with the Genders "Männlich", "Weiblich", "Divers"
    /// </summary>
    public enum Gender
    {
        Männlich, Weiblich, Divers
    }
    #endregion

}
