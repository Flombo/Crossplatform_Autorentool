namespace Autorentool_RMT.ViewModels
{
    public class PasswordPopupViewModel : ViewModel
    {

        private bool isAcceptButtonEnabled;
        private string password;
        private string acceptButtonBackgroundColour;

        public PasswordPopupViewModel()
        {
            isAcceptButtonEnabled = false;
            password = "";
            AcceptButtonBackgroundColour = "LightGray";
        }

        #region SetAcceptButtonBackgroundColour
        private void SetAcceptButtonBackgroundColour()
        {
            AcceptButtonBackgroundColour = GetBackgroundColour(IsAcceptButtonEnabled, "#0091EA");
        }
        #endregion

        #region AcceptButtonBackgroundColour
        public string AcceptButtonBackgroundColour
        {
            get => acceptButtonBackgroundColour;
            set
            {
                acceptButtonBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsAcceptButtonEnabled
        public bool IsAcceptButtonEnabled
        {
            get => isAcceptButtonEnabled;
            set
            {
                isAcceptButtonEnabled = value;
                SetAcceptButtonBackgroundColour();
                OnPropertyChanged();
            }
        }
        #endregion

        #region Password
        public string Password
        {
            get => password;
            set
            {
                password = value;
                IsAcceptButtonEnabled = password.Length > 0;
                OnPropertyChanged();
            }
        }
        #endregion

        #region CheckPasswordInput
        /// <summary>
        /// Checks if the entered password equals the master password.
        /// </summary>
        /// <returns></returns>
        public bool CheckPasswordInput()
        {
            return password.Equals("rememti-hfu");
        }
        #endregion

    }
}
