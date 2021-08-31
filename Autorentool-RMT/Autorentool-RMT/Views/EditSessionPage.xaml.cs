using Autorentool_RMT.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSessionPage : ContentPage
    {
        private EditSessionViewModel editSessionViewModel;
        public EditSessionPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            editSessionViewModel = new EditSessionViewModel();
            BindingContext = editSessionViewModel;
        }

        protected override async void OnAppearing()
        {
            await editSessionViewModel.OnLoadAllSessions();
        }
    }
}