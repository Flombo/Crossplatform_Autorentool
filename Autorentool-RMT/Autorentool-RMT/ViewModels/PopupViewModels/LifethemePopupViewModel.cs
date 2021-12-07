using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Autorentool_RMT.ViewModels
{
    public class LifethemePopupViewModel : ViewModel
    {
        private List<Lifetheme> allExistingLifethemes;
        private string lifethemeEntryText;
        private string searchText;
        private bool isCreateLifethemeButtonEnabled;
        private List<Lifetheme> selectedLifethemes;

        #region Constructor
        /// <summary>
        /// Constructor initializes residents List and request all Residents from database.
        /// </summary>
        public LifethemePopupViewModel(List<Lifetheme> selectedLifethemes)
        {
            this.selectedLifethemes = selectedLifethemes;
            allExistingLifethemes = new List<Lifetheme>();
            IsCreateLifethemesButtonEnabled = true;
        }
        #endregion

        #region IsCreateLifethemesButtonEnabled
        public bool IsCreateLifethemesButtonEnabled
        {
            get => isCreateLifethemeButtonEnabled;
            set
            {
                isCreateLifethemeButtonEnabled = value;
                OnPropertyChanged();
            }
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
        public async void OnLoadAllExistingLifethemes()
        {
            AllExistingLifethemes = await GetLifethemesAndCheckedLifethemes();
        }
        #endregion

        #region GetLifethemesAndCheckedLifethemes
        /// <summary>
        /// Returns all lifethemes and sets the Checked-property to true, if they appear in the selectedLifethemes-List.

        /// </summary>
        /// <returns></returns>
        private async Task<List<Lifetheme>> GetLifethemesAndCheckedLifethemes()
        {
            List<Lifetheme> allLifethemes = await LifethemeDBHandler.GetAllLifethemes();

            foreach (Lifetheme lifetheme in allLifethemes)
            {
                foreach (Lifetheme selectedLifetheme in selectedLifethemes)
                {
                    if(selectedLifetheme.Name.Equals(lifetheme.Name))
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
        public async Task OnAddLifetheme()
        {
            if (lifethemeEntryText != null && lifethemeEntryText.Length > 0)
            {
                try
                {
                    int lifethemeId = await LifethemeDBHandler.AddLifetheme(lifethemeEntryText);
                    Lifetheme addedLifetheme = await LifethemeDBHandler.GetSingleLifetheme(lifethemeId);

                    selectedLifethemes = GetCheckedLifethemesFromAllExistingLifethemes();
                    selectedLifethemes.Add(addedLifetheme);

                    OnLoadAllExistingLifethemes();
                    LifethemeEntryText = "";
                }
                catch (Exception exc)
                {
                    throw new Exception("Add Lifetheme exception:" + exc.Message);
                }
            }
        }
        #endregion

        #region OnSearch
        /// <summary>
        /// Retrieves a list of lifethemes where the searchstring is part of a lifetheme name.
        /// If an empty searchstring is entered, the list will be reseted.
        /// </summary>
        public void OnSearch()
        {

            if(searchText.Length > 0)
            {
                List<Lifetheme> foundLifethemes = new List<Lifetheme>();

                foreach(Lifetheme lifetheme in allExistingLifethemes)
                {
                    if(lifetheme.Name.Contains(searchText))
                    {
                        foundLifethemes.Add(lifetheme);
                    }
                }
                
                selectedLifethemes = GetSelectedLifethemes(selectedLifethemes);

                AllExistingLifethemes = foundLifethemes;
            } else
            {
                OnLoadAllExistingLifethemes();
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
        public async Task OnDeleteLifetheme(Lifetheme tappedLifetheme)
        {
            try
            {
                selectedLifethemes.Remove(tappedLifetheme);

                await ResidentLifethemesDBHandler.UnbindAllResidentLifethemesByLifethemeId(tappedLifetheme.Id);
                await MediaItemLifethemesDBHandler.UnbindMediaItemLifethemesByLifethemeId(tappedLifetheme.Id);
                await LifethemeDBHandler.DeleteLifetheme(tappedLifetheme.Id);

                OnLoadAllExistingLifethemes();

            } catch(Exception exc)
            {
                throw new Exception("Delete Lifetheme exception:" + exc.Message);
            }
        }
        #endregion

        #region GetSelectedLifethemes
        /// <summary>
        /// Returns the selected lifethemes from allExistingLifethemes-list and selectedLifethemes.
        /// If the search was used before accepting the selection, the selectedLifethemes parameter has to be a list of former selected lifethemes.
        /// If not an empty list should be used, because the selected lifethemes are then part of the allExistingLifethemes. 
        /// </summary>
        /// <param name="selectedLifethemes"></param>
        /// <returns></returns>
        public List<Lifetheme> GetSelectedLifethemes(List<Lifetheme> selectedLifethemes)
        {
            HashSet<Lifetheme> result = new HashSet<Lifetheme>();
            List<Lifetheme> checkedLifethemes = GetCheckedLifethemesFromAllExistingLifethemes();

            foreach (Lifetheme checkedLifetheme in checkedLifethemes)
            {
                result.Add(checkedLifetheme);
            }

            foreach (Lifetheme selectedLifetheme in selectedLifethemes)
            {
                result.Add(selectedLifetheme);
            }

            return result.ToList();
        }
        #endregion

        #region GetCheckedLifethemesFromAllExistingLifethemes
        /// <summary>
        /// Retrieves all lifethemes from allExistingLifethemes-list that are checked.
        /// </summary>
        /// <returns></returns>
        private List<Lifetheme> GetCheckedLifethemesFromAllExistingLifethemes()
        {
            return allExistingLifethemes.FindAll(lifetheme => lifetheme.Checked);
        }
        #endregion
    }
}
