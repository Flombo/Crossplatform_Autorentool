using Autorentool_RMT.Models;
using Autorentool_RMT.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSessionPage : ContentPage
    {
        private EditSessionViewModel editSessionViewModel;
        private Session selectedSession;

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
    }
}