using Autorentool_RMT.ViewModels;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views.Popups
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotesPopup : Popup
    {
        public NotesPopup(string notes)
        {
            BindingContext = new NotesPopupViewModel(notes);
            InitializeComponent();
        }

        private void OnCloseButtonClicked(object sender, System.EventArgs e)
        {
            Dismiss(null);
        }
    }
}