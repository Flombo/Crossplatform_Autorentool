using Autorentool_RMT.ViewModels;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views.Popups
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasswordPopup : Popup<PasswordPopup.Result>
    {
        
        private PasswordPopupViewModel passwordPopupViewModel;

        public PasswordPopup(string title)
        {
            InitializeComponent();
            passwordPopupViewModel = new PasswordPopupViewModel(title);
            BindingContext = passwordPopupViewModel;
        }

        #region OnAcceptButtonClicked
        /// <summary>
        /// Checks if password input matches master password and returns result.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAcceptButtonClicked(object sender, System.EventArgs e)
        {

            Result result = new Result
            {
                IsPasswordValid = passwordPopupViewModel.CheckPasswordInput(),
                InsertedPassword = passwordPopupViewModel.Password
            };

            Dismiss(result);
        }
        #endregion

        #region OnAbortButtonClicked
        /// <summary>
        /// Aborts process and closes popup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAbortButtonClicked(object sender, System.EventArgs e)
        {
            Dismiss(null);
        }
        #endregion

        public class Result
        {
            public bool IsPasswordValid { get; set; }
            public string InsertedPassword { get; set; }
        }
    }
}