namespace Autorentool_RMT.ViewModels
{
    public class MediaItemFullscreenPopupViewModel : ViewModel
    {

        private string selectedMediaItemPath;
        
        public MediaItemFullscreenPopupViewModel(string selectedMediaItemPath)
        {
            this.selectedMediaItemPath = selectedMediaItemPath;
        }

        #region SelectedMediaItemPath
        public string SelectedMediaItemPath => selectedMediaItemPath;
        #endregion

    }
}
