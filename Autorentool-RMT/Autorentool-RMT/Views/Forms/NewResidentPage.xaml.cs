using Autorentool_RMT.ViewModels.ResidentViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewResidentPage : ContentPage
    {
        public NewResidentPage()
        {
            InitializeComponent();
            BindingContext = new NewResidentViewModel();
        }
    }
}