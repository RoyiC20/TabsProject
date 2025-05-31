using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabsClassLibrary
{
    public class TabLine
    {
        public int? E { get; set; }
        public int? A { get; set; }
        public int? D { get; set; }
        public int? G { get; set; }
        public int? B { get; set; } // מיתר שני
        public int? E2 { get; set; } // מיתר ראשון (הדק ביותר)
    }
}
