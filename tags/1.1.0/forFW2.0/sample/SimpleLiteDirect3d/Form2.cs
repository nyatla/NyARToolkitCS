using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using NyARToolkitCSUtils.Capture;

namespace SimpleLiteDirect3d
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public DialogResult ShowDialog(CaptureDeviceList i_clist,out int o_selected_no)
        {
            if (i_clist.count < 1)
            {
                throw new Exception("カメラが無いのに選ぼうとしてはいけない。");
            }
            for (int i = 0; i < i_clist.count; i++)
            {
                this.comboBox1.Items.Add(i_clist[i].name + ":");
            }
            this.comboBox1.SelectedIndex = 0;
            DialogResult ret=base.ShowDialog();
            o_selected_no = this.comboBox1.SelectedIndex;
            return ret;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
