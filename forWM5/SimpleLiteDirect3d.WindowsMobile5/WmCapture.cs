using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using jp.nyatla.cs.NyWMCapture;

namespace SimpleLiteDirect3d.WindowsMobile5
{
    public interface IWmCaptureListener
    {
        void onSample(WmCapture i_sender, INySample i_sample);
    }
    public class WmCapture : IDisposable,INySampleCB
    {
        private INyWMCapture _capture;
        private IWmCaptureListener _on_sample_event = null;
        private bool _is_start = false;
        private bool _vertical_flip;
        public bool vertical_flip { get { return this._vertical_flip; } } 
        public WmCapture(Size i_cap_size,bool i_vertical_flip_property)
        {
            //キャプチャ作る。
            NyWMCapture cap = new NyWMCapture();
            INyWMCapture cap_if = (INyWMCapture)cap;
            int hr;
            hr = cap_if.SetCallBack(this);//これInitializeの前にやらないといけないのよね。
            if (hr != 0)
            {
                throw new Exception("cap_if.SetCallBack");
            }
            hr = cap_if.SetSize(i_cap_size.Width, i_cap_size.Height);
            if (hr != 0)
            {
                throw new Exception("cap_if.SetSize");
            }
            hr = cap_if.Initialize(NyWMCapture.DeviceId_WM5, NyWMCapture.MediaSubType_RGB565, NyWMCapture.PinCategory_PREVIEW);
            if (hr != 0)
            {
                throw new Exception("cap_if.Initialize");
            }
            this._capture = cap_if;
            this._vertical_flip = i_vertical_flip_property;
            return;
        }
        public void start()
        {
            Debug.Assert(this._is_start==false);
            this._capture.Start();
            this._is_start = true;
            return;
        }
        public void stop()
        {
            Debug.Assert(this._is_start == true);
            this._capture.Stop();
            this._is_start = false;
            return;
        }
        public void setOnSample(IWmCaptureListener i_handler)
        {
            this._on_sample_event=i_handler;
            return;
        }
        public int OnSample(INySample i_sample)
        {
            if (this._on_sample_event != null)
            {
                this._on_sample_event.onSample(this,i_sample);
            }
            return 0;
        }
        public void Dispose()
        {
            if (this._capture != null)
            {
                if (this._is_start)
                {
                    this._capture.Stop();
                }
                this._capture.Finalize();
                this._capture = null;
            }
            return;
        }
    }
}

