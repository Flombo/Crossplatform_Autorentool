using System;
using System.Threading.Tasks;

namespace Autorentool_RMT.Services.BackendCommunication.Helper
{
    public class BackendIdHelper
    {

        protected HttpRequestHelper httpRequestHelper;

        public BackendIdHelper()
        {
            httpRequestHelper = new HttpRequestHelper();
        }

        /// <summary>
        /// inits HttpRequestHelper.
        /// HttpRequestHelper needs to retrieve a CSRF-Token from backend, for further requests.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="deviceIdentifier"></param>
        /// <returns></returns>
        public async Task Init(string deviceIdentifier)
        {
            try
            {
                await httpRequestHelper.GetCSRFToken(deviceIdentifier);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

    }
}
