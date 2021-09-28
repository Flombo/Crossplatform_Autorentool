using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.BackendCommunication.Helper
{
    public class MediaItemBackendIdHelper : BackendIdHelper
    {

        public MediaItemBackendIdHelper() : base()
        {
        }

        #region SetAppMediaItemID
        /// <summary>
        /// Sets appMediaItemID parameter in backend.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="createdMediaItemId"></param>
        /// <param name="backendMediaItemId"></param>
        public async Task SetAppMediaItemID(int createdMediaItemId, int backendMediaItemId)
        {
            try
            {
                string mediaItemIdJSON = JsonConvert.SerializeObject(
                    new
                    {
                        AppMediaItemID = createdMediaItemId,
                        BackendMediaItemID = backendMediaItemId
                    }
                    );

                HttpResponseMessage response = await httpRequestHelper.SendRequestToBackend(mediaItemIdJSON, "http://141.28.44.195/setappmediaitemid");
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

    }
}