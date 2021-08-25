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

        public LifethemePopup(List<Lifetheme> selectedLifethemes)
        {
            lifethemePopupViewModel = new LifethemePopupViewModel();
            this.selectedLifethemes = selectedLifethemes;
            BindingContext = lifethemePopupViewModel;
            lifethemePopupViewModel.OnLoadAllExistingLifethemes(selectedLifethemes);
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
            Result result = new Result
            {
                selectedLifethemes = lifethemePopupViewModel.GetSelectedLifethemes()
        };

            Dismiss(result);
        }
        #endregion

        #region OnCreateLifethemeButtonClicked
        private async void OnCreateLifethemeButtonClicked(object sender, EventArgs e)
        {
            try
            {
                await lifethemePopupViewModel.OnAddLifetheme(selectedLifethemes);
            } catch(Exception)
            {
                
            }
        }
        #endregion

        #region OnDeleteLifetheme
        private async void OnDeleteLifetheme(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Lifetheme selectedLifetheme = e.CurrentSelection[0] as Lifetheme;
                await lifethemePopupViewModel.OnDeleteLifetheme(selectedLifetheme, selectedLifethemes);
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

    }
}