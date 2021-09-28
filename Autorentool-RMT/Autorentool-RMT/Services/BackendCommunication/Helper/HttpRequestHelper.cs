using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Autorentool_RMT.Models;

namespace Autorentool_RMT.Services.BackendCommunication.Helper
{
    public class HttpRequestHelper
    {

        #region Attributes
        private string csrfToken;
        private HttpClient httpClient;
        #endregion

        #region Constructor
        public HttpRequestHelper()
        {
            httpClient = new HttpClient();
            csrfToken = "";
        }
        #endregion

        #region GetCSRFToken
        /// <summary>
        /// Requests CSRFToken.
        /// Throws an exception if an error occurs.
        /// </summary>
        public async Task GetCSRFToken(string deviceIdentifier)
        {
            try
            {
                string serialNumber = JsonConvert.SerializeObject(
                    new
                    {
                        serial_number = deviceIdentifier
                    }
                );

                StringContent content = new StringContent(serialNumber, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(new Uri("http://141.28.44.195/getcsrftoken"), content);
                
                string body = await response.Content.ReadAsStringAsync();
                HttpMessage httpMessage = JsonConvert.DeserializeObject<HttpMessage>(body);
                
                csrfToken = httpMessage.CSRFToken;

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region SendRequestToBackend
        /// <summary>
        /// sends request to given uri with given data to backend and returns the body content as string.
        /// Throws an exception if an error happens.
        /// </summary>
        /// <param name="data">stringified json data</param>
        /// <param name="uri">Uri that should be accessed</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendRequestToBackend(string data, string uri)
        {
            try
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                content.Headers.Add("x-csrf-token", csrfToken);

                HttpResponseMessage response = await httpClient.PostAsync(new Uri(uri), content);

                return response;
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

    }
}
