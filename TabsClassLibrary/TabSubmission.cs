using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabsClassLibrary
{

    public class TabSubmission
    {
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string Instrument { get; set; } = string.Empty; // Instrument (Guitar, Bass)
        public string Difficulty { get; set; } = string.Empty;
        public List<TabLine> TabLines { get; set; } = new();
        public int UserID { get; set; }
    }
}
