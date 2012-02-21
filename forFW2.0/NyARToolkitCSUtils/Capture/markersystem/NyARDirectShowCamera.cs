using System;
using System.Collections.Generic;
using System.Text;
using jp.nyatla.nyartoolkit.cs.core;
using NyARToolkitCSUtils.Capture;
using DirectShowLib;
using System.Threading;

namespace NyARToolkitCSUtils.markersystem
{
    class NyARDirectShowCamera : NyARSensor
    {
        private CaptureDevice _cdev;
        private DsRgbRaster _raster;
        public NyARDirectShowCamera(CaptureDevice i_cdev, float i_fps)
            : base(new nyARIntSize(i_cdev.video_width, i_cdev.video_height))
        {
            //RGBラスタの生成
            this._raster = new DsRgbRaster(i_cdev.video_width, i_cdev.video_height, NyARBufferType.OBJECT_CS_Bitmap);
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
                    this._raster.setBuffer(i_buffer, false);
                    this.updateTimeStamp();
                }
                catch (Exception e)
                {
                    e.printStackTrace();
                }
            }
        }
    }
}
