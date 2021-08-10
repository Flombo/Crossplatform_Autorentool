using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Autorentool_RMT.ViewModels
{
    /// <summary>
    /// This class retrieves requests from the ResidentsPage and processes them.
    /// For database transactions the ResidentDBHandler is used.
    /// The results of these requests will be sent to back to the UI.
    /// </summary>
    public class ResidentViewModel : INotifyPropertyChanged
    {

        private List<Resident> residents;

        #region Constructor
        /// <summary>
        /// Constructor initializes residents List and request all Residents from database.
        /// </summary>
        public ResidentViewModel()
        {
            residents = new List<Resident>();
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region OnPropertyChanged
        /// <summary>
        /// Calls the corresponding method for the OnPropertyChanged-event.
        /// </summary>
        /// <param name="name"></param>
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Residents
        /// <summary>
        /// Getter and Setter for the residents-List.
        /// UI retrieves over this method the residents and sets new Residents.
        /// Setter calls OnPropertyChanged for updating the UI.
        /// </summary>
        public List<Resident> Residents
        {
            get => residents;
            set
            {
                residents = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region OnLoadAllResidents
        /// <summary>
        /// Loads all existing residents into Residents-property.
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadAllResidents()
        {
            Residents = await ResidentDBHandler.GetAllResidents();
        }
        #endregion

    }
}
