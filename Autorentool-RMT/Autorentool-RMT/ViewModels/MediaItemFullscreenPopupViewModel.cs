using Xamarin.Essentials;
using Xamarin.Forms;

namespace Autorentool_RMT.ViewModels
{
    public class MediaItemFullscreenPopupViewModel : ViewModel
    {

        private string selectedMediaItemPath;
        private double height;
        private double width;

        public MediaItemFullscreenPopupViewModel(string selectedMediaItemPath, double height, double width)
        {
            this.height = height;
            this.width = width;
            this.selectedMediaItemPath = selectedMediaItemPath;
        }

        #region SelectedMediaItemPath
        public string SelectedMediaItemPath => selectedMediaItemPath;
        #endregion

        #region PopupHeight
        public double PopupHeight => height * 0.75;
        #endregion

        #region PopupWidth

        public double PopupWidth => width * 0.65;
        #endregion

        #region ImageWidth
        public double ImageWidth => width * 0.65;
        #endregion

        #region ImageHeight
        public double ImageHeight => height * 0.55;
        #endregion

        #region Size
        public Size Size => new Size(PopupWidth, PopupHeight);
        #endregion

    }
}
