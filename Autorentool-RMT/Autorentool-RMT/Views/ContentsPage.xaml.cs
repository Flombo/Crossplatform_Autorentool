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

        private ContentViewModel viewModel;
        private Session selectedSession;

        #region Empty Constructor
        /// <summary>
        /// Constructor for the management of mediaitems.
        /// </summary>
        public ContentsPage()
        {
            InitializeComponent();
            selectedSession = null;
            NavigationPage.SetHasNavigationBar(this, false);
            viewModel = new ContentManagementViewModel();
            BindingContext = viewModel;
        }
        #endregion

        #region Constructor with the selected session parameter
        /// <summary>
        /// Constructor for the selection of mediaitems.
        /// Is used by EditSessionPage and takes the selected session for binding the selected mediaitems.
        /// </summary>
        /// <param name="selectedSession"></param>
        public ContentsPage(Session selectedSession)
        {
            InitializeComponent();
            this.selectedSession = selectedSession;
            NavigationPage.SetHasNavigationBar(this, false);
            viewModel = new SelectContentViewModel(selectedSession);
            BindingContext = viewModel;
        }
        #endregion

        #region OnAppearing
        /// <summary>
        /// Inits the ContentManagementViewModel or the SelectContentViewModel depending on the selectedSession-attribute.
        /// If it is null, the user was on the main menu before, else the user was on the EditSessionPage.
        /// </summary>
        protected override async void OnAppearing()
        {
            try
            {
                if (selectedSession == null)
                {
                    await viewModel.OnLoadAllMediaItems();
                } else
                {
                    SelectContentViewModel selectContentViewModel = viewModel as SelectContentViewModel;
                    selectContentViewModel.OnFilter();
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Fehler beim Laden der Medieninhalte", "Ein Fehler trat auf beim Laden der Medieninhalte", "Schließen");
            }
        }
        #endregion

        #region OnSelectionChanged
        /// <summary>
        /// Sets the SelectedMediaItem-property if triggered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MediaItem selectedMediaItem = e.CurrentSelection[0] as MediaItem;
            viewModel.SelectedMediaItem = selectedMediaItem;
        }
        #endregion

        #region OnFullscreenButtonClicked
        private async void OnFullscreenButtonClicked(object sender, EventArgs e)
        {
            MediaItem selectedMediaItem = viewModel.SelectedMediaItem;
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
                ContentManagementViewModel contentViewModel = viewModel as ContentManagementViewModel;
                await contentViewModel.OnDeleteMediaItem();
            } catch(Exception)
            {
                string selectedMediaItemName = viewModel.SelectedMediaItem.Name;

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
                LifethemePopup.Result result = await Navigation.ShowPopupAsync(new LifethemePopup(viewModel.CurrentMediaItemLifethemes));

                ContentManagementViewModel contentViewModel = viewModel as ContentManagementViewModel;

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
                            ContentManagementViewModel contentViewModel = viewModel as ContentManagementViewModel;

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
                ContentManagementViewModel contentViewModel = viewModel as ContentManagementViewModel;

                await contentViewModel.ShowFilePicker();

            } catch (Exception exc)
            {
                await DisplayAlert("Fehler beim Hinzufügen neuer Inhalte", exc.Message, "Schließen");
            }
        }
        #endregion

        #region OnAddMediaItemButtonClicked
        /// <summary>
        /// Binds all checked mediaitems to the selected session and Navigates back to the EditSessionPage.
        /// If an error occured, an error message will be prompted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnAddMediaItemButtonClicked(object sender, EventArgs e)
        {
            SelectContentViewModel selectContentViewModel = viewModel as SelectContentViewModel;

            try
            {
                await selectContentViewModel.BindCheckedMediaItemsToSession();
                await Navigation.PopAsync();
            } catch(Exception)
            {
                await DisplayAlert("Fehler beim Hinzufügen der Bausteine","Beim Hinzufügen der Bausteine kam es zu einem Fehler", "Schließen");
            }
        }
        #endregion

        #region OnMediaItemCheckboxChanged
        /// <summary>
        /// Adds or removes a mediaItem from the CheckedMediaItems-list if the checkbox is checked or unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMediaItemCheckboxChanged(object sender, CheckedChangedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            Grid grid = checkBox.Parent as Grid;
            MediaItem mediaItem = grid.BindingContext as MediaItem;

            SelectContentViewModel selectContentViewModel = viewModel as SelectContentViewModel;

            if (checkBox.IsChecked)
            {
                selectContentViewModel.AddMediaItemToCheckedMediaItems(mediaItem);
            } else
            {
                selectContentViewModel.RemoveMediaItemFromCheckedMediaItems(mediaItem);
            }
        }
        #endregion

    }
}