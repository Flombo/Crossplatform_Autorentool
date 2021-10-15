using Autorentool_RMT.Models;
using System.Collections.Generic;

namespace Autorentool_RMT.ViewModels.PopupViewModels
{
    public class TooltipPopupViewModel : ViewModel
    {

        private List<Tooltip> tooltips;
        
        public TooltipPopupViewModel(List<Tooltip> tooltips)
        {
            this.tooltips = tooltips;
        }

        #region Tooltips
        public List<Tooltip> Tooltips
        {
            get => tooltips;
        }
        #endregion

    }
}
