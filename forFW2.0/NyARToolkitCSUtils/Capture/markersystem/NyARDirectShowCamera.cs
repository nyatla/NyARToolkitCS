using System;
using System.Collections.Generic;
using System.Text;
using jp.nyatla.nyartoolkit.cs.core;
using NyARToolkitCSUtils.Capture;
using jp.nyatla.nyartoolkit.cs.markersystem;
using DirectShowLib;
using System.Threading;

namespace NyARToolkitCSUtils.Capture
{
    public class NyARDirectShowCamera : NyARSensor,CaptureListener
    {
        private CaptureDevice _cdev;
        private DsRgbRaster _raster;
        /// <summary>
        /// </summary>
        /// <param name="i_cdev"></param>
        /// <param name="i_raster_type">
        /// OBJECT_CS_Bitmap is slower than BYTE1D_B8G8R8X8_32(20%) but compatible with Bitmap.
        /// OBJECT_CS_Bitmap is fast but not compatible with Bitmap.
        /// </param>
        public NyARDirectShowCamera(CaptureDevice i_cdev,int i_raster_type)
            : base(new NyARIntSize(i_cdev.video_width, i_cdev.video_height))
        {
            //RGBラスタの生成
            this.initInstance(i_cdev);
        }
        /// <summary>
        /// This function as is NyARDirectShowCamera(i_cdev,NyARBufferType.OBJECT_CS_Bitmap)
        /// </summary>
        /// <param name="i_cdev"></param>
        public NyARDirectShowCamera(CaptureDevice i_cdev)
            : base(new NyARIntSize(i_cdev.video_width, i_cdev.video_height))
        {
            //RGBラスタの生成
            this.initInstance(i_cdev);
        }
        private void initInstance(CaptureDevice i_cdev)
        {
            this._raster = new DsRgbRaster(i_cdev.video_width, i_cdev.video_height);
            //ラスタのセット
            this.update(this._raster);
            this._cdev = i_cdev;
            i_cdev.SetCaptureListener(this);
        }
        /**
         * この関数は、JMFの非同期更新を停止します。
         */
        public void stop()
        {
            this._cdev.StopCapture();
        }
        /**
         * この関数は、JMFの非同期更新を開始します。
         */
        public void start()
        {
            this._cdev.StartCapture();
            //1枚目の画像が取得され、RGBラスタにデータがセットされるまで待つ。
            while (!this._raster.hasBuffer())
            {
                Thread.Sleep(200);
            }
        }
        
        public void OnBuffer(CaptureDevice i_sender, double i_sample_time, IntPtr i_buffer, int i_buffer_len)
        {
            //ロックされていなければ、RGBラスタを更新する。
            lock (this)
            {
                try
                {
                    this._raster.setBuffer(i_buffer, i_buffer_len, i_sender.video_vertical_flip);
                    this.updateTimeStamp();
                }
                catch (Exception e)
                {
                    System.Console.Error.WriteLine(e.Message);
                }
            }
        }
    }
}
