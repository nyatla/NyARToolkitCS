/*
 * NyARToolkitCSUtils NyARToolkit for C#
 * SimpleLiteDirect3d for WindowsMobile
 * 
 * (c)2008 nyatla
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
 */
using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace SimpleLiteDirect3d.WindowsMobile5
{
    public partial class Form2 : Form
    {
        ListBox m_dev_list;
        private void OnOK(object sender,EventArgs e)
        {
            int idx = this.m_dev_list.SelectedIndex;
            if (idx < 0)
            {
                MessageBox.Show("機種を選択してください。");
                return;
            }
            this.Close();
        }
        public DeviceAdapter GetSelectedDeviceAdapter()
        {
            switch (this.m_dev_list.SelectedIndex)
            {
                case 0: return new DeviceAdapter_S01SH();
                case 1: return new DeviceAdapter_WS007SH();
                case 2: return new DeviceAdapter_S01SH();
                default: return null;
            }
        }
        public Form2()
        {
            InitializeComponent();
            //よく判らないから適当に作る。
            //
            this.Text = "[NyARToolkit]起動モードの選択";
            //リストボックス
            ListBox dev_list_box=new ListBox();
            dev_list_box.Items.Add("S01SH(emobile)");
            dev_list_box.Items.Add("WS007SH(willcom)");
            dev_list_box.Items.Add("WS011SH(willcom)");
            dev_list_box.Size = new Size(this.ClientSize.Width, this.ClientSize.Width / 3);
            this.Controls.Add(dev_list_box);
            this.m_dev_list=dev_list_box;

            //OKボタン
            Button ok_button = new Button();
            ok_button.Top=this.ClientSize.Width/3;
            ok_button.Size = new Size(this.ClientSize.Width,50);
            ok_button.Text = "この機種で実行";
            ok_button.Click+=new EventHandler(OnOK);
            this.Controls.Add(ok_button);
            //(c)表示
            Label la1 = new Label();
            la1.TextAlign = ContentAlignment.TopCenter;
            la1.Text = "注意！\nDirect3Dで問題が発生することがあるので、縦画面で実行してください。";
            la1.Size = new Size(this.ClientSize.Width, 100);
            la1.Top = ok_button.Bottom+10;
            this.Controls.Add(la1);

            //(c)表示
            Label la2 = new Label();
            la2.TextAlign = ContentAlignment.TopCenter;
            la2.Text = "NyARToolkitCS (c)2008 nyatla.jp";
            la2.Size= new Size(this.ClientSize.Width,50);
            la2.Top = this.ClientSize.Height - la2.Height;
            this.Controls.Add(la2);

        }
    }
}