using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using NyARToolkitCSUtils.Capture;
#if NyartoolkitCS_FRAMEWORK_CFW
using Microsoft.WindowsMobile.DirectX.Direct3D;
using Microsoft.WindowsMobile.DirectX;
#else
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
#endif
namespace NyARToolkitCSUtils.Direct3d
{
    public abstract class D3dSketch
    {
        /** メインフォーム
         */ 
        protected Form form;
        private PresentParameters _dpp=new PresentParameters();
        private Device _d3d;
        public D3dSketch()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


        }
        /**
         * This function start sketch system without camera device.
         * An Argument i_cap of setup will be NULL.
         */
        public void runWithoutCamera()
        {
            // フォームとメインサンプルクラスを作成
            using (D3dSketchForm mwin = new D3dSketchForm())
            {
                this.form = mwin;
                mwin.Show();
                //setup
                this.setup(null);
                if (this._d3d == null)
                {
                    this._d3d = prepareD3dDevice(this.form, this._dpp);
                }
                //loop
                while (mwin.Created)
                {
                    this.loop(this._d3d);
                    //スレッドスイッチ
                    Thread.Sleep(1);
                    // イベントがある場合はその処理する
                    Application.DoEvents();
                }
                this.cleanup();
                this._d3d.Dispose();
            }
        }
        /**
         * This function start sketch system with camera device.
         */
        public void run()
        {
            //キャプチャデバイスリストを取得
            CaptureDeviceList capture_device_list = new CaptureDeviceList();
            if (capture_device_list.count < 1)
            {
                MessageBox.Show("The capture system is not found.");
                return;
            }
            //キャプチャデバイスを選択してもらう。
            int cdevice_number = 0;
            using (CameraSelectDialog camera_select = new CameraSelectDialog())
            {
                camera_select.ShowDialog(capture_device_list, out cdevice_number);
            }
            // フォームとメインサンプルクラスを作成
            using (D3dSketchForm mwin = new D3dSketchForm())
            {
                this.form = mwin;
                using (CaptureDevice capture_device = capture_device_list[cdevice_number])
                {
                    mwin.Show();
                    //setup
                    this.setup(capture_device);
                    if (this._d3d == null)
                    {
                        this._d3d = prepareD3dDevice(this.form, this._dpp);
                    }
                    //loop
                    while (mwin.Created)
                    {
                        this.loop(this._d3d);
                        //スレッドスイッチ
                        Thread.Sleep(1);
                        // イベントがある場合はその処理する
                        Application.DoEvents();
                    }
                }
                this.cleanup();
                this._d3d.Dispose();
            }
        }
        /* Direct3Dデバイスを準備する関数
         */
        private static Device prepareD3dDevice(Control i_window, PresentParameters pp)
        {
            pp.Windowed = true;
            pp.SwapEffect = SwapEffect.Flip;
            pp.BackBufferFormat = Format.X8R8G8B8;
            pp.BackBufferCount = 1;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = DepthFormat.D16;
            CreateFlags fl_base = CreateFlags.FpuPreserve;
            try
            {
                return new Device(0, DeviceType.Hardware, i_window.Handle, fl_base | CreateFlags.HardwareVertexProcessing, pp);
            }
            catch (Exception ex1)
            {
                Debug.WriteLine(ex1.ToString());
                try
                {
                    return new Device(0, DeviceType.Hardware, i_window.Handle, fl_base | CreateFlags.SoftwareVertexProcessing, pp);
                }
                catch (Exception ex2)
                {
                    // 作成に失敗
                    Debug.WriteLine(ex2.ToString());
                    try
                    {
                        return new Device(0, DeviceType.Reference, i_window.Handle, fl_base | CreateFlags.SoftwareVertexProcessing, pp);
                    }
                    catch (Exception ex3)
                    {
                        throw ex3;
                    }
                }
            }
        }
        /// <summary>
        /// 画面サイズをセットします。
        /// この関数は、setup関数で1度だけコールできます。
        /// </summary>
        /// <param name="i_width"></param>
        /// <param name="i_height"></param>
        /// <returns></returns>
        public Device size(int i_width, int i_height)
        {
            Debug.Assert(this._d3d == null);
            this.form.ClientSize = new System.Drawing.Size(i_width, i_height);
            Device d = prepareD3dDevice(this.form, this._dpp);
            this._d3d = d;
            return d;
        }
        public abstract void setup(CaptureDevice i_cap);
        public virtual void cleanup(){}
        public abstract void loop(Device i_d3d);
    }
}
