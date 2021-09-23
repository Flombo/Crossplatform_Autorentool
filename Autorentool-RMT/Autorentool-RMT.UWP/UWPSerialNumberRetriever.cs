using System;
using System.Globalization;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Xamarin.Forms;

[assembly: Dependency(typeof(Autorentool_RMT.UWP.UWPSerialNumberRetriever))]
namespace Autorentool_RMT.UWP
{
    class UWPSerialNumberRetriever : ISerialNumberRetriever
    {
        /// <summary>
        /// Returns the serial-number of this device.
        /// The serial-number is a combination between the Application-ID and the Device-ID.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <returns></returns>
        public string GetDeviceSerialNumber()
        {
            try
            {
                Guid appId = GetApplicationId();
                Guid deviceId = GetDeviceId();
                return Math.Abs(appId.GetHashCode() + deviceId.GetHashCode()).ToString(new CultureInfo("de-DE"));

            } catch(Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Gets unique applicationID.
        /// Throws an exception if an error happens.
        /// </summary>
        /// <returns></returns>
        private Guid GetApplicationId()
        {
            try
            {
                EasClientDeviceInformation deviceInformation = new EasClientDeviceInformation();
                Guid appId = deviceInformation.Id;

                return appId;

            } catch(Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Gets unique deviceID, if the device is able to generate the deviceID.
        /// Throws an exception if an error occured during creation.
        /// </summary>
        /// <returns></returns>
        private Guid GetDeviceId()
        {
            try
            {
                Guid deviceId = new Guid();
                SystemIdentificationInfo systemId = SystemIdentification.GetSystemIdForPublisher();

                if (systemId.Source != SystemIdentificationSource.None)
                {
                    DataReader dataReader = DataReader.FromBuffer(systemId.Id);
                    deviceId = dataReader.ReadGuid();
                }
                return deviceId;

            } catch(Exception exc)
            {
                throw exc;
            }
        }

    }
}
