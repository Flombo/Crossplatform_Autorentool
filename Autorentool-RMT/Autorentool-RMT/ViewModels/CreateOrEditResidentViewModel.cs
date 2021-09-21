using Autorentool_RMT.Models;
using Autorentool_RMT.Services;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Autorentool_RMT.ViewModels
{
    /// <summary>
    /// This class retrieves requests from the ResidentsPage and processes them.
    /// For database transactions the ResidentDBHandler is used.
    /// The results of these requests will be sent to back to the UI.
    /// </summary>
    public class CreateOrEditResidentViewModel : ViewModel
    {
        private Resident residentForEditing;
        private bool isCompleteButtonEnabled;
        private bool isDeleteProfilePicButtonEnabled;
        private List<Lifetheme> residentLifethemes;
        private List<Session> residentSessions;
        private ImageSource selectedImage = ImageSource.FromFile("ImageOld.png");
        private FileResult fileResult;
        private string selectedImagePath = "";
        private string firstname = "";
        private string lastname = "";
        private Gender gender = Gender.Weiblich;
        private int age = 0;
        private string notes;
        private string completeButtonColour = "LightGray";
        private string deleteProfilePicButtonColour = "LightGray";
        private string title;
        private Session selectedSession;
        private bool isStartSessionButtonEnabled;
        private string startSessionButtonBackgroundColour = "LightGray";
        private bool isDeleteSessionButtonEnabled;
        private string deleteSessionButtonBackgroundColour = "LightGray";
        private bool isEditSessionButtonEnabled;
        private string editSessionButtonBackgroundColour = "LightGray";
        public ICommand ShowFilePicker { get; }
        public ICommand DeleteSelectedImage { get; }

        #region Constructor
        /// <summary>
        /// Constructor initializes residents List and request all Residents from database.
        /// </summary>
        public CreateOrEditResidentViewModel()
        {
            ShowFilePicker = new Command(OnShowFilePicker);
            DeleteSelectedImage = new Command(ResetSelectedImageProperties);
            residentLifethemes = new List<Lifetheme>();
            residentSessions = new List<Session>();
            Title = "BEWOHNER HINZUFÜGEN";
        }
        #endregion

        #region Constructor for editing
        public CreateOrEditResidentViewModel(Resident residentForEditing)
        {
            ShowFilePicker = new Command(OnShowFilePicker);
            DeleteSelectedImage = new Command(ResetSelectedImageProperties);
            residentLifethemes = new List<Lifetheme>();
            residentSessions = new List<Session>();

            if (residentForEditing != null)
            {
                this.residentForEditing = residentForEditing;
                this.residentForEditing.SetProfilePicImageSource();
                Notes = residentForEditing.Notes;
                Firstname = residentForEditing.Firstname;
                Lastname = residentForEditing.Lastname;
                Age = residentForEditing.Age;
                Gender = residentForEditing.Gender;
                SelectedImage = residentForEditing.ProfilePicImageSource;
                selectedImagePath = residentForEditing.GetFullProfilePicPath;
                Title = residentForEditing.ResidentOneLineSummary;
                SetIsDeleteProfilePicEnabled();
            }
        }
        #endregion

        #region IsStartSessionButtonEnabled
        public bool IsStartSessionButtonEnabled
        {
            get => isStartSessionButtonEnabled;
            set
            {
                isStartSessionButtonEnabled = value;
                StartSessionButtonBackgroundColour = GetBackgroundColour(isStartSessionButtonEnabled, "Orange");
                OnPropertyChanged();
            }
        }
        #endregion

        #region StartSessionButtonBackgroundColour
        public string StartSessionButtonBackgroundColour
        {
            get => startSessionButtonBackgroundColour;
            set
            {
                startSessionButtonBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region DeleteSessionButtonBackgroundColour
        public string DeleteSessionButtonBackgroundColour
        {
            get => deleteSessionButtonBackgroundColour;
            set
            {
                deleteSessionButtonBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsDeleteSessionButtonEnabled
        public bool IsDeleteSessionButtonEnabled
        {
            get => isDeleteSessionButtonEnabled;
            set
            {
                isDeleteSessionButtonEnabled = value;
                DeleteSessionButtonBackgroundColour = GetBackgroundColour(isDeleteSessionButtonEnabled, "Orange");
                OnPropertyChanged();
            }
        }
        #endregion

        #region EditSessionButtonBackgroundColour
        public string EditSessionButtonBackgroundColour
        {
            get => editSessionButtonBackgroundColour;
            set
            {
                editSessionButtonBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsEditSessionButtonEnabled
        public bool IsEditSessionButtonEnabled
        {
            get => isEditSessionButtonEnabled;
            set
            {
                isEditSessionButtonEnabled = value;
                EditSessionButtonBackgroundColour = GetBackgroundColour(isEditSessionButtonEnabled, "Orange");
                OnPropertyChanged();
            }
        }
        #endregion

        #region SelectedSession
        public Session SelectedSession
        {
            get => selectedSession;
            set
            {
                selectedSession = value;
                IsStartSessionButtonEnabled = selectedSession != null;
                IsEditSessionButtonEnabled = selectedSession != null;
                IsDeleteSessionButtonEnabled = selectedSession != null;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Title
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SetIsDeleteProfilePicEnabled
        /// <summary>
        /// Sets the isDeleteProfilePicButtonEnabled-property depending on the selectedImagePath.
        /// If the selectedImagePath points on the default pic, then the button should be disabled.
        /// </summary>
        public void SetIsDeleteProfilePicEnabled()
        {
            IsDeleteProfilePicButtonEnabled = selectedImagePath.Length > 0 && !selectedImagePath.Contains("ImageOld.png") ? true : false;
        }
        #endregion

        #region IsDeleteProfilePicButtonEnabled
        /// <summary>
        /// Sets and returns the isDeleteProfilePicButtonEnabled-property.
        /// </summary>
        public bool IsDeleteProfilePicButtonEnabled
        {
            get => isDeleteProfilePicButtonEnabled;
            set
            {
                isDeleteProfilePicButtonEnabled = value;
                DeleteProfilePicButtonColour = GetBackgroundColour(isDeleteProfilePicButtonEnabled, "Orange");
                OnPropertyChanged();
            }
        }
        #endregion

        #region DeleteProfilePicButtonColour
        /// <summary>
        /// Sets and returns the DeleteProfilePicButtonColour depending on the isCompleteButtonEnabled-property.
        /// </summary>
        public string DeleteProfilePicButtonColour
        {
            get => deleteProfilePicButtonColour;
            set
            {
                deleteProfilePicButtonColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region CompleteButtonColour
        /// <summary>
        /// Sets and returns the CompleteButtonColour depending on the isCompleteButtonEnabled-property.
        /// </summary>
        public string CompleteButtonColour
        {
            get => completeButtonColour;
            set
            {
                completeButtonColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsDeleteResidentButtonVisible
        /// <summary>
        /// Checks if the DeleteResidentButton should be visible.
        /// </summary>
        public bool IsDeleteResidentButtonVisible
        {
            get => residentForEditing != null;
        }
        #endregion

        #region IsCompleteButtonEnabled
        /// <summary>
        /// Checks if the IsCompleteButton should be enabled.
        /// </summary>
        public bool IsCompleteButtonEnabled
        {
            get => isCompleteButtonEnabled;
            set
            {
                isCompleteButtonEnabled = value;
                CompleteButtonColour = GetBackgroundColour(IsCompleteButtonEnabled, "Orange");
                OnPropertyChanged();
            }
        }
        #endregion


        #region SetCompleteButton
        /// <summary>
        /// Sets the isCompleteButtonEnabled property according to the length of the firstname/lastname and the age.
        /// </summary>
        private void SetCompleteButton()
        {
            bool shouldCompleteButtonBeEnabled = Firstname.Length > 0 && Lastname.Length > 0 && Age > 0;
            IsCompleteButtonEnabled = shouldCompleteButtonBeEnabled;
        }
        #endregion

        #region Firstname
        /// <summary>
        /// Setter and Getter for the firstname property.
        /// </summary>
        public string Firstname
        {
            get => firstname;
            set
            {
                firstname = value;
                SetCompleteButton();
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
                SetCompleteButton();
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
                SetCompleteButton();
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
        /// Setter and Getter for the notes property.
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

        #region ResidentLifethemes
        /// <summary>
        /// Getter and Setter for the residentLifethemes-List.
        /// UI retrieves over this method the residentLifethemes.
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

        #region ResidentSessions
        /// <summary>
        /// Getter and Setter for the residentSessions-List.
        /// UI retrieves over this method the residentSessions and sets new Sessions.
        /// Setter calls OnPropertyChanged for updating the UI.
        /// </summary>
        public List<Session> ResidentSessions
        {
            get => residentSessions;
            set
            {
                residentSessions = value;
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
                SetIsDeleteProfilePicEnabled();
                OnPropertyChanged();
            }
        }

        public object StorageApplicationPermissions { get; private set; }
        #endregion

        #region OnShowFilePicker
        /// <summary>
        /// Shows Filepicker and sets if a file was successfully picked the result into the SelectedImage property.
        /// Writes the picked file to the LocalApplicationData-Folder.
        /// </summary>
        public async void OnShowFilePicker()
        {
            try
            {

                FilePickerFileType filePickerFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>> {
                        { DevicePlatform.iOS, new [] { "jpeg", "png" } },
                        { DevicePlatform.Android, new [] { "image/jpeg", "image/png" } },
                        { DevicePlatform.UWP, new []{ "*.jpg", "*.jpeg", "*.png" } }
                    });

                PickOptions pickOptions = new PickOptions
                {
                    PickerTitle = "Wählen Sie eine Bilddatei aus, welche als Bewohnerprofilbild dienen soll",
                    FileTypes = filePickerFileType,
                };

                fileResult = await FilePicker.PickAsync(pickOptions);

                if (fileResult != null)
                {

                    selectedImagePath = fileResult.FullPath;

                    using (Stream stream = await fileResult.OpenReadAsync())
                    {
                        byte[] imageBytes = new byte[stream.Length];
                        stream.Read(imageBytes, 0, (int)stream.Length);

                        SelectedImage = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                    }
                }

            }
            catch (Exception)
            {
                if (selectedImagePath.Length > 0)
                {
                    ResetSelectedImageProperties();
                }
            }
        }
        #endregion

        #region SaveProfilePic
        /// <summary>
        /// Saves picked profile pic.
        /// if an error occured an exception will thrown.
        /// </summary>
        /// <returns></returns>
        private async Task<string> SaveProfilePic()
        {
            try
            {
                Stream stream = await fileResult.OpenReadAsync();

                string directoryPath = FileHandler.CreateDirectory("ResidentProfilePics");

                string filename = FileHandler.GetUniqueFilename(fileResult.FileName, directoryPath);

                selectedImagePath = Path.Combine(directoryPath, filename);

                FileHandler.SaveFile(stream, selectedImagePath);
                string thumbnailPath = FileHandler.CreateThumbnailAndReturnThumbnailPath(filename, selectedImagePath, 10);

                SelectedImage = FileHandler.GetImageSource(selectedImagePath);

                return thumbnailPath;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region DeleteOldThumbnail
        /// <summary>
        /// Deletes the old thumbnail if it exists.
        /// Throws an exception if an error occured during deletion.
        /// </summary>
        private void DeleteOldThumbnail()
        {
            try
            {
                if (selectedImagePath.Length > 0)
                {
                    if (!residentForEditing.GetThumbnailPath.Contains("ImageOld.png") && residentForEditing.GetThumbnailPath.Length > 0)
                    {
                        File.Delete(residentForEditing.GetThumbnailPath);
                    }
                }
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region DeleteOldProfilPic
        /// <summary>
        /// Deletes the old profile pic if it exists.
        /// Throws an exception if an error occured during deletion.
        /// </summary>
        private void DeleteOldProfilPic()
        {
            try
            {
                if (!selectedImagePath.Contains("ImageOld.png") && !residentForEditing.GetFullProfilePicPath.Contains("ImageOld.png"))
                {
                    if (residentForEditing.GetFullProfilePicPath.Length > 0 && File.Exists(residentForEditing.GetFullProfilePicPath))
                    {
                        File.Delete(residentForEditing.GetFullProfilePicPath);
                    }
                }

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region ResetSelectedImageProperties
        /// <summary>
        /// Resets selectedImagePath and SelectedImage.
        /// </summary>
        private void ResetSelectedImageProperties()
        {
            selectedImagePath = "";
            fileResult = null;
            SelectedImage = ImageSource.FromFile("ImageOld.png");
        }
        #endregion

        #region OnAddResident
        /// <summary>
        /// Adds new Residents to database.
        /// If there are checked Lifethemes, then they will be binded to the resident.
        /// If an exception occurs while adding, the exception will be re-thrown.
        /// </summary>
        public async Task OnAddResident()
        {
            try
            {
                notes = notes ?? "";
                string thumbnailPath = "";

                if (fileResult != null)
                {
                    thumbnailPath = await SaveProfilePic();
                }

                int residentId = await ResidentDBHandler.AddResident(firstname, lastname, gender, age, selectedImagePath, thumbnailPath, notes);

                await BindCheckedLifethemesToResident(residentId);

                ResidentLifethemes = await ResidentLifethemesDBHandler.GetLifethemesOfResident(residentId);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region OnEditResident
        /// <summary>
        /// Updates resident with changed properties.
        /// If an error occurs an exception will be thrown.
        /// </summary>
        /// <returns></returns>
        public async Task OnEditResident()
        {
            try
            {
                notes = notes ?? "";
                string thumbnailPath = "";

                if (selectedImagePath.Length > 0 && !residentForEditing.GetFullProfilePicPath.Equals(selectedImagePath))
                {
                    DeleteOldProfilPic();
                    DeleteOldThumbnail();
                    thumbnailPath = await SaveProfilePic();
                } else
                {
                    if (selectedImagePath.Length > 0)
                    {
                        thumbnailPath = residentForEditing.ThumbnailPath;
                    }
                }

                await ResidentDBHandler.UpdateResident(residentForEditing.Id, firstname, lastname, age, gender, selectedImagePath, thumbnailPath ,notes);

                await ResidentLifethemesDBHandler.UnbindAllResidentLifethemesByResidentId(residentForEditing.Id);

                await BindCheckedLifethemesToResident(residentForEditing.Id);

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region OnDeleteResident
        /// <summary>
        /// Unbinds current resident from all sessions and lifethemes and deletes it.
        /// If an exception occurs it will be catched, and re-thrown.
        /// </summary>
        /// <returns></returns>
        public async Task OnDeleteResident()
        {
            if (residentForEditing != null)
            {
                try
                {
                    await ResidentLifethemesDBHandler.UnbindAllResidentLifethemesByResidentId(residentForEditing.Id);
                    await ResidentSessionsDBHandler.UnbindAllResidentSessionsByResidentId(residentForEditing.Id);
                    DeleteOldProfilPic();
                    DeleteOldThumbnail();
                    await ResidentDBHandler.DeleteResident(residentForEditing.Id);
                    residentForEditing = null;
                }
                catch (Exception exc)
                {
                    throw exc;
                }

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
            try
            {
                foreach (Lifetheme residentLifetheme in residentLifethemes)
                {
                    if (residentLifetheme.Checked)
                    {
                        await ResidentLifethemesDBHandler.BindResidentLifethemes(residentId, residentLifetheme.Id);
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region GetResidentLifethemes
        /// <summary>
        /// Loads lifethemes of resident by given id.
        /// </summary>
        /// <param name="residentId"></param>
        public async void LoadResidentLifethemes(int residentId)
        {
            ResidentLifethemes = await ResidentLifethemesDBHandler.GetLifethemesOfResident(residentId);
        }
        #endregion

        #region GetResidentSessions
        /// <summary>
        /// Loads sessions of resident by given id.
        /// </summary>
        /// <param name="residentId"></param>
        public async void LoadResidentSessions(int residentId)
        {
            List<Session> sessions = await ResidentSessionsDBHandler.GetSessionsOfResident(residentId);

            foreach(Session session in sessions)
            {
                Rating rating = await RatingDBHandler.GetRatingOfSessionAndResident(session.Id, residentId);

                if(rating != null)
                {
                    session.RatingValue = rating.RatingValue;
                    session.DurationInSeconds = rating.DurationInSeconds;
                }
            }

            ResidentSessions = sessions;
            SelectedSession = null;
        }
        #endregion

        #region UnbindResidentAndSession
        public async Task UnbindResidentAndSession()
        {
            try
            {
                int residentSessionID = await ResidentSessionsDBHandler.GetID(selectedSession.Id, residentForEditing.Id);

                await ResidentSessionsDBHandler.UnbindCertainResidentSession(residentSessionID);

                SelectedSession = null;
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

    }
}