using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SorterTCP
{
    public class Barcode
    {
        public int barcodeID { get; set; }
        public string PNC { get; set; }
        public int activeCnt{ get; set; }
        public int passiveCnt{ get; set; }
        public bool present { get; set; }

        public Barcode(ushort xbarcodeID, string xPNC, ushort xactiveCNt, ushort xpassiveCnt, bool xpres)
        {
            barcodeID = xbarcodeID;
            PNC = xPNC;
            activeCnt = xactiveCNt;
            passiveCnt = xpassiveCnt;
            present = xpres;
        }
    }
}
