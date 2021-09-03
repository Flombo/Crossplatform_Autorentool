using Autorentool_RMT.Models;
using Autorentool_RMT.ViewModels;
using System;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaItemFullscreenPopup : Popup
    {
        public MediaItemFullscreenPopup(MediaItem selectedMediaItem, double height, double width)
        {
            BindingContext = new MediaItemFullscreenPopupViewModel(selectedMediaItem.GetFullPath, height, width);
            InitializeComponent();
        }

        #region OnAcceptButtonClicked
        /// <summary>
        /// Closes the popup, when the close button was clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseButtonClicked(object sender, EventArgs e)
        {
            Dismiss(null);
        }
        #endregion

    }
}