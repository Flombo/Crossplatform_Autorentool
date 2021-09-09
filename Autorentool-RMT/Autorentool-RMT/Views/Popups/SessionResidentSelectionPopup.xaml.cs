using Autorentool_RMT.Models;
using Autorentool_RMT.ViewModels;
using System;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views.Popups
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionResidentSelectionPopup : Popup<SessionResidentSelectionPopup.Result>
    {

        private SessionResidentSelectionPopupViewModel sessionResidentSelectionPopupViewModel;

        public SessionResidentSelectionPopup()
        {
            sessionResidentSelectionPopupViewModel = new SessionResidentSelectionPopupViewModel();
            sessionResidentSelectionPopupViewModel.OnLoadAllResidents();
            BindingContext = sessionResidentSelectionPopupViewModel;
            InitializeComponent();
        }

        #region OnAcceptButtonClicked
        /// <summary>
        /// Returns the selectedResident in the Result-container-class and closes the popup.
        /// If no resident was selected, the SelectedResident will be null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAcceptButtonClicked(object sender, EventArgs e)
        {
            Resident selectedResident = sessionResidentSelectionPopupViewModel.SelectedResident;

            Result result = new Result()
            {
                SelectedResident = selectedResident
            };

            Dismiss(result);
        }
        #endregion

        #region OnAbortButtonClicked
        /// <summary>
        /// Closes the popup and returns null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAbortButtonClicked(object sender, EventArgs e)
        {
            Dismiss(null);
        }
        #endregion

        #region OnSelectionChanged
        /// <summary>
        /// Sets the SelectedResident-property of the viewmodel by the current selected resident.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Resident selectedResident = e.CurrentSelection[0] as Resident;
            sessionResidentSelectionPopupViewModel.SelectedResident = selectedResident;
        }
        #endregion

        #region Result-container-class
        /// <summary>
        /// Will be returned when the AcceptButton was clicked.
        /// Contains the selected resident, if an resident was selected.
        /// Else null.
        /// </summary>
        public class Result
        {
            public Resident SelectedResident { get; set; }
        }
        #endregion
    }
}