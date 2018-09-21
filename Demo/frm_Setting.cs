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
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            ReadIniSettings.ReadIni.SetAll();

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
            
        }
    }
}
