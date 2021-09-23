using System.Collections.Generic;

namespace Autorentool_RMT.Models
{
    /// <summary>
    /// Model that displays a webSocketMessage
    /// </summary>
    public class WebSocketMessage
    {
        public string SerialNumber { get; set; }

        public List<int> AppMediaItemIDs { get; set; }

        public List<int> AppSessionIDs { get; set; }

        public List<string> LifethemeNames { get; set; }

        public bool ShouldPullContent { get; set; }

        public bool ShouldPullSessions { get; set; }
    }
}