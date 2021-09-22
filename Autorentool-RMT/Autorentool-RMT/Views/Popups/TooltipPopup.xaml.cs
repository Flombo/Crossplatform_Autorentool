using Autorentool_RMT.Models;
using Autorentool_RMT.ViewModels.PopupViewModels;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views.Popups
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TooltipPopup : Popup
    {
        public TooltipPopup(List<Tooltip> tooltips)
        {
            BindingContext = new TooltipPopupViewModel(tooltips);
            InitializeComponent();
        }

        private void OnCloseButtonClicked(object sender, System.EventArgs e)
        {
            Dismiss(null);
        }
    }
}