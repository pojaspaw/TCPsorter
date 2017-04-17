using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SorterTCP.PlcConnectivity;
using System.Threading;
using System.Diagnostics;



namespace SorterTCP
{
    public partial class MainApp : Form
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timerr = new System.Windows.Forms.Timer();
        DateTime processStart = new DateTime();
        public static int a = 0;
        public static List<byte> listabajtow = new List<byte>();
        private int heightOfScreen = Screen.PrimaryScreen.WorkingArea.Height;
        private int widthOfScreen = Screen.PrimaryScreen.WorkingArea.Width;
        public Label lblpnc;
        public Label lblact;
        public Label lblpas;
        public Label lblemp;
        public Label lblful;
        public Label lblres;
        public Label lblcod;
        public GroupBox gpbox;
        int sizefontPNC = 34;//35;//42;
        int sizefontrest = 21;//30;
        int sizefontbay = 25;//;//40;
        int sizefontcodes = 23;
        int timetrytoConnect = 10;
        int timeToChangeScreen = 20;
        int numberOfcodesInOneScreen = 30;
        int sizeTOPinBay_pnc = 40;
        int sizeTOPinBay_act = 90;
        int sizeTOPinBay_pas = 130;
        int sizeTOPinBay_emp = 170;
        int sizeTOPinBay_ful = 210;
        int sizeTOPinBay_res = 250;
        int sizewidthbay = 430;
        int sizeheightbay = 288;
        string IPsorter = "10.26.39.7";
        int interval_mainapp = 1000;
        ulong secondFromStart = 0;
        public MainApp()
        {
            Thread th = new Thread(new ThreadStart(startSplashScreen));
            th.Start();
            Thread.Sleep(5000);
            InitializeComponent();
            tbPage_BAYS.Size = new Size(widthOfScreen, heightOfScreen);
            tbPage_PNC.Size = new Size(widthOfScreen, heightOfScreen);
            tbPage_PNC_2.Size = new Size(widthOfScreen, heightOfScreen);

            initializeBaysScreen();
            initializeCodesScreen();
            Plc.Instance.InitializeInstancesOfList();
            Plc.Instance.InitializeInstancesOfListVISU();
            initializeHeader();
            th.Abort();

            processStart = Process.GetCurrentProcess().StartTime;

            this.ShowInTaskbar = true;
            timer.Interval = interval_mainapp;
            timer.Tick += new EventHandler(tick_Timer);
            timer.Enabled = true;
        }

        public void startSplashScreen()
        {
            try
            {

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            Application.Run(new SplashScreen());
        }
        public void startConnectionError()
        {
            try
            {

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            Application.Run(new ErrorConnection());
        }

        private void tick_Timer(object sender, EventArgs e)
        {
            secondFromStart += 1;
            if (a > (timeToChangeScreen * tbCtrl.TabPages.Count)) { a = 1; } else { a += 1; }
            label3.Text = heightOfScreen.ToString();
            label4.Text = widthOfScreen.ToString();
            lbl_xBay1_PNC.Text = a.ToString();
            label1.Text = Plc.Instance.timeOfRead.ToString();
            state_con.Text = Plc.Instance.ConnectionState.ToString();
            state_con.ForeColor = (Plc.Instance.ConnectionState == S7NetWrapper.ConnectionStates.Offline) ? Color.OrangeRed : Color.ForestGreen;
            label2.Text = Plc.Instance.listOfBarcodesVISU.Count.ToString();
            time_now.Text = DateTime.Now.ToString();

            if (Plc.Instance.ConnectionState == S7NetWrapper.ConnectionStates.Online)
            {
                min_sort.Text = "Minimalna ilość do sortowania: "+ ((Plc.Instance.listOfBarcodes[98] != null )?Plc.Instance.listOfBarcodes[98].activeCnt.ToString():"x"); // aby nie przerabiac kodu programu, uzylem kodu 99 aby wyswietlic min_sort oraz max_cnt
                max_pas.Text = "Maksymalna ilość do usunięcia z bazy(pasywnie): " + ((Plc.Instance.listOfBarcodes[98] != null) ? Plc.Instance.listOfBarcodes[98].passiveCnt.ToString() : "x");
            }

            #region connecting to PLC
            if (a % timetrytoConnect == (timetrytoConnect - 1) && Plc.Instance.ConnectionState == S7NetWrapper.ConnectionStates.Offline)
            {
                try
                {
                    Plc.Instance.Connect(IPsorter);
                }
                catch (Exception ex)
                {
                    ErrorConnection f = new ErrorConnection();
                    f.Show();
                    f.Refresh();
                    System.Threading.Thread.Sleep(3000);
                    f.Close();
                }
            }
            #endregion

            #region update texts of labels in BAYS
            for (int i = 0; i < 12; i++)
            {

                gpbox = this.Controls.Find("gpb_bay" + (i + 1).ToString(), true).FirstOrDefault() as GroupBox;
                if (gpbox != null)
                {
                    if(Plc.Instance.listOfBays[i].automatic != true)
                    {
                        //gpbox.ForeColor = Color.Gray;
                        gpbox.Text = "ZAT " + (i + 1).ToString()+" RĘCZNIE";
                        gpbox.BackColor = Color.LightGray;
                    }
                    else
                    {
                        gpbox.Text = "ZATOKA " + (i+1).ToString();
                        gpbox.BackColor = Color.White;
                    }
                    
                }
                lblpnc = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_PNC", true).FirstOrDefault() as Label;
                if (lblpnc != null)
                {
                    lblpnc.Text = "PNC: " + Plc.Instance.listOfBays[i].PNC.ToString();
                }
                lblact = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_active", true).FirstOrDefault() as Label;
                if (lblact != null)
                {
                    lblact.Text = "Ilość: " + ((Plc.Instance.listOfBays[i].activeCnt==-1)? "Brak PNC w bazie" : Plc.Instance.listOfBays[i].activeCnt.ToString());
                }
                lblpas = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_passive", true).FirstOrDefault() as Label;
                if (lblpas != null)
                {
                    lblpas.Text = "Pasywnie: " + ((Plc.Instance.listOfBays[i].passiveCnt == -1) ? "Brak PNC w bazie" : Plc.Instance.listOfBays[i].passiveCnt.ToString());
                }
                lblemp = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_empty", true).FirstOrDefault() as Label;
                if (lblemp != null)
                {
                    lblemp.Text = "Pusta: " + (Plc.Instance.listOfBays[i].empty != false ? "Tak" : "Nie");
                }
                lblful = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_full", true).FirstOrDefault() as Label;
                if (lblful != null)
                {
                    lblful.Text = "Pełna: " + (Plc.Instance.listOfBays[i].full != false ? "Tak" : "Nie");
                }
                lblres = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_reserved", true).FirstOrDefault() as Label;
                if (lblres != null)
                {
                    lblres.Text = "Zarezerwowana: " + (Plc.Instance.listOfBays[i].reserved != false ? "Tak" : "Nie");
                }
            }
            #endregion

            #region change size of labels in CODES

            for (int i = 0; i < Plc.Instance.listOfBarcodesVISU.Count; i++)
            {
                if (i < 10)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).FirstOrDefault() as Label;
                    if (lblcod != null)
                    {
                        if (Plc.Instance.listOfBarcodesVISU[i] != null)
                        {
                            lblcod.Visible = true;
                            lblcod.Text ="PNC: " + Plc.Instance.listOfBarcodesVISU[i].PNC + "   Ilość: " + Plc.Instance.listOfBarcodesVISU[i].activeCnt + "   Pasywnie: " + Plc.Instance.listOfBarcodesVISU[i].passiveCnt;
                        }
                    }
                }
                else if (i < 20)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).FirstOrDefault() as Label;
                    if (lblcod != null)
                    {
                        if (Plc.Instance.listOfBarcodesVISU[i] != null)
                        {
                            lblcod.Visible = true;
                            lblcod.Text ="PNC: " + Plc.Instance.listOfBarcodesVISU[i].PNC + "   Ilość: " + Plc.Instance.listOfBarcodesVISU[i].activeCnt + "   Pasywnie: " + Plc.Instance.listOfBarcodesVISU[i].passiveCnt;
                        }
                    }
                }
                else if (i<30)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).FirstOrDefault() as Label;
                    if (lblcod != null)
                    {
                        if (Plc.Instance.listOfBarcodesVISU[i] != null)
                        {
                            lblcod.Visible = true;
                            lblcod.Text = "PNC: " + Plc.Instance.listOfBarcodesVISU[i].PNC + "   Ilość: " + Plc.Instance.listOfBarcodesVISU[i].activeCnt + "   Pasywnie: " + Plc.Instance.listOfBarcodesVISU[i].passiveCnt;
                        }
                    }
                }
                else if (i < 40)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).FirstOrDefault() as Label;
                    if (lblcod != null)
                    {
                        if (Plc.Instance.listOfBarcodesVISU[i] != null)
                        {
                            lblcod.Visible = true;
                            lblcod.Text = "PNC: " + Plc.Instance.listOfBarcodesVISU[i].PNC + "   Ilość: " + Plc.Instance.listOfBarcodesVISU[i].activeCnt + "   Pasywnie: " + Plc.Instance.listOfBarcodesVISU[i].passiveCnt;
                        }
                    }
                }
                else if (i < 50)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).FirstOrDefault() as Label;
                    if (lblcod != null)
                    {
                        if (Plc.Instance.listOfBarcodesVISU[i] != null)
                        {
                            lblcod.Visible = true;
                            lblcod.Text = "PNC: " + Plc.Instance.listOfBarcodesVISU[i].PNC + "   Ilość: " + Plc.Instance.listOfBarcodesVISU[i].activeCnt + "   Pasywnie: " + Plc.Instance.listOfBarcodesVISU[i].passiveCnt;
                        }
                    }
                }
                else if (i < 60)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).FirstOrDefault() as Label;
                    if (lblcod != null)
                    {
                        if (Plc.Instance.listOfBarcodesVISU[i] != null)
                        {
                            lblcod.Visible = true;
                            lblcod.Text = "PNC: " + Plc.Instance.listOfBarcodesVISU[i].PNC + "   Ilość: " + Plc.Instance.listOfBarcodesVISU[i].activeCnt + "   Pasywnie: " + Plc.Instance.listOfBarcodesVISU[i].passiveCnt;
                        }
                    }
                }
            }
            ////////////////////////////////////////////////////////////////////  ukrywanie jeśli nie ma
            for (int i = 0; i < 60; i++)
            {
                if (i < 10)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).FirstOrDefault() as Label;
                    if (lblcod != null)
                    {
                        if (i > Plc.Instance.listOfBarcodesVISU.Count-1)
                        {
                            lblcod.Visible = false;
                        }
                    }
                }
                else if (i < 20)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).FirstOrDefault() as Label;
                    if (lblcod != null)
                    {
                        if (i > Plc.Instance.listOfBarcodesVISU.Count-1)
                        {
                            lblcod.Visible = false;
                        }
                    }
                }
                else if (i<30)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).FirstOrDefault() as Label;
                    if (lblcod != null)
                    {
                        if (i > Plc.Instance.listOfBarcodesVISU.Count-1)
                        {
                            lblcod.Visible = false;
                        }
                    }
                }
                else if (i < 40)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).FirstOrDefault() as Label;
                    if (lblcod != null)
                    {
                        if (i > Plc.Instance.listOfBarcodesVISU.Count-1)
                        {
                            lblcod.Visible = false;
                        }
                    }
                }
                else if (i < 50)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).FirstOrDefault() as Label;
                    if (lblcod != null)
                    {
                        if (i > Plc.Instance.listOfBarcodesVISU.Count-1)
                        {
                            lblcod.Visible = false;
                        }
                    }
                }
                else if (i < 60)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).FirstOrDefault() as Label;
                    if (lblcod != null)
                    {
                        if (i > Plc.Instance.listOfBarcodesVISU.Count-1)
                        {
                            lblcod.Visible = false;
                        }
                    }
                }
            }
            #endregion

            #region changing tabs
            if ((a % (timeToChangeScreen * tbCtrl.TabPages.Count)) < timeToChangeScreen)
            {
                tbCtrl.SelectedIndex = 0;
            }
            else if ((a % (timeToChangeScreen * tbCtrl.TabPages.Count)) < timeToChangeScreen * 2)
            {
                tbCtrl.SelectedIndex = 1;
            }
            else
            {
                if (Plc.Instance.listOfBarcodesVISU.Count > numberOfcodesInOneScreen)
                {
                    tbCtrl.SelectedIndex = 2;
                }
                else
                {
                    a = 0;
                }

            }
            #endregion

        }

        private void lbl_Minimize_Enter(object sender, EventArgs e)
        {
            lbl_Minimize.BackColor = Color.AliceBlue;
        }

        private void lbl_Close_MouseClick_1(object sender, MouseEventArgs e)
        {
            try
            {
                Plc.Instance.Disconnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.Close();
        }

        private void lbl_Minimize_MouseClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Plc.Instance.Connect("10.26.39.7");
                //Plc.Instance.Connect("192.168.56.1");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " - sie zjebalo i nie wiem czemu");
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                Plc.Instance.Disconnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " - sie zjebalo i nie wiem czemu");
            }
        }

        public void initializeHeader()
        {
      //      time_now.Location = new Point(((tbPage_BAYS.Width / 4) * 3), 20);//widthOfScreen-pnl_Header_Right.Width-time_now.Width,20);
        }
        public void initializeBaysScreen()
        {
            tbCtrl.SelectTab(0);

            #region minimum to sort
            min_sort.Font = new Font("Electrolux Sans SemiBold", sizefontrest);
            min_sort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            min_sort.AutoSize = true;
            min_sort.Location = new Point(40,0);
            min_sort.Text = "Minimalna ilość do sortowania: x";
            #endregion

            #region maximum passive
            max_pas.Font = new Font("Electrolux Sans SemiBold", sizefontrest);
            max_pas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            max_pas.AutoSize = true;
            max_pas.Location = new Point((widthOfScreen/2)+40, 0);
            max_pas.Text = "Maksymalna ilość do usunięcia z bazy(pasywnie): x";
            #endregion

            #region initialize bays

            for (int i = 0; i < 12; i++)
            {

                gpbox = this.Controls.Find("gpb_bay" + (i + 1).ToString(), true).FirstOrDefault() as GroupBox;
                if (gpbox != null)
                {
                    gpbox.Font = new Font("Electrolux Sans SemiBold", sizefontbay);
                    gpbox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
                    if(i<4)
                    {
                        gpbox.Location = new Point(((tbPage_BAYS.Width / 4) * (i%4)) + 30, (tbPage_BAYS.Height / 4 * 0) + 30);
                    }
                    else if(i<8)
                    {
                        gpbox.Location = new Point(((tbPage_BAYS.Width / 4) * (i%4)) + 30, (tbPage_BAYS.Height / 4 * 1) + 60);
                    }
                    else
                    {
                        gpbox.Location = new Point(((tbPage_BAYS.Width / 4) * (i%4)) + 30, (tbPage_BAYS.Height / 4 * 2) + 90);
                    }
                    
                    gpbox.Text = "ZATOKA "+(i+1).ToString();
                    gpbox.Size = new Size(sizewidthbay,sizeheightbay);
                    gpbox.AutoSize = false;
                }                  
                lblpnc = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_PNC", true).FirstOrDefault() as Label;
                if (lblpnc != null)
                {
                    lblpnc.Text = "x";
                    lblpnc.Font = new Font("Electrolux Sans SemiBold", sizefontPNC);
                    lblpnc.AutoSize = true;
                    lblpnc.Left = 30;
                    lblpnc.Top = sizeTOPinBay_pnc;
                    gpbox.Controls.Add(lblpnc);
                }
                lblact = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_active", true).FirstOrDefault() as Label;
                if (lblact != null)
                {
                    lblact.Text = "x";
                    lblact.AutoSize = true;
                    lblact.Left = 30;
                    lblact.Top = sizeTOPinBay_act;
                    gpbox.Controls.Add(lblact);
                }
                lblpas = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_passive", true).FirstOrDefault() as Label;
                if (lblpas != null)
                {
                    lblpas.Text = "x";
                    lblpas.AutoSize = true;
                    lblpas.Left = 30;
                    lblpas.Top = sizeTOPinBay_pas;
                    gpbox.Controls.Add(lblpas);
                }
                lblemp = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_empty", true).FirstOrDefault() as Label;
                if (lblemp != null)
                {
                    lblemp.Text = "x";
                    lblemp.AutoSize = true;
                    lblemp.Left = 30;
                    lblemp.Top = sizeTOPinBay_emp;
                    gpbox.Controls.Add(lblemp);
                }
                lblful = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_full", true).FirstOrDefault() as Label;
                if (lblful != null)
                {
                    lblful.Text = "x";
                    lblful.AutoSize = true;
                    lblful.Left = 30;
                    lblful.Top = sizeTOPinBay_ful;
                    gpbox.Controls.Add(lblful);
                }
                lblres = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_reserved", true).FirstOrDefault() as Label;
                if (lblres != null)
                {
                    lblres.Text = "x";
                    lblres.AutoSize = true;
                    lblres.Left = 30;
                    lblres.Top = sizeTOPinBay_res;
                    gpbox.Controls.Add(lblres);
                }

                this.tbPage_BAYS.Controls.Add(gpbox);
            }
            #endregion
            
            #region change size of labels in bays
            for (int i = 0; i < 12; i++)
            {
                lblpnc = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_PNC", true).FirstOrDefault() as Label;
                if (lblpnc != null)
                {
                    lblpnc.Font = new Font("Electrolux Sans SemiBold", sizefontPNC);
                }
                lblact = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_active", true).FirstOrDefault() as Label;
                if (lblact != null)
                {
                    lblact.Font = new Font("Electrolux Sans SemiBold", sizefontrest);
                }
                lblpas = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_passive", true).FirstOrDefault() as Label;
                if (lblpas != null)
                {
                    lblpas.Font = new Font("Electrolux Sans SemiBold", sizefontrest);
                }
                lblemp = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_empty", true).FirstOrDefault() as Label;
                if (lblemp != null)
                {
                    lblemp.Font = new Font("Electrolux Sans SemiBold", sizefontrest);
                }
                lblful = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_full", true).FirstOrDefault() as Label;
                if (lblful != null)
                {
                    lblful.Font = new Font("Electrolux Sans SemiBold", sizefontrest);
                }
                lblres = this.Controls.Find("lbl_bay" + (i + 1).ToString() + "_reserved", true).FirstOrDefault() as Label;
                if (lblres != null)
                {
                    lblres.Font = new Font("Electrolux Sans SemiBold", sizefontrest);
                }
            }
            #endregion


        }
        
        public void initializeCodesScreen()
        {
            #region change size of labels in CODES 1st
            for (int i = 0; i < 30; i++)
            {
                if (i < 10)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).First() as Label;
                    if (lblcod != null)
                    {
                        lblcod.Font = new Font("Electrolux Sans SemiBold", sizefontcodes);
                        lblcod.Location = new Point(((tbPage_PNC.Width / 4) * 0) + 20, (tbPage_PNC.Height / 11 * (i%10)) + 50);
 

                    }
                }
                else if(i < 20)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).First() as Label;
                    if (lblcod != null)
                    {
                        lblcod.Font = new Font("Electrolux Sans SemiBold", sizefontcodes);
                        lblcod.Location = new Point(((tbPage_PNC.Width / 3) * 1) + 30, (tbPage_PNC.Height / 11 * (i%10)) + 50);
                    }
                }
                else
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).First() as Label;
                    if (lblcod != null)
                    {
                        lblcod.Font = new Font("Electrolux Sans SemiBold", sizefontcodes);
                        lblcod.Location = new Point(((tbPage_PNC.Width / 3) * 2) + 45, (tbPage_PNC.Height / 11 * (i%10)) + 50);
                    }
                }

            }
            #endregion

            #region change size of labels in CODES 2nd
            for (int i = 30; i < 60; i++)
            {
                if (i < 40)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).First() as Label;
                    if (lblcod != null)
                    {
                        lblcod.Font = new Font("Electrolux Sans SemiBold", sizefontcodes);
                        lblcod.Location = new Point(((tbPage_PNC_2.Width / 4) * 0) + 20, (tbPage_PNC_2.Height / 11 * (i % 10)) + 50);


                    }
                }
                else if (i < 50)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).First() as Label;
                    if (lblcod != null)
                    {
                        lblcod.Font = new Font("Electrolux Sans SemiBold", sizefontcodes);
                        lblcod.Location = new Point(((tbPage_PNC_2.Width / 3) * 1) + 30, (tbPage_PNC_2.Height / 11 * (i % 10)) + 50);
                    }
                }
                else if(i<60)
                {
                    lblcod = this.Controls.Find("code_" + (i + 1).ToString(), true).First() as Label;
                    if (lblcod != null)
                    {
                        lblcod.Font = new Font("Electrolux Sans SemiBold", sizefontcodes);
                        lblcod.Location = new Point(((tbPage_PNC_2.Width / 3) * 2) + 45, (tbPage_PNC_2.Height / 11 * (i % 10)) + 50);
                    }
                }

            }
            #endregion
        }

        private void lbl_Minimize_MouseHover(object sender, EventArgs e)
        {
            lbl_Minimize.BackColor = Color.DodgerBlue;
        }

        private void lbl_Minimize_MouseLeave(object sender, EventArgs e)
        {
            lbl_Minimize.BackColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
        }

        private void lbl_Close_MouseHover(object sender, EventArgs e)
        {
            lbl_Close.BackColor = Color.DodgerBlue;
        }

        private void lbl_Close_MouseLeave(object sender, EventArgs e)
        {
            lbl_Close.BackColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }
        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }
    }
}