using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.match;
using jp.nyatla.nyartoolkit.cs.raster;
using jp.nyatla.nyartoolkit.cs.detector;
using NyARToolkitCSUtils.Capture;
using NyARToolkitCSUtils.Raster;

namespace CaptureTest
{
    public partial class Form1 : Form, CaptureListener
    {
        private CaptureDevice m_cap;
        private const String AR_CODE_FILE = "../../../../../data/patt.hiro";
        private const String AR_CAMERA_FILE = "../../../../../data/camera_para.dat";
        private NyARSingleDetectMarker m_ar;
        private DsXRGB32Raster m_raster;
        public Form1()
        {
            InitializeComponent();
            //ARの設定
            //AR用カメラパラメタファイルをロード
            NyARParam ap = new NyARParam();
            ap.loadFromARFile(AR_CAMERA_FILE);
            ap.changeSize(320, 240);

            //AR用のパターンコードを読み出し	
            NyARCode code = new NyARCode(16, 16);
            code.loadFromARFile(AR_CODE_FILE);

            //１パターンのみを追跡するクラスを作成
            this.m_ar = new NyARSingleDetectMarker(ap, code, 80.0);
            NyARTransMatResult result_mat = new NyARTransMatResult();
            //計算モードの設定
            this.m_ar.setContinueMode(false);
            //キャプチャを作る
			/**************************************************
			このコードは、0番目（一番初めに見つかったキャプチャデバイス）
			を使用するようにハードコーディングされています。
			複数のキャプチャデバイスを持つシステムの場合、うまく動作しないかもしれません。
			n番目のデバイスを使いたいときには、CaptureDevice cap=cl[0];←ここの0を変えてください。
			手動で選択させる方法は、SimpleLiteDirect3Dを参考にしてください。
			**************************************************/
            CaptureDeviceList cl=new CaptureDeviceList();
            CaptureDevice cap=cl[0];
            cap.SetCaptureListener(this);
            cap.PrepareCapture(320, 240,30);
            this.m_cap = cap;
            //ラスタを作る。
            this.m_raster = new DsXRGB32Raster(cap.video_width,cap.video_height,cap.video_width*cap.video_bit_count/8);
        }
        public void OnBuffer(CaptureDevice i_sender, double i_sample_time, IntPtr i_buffer, int i_buffer_len)
        {
            int w = i_sender.video_width;
            int h = i_sender.video_height;
            int s = w * (i_sender.video_bit_count / 8);            

            
            Bitmap b = new Bitmap(w, h, s, PixelFormat.Format32bppRgb, i_buffer);
            

            // If the image is upsidedown
            b.RotateFlip(RotateFlipType.RotateNoneFlipY);
            pictureBox1.Image = b;

            //ARの計算
            this.m_raster.setBuffer(i_buffer);
            if (this.m_ar.detectMarkerLite(this.m_raster, 100))
            {
                NyARTransMatResult result_mat = new NyARTransMatResult();
                this.m_ar.getTransmationMatrix(result_mat);
                this.Invoke(
                    (MethodInvoker)delegate()
                {
                    label1.Text = this.m_ar.getConfidence().ToString();
                    label2.Text = this.m_ar.getDirection().ToString();
                    label3.Text = result_mat.getArray()[0][0].ToString();
                    label4.Text = result_mat.getArray()[0][1].ToString();
                    label5.Text = result_mat.getArray()[0][2].ToString();
                    label6.Text = result_mat.getArray()[0][3].ToString();
                    label7.Text = result_mat.getArray()[1][0].ToString();
                    label8.Text = result_mat.getArray()[1][1].ToString();
                    label9.Text = result_mat.getArray()[1][2].ToString();
                    label10.Text = result_mat.getArray()[1][3].ToString();
                    label11.Text = result_mat.getArray()[2][0].ToString();
                    label12.Text = result_mat.getArray()[2][1].ToString();
                    label13.Text = result_mat.getArray()[2][2].ToString();
                    label14.Text = result_mat.getArray()[2][3].ToString();
                }
                );
            }else{
                this.Invoke(
                    (MethodInvoker)delegate(){
                        label1.Text = "マーカー未検出";
                        label2.Text = "-";
                        label3.Text = "-";
                        label4.Text = "-";
                        label5.Text = "-";
                        label6.Text = "-";
                        label7.Text = "-";
                        label8.Text = "-";
                        label9.Text = "-";
                        label10.Text = "-";
                        label11.Text = "-";
                        label12.Text = "-";
                        label13.Text = "-";
                        label14.Text = "-";

                    }
                );
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.m_cap.StartCapture();
        }
    }
}
