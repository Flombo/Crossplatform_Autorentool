namespace Autorentool_RMT.Models
{
    /// <summary>
    /// Model for the Shoudl
    /// </summary>
    class HttpMessage
    {
        public bool ShouldDownload { get; set; }

        public string CSRFToken { get; set; }
    
    }
}
