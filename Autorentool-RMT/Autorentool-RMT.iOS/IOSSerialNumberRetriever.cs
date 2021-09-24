using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(Autorentool_RMT.iOS.IOSSerialNumberRetriever))]
namespace Autorentool_RMT.iOS
{
    public class IOSSerialNumberRetriever : ISerialNumberRetriever
    {

        #region GetDeviceSerialNumber
        /// <summary>
        /// Returns the IdentifierForVendor as the serial-number.
        /// </summary>
        /// <returns></returns>
        public string GetDeviceSerialNumber()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor.AsString();
        }
        #endregion

    }
}