using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{
    public partial class frm_Setting : Form
    {
        private clsAlgorithm_Server objServer = null;

        public clsAlgorithm_Server Server
        {
            set
            {
                objServer = value;
            }
        }
        public frm_Setting()
        {
            InitializeComponent();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            try
            {
                ReadIniSettings.ReadIni.objIniValue.iniScanner.frequency = double.Parse(tb_fre.Text);
                ReadIniSettings.ReadIni.objIniValue.iniScanner.speed = double.Parse(tb_speed.Text);
                ReadIniSettings.ReadIni.objIniValue.iniScanner.Ip = tb_scannerip.Text;
                ReadIniSettings.ReadIni.objIniValue.iniScanner.step =double.Parse( tb_scannerstep.Text);


                ReadIniSettings.ReadIni.objIniValue.iniRobot.Ip = tb_robotip.Text;
                ReadIniSettings.ReadIni.objIniValue.iniRobot.Port = tb_RobotPort.Text;

                ReadIniSettings.ReadIni.objIniValue.iniServer.Port = tb_ServerPort.Text;
                ReadIniSettings.ReadIni.objIniValue.iniServer.Ip = tb_ServerIp.Text;

                //ICP
                ReadIniSettings.ReadIni.objIniValue.iniIcp.filter = double.Parse(tb_filter.Text);
                ReadIniSettings.ReadIni.objIniValue.iniIcp.maxIterationTimes = int.Parse(tb_maxIterTimes.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            ReadIniSettings.ReadIni.SetAll();

            UpdateIcpServer();

            this.DialogResult = DialogResult.OK;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void frm_Setting_Load(object sender, EventArgs e)
        {
            tb_speed.Text = ReadIniSettings.ReadIni.objIniValue.iniScanner.speed.ToString();
            tb_fre.Text = ReadIniSettings.ReadIni.objIniValue.iniScanner.frequency.ToString();
            tb_scannerip.Text = ReadIniSettings.ReadIni.objIniValue.iniScanner.Ip;
            tb_scannerstep.Text = ReadIniSettings.ReadIni.objIniValue.iniScanner.step.ToString();


            tb_robotip.Text = ReadIniSettings.ReadIni.objIniValue.iniRobot.Ip;
            tb_RobotPort.Text = ReadIniSettings.ReadIni.objIniValue.iniRobot.Port;

            tb_ServerIp.Text = ReadIniSettings.ReadIni.objIniValue.iniServer.Ip;
            tb_ServerPort.Text = ReadIniSettings.ReadIni.objIniValue.iniServer.Port;

            //ICP
            tb_filter.Text =  ReadIniSettings.ReadIni.objIniValue.iniIcp.filter.ToString();
            tb_maxIterTimes.Text = ReadIniSettings.ReadIni.objIniValue.iniIcp.maxIterationTimes.ToString();
        }

        private bool UpdateIcpServer()
        {
            if (objServer != null)
            {
                objServer.SetIcpMaxIte(ReadIniSettings.ReadIni.objIniValue.iniIcp.maxIterationTimes);
                objServer.SetIcpFilter(ReadIniSettings.ReadIni.objIniValue.iniIcp.filter);
            }
            return true;
        }

        private void tb_maxIterTimes_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar == 8))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

        }

        private void tb_filter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar == 8) || e.KeyChar == '.')
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
