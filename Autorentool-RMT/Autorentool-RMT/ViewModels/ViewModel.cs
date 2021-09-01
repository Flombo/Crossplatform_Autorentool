using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Autorentool_RMT.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;

        #region OnPropertyChanged
        /// <summary>
        /// Calls the corresponding method for the OnPropertyChanged-event.
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region GetBackgroundColour
        /// <summary>
        /// Returns the backgroundcolour depending on the isEnabled-parameter.
        /// Is used in the xaml to get the current backgroundcolour for buttons.
        /// </summary>
        /// <param name="isEnabled"></param>
        /// <param name="enabledBackgroundColour"></param>
        /// <returns></returns>
        public string GetBackgroundColour(bool isEnabled, string enabledBackgroundColour)
        {
            return isEnabled ? enabledBackgroundColour : "lightGray";
        }
        #endregion
    }

}
