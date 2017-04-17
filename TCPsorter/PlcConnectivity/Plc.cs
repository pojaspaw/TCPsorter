using S7.Net;
using S7NetWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using S7.Net.Types;

namespace SorterTCP.PlcConnectivity
{
    class Plc
    {
        #region Singleton

        // For implementation refer to: http://geekswithblogs.net/BlackRabbitCoder/archive/2010/05/19/c-system.lazylttgt-and-the-singleton-design-pattern.aspx        
        private static readonly Lazy<Plc> _instance = new Lazy<Plc>(() => new Plc());

        public static Plc Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion

        #region Public properties

        public ConnectionStates ConnectionState { get { return plcDriver != null ? plcDriver.ConnectionState : ConnectionStates.Offline; } }

        public DB10 Db10 { get; set; }

        public TimeSpan CycleReadTime { get; private set; }

        public TimeSpan timeOfRead { get; set; }



        #endregion

        #region Private fields

        IPlcSyncDriver plcDriver;

        System.Timers.Timer timer = new System.Timers.Timer();

        public DateTime lastReadTime;

        public List<byte> listOfBytesFromPLC = new List<byte>();

        public List<DataItem> listOfadditional = new List<DataItem>();

        public List<Barcode> listOfBarcodes = new List<Barcode>();

        public List<Barcode> listOfBarcodesVISU = new List<Barcode>();

        public List<Bay> listOfBays = new List<Bay>();

        public List<Bay> listOfBaysVISU = new List<Bay>();

        public Stopwatch Stoper = new Stopwatch();

        ASCIIEncoding enc = new ASCIIEncoding();

        

        public int interval_plc = 4000;


        #endregion

        #region Constructor

        private Plc()
        {
            //  Db10 = new DB10();
            timer.Interval = interval_plc; // ms
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;
            lastReadTime = DateTime.Now;
        }

        #endregion

        #region Event handlers

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (plcDriver == null || plcDriver.ConnectionState != ConnectionStates.Online)
            {
                return;
            }
            timer.Enabled = false;
            CycleReadTime = DateTime.Now - lastReadTime;
            try
            {
                Stoper.Reset();
                Stoper.Start();
                listOfBytesFromPLC = ReadMultiple();
                //listOfadditional = ReadTwoInts();
                ConvertingBytes(listOfBytesFromPLC);
                ConvertingBytesForVISU(listOfBarcodes, listOfBarcodesVISU);
                Stoper.Stop();
                timeOfRead = Stoper.Elapsed;
            }
            catch(Exception ex)
            {
                plcDriver.Disconnect();
         
            }
            finally
            {
                timer.Enabled = true;
                lastReadTime = DateTime.Now;
            }
        }

        #endregion

        #region Public methods

        public void Connect(string ipAddress)
        {
            if (!IsValidIp(ipAddress))
            {
                throw new ArgumentException("Ip address is not valid");
            }
            plcDriver = new S7NetPlcDriver(CpuType.S7300, ipAddress, 0, 2);
            plcDriver.Connect();
        }

        public void Disconnect()
        {
            if (plcDriver == null || this.ConnectionState == ConnectionStates.Offline)
            {
                return;
            }
            plcDriver.Disconnect();
        }

        public void Write(string name, object value)
        {
            if (plcDriver == null || plcDriver.ConnectionState != ConnectionStates.Online)
            {
                return;
            }
            Tag tag = new Tag(name, value);
            List<Tag> tagList = new List<Tag>();
            tagList.Add(tag);
            plcDriver.WriteItems(tagList);
        }

        public void Write(List<Tag> tags)
        {
            if (plcDriver == null || plcDriver.ConnectionState != ConnectionStates.Online)
            {
                return;
            }
            plcDriver.WriteItems(tags);
        }

        public List<byte> ReadMultiple()
        {
            if (plcDriver == null || plcDriver.ConnectionState != ConnectionStates.Online)
            {
                return null;
            }
            listOfBytesFromPLC = plcDriver.ReadMultipleBytes(1726, 4, 0);
            return listOfBytesFromPLC;
        }
        //public List<DataItem> ReadTwoInts()
        //{
        //    if (plcDriver == null || plcDriver.ConnectionState != ConnectionStates.Online)
        //    {
        //        return null;
        //    }
        //    listOfadditional = plcDriver.ReadAdditional(529, 525);
        //    return listOfadditional;
        //}

        public void InitializeInstancesOfList()
        {
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    listOfBarcodes.Add(new Barcode((ushort)(listOfBarcodes.Count + 1), "", 0, 0, false));
                }
                catch (Exception ex)
                {

                }

            }
            for (int i = 0; i < 12; i++)
            {
                try
                {
                    listOfBays.Add(new Bay((ushort)(listOfBarcodes.Count + 1), "", 0, 0, false, false, false, false, false, false));
                }
                catch (Exception ex)
                {

                }

            }
        }
        public void InitializeInstancesOfListVISU()
        {
            for (int i = 0; i < 100; i++)
            {
                listOfBarcodesVISU.Add(new Barcode((ushort)(listOfBarcodes.Count + 1), "", 0, 0, false));
            }
            for (int i = 0; i < 12; i++)
            {
                listOfBaysVISU.Add(new Bay((ushort)(listOfBarcodes.Count + 1), "", 0, 0, false, false, false, false, false, false));
            }
        }

        public void ConvertingBytes(List<byte> bytelist)
        {
            string _pnc = "";
            ushort _act = 0;
            ushort _pas = 0;
            bool _pre = false;
            bool _ena = false;
            bool _emp = false;
            bool _ful = false;
            bool _res = false;
            bool _ano = false;
            bool _aut = false;
            byte sta = 0;
            ushort k = 0;
            ushort m = 0;
            #region do wyswietlania kodów
            for (int i = 0; i < (bytelist.Count - 142); i = i + 16)
            {
                _pnc = enc.GetString(bytelist.GetRange(i, 9).ToArray());
                _act = BitConverter.ToUInt16(swapArray(0, 1, bytelist.GetRange(i + 10, 2).ToArray()), 0);
                _pas = BitConverter.ToUInt16(swapArray(0, 1, bytelist.GetRange(i + 12, 2).ToArray()), 0);
                _pre = Convert.ToBoolean(bytelist.ElementAt(i + 14));

                listOfBarcodes[k].PNC = _pnc;
                listOfBarcodes[k].activeCnt = _act;
                listOfBarcodes[k].passiveCnt = _pas;
                listOfBarcodes[k].present = _pre;
                k += 1;
            }
            #endregion
            #region do wyswietlania zatok
            for (int i = bytelist.Count - 142; i < (bytelist.Count); i = i + 12)
            {

                sta = bytelist.ElementAt(i);
                _ena = (sta & (1 << 0)) != 0;
                _emp = (sta & (1 << 1)) != 0;
                _ful = (sta & (1 << 2)) != 0;
                _res = (sta & (1 << 3)) != 0;
                _ano = (sta & (1 << 4)) != 0;
                _aut = (sta & (1 << 5)) != 0;
                _pnc = enc.GetString(bytelist.GetRange(i + 1, 9).ToArray());
                listOfBays[m].PNC = _pnc;
                listOfBays[m].enable = _ena;
                listOfBays[m].empty = _emp;
                listOfBays[m].full = _ful;
                listOfBays[m].reserved = _res;
                listOfBays[m].anomaly = _ano;
                listOfBays[m].automatic = _aut;

                for (int j = 0; j <= listOfBarcodes.Count; j++)
                {
                    if (j == 100)
                    {
                        listOfBays[m].activeCnt = -1;
                        listOfBays[m].passiveCnt = -1;
                        break;
                    }
                    if (listOfBarcodes[j].PNC == listOfBays[m].PNC)
                    {
                        listOfBays[m].activeCnt = listOfBarcodes[j].activeCnt;
                        listOfBays[m].passiveCnt = listOfBarcodes[j].passiveCnt;
                        break;
                    }
                }
                m += 1;
            }
            #endregion
        }

        public void ConvertingBytesForVISU(List<Barcode> listBarcode, List<Barcode> listVisu)
        {
            listVisu.Clear();
            for (int i = 0; i < listBarcode.Count; i++)
            {
                if (listBarcode[i].present)
                {
                    listVisu.Add(listBarcode[i]);
                }
            }
            //listOfBaysVISU = listOfBays;
        }
        #endregion        

        #region Private methods

        private bool IsValidIp(string addr)
        {
            IPAddress ip;
            bool valid = !string.IsNullOrEmpty(addr) && IPAddress.TryParse(addr, out ip);
            return valid;
        }

        private void RefreshTags()
        {
            //plcDriver.ReadClass(Db10, 1);
            //plcDriver.w


        }

        private byte[] swapArray(int a, int b, byte[] lis)
        {
            byte tmp = lis[a];
            lis[a] = lis[b];
            lis[b] = tmp;
            return lis;
        }
        #endregion


    }
}
