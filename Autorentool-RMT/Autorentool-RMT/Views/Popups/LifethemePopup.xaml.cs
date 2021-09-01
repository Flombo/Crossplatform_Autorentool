using Autorentool_RMT.Models;
using Autorentool_RMT.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace Autorentool_RMT.Views.Popups
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LifethemePopup : Popup<LifethemePopup.Result>
    {
        private LifethemePopupViewModel lifethemePopupViewModel;
        private List<Lifetheme> selectedLifethemes;
        private bool isSearchLastAction;

        public LifethemePopup(List<Lifetheme> selectedLifethemes)
        {
            this.selectedLifethemes = selectedLifethemes;
            isSearchLastAction = false;
            lifethemePopupViewModel = new LifethemePopupViewModel(selectedLifethemes);
            BindingContext = lifethemePopupViewModel;
            lifethemePopupViewModel.OnLoadAllExistingLifethemes();
            InitializeComponent();
        }

        #region OnAcceptButtonClicked
        /// <summary>
        /// Returns the selected lifethemes within the Result-containerclass and closes the popup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAcceptButtonClicked(object sender, EventArgs e)
        {
            selectedLifethemes = isSearchLastAction
                ? lifethemePopupViewModel.GetSelectedLifethemes(selectedLifethemes)
                : lifethemePopupViewModel.GetSelectedLifethemes(new List<Lifetheme>());

            Result result = new Result
            {
                selectedLifethemes = selectedLifethemes
            };

            Dismiss(result);
        }
        #endregion

        #region OnCreateLifethemeButtonClicked
        private async void OnCreateLifethemeButtonClicked(object sender, EventArgs e)
        {
            isSearchLastAction = false;

            try
            {
                await lifethemePopupViewModel.OnAddLifetheme();
            } catch(Exception)
            {
                
            }
        }
        #endregion

        #region OnDeleteLifetheme
        private async void OnDeleteLifetheme(object sender, SelectionChangedEventArgs e)
        {
            isSearchLastAction = false;

            try
            {
                Lifetheme selectedLifetheme = e.CurrentSelection[0] as Lifetheme;
                await lifethemePopupViewModel.OnDeleteLifetheme(selectedLifetheme);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region Result-containerclass
        /// <summary>
        /// containerclass for the selectedLifethemes-List.
        /// It is necessary for defining the return type of the popup, because there isn't an option for returning lists.
        /// </summary>
        public class Result
        {
            public List<Lifetheme> selectedLifethemes { get; set; }
        }
        #endregion

        #region OnSearchButtonPressed
        private void OnSearchButtonPressed(object sender, EventArgs e)
        {
            lifethemePopupViewModel.OnSearch();
            isSearchLastAction = true;
        }
        #endregion
    }
}