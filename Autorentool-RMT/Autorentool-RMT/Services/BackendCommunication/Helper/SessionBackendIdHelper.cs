using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.BackendCommunication.Helper
{
    class SessionBackendIdHelper : BackendIdHelper
    {

        public SessionBackendIdHelper() : base()
        {
        }

        #region SetAppSessionID
        /// <summary>
        /// sets appSessionID parameter in backend.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="createdSessionId"></param>
        /// <param name="backendSessionId"></param>
        public async Task SetAppSessionID(int createdSessionId, int backendSessionId)
        {
            try
            {
                string mediaItemIdJSON = JsonConvert.SerializeObject(
                    new
                    {
                        AppSessionID = createdSessionId,
                        BackendSessionID = backendSessionId
                    }
                );
                HttpResponseMessage response = await httpRequestHelper.SendRequestToBackend(mediaItemIdJSON, "http://141.28.44.195/setappsessionid").ConfigureAwait(true);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

    }
}
