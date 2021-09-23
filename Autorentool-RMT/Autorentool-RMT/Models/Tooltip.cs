using Xamarin.Forms;

namespace Autorentool_RMT.Models
{
    public class Tooltip : Model
    {
        #region Attributes
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public ImageSource ImageIcon { get; set; }
        #endregion

        #region Constructor with all attributes
        public Tooltip(string title, string description, string icon, string imageIcon)
        {
            Title = title;
            Description = description;
            Icon = icon;
            ImageIcon = imageIcon;
        }
        #endregion

        #region Empty constructor
        public Tooltip()
        {
        }
        #endregion

        #region HasImageSource
        public bool HasImageSource => Icon == null;
        #endregion

        #region HasIcon
        public bool HasIcon => Icon != null;
        #endregion

    }
}
