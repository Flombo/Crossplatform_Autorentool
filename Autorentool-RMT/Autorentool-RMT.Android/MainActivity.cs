using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Views;
using Android.Content;
using System.IO;
using Autorentool_RMT.Services;
using Autorentool_RMT.Services.DBHandling;
using Android.Provider;
using static Android.Content.ClipData;
using Autorentool_RMT.Views;
using System.Threading.Tasks;

namespace Autorentool_RMT.Droid
{
    /// <summary>
    /// Forced screen orientation to landscape via ScreenOrientation = ScreenOrientation.Landscape.
    /// IntentFilters for reacting to sent file(-s).
    /// </summary>
    [Activity(Label = "Autorentool_RMT", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Landscape)]
    [IntentFilter(new[] { Intent.ActionSendMultiple }, Categories = new[] { Intent.CategoryDefault }, DataMimeTypes = new[] { "image/jpeg", "image/png", "audio/mp3", "audio/mpeg", "video/mp4", "text/*", "text/html" })]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeTypes = new[] { "image/jpeg", "image/png", "audio/mp3", "audio/mpeg", "video/mp4", "text/*", "text/html" })]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            // ab: Hide status bar (here programatically, in iOS via Info.plist flags)
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            if (Intent.Action.Equals(Intent.ActionSendMultiple) || Intent.Action.Equals(Intent.ActionSend))
            {
                SaveRecievedFiles();
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #region SaveRecievedFiles
        /// <summary>
        /// Displays ContentsPage, saves all shared files, created MediaItems and displays them.
        /// If an error occurs, an error prompt will be displayed.
        /// </summary>
        private async void SaveRecievedFiles()
        {
            ContentsPage contentsPage = new ContentsPage();

            try
            {
                await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(contentsPage);

                for (int i = 0; i < Intent.ClipData.ItemCount; i++)
                {
                    // Get the info from ClipData 
                    using Item selectedFile = Intent.ClipData.GetItemAt(i);

                    // Open a stream from the URI 
                    using Stream stream = ContentResolver.OpenInputStream(selectedFile.Uri);

                    string hash = FileHandler.GetFileHashAsString(stream);

                    int duplicate = await MediaItemDBHandler.CountMediaItemDuplicates(hash);

                    if (duplicate == 0)
                    {
                        await SaveFileAndCreateMediaItem(selectedFile, hash);
                    }
                }

                await contentsPage.LoadAllMediaItems();
                await contentsPage.DisplayAlert("Erfolgreiches Hinzufügen der geteilten Inhalte", "Das Hinzufügen der geteilten Medien war erfolgreich", "Alles klar!");
            }
            catch (Exception)
            {
                await contentsPage.DisplayAlert("Fehler beim Hinzufügen der geteilten Medien", "Beim Hinzufügen der geteilten Medien kam es zu einem Fehler", "Schließen");
            }
        }
        #endregion

        #region SaveFileAndCreateMediaItem
        /// <summary>
        /// Saves file, thumbnail and creates a new MediaItem.
        /// Throws an exception if an error occurs.
        /// </summary>
        /// <param name="selectedFile"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        private async Task SaveFileAndCreateMediaItem(Item selectedFile, string hash)
        {
            try
            {
                string directoryPath = FileHandler.CreateDirectory("MediaItems");

                string filename = GetFilename(selectedFile);
                filename = FileHandler.GetUniqueFilename(filename, directoryPath);

                string filetype = FileHandler.ExtractFiletypeFromPath(filename);

                string filepath = Path.Combine(directoryPath, filename);
                string thumbnailPath = "";

                using Stream saveFileStream = ContentResolver.OpenInputStream(selectedFile.Uri);
                FileHandler.SaveFile(saveFileStream, filepath);

                if (filetype.Contains("jpg") || filetype.Contains("jpeg") || filetype.Contains("png"))
                {
                    thumbnailPath = FileHandler.CreateThumbnailAndReturnThumbnailPath(filename, filepath, 10);
                }

                await MediaItemDBHandler.AddMediaItem(filename, filepath, thumbnailPath, filetype, hash, "", 0);

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region GetFilename
        /// <summary>
        /// Extracts the filename of the given ClipData.Item.
        /// If an exception occurs, an error prompt will be displayed.
        /// </summary>
        /// <param name="selectedFile"></param>
        /// <returns></returns>
        private string GetFilename(Item selectedFile)
        {
            try
            {
                var returnCursor = ContentResolver.Query(selectedFile.Uri, null, null, null, null);
                int nameIndex = returnCursor.GetColumnIndex(IOpenableColumns.DisplayName);

                returnCursor.MoveToFirst();

                return returnCursor.GetString(nameIndex);

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

    }
}