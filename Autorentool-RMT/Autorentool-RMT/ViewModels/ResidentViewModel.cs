using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autorentool_RMT.ViewModels
{
    /// <summary>
    /// This class retrieves requests from the ResidentsPage and processes them.
    /// For database transactions the ResidentDBHandler is used.
    /// The results of these requests will be sent to back to the UI.
    /// </summary>
    public class ResidentViewModel : ViewModel
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
            Residents = await ResidentDBHandler.GetAllResidents(true);
        }
        #endregion

        #region OnFreeResources
        public void OnFreeResources()
        {
            Residents = new List<Resident>();
        }
        #endregion

    }
}
