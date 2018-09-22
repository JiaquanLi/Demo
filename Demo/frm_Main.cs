﻿using System;
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

namespace Demo
{
    public partial class frm_Main : Form
    {
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
            catch(Exception ex)
            {

            }
            btn_start.Enabled = false;
            objScanner.StartGetPoint();
            //double[,] mtric = new double[4, 4] { { }, { }, { }, { } };
            //clsAlgorithm ag = new clsAlgorithm();
            //ag.MatricOperation();
        }


        private void btn_Stop_Click(object sender, EventArgs e)
        {
            string strRpcMsg = "";
            btn_Stop.Enabled = false;
            pb_image.Load("img.PNG");
            Application.DoEvents();
            objScanner.StopGetPoint();
            
            System.Threading.Thread.Sleep(100);
            Application.DoEvents();
            objAlgorithm.StartOneRpc(ref strRpcMsg);
            OnLog("Get Matric: " + strRpcMsg);

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

            objAlgorithm = new  clsAlgorithm_Server();
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
            robotCn.port = 49152;
            robotCn.ipAddress = "192.168.11.5";
            objRobot = new clsRobotTcp(robotCn);
            objRobot.Callback += new clsRobotTcp.OnMessageCallback(OnLog);

            //tcp
            bStatue = objRobot.StartServer();
            if (bStatue == false)
            {
                bInitHal = false;
                //MessageBox.Show("robot connect fail");
                OnLog("robot connect fail");
                return false;
            }

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

        }

    }
}
