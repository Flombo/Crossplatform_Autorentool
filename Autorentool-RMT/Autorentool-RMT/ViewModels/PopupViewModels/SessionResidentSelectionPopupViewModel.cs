using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using System.Collections.Generic;

namespace Autorentool_RMT.ViewModels
{
    public class SessionResidentSelectionPopupViewModel : ViewModel
    {
        #region Attributes
        private List<Resident> residents;
        private string acceptButtonText;
        private Resident selectedResident;
        #endregion

        public SessionResidentSelectionPopupViewModel()
        {
            residents = new List<Resident>();
            selectedResident = null;
            acceptButtonText = "Weiter ohne Bewohner";
        }

        #region SelectedResident
        /// <summary>
        /// Getter and Setter for the SelectedResident-property.
        /// Updates UI after setting.
        /// Sets the AcceptButtonText-property depending on the selectedResident.
        /// </summary>
        public Resident SelectedResident
        {
            get => selectedResident;
            set
            {
                selectedResident = value;
                AcceptButtonText = selectedResident != null ? "Sitzung mit ausgewähltem Bewohner starten" : "Weiter ohne Bewohner";
                OnPropertyChanged();
            }
        }
        #endregion

        #region Residents
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

        #region AcceptButtonText
        public string AcceptButtonText
        {
            get => acceptButtonText;
            set
            {
                acceptButtonText = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region OnLoadAllResidents
        /// <summary>
        /// Loads all existing residents into the Residents-property.
        /// </summary>
        /// <returns></returns>
        public async void OnLoadAllResidents()
        {
            Residents = await ResidentDBHandler.GetAllResidents(true);
        }
        #endregion
    }
}
