using Autorentool_RMT.Models;
using Autorentool_RMT.ViewModels.PopupViewModels;
using System;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Autorentool_RMT.ViewModels.PopupViewModels.SessionRatingPopupViewModel;

namespace Autorentool_RMT.Views.Popups
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionRatingPopup : Popup<SessionRatingPopup.Result>
    {

        private SessionRatingPopupViewModel sessionRatingPopupViewModel;

        public SessionRatingPopup(Session session, Resident resident)
        {
            sessionRatingPopupViewModel = new SessionRatingPopupViewModel(resident, session);
            BindingContext = sessionRatingPopupViewModel;
            InitializeComponent();
        }

        #region OnAcceptButtonClicked
        /// <summary>
        /// Persists rating in db, returns the Result-object and closes the popup.
        /// If an error occured during persisting the WasSuccessfull-attribute will be set to false.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnAcceptButtonClicked(object sender, EventArgs e)
        {
            Result result = new Result();

            try
            {
                await sessionRatingPopupViewModel.PersistRating();

                result.WasSuccessfull = true;
                Dismiss(result);
            }
            catch (Exception)
            {
                result.WasSuccessfull = false;
                Dismiss(result);
            }
        }
        #endregion

        #region OnRatingChanged
        /// <summary>
        /// Switches the empty star icons to filled star icons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRatingChanged(object sender, SelectionChangedEventArgs e)
        {
            Star star = e.CurrentSelection[0] as Star;

            sessionRatingPopupViewModel.OnRatingChanged(star.Position);
        }
        #endregion

        #region Result-container-class
        /// <summary>
        /// Container class which will be returned, when the popup was closed.
        /// The WasSuccessfull-property delivers information if the persistance process was successfull or not.
        /// </summary>
        public class Result
        {
           public bool WasSuccessfull { get; set; }
        }
        #endregion

    }
}