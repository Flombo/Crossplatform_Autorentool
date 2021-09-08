using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Autorentool_RMT.ViewModels
{
    public class SelectContentViewModel : ContentViewModel
    {

        public List<MediaItem> checkedMediaItems { get; set; }
        private Session selectedSession;
        public ICommand Search { get; }

        public SelectContentViewModel(Session selectedSession) : base()
        {
            Title = "BAUSTEINE HINZUFÜGEN";
            checkedMediaItems = new List<MediaItem>();
            IsContentPage = false;
            IsLifethemesButtonVisible = false;
            this.selectedSession = selectedSession;
            IsAddMediaItemButtonVisible = true;
            Search = new Command(OnSearch);
        }

        #region IsPhotosFilterChecked
        public bool IsPhotosFilterChecked
        {
            get => isPhotosFilterChecked;
            set
            {
                isPhotosFilterChecked = value;

                if (searchText.Length > 0)
                {
                    OnSearch();
                }
                else
                {
                    OnFilter();
                }

                OnPropertyChanged();
            }
        }
        #endregion

        #region IsLinksFilterChecked
        public bool IsLinksFilterChecked
        {
            get => isLinksFilterChecked;
            set
            {
                isLinksFilterChecked = value;

                if (searchText.Length > 0)
                {
                    OnSearch();
                }
                else
                {
                    OnFilter();
                }

                OnPropertyChanged();
            }
        }
        #endregion

        #region IsDocumentsFilterChecked
        public bool IsDocumentsFilterChecked
        {
            get => isDocumentsFilterChecked;
            set
            {
                isDocumentsFilterChecked = value;

                if (searchText.Length > 0)
                {
                    OnSearch();
                }
                else
                {
                    OnFilter();
                }

                OnPropertyChanged();
            }
        }
        #endregion

        #region IsMusicFilterChecked
        public bool IsMusicFilterChecked
        {
            get => isMusicFilterChecked;
            set
            {
                isMusicFilterChecked = value;

                if (searchText.Length > 0)
                {
                    OnSearch();
                }
                else
                {
                    OnFilter();
                }

                OnPropertyChanged();
            }
        }
        #endregion

        #region IsFilmsFilterChecked
        public bool IsFilmsFilterChecked
        {
            get => isFilmsFilterChecked;
            set
            {
                isFilmsFilterChecked = value;

                if (searchText.Length > 0)
                {
                    OnSearch();
                }
                else
                {
                    OnFilter();
                }

                OnPropertyChanged();
            }
        }
        #endregion

        #region AddMediaItemToCheckedMediaItems
        public void AddMediaItemToCheckedMediaItems(MediaItem mediaItem)
        {
            checkedMediaItems.Add(mediaItem);
            IsAddMediaItemButtonEnabled = true;
        }
        #endregion

        #region RemoveMediaItemFromCheckedMediaItems
        public void RemoveMediaItemFromCheckedMediaItems(MediaItem mediaItem)
        {
            checkedMediaItems.Remove(mediaItem);

            IsAddMediaItemButtonEnabled = checkedMediaItems.Count > 0;
        }
        #endregion

        #region BindCheckedMediaItemsToSession
        /// <summary>
        /// Binds every checked mediaitem to the selected session.
        /// If an error occurs an exception will be thrown to the code behind.
        /// </summary>
        public async Task BindCheckedMediaItemsToSession()
        {
            try
            {
                int position = 1;

                foreach (MediaItem checkedMediaItem in checkedMediaItems)
                {
                    await SessionMediaItemsDBHandler.BindSessionMediaItems(checkedMediaItem.Id, selectedSession.Id);
                    await MediaItemDBHandler.UpdatePosition(checkedMediaItem.Id, position);
                    position++;
                }
            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion


        #region OnSearch
        /// <summary>
        /// Searches MediaItems which contain the search string.
        /// If no search string was given, the MediaItems will be reloaded.
        /// </summary>
        public async void OnSearch()
        {
            SelectedMediaItem = null;
            CurrentMediaItemLifethemes = new List<Lifetheme>();

            if (searchText.Length > 0)
            {
                List<MediaItem> foundMediaItems = await MediaItemDBHandler.SearchMediaItems(
                    searchText,
                    IsPhotosFilterChecked,
                    IsMusicFilterChecked,
                    IsDocumentsFilterChecked,
                    IsFilmsFilterChecked,
                    IsLinksFilterChecked
                    );

                SetBindableMediaItems(foundMediaItems);
            }
            else
            {
                OnFilter();
            }
        }
        #endregion

        #region SetBindableMediaItems
        /// <summary>
        /// Removes all mediaItems that are already bound to the selected session.
        /// </summary>
        /// <param name="mediaItems"></param>
        /// <returns></returns>
        private async void SetBindableMediaItems(List<MediaItem> mediaItems)
        {
            List<MediaItem> sessionMediaItems = await SessionMediaItemsDBHandler.GetMediaItemsOfSession(selectedSession.Id);
            
            foreach (MediaItem sessionMediaItem in sessionMediaItems)
            {
                mediaItems.Remove(sessionMediaItem);
            }

            MediaItems = mediaItems;
        }
        #endregion

        #region OnFilter
        /// <summary>
        /// Filters MediaItems depending on, which filter is disabled/enabled.
        /// Removes all mediaItems that are already bound to the selected session.
        /// </summary>
        public async new void OnFilter()
        {
            SelectedMediaItem = null;
            CurrentMediaItemLifethemes = new List<Lifetheme>();

            List<MediaItem> filteredMediaItems = await MediaItemDBHandler.FilterMediaItems(
                IsPhotosFilterChecked,
                IsMusicFilterChecked,
                IsDocumentsFilterChecked,
                IsFilmsFilterChecked,
                IsLinksFilterChecked
                );

            SetBindableMediaItems(filteredMediaItems);
        }
        #endregion

    }
}
