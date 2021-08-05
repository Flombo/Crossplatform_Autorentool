using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Autorentool_RMT.ViewModels.ResidentViewModels
{
    /// <summary>
    /// This class retrieves requests from the ResidentsPage and processes them.
    /// For database transactions the ResidentDBHandler is used.
    /// The results of these requests will be sent to back to the UI.
    /// </summary>
    public class NewResidentViewModel : INotifyPropertyChanged
    {
        private List<Lifetheme> residentLifethemes;
        private ImageSource selectedImage = ImageSource.FromFile("ImageOld.png");
        private string selectedImagePath = "";
        List<Lifetheme> allExistingLifethemes;
        private string firstname;
        private string lastname;
        private Gender gender;
        private int age;
        private string notes;
        public ICommand ShowFilePicker { get; }
        public ICommand DeleteSelectedImage { get; }
        public ICommand AddResident { get; }

        #region Constructor
        /// <summary>
        /// Constructor initializes residents List and request all Residents from database.
        /// </summary>
        public NewResidentViewModel()
        {
            ShowFilePicker = new Command(OnShowFilePicker);
            DeleteSelectedImage = new Command(OnDeleteSelectedImage);
            AddResident = new Command(OnAddResident);
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region OnPropertyChanged
        /// <summary>
        /// Calls the corresponding method for the OnPropertyChanged-event.
        /// </summary>
        /// <param name="name"></param>
        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Firstname
        /// <summary>
        /// Setter for the firstname property.
        /// </summary>
        public string Firstname
        {
            get => firstname;
            set
            {
                firstname = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Lastname
        /// <summary>
        /// Setter for the lastname property.
        /// </summary>
        public string Lastname
        {
            get => lastname;
            set
            {
                lastname = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Age
        /// <summary>
        /// Setter for the age property.
        /// </summary>
        public int Age
        {
            get => age;
            set
            {
                age = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Gender
        /// <summary>
        /// Setter for the gender property.
        /// </summary>
        public Gender Gender
        {
            get => gender;
            set
            {
                gender = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Notes
        /// <summary>
        /// Setter for the notes property.
        /// </summary>
        public string Notes
        {
            get => notes;
            set
            {
                notes = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region AllExistingLifethemes
        /// <summary>
        /// Getter and Setter for the allExistingLifethemes-List.
        /// UI retrieves over this method the residents and sets new Residents(ToDo).
        /// Setter calls OnPropertyChanged for updating the UI.
        /// </summary>
        public List<Lifetheme> AllExistingLifethemes
        {
            get => allExistingLifethemes;
            set
            {
                allExistingLifethemes = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region ResidentLifethemes
        /// <summary>
        /// Getter and Setter for the residentLifethemes-List.
        /// UI retrieves over this method the residentLifethemes and sets new Lifethemes(ToDo).
        /// Setter calls OnPropertyChanged for updating the UI.
        /// </summary>
        public List<Lifetheme> ResidentLifethemes
        {
            get => residentLifethemes;
            set
            {
                residentLifethemes = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SelectedImage
        /// <summary>
        /// Getter and setter for the selectedImage-property.
        /// </summary>
        public ImageSource SelectedImage
        {
            get => selectedImage;
            set
            {
                selectedImage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region OnShowFilePicker
        /// <summary>
        /// Shows Filepicker and sets if a file was successfully picked the result into the SelectedImage property.
        /// Writes the picked file to the LocalApplicationData-Folder.
        /// </summary>
        async void OnShowFilePicker()
        {
            try
            {
                FileResult result = await FilePicker.PickAsync();

                if (result != null)
                {

                    if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        Stream stream = await result.OpenReadAsync();

                        selectedImagePath = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            result.FileName
                            );

                        byte[] bArray = new byte[stream.Length];

                        using (FileStream fs = new FileStream(selectedImagePath, FileMode.OpenOrCreate))
                        {
                            stream.Read(bArray, 0, (int)stream.Length);
                            int length = bArray.Length;
                            fs.Write(bArray, 0, length);
                        }

                        SelectedImage = ImageSource.FromFile(selectedImagePath);
                    }
                }

            }
            catch (Exception)
            {
                if(selectedImagePath.Length > 0)
                {
                    OnDeleteSelectedImage();
                }
            }
        }
        #endregion

        #region OnDeleteSelectedImage
        /// <summary>
        /// Resets the SelectedImage to the default image.
        /// </summary>
        void OnDeleteSelectedImage()
        {
            File.Delete(selectedImagePath);
            selectedImagePath = "";
            SelectedImage = ImageSource.FromFile("old.png");
        }
        #endregion

        #region OnAddResident
        /// <summary>
        /// Adds new Residents to database.
        /// If there are checked Lifethemes, then they will be binded to the resident.
        /// </summary>
        async void OnAddResident()
        {
            try
            {
                notes = notes ?? "";

                if (firstname != null && lastname != null)
                {
                    int residentId = await ResidentDBHandler.AddResident(firstname, lastname, gender, age, selectedImagePath, notes);

                    await BindCheckedLifethemesToResident(residentId);

                    ResidentLifethemes = await ResidentLifethemesDBHandler.GetLifethemesOfResident(residentId);
                }
            }
            catch (Exception sqlException)
            {
                Debug.WriteLine(sqlException.StackTrace);
            }
        }
        #endregion

        #region BindCheckedLifethemesToResident
        /// <summary>
        /// Binds checked Lifethemes to resident by given ID.
        /// </summary>
        /// <param name="residentId"></param>
        /// <returns></returns>
        private async Task BindCheckedLifethemesToResident(int residentId)
        {
            foreach (Lifetheme selectedLifetheme in allExistingLifethemes)
            {
                if (selectedLifetheme.Checked)
                {
                    await ResidentLifethemesDBHandler.BindResidentLifethemes(residentId, selectedLifetheme.Id);
                }
            }
        }
        #endregion

        #region OnLoadAllExistingLifethemes
        /// <summary>
        /// Loads all existing Lifethemes.
        /// </summary>
        async void OnLoadAllExistingLifethemes()
        {
            AllExistingLifethemes = await LifethemeDBHandler.GetAllLifethemes();
        }
        #endregion

    }
}