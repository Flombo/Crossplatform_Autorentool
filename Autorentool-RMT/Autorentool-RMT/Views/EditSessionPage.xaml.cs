using Autorentool_RMT.Models;
using Autorentool_RMT.ViewModels;
using Autorentool_RMT.Views.Popups;
using System;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSessionPage : ContentPage, ITooltipProvider
    {
        private EditSessionViewModel editSessionViewModel;
        private Session selectedSession;
        private List<Tooltip> tooltips;

        public EditSessionPage(Session selectedSession)
        {
            InitializeComponent();
            this.selectedSession = selectedSession;
            NavigationPage.SetHasNavigationBar(this, false);
            editSessionViewModel = new EditSessionViewModel(selectedSession);
            BindingContext = editSessionViewModel;
        }

        #region OnAppearing
        /// <summary>
        /// Loads all mediaitems of the selected session.
        /// </summary>
        protected override async void OnAppearing()
        {
            GenerateTooltips();
            await editSessionViewModel.OnLoadAllSessionMediaItems();
            editSessionViewModel.SelectedMediaItem = null;
        }
        #endregion

        #region OnSelectionChanged
        /// <summary>
        /// Sets the SelectedMediaItem property of the EditSessionViewModel for further actions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MediaItem selectedMediaItem = e.CurrentSelection[0] as MediaItem;
            editSessionViewModel.SelectedMediaItem = selectedMediaItem;
        }
        #endregion

        #region OnAddMediaItemButtonClicked
        /// <summary>
        /// Navigates to the ContentsPage where mediaitems are selectable.
        /// The ContentsPage needs the selectedSession for further actions and to decide which ViewModel should be used.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnAddMediaItemButtonClicked(object sender, EventArgs e)
        {
            editSessionViewModel.SelectedMediaItem = null;
            await Navigation.PushAsync(new ContentsPage(selectedSession));
        }
        #endregion

        #region OnUnbindMediaItemButtonClicked
        /// <summary>
        /// Unbinds the current selected mediaitem from the selected session.
        /// If an error occurs, an error prompt will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnUnbindMediaItemButtonClicked(object sender, EventArgs e)
        {
            try
            {
                await editSessionViewModel.OnUnbindMediaItemFromSession();
            } catch(Exception)
            {
                await DisplayAlert(
                    $"Fehler beim Löschen des Bausteins {editSessionViewModel.SelectedMediaItem}",
                    $"Ein Fehler trat beim Löschen des Bausteins {editSessionViewModel.SelectedMediaItem} auf",
                    "Schließen");
            }
        }
        #endregion

        #region OnCompleteButtonClicked
        /// <summary>
        /// Navigates back to the SessionPage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCompleteButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        #endregion

        #region OnDragStart
        /// <summary>
        /// Selects the dragged mediaitem and sets the DraggedMediaItem property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDrag(DragGestureRecognizer sender, DragStartingEventArgs e)
        {
            MediaItem draggedMediaItem = sender.BindingContext as MediaItem;
            editSessionViewModel.DraggedMediaItem = draggedMediaItem;
        }
        #endregion

        #region OnDrop
        /// <summary>
        /// Selects the mediaitem, where the dragged mediaitem was dropped and changes the position-attribute of both.
        /// In the end the UI will be refreshed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnDrop(DragGestureRecognizer sender, DropEventArgs e)
        {
            MediaItem targetMediaItem = sender.BindingContext as MediaItem;
            await editSessionViewModel.ChangePosition(targetMediaItem);
        }
        #endregion

        #region OnTap
        /// <summary>
        /// Sets the SelectedMediaItem property of the EditSessionViewModel for further actions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTap(object sender, EventArgs e)
        {
            StackLayout stackLayout = sender as StackLayout;
            MediaItem selectedMediaItem = stackLayout.BindingContext as MediaItem;
            editSessionViewModel.SelectedMediaItem = selectedMediaItem;
        }
        #endregion

        #region OnStartSessionButtonClicked
        private async void OnStartSessionButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PlaySessionContentPage(selectedSession, null));
        }
        #endregion

        #region DisplayTooltip
        /// <summary>
        /// Displays TooltipPopup with the generated Tooltips.
        /// </summary>
        public void DisplayTooltip()
        {
            Navigation.ShowPopup(new TooltipPopup(tooltips));
        }
        #endregion

        #region GenerateTooltips
        /// <summary>
        /// Generates the Tooltips for adding and deleting MediaItems from the session, starting and editing sessions as well as a Tooltip for the CollectionView and adds them to the Tooltips-list
        /// </summary>
        public void GenerateTooltips()
        {
            ResourceDictionary resourceDictionary = Application.Current.Resources;

            Tooltip mediaItemListTooltip = new Tooltip()
            {
                Title = "Liste der Inhaltsbausteine",
                Description = "Hier sehen Sie alle Inhaltsbausteine der ausgewähten Sitzung und deren Reihenfolge. "
                              +"Die Reihenfolge können Sie per Drag and Drop (langes Daraufbleiben, verschieben und loslassen) ändern.",
                Icon = ""
            };

            Tooltip addMediaItemsTooltip = new Tooltip()
            {
                Title = "Inhaltsbausteine hinzufügen",
                Description = "Durch diesen Button können Sie verschiedene Inhaltsbausteine wie Bilder, Videos, Musik und Texte in die Sitzung hinzufügen.",
                Icon = resourceDictionary["AddIcon"].ToString()
            };

            Tooltip playSessionTooltip = new Tooltip()
            {
                Title = "Sitzungen abspielen",
                Description = "Durch diesen Button können Sie die ausgewählte Sitzung abpielen (und am Ende bewerten, falls Sie die Sitzung einem Bewohner zuordnen).",
                Icon = resourceDictionary["PlayIcon"].ToString()
            };

            Tooltip deleteMediaItemsFromSessionTooltip = new Tooltip()
            {
                Title = "Inhaltsbausteine aus der Sitzung entfernen",
                Description = "Durch diesen Button können Sie den ausgewählten Inhaltsbaustein aus der Sitzung entfernen.",
                Icon = resourceDictionary["DeleteIcon"].ToString()
            };

            tooltips = new List<Tooltip>()
            {
                mediaItemListTooltip,
                addMediaItemsTooltip,
                playSessionTooltip,
                deleteMediaItemsFromSessionTooltip
            };
        }
        #endregion

    }
}