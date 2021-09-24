using Autorentool_RMT.Models;
using Autorentool_RMT.Services.DBHandling.ReferenceTablesDBHandler;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autorentool_RMT.ViewModels.PopupViewModels
{
    public class SessionRatingPopupViewModel : ViewModel
    {

        private string selectedRatingEmoji = "★";
        private string unselectedRatingEmoji = "✩";
        private List<Star> ratingStars;
        private Resident resident;
        private Session session;

        public SessionRatingPopupViewModel(Resident resident, Session session)
        {
            this.resident = resident;
            this.session = session;
            InitRatingStars();
        }

        #region RatringStars
        public List<Star> RatingStars
        {
            get => ratingStars;
            set
            {
                ratingStars = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region InitRatingStars
        /// <summary>
        /// Inits the RatingStars-List.
        /// </summary>
        private void InitRatingStars()
        {
            List<Star> stars = new List<Star>();

            for(int i = 0; i < 5; i++)
            {
                Star star = new Star()
                {
                    Position = i,
                    Selected = false,
                    Icon = unselectedRatingEmoji
                };

                stars.Add(star);
            }

            RatingStars = stars;
        }
        #endregion

        #region OnRatingChanged
        /// <summary>
        /// Sets the Icon and Selected properties of selected and unselected stars.
        /// </summary>
        /// <param name="position"></param>
        public void OnRatingChanged(int position)
        {
            List<Star> ratingStarsCopy = new List<Star>();
            ratingStarsCopy.AddRange(RatingStars);

            for(int i = 0; i <= position; i++)
            {
                ratingStarsCopy[i].Icon = selectedRatingEmoji;
                ratingStarsCopy[i].Selected = true;
            }

            for(int j = 4; j > position; j--)
            {
                ratingStarsCopy[j].Icon = unselectedRatingEmoji;
                ratingStarsCopy[j].Selected = false;
            }

            RatingStars = ratingStarsCopy;
        }
        #endregion

        #region PersistRating
        /// <summary>
        /// Calculates the rating value and persists it in db.
        /// Throws an exception if the process failed.
        /// </summary>
        /// <returns></returns>
        public async Task PersistRating()
        {
            try
            {
                int ratingValue = 0;

                foreach(Star star in ratingStars)
                {
                    if (star.Selected)
                    {
                        ratingValue++;
                    }
                }

                Rating rating = await RatingDBHandler.GetRatingOfSessionAndResident(session.Id, resident.Id);

                if(rating == null)
                {
                    await RatingDBHandler.AddRating(session.Id, resident.Id, session.DurationInSeconds, ratingValue);
                }
                else
                {
                    await RatingDBHandler.UpdateRating(rating.Id, session.DurationInSeconds, ratingValue);
                }

            } catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region Star
        /// <summary>
        /// Star-Model for the rating popup.
        /// </summary>
        public class Star
        {
            public int Position { get; set; }
            public bool Selected { get; set; }
            public string Icon { get; set; }
        }
        #endregion

    }
}
