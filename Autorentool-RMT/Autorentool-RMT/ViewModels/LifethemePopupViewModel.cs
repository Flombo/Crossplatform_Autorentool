using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Autorentool_RMT.ViewModels
{
    public class LifethemePopupViewModel : INotifyPropertyChanged
    {
        private List<Lifetheme> allExistingLifethemes;
        private Lifetheme selectedLifetheme;
        public ICommand AddLifetheme { get; }
        public ICommand DeleteLifetheme { get; }
        private string lifethemeEntryText;

        #region Constructor
        /// <summary>
        /// Constructor initializes residents List and request all Residents from database.
        /// </summary>
        public LifethemePopupViewModel()
        {
            allExistingLifethemes = new List<Lifetheme>();
            AddLifetheme = new Command(OnAddLifetheme);
            DeleteLifetheme = new Command(OnDeleteLifetheme);
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

        #region SelectedLifetheme
        /// <summary>
        /// Getter and setter for the selectedLifetheme-property
        /// </summary>
        public Lifetheme SelectedLifetheme
        {
            get => selectedLifetheme;
            set
            {
                if(selectedLifetheme != value) 
                {
                    selectedLifetheme = value;
                    OnDeleteLifetheme();
                }
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
        /// </summary>
        public async void OnLoadAllExistingLifethemes()
        {
            AllExistingLifethemes = await LifethemeDBHandler.GetAllLifethemes();
        }
        #endregion

        #region OnAddLifetheme
        public async void OnAddLifetheme()
        {
            if (lifethemeEntryText.Length > 0)
            {
                try
                {
                    int lifethemeId = await LifethemeDBHandler.AddLifetheme(lifethemeEntryText);
                    Lifetheme addedLifetheme = await LifethemeDBHandler.GetSingleLifetheme(lifethemeId);
                    OnLoadAllExistingLifethemes();
                    LifethemeEntryText = "";
                }
                catch (Exception e)
                {

                }
            }
        }
        #endregion

        #region OnAcceptLifethemeSelection
        /// <summary>
        /// Returns all checked Lifethemes
        /// </summary>
        public List<Lifetheme> OnAcceptLifethemeSelection()
        {
            List<Lifetheme> selectedLifethemes = new List<Lifetheme>();

            foreach (Lifetheme lifetheme in AllExistingLifethemes)
            {
                if (lifetheme.Checked)
                {
                    selectedLifethemes.Add(lifetheme);
                }
            }

            return selectedLifethemes;
        }
        #endregion

        #region OnDeleteLifetheme
        /// <summary>
        /// Deletes the current selected lifetheme from db.
        /// </summary>
        public async void OnDeleteLifetheme()
        {
            try
            {
                await LifethemeDBHandler.DeleteLifetheme(selectedLifetheme.Id);
                AllExistingLifethemes = await LifethemeDBHandler.GetAllLifethemes();
            } catch(Exception exc)
            {

            }
        }
        #endregion

    }
}
