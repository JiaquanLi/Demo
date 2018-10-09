using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvertXML;
using System.Windows.Forms;
using MathNet;
using System.Net.Sockets;
using System.Xml;
using System.Net;

namespace Demo
{
    public partial class frm_Main : Form
    {
        public struct SCloutdPoint
        {
            public double X;
            public double Y;
            public double Z;
            public double A;
            public double B;
            public double C;
        }

        List<SCloutdPoint> lstPrePt = new List<SCloutdPoint>();
        List<SCloutdPoint> lstNewPt = new List<SCloutdPoint>();
        double[,] Matric_input;

        bool bInitHal;
        RobotTcpInfo robotCn;

        private clsXML objXml;
        private clsRobotTcp objRobot;
        private clsAlgorithm_Server objAlgorithm;
        private clsScanner objScanner;
        public frm_Main()
        {
            InitializeComponent();
        }

        private void frm_Main_Load(object sender, EventArgs e)
        {
            InitialHal();

            System.Threading.Thread secondThread;
            secondThread = new System.Threading.Thread(new System.Threading.ThreadStart(StartListening));
            secondThread.IsBackground = true;
            secondThread.Start();
        }

        public static double[,] inverse_matrix(double[,] Array)
        {
            int m = 0;
            int n = 0;
            m = Array.GetLength(0);
            n = Array.GetLength(1);
            double[,] array = new double[2 * m + 1, 2 * n + 1];
            for (int k = 0; k < 2 * m + 1; k++)  //初始化数组
            {
                for (int t = 0; t < 2 * n + 1; t++)
                {
                    array[k, t] = 0.00000000;
                }
            }
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    array[i, j] = Array[i, j];
                }
            }

            for (int k = 0; k < m; k++)
            {
                for (int t = n; t <= 2 * n; t++)
                {
                    if ((t - k) == m)
                    {
                        array[k, t] = 1.0;
                    }
                    else
                    {
                        array[k, t] = 0;
                    }
                }
            }
            //得到逆矩阵
            for (int k = 0; k < m; k++)
            {
                if (array[k, k] != 1)
                {
                    double bs = array[k, k];
                    array[k, k] = 1;
                    for (int p = k + 1; p < 2 * n; p++)
                    {
                        array[k, p] /= bs;
                    }
                }
                for (int q = 0; q < m; q++)
                {
                    if (q != k)
                    {
                        double bs = array[q, k];
                        for (int p = 0; p < 2 * n; p++)
                        {
                            array[q, p] -= bs * array[k, p];
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            double[,] NI = new double[m, n];
            for (int x = 0; x < m; x++)
            {
                for (int y = n; y < 2 * n; y++)
                {
                    NI[x, y - n] = array[x, y];
                }
            }
            return NI;
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            //if(bInitHal == false)
            //{
            //    InitialHal();
            //    return;
            //}
            //if(pb_image.Image != null)
            try
            {
                pb_image.Visible = false;
                pb_image.Image.Dispose();
            }
            catch (Exception ex)
            {

            }
            btn_start.Enabled = false;
            objScanner.StartGetPoint();

        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            string strRpcMsg = "";
            btn_Stop.Enabled = false;
            pb_image.Load("img.PNG");
            Application.DoEvents();
            objScanner.StopGetPoint();

            pb_image.Visible = true;

            System.Threading.Thread.Sleep(100);
            Application.DoEvents();
            DateTime dtStart = DateTime.Now;

            //objAlgorithm.SetIcpMaxIte(50);
            objAlgorithm.StartOneRpc(ref strRpcMsg);

            DateTime dtEnd = DateTime.Now;
            OnLog("Total RPC time: " + (dtEnd - dtStart).TotalSeconds.ToString() + "s");
            OnLog("Get Matrix: " + strRpcMsg);

            CreateArryFromStr(strRpcMsg);
            CreateOriginalPoints();
            GetNewPointByMatrix();

            btn_start.Enabled = true;
            btn_Stop.Enabled = true;
            pb_image.Visible = true;
        }

        private void OnLog(string strLog)
        {
            if (strLog.EndsWith("\r\n") == false) strLog += "\r\n";
            strLog = string.Format("{0}: {1}", System.DateTime.Now, strLog);
            rtb_debug.AppendText(strLog);
            rtb_debug.SelectionStart = rtb_debug.Text.Length;
            rtb_debug.ScrollToCaret();
            Application.DoEvents();
        }

        private bool InitialHal()
        {
            bool bStatue;

            objAlgorithm = new clsAlgorithm_Server();
            objAlgorithm.Callback += new clsAlgorithm_Server.OnMessageCallback(OnLog);
            objXml = new clsXML();

            //scanner
            try
            {
                clsScanner.Callback += new clsScanner.OnMessageCallback(OnLog);
                objScanner = new clsScanner();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //robot

            //objRobot = new clsRobotTcp(robotCn);
            //objRobot.Callback += new clsRobotTcp.OnMessageCallback(OnLog);

            //tcp
            //bStatue = objRobot.StartServer();
            //if (bStatue == false)
            //{
            //    bInitHal = false;
            //    //MessageBox.Show("robot connect fail");
            //    OnLog("robot connect fail");
            //    return false;
            //}

            bInitHal = true;
            return true;
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_Setting fmSetting = new frm_Setting();
            fmSetting.ShowDialog();
        }

        private void CreateArryFromStr(string str)
        {
            string strTemp;

            int iStart, iEnd;
            double[] r0 = new double[3];
            double[] r1 = new double[3];
            double[] r2 = new double[3];
            double[] t = new double[3];

            iStart = str.IndexOf("R0=");
            iEnd = str.IndexOf("R1=");

            strTemp = str.Substring(iStart + 3, iEnd - iStart - 3);
            string[] strArry = strTemp.Split(' ');

            int index = 0;
            for (int i = 0; i < strArry.Length; i++)
            {
                if (strArry[i].Length > 0)
                {
                    r0[index++] = double.Parse(strArry[i]);
                }
            }

            iStart = str.IndexOf("R1=");
            iEnd = str.IndexOf("R2=");

            strTemp = str.Substring(iStart + 3, iEnd - iStart - 3);
            string[] strArry2 = strTemp.Split(' ');

            index = 0;
            for (int i = 0; i < strArry2.Length; i++)
            {
                if (strArry2[i].Length > 0)
                {
                    r1[index++] = double.Parse(strArry2[i]);
                }
            }

            iStart = str.IndexOf("R2=");
            iEnd = str.IndexOf("T=");

            strTemp = str.Substring(iStart + 3, iEnd - iStart - 3);
            string[] strArry3 = strTemp.Split(' ');

            index = 0;
            for (int i = 0; i < strArry3.Length; i++)
            {
                if (strArry3[i].Length > 0)
                {
                    r2[index++] = double.Parse(strArry3[i]);
                }
            }

            iStart = str.IndexOf("T=");
            iEnd = str.IndexOf("\r\n");

            strTemp = str.Substring(iStart + 3, iEnd - iStart - 3);
            string[] strArry4 = strTemp.Split(' ');
            index = 0;
            for (int i = 0; i < strArry4.Length; i++)
            {
                if (strArry4[i].Length > 0)
                {
                    t[index++] = double.Parse(strArry4[i]);
                }
            }

            Matric_input = new double[4, 4] { { r0[0], r0[1], r0[2], t[0] }, { r1[0], r1[1], r1[2], t[1] }, { r2[0], r2[1], r2[2], t[2] }, { 0.0, 0.0, 0.0, 1 } };
          
        }

        private void GetNewPointByMatrix()
        {
            string strLog;
            //double[,] Matric = inverse_matrix(Matric_input);
            double[,] Matric = Matric_input;
            SCloutdPoint sNewPT1, sTempPT;
            sNewPT1.X = 0;
            sNewPT1.Y = 0;
            sNewPT1.Z = 0;
            sNewPT1.A = 0;
            sNewPT1.B = 0;
            sNewPT1.C = 0;

            CreateOriginalPoints();


            for (int count = 0; count < lstPrePt.Count; count++)
            {
                sTempPT = lstPrePt[count];

                double[,] fromData = new double[4, 1] { { sTempPT.X }, { sTempPT.Y }, { sTempPT.Z }, { 1.0 } };
                double[,] getData = clsAlgorithm.MatrixOperation(Matric_input, fromData);

                sNewPT1.A = sTempPT.A;
                sNewPT1.B = sTempPT.B;
                sNewPT1.C = sTempPT.C;

                sNewPT1.X = getData[0, 0];
                sNewPT1.Y = getData[1, 0];
                sNewPT1.Z = getData[2, 0];
                lstNewPt.Add(sNewPT1);

                strLog = string.Format("<{0},{1},{2}>", sNewPT1.X, sNewPT1.Y, sNewPT1.Z);
                OnLog(strLog);

            }
            clsXML.WritePoint(lstNewPt, 1, "points.xml");
        }

        private void CreateOriginalPoints()
        {
            SCloutdPoint pt_temp;

            double z_down = 3.0;
            double z_up = 5.0;

            lstPrePt.Clear();
            lstNewPt.Clear();

            pt_temp.X = 248.88;
            pt_temp.Y = 465;
            pt_temp.Z = 46;
            pt_temp.A = -45.48;
            pt_temp.B = 0.33;
            pt_temp.C = 179.17;
            lstPrePt.Add(pt_temp);

            //2 bottom_1_inner
            pt_temp.X = -392.87;
            pt_temp.Y = 861.85;
            pt_temp.Z = 40.03;
            pt_temp.A = -45.48;
            pt_temp.B = 0.33;
            pt_temp.C = 179.17;
            lstPrePt.Add(pt_temp);
            /*
        //1 top_12_inner
        pt_temp.X = -295.67;
        pt_temp.Y = 483.65;
        pt_temp.Z = 36.93 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //2 bottom_1_inner
        pt_temp.X = 312.67;
        pt_temp.Y = 479.00;
        pt_temp.Z = 37.10 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //3 bottom_2_inner
        pt_temp.X = 312.67;
        pt_temp.Y = 553.00;
        pt_temp.Z = 42.10 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //4 bottom_3_inner
        pt_temp.X = 312.67;
        pt_temp.Y = 610.00;
        pt_temp.Z = 45.10 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //5 bottom_4_inner
        pt_temp.X = 312.67;
        pt_temp.Y = 670.00;
        pt_temp.Z = 46.10 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //6 bottom_5_inner
        pt_temp.X = 312.67;
        pt_temp.Y = 730.00;
        pt_temp.Z = 46.50 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //7 bottom_6_inner
        pt_temp.X = 312.67;
        pt_temp.Y = 790.00;
        pt_temp.Z = 46.20 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //8 bottom_7_inner
        pt_temp.X = 312.67;
        pt_temp.Y = 850.00;
        pt_temp.Z = 45.80 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //9 bottom_8_inner
        pt_temp.X = 312.67;
        pt_temp.Y = 910.00;
        pt_temp.Z = 44.40 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //10 bottom_9_inner
        pt_temp.X = 312.67;
        pt_temp.Y = 980.00;
        pt_temp.Z = 42.60 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //11 bottom_10_inner
        pt_temp.X = 312.67;
        pt_temp.Y = 1050.00;
        pt_temp.Z = 40.00 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //12 bottom_11_inner
        pt_temp.X = 312.67;
        pt_temp.Y = 1100.00;
        pt_temp.Z = 38.00 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //13 bottom_12_inner
        pt_temp.X = 312.67;
        pt_temp.Y = 1140.00;
        pt_temp.Z = 36.70 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //14 top_1_inner
        pt_temp.X = -295.67;
        pt_temp.Y = 1140.00;
        pt_temp.Z = 38.50 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //15 top_2_inner
        pt_temp.X = -295.67;
        pt_temp.Y = 1100.00;
        pt_temp.Z = 39.10 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //16 top_3_inner
        pt_temp.X = -295.67;
        pt_temp.Y = 1050.00;
        pt_temp.Z = 41.30 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //17 top_4_inner
        pt_temp.X = -295.67;
        pt_temp.Y = 980.00;
        pt_temp.Z = 43.80 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //18 top_5_inner
        pt_temp.X = -295.67;
        pt_temp.Y = 910.00;
        pt_temp.Z = 45.20 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //19 top_6_inner
        pt_temp.X = -295.67;
        pt_temp.Y = 850.0;
        pt_temp.Z = 46.40 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //20 top_7_inner
        pt_temp.X = -295.67;
        pt_temp.Y = 790.00;
        pt_temp.Z = 46.75 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //21 top_8_inner
        pt_temp.X = -295.67;
        pt_temp.Y = 730.00;
        pt_temp.Z = 46.10 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //22 top_9_inner
        pt_temp.X = -295.67;
        pt_temp.Y = 670.00;
        pt_temp.Z = 46.80 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //23 top_10_inner
        pt_temp.X = -295.67;
        pt_temp.Y = 610.00;
        pt_temp.Z = 46.00 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //24 top_11_inner
        pt_temp.X = -295.67;
        pt_temp.Y = 550.00;
        pt_temp.Z = 43.60 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //25 top_12_inner
        pt_temp.X = -295.67;
        pt_temp.Y = 483.65;
        pt_temp.Z = 36.93 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //26 top_12_inner_up
        pt_temp.X = -295.67;
        pt_temp.Y = 483.65;
        pt_temp.Z = 36.93 + z_up;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //27 top_18_outer_up
        pt_temp.X = -367.67;
        pt_temp.Y = 380.65;
        pt_temp.Z = 31.5 + z_up;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //28 top_18_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 380.65;
        pt_temp.Z = 31.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //29 left_1_outer
        pt_temp.X = -295.67;
        pt_temp.Y = 380.65;
        pt_temp.Z = 29.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //30 left_2_outer
        pt_temp.X = -200.67;
        pt_temp.Y = 380.65;
        pt_temp.Z = 28.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //31 left_3_outer
        pt_temp.X = -100.67;
        pt_temp.Y = 380.65;
        pt_temp.Z = 27.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //32 left_4_outer
        pt_temp.X = -0.67;
        pt_temp.Y = 380.65;
        pt_temp.Z = 26.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //33 left_5_outer
        pt_temp.X = 100.67;
        pt_temp.Y = 380.65;
        pt_temp.Z = 24.8 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //34 left_6_outer
        pt_temp.X = 200.67;
        pt_temp.Y = 380.65;
        pt_temp.Z = 24.0 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //35 left_7_outer
        pt_temp.X = 300.67;
        pt_temp.Y = 380.65;
        pt_temp.Z = 24.6 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //36 bottom_1_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 380.65;
        pt_temp.Z = 25.7 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //37 bottom_2_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 400.65;
        pt_temp.Z = 29.3 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //38 bottom_3_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 410.65;
        pt_temp.Z = 32.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //39 bottom_4_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 420.65;
        pt_temp.Z = 34.0 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //40 bottom_5_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 440.65;
        pt_temp.Z = 36.2 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //41 bottom_6_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 490.65;
        pt_temp.Z = 39.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //42 bottom_7_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 540.65;
        pt_temp.Z = 42.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //43 bottom_8_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 560.65;
        pt_temp.Z = 42.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //44 bottom_9_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 590.65;
        pt_temp.Z = 44.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //45 bottom_10_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 690.65;
        pt_temp.Z = 44.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //46 bottom_11_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 750.65;
        pt_temp.Z = 46.0 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //47 bottom_12_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 940.65;
        pt_temp.Z = 43.1 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //48 bottom_13_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 1000.65;
        pt_temp.Z = 39.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //49 bottom_14_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 1050.65;
        pt_temp.Z = 37.0 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //50 bottom_15_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 1100.65;
        pt_temp.Z = 35.0 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //51 bottom_16_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 1120.65;
        pt_temp.Z = 35.0 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //52 bottom_17_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 1150.65;
        pt_temp.Z = 32.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //53 bottom_18_outer
        pt_temp.X = 390.67;
        pt_temp.Y = 1180.65;
        pt_temp.Z = 31.0 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //54 bottom_19_outer
        pt_temp.X = 310.67;
        pt_temp.Y = 1180.65;
        pt_temp.Z = 34.6 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //55 bottom_20_outer
        pt_temp.X = 310.67;
        pt_temp.Y = 1230.65;
        pt_temp.Z = 32.2 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //56 top_1_outer
        pt_temp.X = -280.67;
        pt_temp.Y = 1230.65;
        pt_temp.Z = 33.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //57 top_2_outer
        pt_temp.X = -280.67;
        pt_temp.Y = 1180.65;
        pt_temp.Z = 36.0 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //58 top_3_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 1180.65;
        pt_temp.Z = 36.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //59 top_4_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 1120.65;
        pt_temp.Z = 39.0 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //60 top_5_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 1060.65;
        pt_temp.Z = 40.6 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //61 top_6_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 1030.65;
        pt_temp.Z = 40.6 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //62 top_7_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 1000.65;
        pt_temp.Z = 42.8 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //63 top_8_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 900.65;
        pt_temp.Z = 42.8 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //64 top_9_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 800.65;
        pt_temp.Z = 44.2 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //65 top_10_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 670.65;
        pt_temp.Z = 46.2 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //66 top_11_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 570.65;
        pt_temp.Z = 43.6 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //67 top_12_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 540.65;
        pt_temp.Z = 42.0 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //68 top_13_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 500.65;
        pt_temp.Z = 39.0 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //69 top_14_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 470.65;
        pt_temp.Z = 36.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //70 top_15_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 440.65;
        pt_temp.Z = 35.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //71 top_16_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 420.65;
        pt_temp.Z = 35.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //72 top_17_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 400.65;
        pt_temp.Z = 34.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);

        //73 top_18_outer
        pt_temp.X = -367.67;
        pt_temp.Y = 380.65;
        pt_temp.Z = 31.5 - z_down;
        pt_temp.A = -45.48;
        pt_temp.B = 0.33;
        pt_temp.C = 179.17;
        lstPrePt.Add(pt_temp);
        */
        }

        private static void StartListening()
        {
            return;
            int a = 0;

            TcpListener server = null;
            IPAddress IP = IPAddress.Parse("192.168.11.5");
            server = new TcpListener(IP, 49152);
            server.Start();
            String strSend = null;

            byte[] bytes = new Byte[1024];
            Socket handler;
            while (true)
            {
                String strReceive = null;
                handler = server.AcceptSocket();
                XmlDocument SendXML = new XmlDocument();
                SendXML.PreserveWhitespace = true;
                SendXML.Load("points.xml");
                strSend = SendXML.InnerXml;
                int bytesRec = handler.Receive(bytes);
                strReceive = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRec);
                Console.WriteLine(strReceive);
                if (strReceive != null)
                {
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(strSend);
                    Console.WriteLine(strSend);
                    Console.WriteLine("\n");
                    handler.Send(msg, 0, msg.Length, System.Net.Sockets.SocketFlags.None);
                }
            }
        }

        private void btn_templet_Click(object sender, EventArgs e)
        {
            objAlgorithm.SetIcpTempletFile();
        }
    }
}