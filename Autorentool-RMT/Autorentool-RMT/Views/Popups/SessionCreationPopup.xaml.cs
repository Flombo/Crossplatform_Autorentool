using Autorentool_RMT.ViewModels;
using System;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views.Popups
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionCreationPopup : Popup<SessionCreationPopup.Result>
    {

        private SessionCreationViewModel sessionCreationViewModel;

        public SessionCreationPopup()
        {
            sessionCreationViewModel = new SessionCreationViewModel();
            BindingContext = sessionCreationViewModel;
            InitializeComponent();
        }

        #region OnCreateButtonClicked
        /// <summary>
        /// Adds a new session and returns the result object while closing.
        /// If an error occured, the popup will be closed and the result object will be returned.
        /// The WasCreationSuccessful-property will be set depending on the process status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCreateButtonClicked(object sender, EventArgs e)
        {
            Result result = new Result();

            try
            {
                await sessionCreationViewModel.OnAddSession();

                result.SessionName = sessionCreationViewModel.SessionName;
                result.WasCreationSuccessful = true;
                Dismiss(result);
            }
            catch (Exception)
            {
                result.SessionName = sessionCreationViewModel.SessionName;
                result.WasCreationSuccessful = false;
                Dismiss(result);
            }
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

        #region Result-container object
        /// <summary>
        /// The WasEditingSuccessful tells the code behind if the process was sucessfull.
        /// The SessionName property is important for the error prompt, that will be displayed if the process failed.
        /// </summary>
        public class Result
        {
            public bool WasCreationSuccessful { get; set; }
            public string SessionName { get; set; }

        }
        #endregion
    }
}