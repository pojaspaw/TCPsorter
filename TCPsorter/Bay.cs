using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SorterTCP
{
    public class Bay
    {
        public int bayID { get; set; }
        public string PNC { get; set; }
        public int activeCnt { get; set; }
        public int passiveCnt { get; set; }
        public bool enable { get; set; }
        public bool empty { get; set; }
        public bool full { get; set; }
        public bool reserved { get; set; }
        public bool anomaly { get; set; }
        public bool automatic { get; set; }

        public Bay(ushort xbayID, string xPNC, ushort xactiveCNt, ushort xpassiveCnt, bool xenable, bool xempty, bool xfull, bool xreserved, bool xanomaly, bool xautomatic)
        {
            bayID = xbayID;
            PNC = xPNC;
            activeCnt = xactiveCNt;
            passiveCnt = xpassiveCnt;
            enable = xenable;
            empty = xempty;
            full = xfull;
            reserved = xreserved;
            anomaly = xanomaly;
            automatic = xautomatic;
        }
    }
}
