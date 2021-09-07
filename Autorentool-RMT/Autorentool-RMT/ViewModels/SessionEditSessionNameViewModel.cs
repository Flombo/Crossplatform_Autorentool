using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using System;
using System.Threading.Tasks;

namespace Autorentool_RMT.ViewModels
{
    public class SessionEditSessionNameViewModel : ViewModel
    {

        private string sessionName;
        private bool isEditButtonEnabled;
        private string editButtonBackgroundColour;
        private Session selectedSession;

        public SessionEditSessionNameViewModel(Session selectedSession)
        {
            SessionName = selectedSession.Name;
            isEditButtonEnabled = false;
            editButtonBackgroundColour = "LightGray";
            this.selectedSession = selectedSession;
        }

        #region IsEditButtonEnabled
        public bool IsEditButtonEnabled
        {
            get => isEditButtonEnabled;
            set
            {
                isEditButtonEnabled = value;
                EditButtonBackgroundColour = GetBackgroundColour(isEditButtonEnabled, "#0091EA");
                OnPropertyChanged();
            }
        }
        #endregion

        #region EditButtonBackgroundColour
        public string EditButtonBackgroundColour
        {
            get => editButtonBackgroundColour;
            set
            {
                editButtonBackgroundColour = value;
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
                IsEditButtonEnabled = sessionName.Length > 0;
                OnPropertyChanged();
            }
        }
        #endregion

        #region OnEditSession
        /// <summary>
        /// Changes session name of selected session if there isn't already a session with that name.
        /// Throws an exception if an error occured or the name already exists.
        /// </summary>
        /// <returns></returns>
        public async Task OnEditSession()
        {
            try
            {
                if (sessionName.Length > 0)
                {
                    Session existingSession = await SessionDBHandler.GetSessionByName(sessionName);

                    if(existingSession == null)
                    {
                        await SessionDBHandler.UpdateSession(selectedSession.Id, 0, sessionName);
                    } else
                    {
                        throw new Exception();
                    }
                }
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion
    }
}
