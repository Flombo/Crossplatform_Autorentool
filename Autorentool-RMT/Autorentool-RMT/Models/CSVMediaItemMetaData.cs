using System.Collections.Generic;

namespace Autorentool_RMT.Models
{
    public class CSVMediaItemMetaData
    {

        public string FileName { get; set; }
        public string Notes { get; set; }
        public List<Lifetheme> Lifethemes { get; set; }

    }
}
