using Autorentool_RMT.Models;
using Autorentool_RMT.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSessionPage : ContentPage
    {
        private EditSessionViewModel editSessionViewModel;
        public EditSessionPage(Session selectedSession)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            editSessionViewModel = new EditSessionViewModel(selectedSession);
            BindingContext = editSessionViewModel;
        }

        protected override async void OnAppearing()
        {
            await editSessionViewModel.OnLoadAllSessions();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MediaItem selectedMediaItem = e.CurrentSelection[0] as MediaItem;
            editSessionViewModel.SelectedMediaItem = selectedMediaItem;
        }
    }
}