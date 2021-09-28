using Autorentool_RMT.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutUsPage : ContentPage
    {

        private AboutUsViewModel aboutUsViewModel;

        public AboutUsPage()
        {
            aboutUsViewModel = new AboutUsViewModel();
            BindingContext = aboutUsViewModel;
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }

        #region OnAppearing
        /// <summary>
        /// Loads on appearing the MediaMetaDataList of 'Bildernachweis.csv' and loads the user preferences for the backend options.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            aboutUsViewModel.SetPreferenceSwitches();
            await aboutUsViewModel.LoadMediaMetaDataList();
        }
        #endregion

        #region OnShowPushDialogSwitchToggled
        /// <summary>
        /// This preference is used, for deciding if the app should display a push-dialog, if a Session or MediaItem was created/edited in the backend.
        /// If this preference is false, only the ImportFromBackend-button will be enabled in the SessionsPage or ContentsPage (depending which of these pages are the current one).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowPushDialogSwitchToggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("ShowPushDialog", e.Value);
        }
        #endregion

        #region OnAdminsDeletePermissionSwitchToggled
        /// <summary>
        /// This preference is used, for deciding if admins are allowed to delete Sessions and MediaItems/Tags over the backend.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAdminsDeletePermissionSwitchToggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("AllowAdminsDeletion", e.Value);
        }
        #endregion

        #region OnBackendCommunicationSwitchToggled
        /// <summary>
        /// This preference is used, for deciding if the app should connect itself with the backend.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBackendCommunicationSwitchToggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("ConnectToBackend", e.Value);

            if (!e.Value)
            {
                aboutUsViewModel.IsAdminsDeletePermissionSwitchToggled = false;
                aboutUsViewModel.IsShowPushDialogSwitchToggled = false;
            }
        }
        #endregion

    }
}