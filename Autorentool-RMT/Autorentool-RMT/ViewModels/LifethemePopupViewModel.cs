using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Autorentool_RMT.ViewModels
{
    public class LifethemePopupViewModel : INotifyPropertyChanged
    {
        private List<Lifetheme> allExistingLifethemes;
        private string lifethemeEntryText;
        private string searchText;

        #region Constructor
        /// <summary>
        /// Constructor initializes residents List and request all Residents from database.
        /// </summary>
        public LifethemePopupViewModel()
        {
            allExistingLifethemes = new List<Lifetheme>();
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region OnPropertyChanged
        /// <summary>
        /// Calls the corresponding method for the OnPropertyChanged-event.
        /// </summary>
        /// <param name="name"></param>
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region SearchText
        /// <summary>
        /// Getter and setter for the searchText-string
        /// </summary>
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region LifethemeEntryText
        /// <summary>
        /// Getter and setter for the lifethemeEntryText-string
        /// </summary>
        public string LifethemeEntryText
        {
            get => lifethemeEntryText;
            set
            {
                lifethemeEntryText = value;
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

        #region OnLoadAllExistingLifethemes
        /// <summary>
        /// Loads all existing Lifethemes.
        /// Checks if the selectedLifethemes are part of the requested lifethemes from db.
        /// If thats the case, the Checked-property of the current lifetheme will be set to true.
        /// </summary>
        /// <param name="selectedLifethemes"></param>
        public async void OnLoadAllExistingLifethemes(List<Lifetheme> selectedLifethemes)
        {
            AllExistingLifethemes = await GetLifethemesAndCheckedLifethemes(selectedLifethemes);
        }
        #endregion

        #region GetLifethemesAndCheckedLifethemes
        /// <summary>
        /// Returns all lifethemes and sets the Checked-property to true, if they appear in the selectedLifethemes-List.
        /// </summary>
        /// <returns></returns>
        private async Task<List<Lifetheme>> GetLifethemesAndCheckedLifethemes(List<Lifetheme> selectedLifethemes)
        {
            List<Lifetheme> allLifethemes = await LifethemeDBHandler.GetAllLifethemes();

            foreach (Lifetheme lifetheme in allLifethemes)
            {
                foreach (Lifetheme selectedLifetheme in selectedLifethemes)
                {
                    if (lifetheme.Name.Equals(selectedLifetheme.Name))
                    {
                        lifetheme.Checked = true;
                    }
                }
            }

            return allLifethemes;
        }
        #endregion

        #region OnAddLifetheme
        /// <summary>
        /// Persists lifetheme in db, sets the Checked-property to true and adds it to the selectedLifethemes-List.
        /// If this process fails an exception will be thrown.
        /// </summary>
        /// <returns></returns>
        public async Task OnAddLifetheme(List<Lifetheme> selectedLifethemes)
        {
            if (lifethemeEntryText.Length > 0)
            {
                try
                {
                    int lifethemeId = await LifethemeDBHandler.AddLifetheme(lifethemeEntryText);
                    Lifetheme addedLifetheme = await LifethemeDBHandler.GetSingleLifetheme(lifethemeId);
                    
                    selectedLifethemes.Add(addedLifetheme);

                    OnLoadAllExistingLifethemes(selectedLifethemes);
                    LifethemeEntryText = "";
                }
                catch (Exception exc)
                {
                    throw new Exception("Add Lifetheme exception:" + exc.Message);
                }
            }
        }
        #endregion

        #region OnDeleteLifetheme
        /// <summary>
        /// Deletes the current selected lifetheme from db.
        /// Also unbinds the lifetheme from all mediaitems and residents.
        /// If this process fails, an exception will be re-thrown.
        /// </summary>
        /// <param name="tappedLifetheme"></param>
        /// <returns></returns>
        public async Task OnDeleteLifetheme(Lifetheme tappedLifetheme, List<Lifetheme> selectedLifethemes)
        {
            try
            {
                selectedLifethemes.Remove(tappedLifetheme);

                await ResidentLifethemesDBHandler.UnbindAllResidentLifethemesByLifethemeId(tappedLifetheme.Id);
                await MediaItemLifethemesDBHandler.UnbindMediaItemLifethemesByLifethemeId(tappedLifetheme.Id);
                await LifethemeDBHandler.DeleteLifetheme(tappedLifetheme.Id);

                AllExistingLifethemes = await GetLifethemesAndCheckedLifethemes(selectedLifethemes);

            } catch(Exception exc)
            {
                throw new Exception("Delete Lifetheme exception:" + exc.Message);
            }
        }
        #endregion

        #region GetSelectedLifethemes
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Lifetheme> GetSelectedLifethemes()
        {
            List<Lifetheme> checkedLifethemes = new List<Lifetheme>();

            foreach(Lifetheme lifetheme in allExistingLifethemes)
            {
                if(lifetheme.Checked)
                {
                    checkedLifethemes.Add(lifetheme);
                }
            }

            return checkedLifethemes;
        }
        #endregion

    }
}
