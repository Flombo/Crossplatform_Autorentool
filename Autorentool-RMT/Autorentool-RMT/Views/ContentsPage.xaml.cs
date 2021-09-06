﻿using Autorentool_RMT.Models;
using Autorentool_RMT.ViewModels;
using Autorentool_RMT.Views.Popups;
using System;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContentsPage : ContentPage
    {

        private ContentViewModel contentViewModel;
        public ContentsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            contentViewModel = new ContentViewModel();
            BindingContext = contentViewModel;
        }

        protected override async void OnAppearing()
        {
            try
            {
                await contentViewModel.OnLoadAllMediaItems();
            }
            catch (Exception)
            {
                await DisplayAlert("Fehler beim Laden der Medieninhalte", "Ein Fehler trat auf beim Laden der Medieninhalte", "Schließen");
            }
        }

        #region OnSelectionChanged
        /// <summary>
        /// Sets the SelectedMediaItem-property if triggered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MediaItem selectedMediaItem = e.CurrentSelection[0] as MediaItem;
            contentViewModel.SelectedMediaItem = selectedMediaItem;
        }
        #endregion

        #region OnFullscreenButtonClicked
        private async void OnFullscreenButtonClicked(object sender, EventArgs e)
        {
            MediaItem selectedMediaItem = contentViewModel.SelectedMediaItem;
            MediaItemFullscreenPopup mediaItemFullscreenPopup = new MediaItemFullscreenPopup(selectedMediaItem, Height, Width);

            await Navigation.ShowPopupAsync(mediaItemFullscreenPopup);
        }
        #endregion

        #region OnCompleteButtonClicked
        private async void OnCompleteButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        #endregion

        #region OnDeleteSelectedMediaItemButtonClicked
        /// <summary>
        /// Deletes selected mediaitem.
        /// If an error occurs, an error message will be prompted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnDeleteSelectedMediaItemButtonClicked(object sender, EventArgs e)
        {
            try
            {
                await contentViewModel.OnDeleteMediaItem();
            } catch(Exception)
            {
                string selectedMediaItemName = contentViewModel.SelectedMediaItem.Name;

                await DisplayAlert("Fehler beim Löschen des Mediums: " + selectedMediaItemName, "Ein Fehler trat auf beim Löschen des Mediums " + selectedMediaItemName, "Schließen");
            }
        }
        #endregion

        #region OnLifethemesButtonClicked
        /// <summary>
        /// Opens LifethemePopup and binds the selected lifethemes with the selected mediaitem.
        /// If an error occurs an error prompt will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnLifethemesButtonClicked(object sender, EventArgs e)
        {
            try
            {
                LifethemePopup.Result result = await Navigation.ShowPopupAsync(new LifethemePopup(contentViewModel.CurrentMediaItemLifethemes));

                await contentViewModel.SetCurrentMediaItemLifethemes(result.selectedLifethemes);
            }
            catch (Exception)
            {
                await DisplayAlert("Fehler beim Auswählen von Lebensthemen", "Beim Auswählen der Lebensthemen kam es zu einem Fehler", "Schließen");
            }
        }
        #endregion

        #region OnDeleteAllMediaItemsButtonClicked
        private async void OnDeleteAllMediaItemsButtonClicked(object sender, EventArgs e)
        {
            try
            {
                bool shouldDeleteAllMediaItems = await DisplayAlert(
                    "Alle Bausteine löschen?",
                    "Sind Sie sicher, dass Sie alle Inhaltsbausteine endgültig löschen wollen? (Dabei werden auch alle mit diesen Bausteinen verknüpften Sitzungen geleert.)",
                    "Alles klar!",
                    "Abbrechen"
                    );

                while (shouldDeleteAllMediaItems)
                {
                    PasswordPopup.Result result = await Navigation.ShowPopupAsync(new PasswordPopup());

                    if(result != null)
                    {
                        if (result.IsPasswordValid)
                        {
                            await contentViewModel.OnDeleteAllMediaItems();
                            shouldDeleteAllMediaItems = false;
                        } else
                        {
                            shouldDeleteAllMediaItems = await DisplayAlert("Falsches Passwort!", "Das eingegebenen Passwort " + result.InsertedPassword + " ist leider falsch.", "Alles klar!", "Abbrechen");
                        }
                    } else
                    {
                        shouldDeleteAllMediaItems = false;
                    }
                    
                }
            } catch(Exception)
            {
                await DisplayAlert("Fehler beim Löschen der Medien", "Ein Fehler trat auf beim Löschen der Medien", "Schließen");
            }
        }
        #endregion

        #region OnImportButtonClicked
        /// <summary>
        /// Shows file picker, saves selected files and displays them.
        /// If an error occured an error prompt will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnImportButtonClicked(object sender, EventArgs e)
        {
            try
            {
                contentViewModel.ShowFilePicker();
            } catch(Exception)
            {
                await DisplayAlert("Fehler beim Hinzufügen neuer Inhalte", "Beim Hinzufügen neuer Inhalte trat ein Fehler auf", "Schließen");
            }
        }
        #endregion

    }
}