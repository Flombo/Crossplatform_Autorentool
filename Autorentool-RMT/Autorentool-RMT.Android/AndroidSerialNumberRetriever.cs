using Xamarin.Forms;
using static Android.Provider.Settings;

[assembly: Dependency(typeof(Autorentool_RMT.Droid.AndroidSerialNumberRetriever))]
namespace Autorentool_RMT.Droid
{
    public class AndroidSerialNumberRetriever : ISerialNumberRetriever
    {
        #region GetDeviceSerialNumber
        /// <summary>
        /// Returns the AndroidID as DeviceSerialNumber.
        /// </summary>
        /// <returns></returns>
        public string GetDeviceSerialNumber() => Secure.GetString(Android.App.Application.Context.ContentResolver, Secure.AndroidId);
        #endregion

    }
}