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
            clsAlgorithm ag = new clsAlgorithm();
            ag.MatricOperation();

            string strRpcMsg = "";
            if(bInitHal == false)
            {
                InitialHal();
                return;
            }

            objScanner.StartGetPoint();
            objScanner.StartGetPoint();

            objAlgorithm.StartOneRpc(ref strRpcMsg);





        }

        private void OnLog(string strLog)
        {

        }
        
        private bool InitialHal()
        {
            bool bStatue;

            objAlgorithm = new  clsAlgorithm_Server();
            objXml = new clsXML();
            try
            {
                objScanner = new clsScanner();
            }
            catch (Exception ex)
            {

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
                MessageBox.Show("robot connection error");
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
    }
}
