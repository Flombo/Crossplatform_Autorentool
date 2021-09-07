using Autorentool_RMT.Services.DBHandling;
using System;
using System.Threading.Tasks;

namespace Autorentool_RMT.ViewModels
{
    public class SessionCreationViewModel : ViewModel
    {

        private string sessionName;
        private bool isCreateButtonEnabled;
        private string createButtonBackgroundColour;

        public SessionCreationViewModel()
        {
            sessionName = "";
            createButtonBackgroundColour = "LightGray";
        }

        #region CreateButtonBackgroundColour
        public string CreateButtonBackgroundColour
        {
            get => createButtonBackgroundColour;
            set
            {
                createButtonBackgroundColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsCreateButtonEnabled
        public bool IsCreateButtonEnabled
        {
            get => isCreateButtonEnabled;
            set
            {
                isCreateButtonEnabled = value;
                CreateButtonBackgroundColour = GetBackgroundColour(isCreateButtonEnabled, "#0091EA");
                OnPropertyChanged();
            }
        }
        #endregion

        #region SessionName
        public string SessionName
        {
            get => sessionName;
            set
            {
                sessionName = value;
                IsCreateButtonEnabled = sessionName.Length > 0;
                OnPropertyChanged();
            }
        }
        #endregion

        #region OnAddSession
        /// <summary>
        /// Creates and persists session in db by sessionName.
        /// Throws an exception if an error occured.
        /// </summary>
        /// <returns></returns>
        public async Task OnAddSession()
        {
            try
            {
                if (sessionName.Length > 0)
                {
                    await SessionDBHandler.AddSession(sessionName, 0);
                }

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

    }

}
