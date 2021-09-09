namespace Autorentool_RMT.ViewModels
{
    public class NotesPopupViewModel : ViewModel
    {

        private string notes;

        public NotesPopupViewModel(string notes)
        {
            this.notes = notes;
        }

        #region Notes
        public string Notes
        {
            get => notes;
            set
            {
                notes = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
