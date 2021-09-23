using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.BackendCommunication.Helper
{
    class ResidentBackendIdHelper : BackendIdHelper
    {

        public ResidentBackendIdHelper() : base()
        {
        }

        /// <summary>
        /// Sets appResidentID parameter in backend.
        /// Throws an exception if the process fails.
        /// </summary>
        /// <param name="createdResidentId"></param>
        /// <param name="backendResidentId"></param>
        public async Task SetAppResidentID(int createdResidentId, int backendResidentId)
        {
            try
            {
                string mediaItemIdJSON = JsonConvert.SerializeObject(
                    new
                    {
                        AppResidentID = createdResidentId,
                        BackendResidentID = backendResidentId
                    }
                );
                HttpResponseMessage response = await httpRequestHelper.SendRequestToBackend(mediaItemIdJSON, "http://127.0.0.1:8000/setappresidentid").ConfigureAwait(true);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

    }
}
