using Autorentool_RMT.Models;
using Autorentool_RMT.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views.Popups
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LifethemePopup : Popup<LifethemePopup.Result>
    {
        private List<Lifetheme> selectedLifethemes;
        private LifethemePopupViewModel lifethemePopupViewModel;

        public LifethemePopup()
        {
            lifethemePopupViewModel = new LifethemePopupViewModel();
            BindingContext = lifethemePopupViewModel;
            selectedLifethemes = new List<Lifetheme>();
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
            Result result = new Result
            {
                selectedLifethemes = lifethemePopupViewModel.OnAcceptLifethemeSelection()
            };

            Dismiss(result);
        }
        #endregion

        protected override Result GetLightDismissResult()
        {
            return new Result
            {
                selectedLifethemes = selectedLifethemes
            };
        }

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