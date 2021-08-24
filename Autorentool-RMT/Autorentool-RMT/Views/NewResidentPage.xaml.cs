using Autorentool_RMT.ViewModels;
using Autorentool_RMT.Views.Popups;
using System;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewResidentPage : ContentPage
    {

        private NewResidentViewModel newResidentViewModel;

        public NewResidentPage()
        {
            InitializeComponent();
            newResidentViewModel = new NewResidentViewModel();
            BindingContext = newResidentViewModel;
        }

        #region OnCompleteButtonClicked
        /// <summary>
        /// Persists created resident in db and navigates to the ResidentsPage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCompleteButtonClicked(object sender, EventArgs e)
        {
            await newResidentViewModel.OnAddResident();
            await Navigation.PopAsync();
        }
        #endregion

        #region OnShowLifethemePopupButtonClicked
        /// <summary>
        /// Shows the LifethemePopup.
        /// After the popup was closed, the Result-containerclass will be returned.
        /// The selectedLifethemes-attribute will be finally used for setting the ResidentLifethemes-property of the NewResidentViewModel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnShowLifethemePopupButtonClicked(object sender, EventArgs e)
        {
            LifethemePopup lifethemePopup = new LifethemePopup();

            LifethemePopup.Result result =  await Navigation.ShowPopupAsync(lifethemePopup);

            newResidentViewModel.ResidentLifethemes = result.selectedLifethemes;        
        }
        #endregion

    }
}