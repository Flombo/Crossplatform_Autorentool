using System;
using Xamarin.Forms;

namespace Autorentool_RMT.Services.BackendCommunication.Helper
{
    public class DeviceSerialNumberHelper
    {

        #region GetDeviceSerialNumber
        /// <summary>
        /// Retrieves the platformspecific device serial-number.
        /// Returns an empty string if an error occurs.
        /// </summary>
        /// <returns></returns>
        public static string GetDeviceSerialNumber()
        {
            try
            {

                ISerialNumberRetriever serialNumberRetriever = DependencyService.Get<ISerialNumberRetriever>();
                return serialNumberRetriever.GetDeviceSerialNumber();

            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion

    }
}