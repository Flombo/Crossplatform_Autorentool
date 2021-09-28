using Autorentool_RMT.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Autorentool_RMT.ViewModels
{
    public class AboutUsViewModel : ViewModel
    {

        #region Attributes
        public ICommand ClickedHyperlink { get; }
        private string appVersion;
        private List<MediaMetaData> mediaMetaDataList;
        private bool isBackendCommunicationSwitchToggled;
        private bool isAdminsDeletePermissionSwitchToggled;
        private bool isShowPushDialogSwitchToggled;
        #endregion

        public AboutUsViewModel()
        {
            ClickedHyperlink = new Command<string>(async (url) => await Launcher.OpenAsync(url));
            SetAppVersion();
        }

        #region IsBackendCommunicationSwitchToggled
        /// <summary>
        /// Getter and Setter for the IsBackendCommunicationSwitchToggled-switch .
        /// </summary>
        public bool IsBackendCommunicationSwitchToggled
        {
            get => isBackendCommunicationSwitchToggled;
            set
            {
                isBackendCommunicationSwitchToggled = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsShowPushDialogSwitchToggled
        /// <summary>
        /// Getter and Setter for the ShowPushDialog switch and preference.
        /// This preference is used, for deciding if the app should display a push-dialog, if a Session or MediaItem was created/edited in the backend.
        /// If this preference is false, only the ImportFromBackend-button will be enabled in the SessionsPage or ContentsPage (depending which of these pages are the current one).
        /// </summary>
        public bool IsShowPushDialogSwitchToggled
        {
            get => isShowPushDialogSwitchToggled;
            set
            {
                isShowPushDialogSwitchToggled = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsAdminsDeletePermissionSwitchToggled
        /// <summary>
        /// Getter and Setter for the IsAdminsDeletePermissionSwitchToggled switch and preference.
        /// This preference is used, for deciding if admins are allowed to delete Sessions and MediaItems/Tags over the backend.
        /// </summary>
        public bool IsAdminsDeletePermissionSwitchToggled
        {
            get => isAdminsDeletePermissionSwitchToggled;
            set
            {
                isAdminsDeletePermissionSwitchToggled = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region AppVersion
        public string AppVersion
        {
            get => appVersion;
            set
            {
                appVersion = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region MediaMetaDataList
        public List<MediaMetaData> MediaMetaDataList
        {
            get => mediaMetaDataList;
            set
            {
                mediaMetaDataList = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region SetPreferenceSwitches
        /// <summary>
        /// Sets the IsToggled-value of the preference-switches.
        /// </summary>
        public void SetPreferenceSwitches()
        {
            IsBackendCommunicationSwitchToggled = Preferences.Get("ConnectToBackend", false);
            IsAdminsDeletePermissionSwitchToggled = Preferences.Get("AllowAdminsDeletion", false);
            IsShowPushDialogSwitchToggled = Preferences.Get("ShowPushDialog", false);
        }
        #endregion

        #region SetAppVersion
        /// <summary>
        /// Sets the AppVersion by reading the current version of the VersionTracking-class.
        /// The Track-method of the VersionTracking-class must be called first.
        /// </summary>
        public void SetAppVersion()
        {
            VersionTracking.Track();
            AppVersion = $"Version {VersionTracking.CurrentVersion}";
        }
        #endregion

        #region LoadMediaMetaDataList
        /// <summary>
        /// Loads and processes the Bildernachweis.csv and builds with the data the MediaMetaData-list.
        /// </summary>
        /// <returns></returns>
        public async Task LoadMediaMetaDataList()
        {

            List<MediaMetaData> mediaMetaDatas = new List<MediaMetaData>();

            string csvAsString = await HttpRequest("https://imtt.hs-furtwangen.de/rememti-bildernachweise/Bildernachweis.csv");

            if (csvAsString.StartsWith("Error"))
            {
                return;
            }

            string[] csvRows = csvAsString.Split('\n');

            foreach (string row in csvRows)
            {
                //dateiname, autor, link
                string[] rowSplit = row.Split(';');
                MediaMetaData mediaMetaData = new MediaMetaData();

                for (int i = 0; i < rowSplit.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            mediaMetaData.Name = rowSplit[i];
                            break;
                        case 1:
                            mediaMetaData.Author = rowSplit[i];
                            break;
                        case 2:
                            mediaMetaData.Link = rowSplit[i];
                            break;
                        default:
                            break;
                    }
                }
                mediaMetaDatas.Add(mediaMetaData);
            }

            MediaMetaDataList = mediaMetaDatas;
        }
        #endregion

        #region HttpRequest
        /// <summary>
        /// Requests 'Bildernachweis.csv' over http.
        /// If the process failed, an error response message will be returned instead of the csv.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static async Task<string> HttpRequest(string path)
        {
            //Create an HTTP client object
            HttpClient httpClient = new HttpClient();

            //Add a user-agent header to the GET request. 
            HttpRequestHeaders headers = httpClient.DefaultRequestHeaders;

            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";

            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            Uri requestUri = new Uri(path);

            //Send the GET request asynchronously and retrieve the response as a string.
            string httpResponseBody;

            try
            {
                //Send the GET request
                HttpResponseMessage httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
            return httpResponseBody;
        }
        #endregion

    }
}
