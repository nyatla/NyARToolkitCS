/* 
 * PROJECT: NyARToolkitCSUtils NyARToolkit for C# 支援ライブラリ
 * --------------------------------------------------------------------------------
 * The MIT License
 * Copyright (c) 2008 nyatla
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/nyartoolkit/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 */
using System;
using System.Collections.Generic;
using System.Collections;
using DirectShowLib;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NyARToolkitCSUtils.Capture
{

    /* キャプチャデバイス1個に対応するオブジェクト
     * 1キャプチャデバイスに対応するクラスです。 
     */
    public class CaptureDevice : ISampleGrabberCB,IDisposable
    {
        private IFilterGraph2   m_FilterGraph;
        private DsDevice        m_dev;
        private IAMVideoControl m_VidControl    = null;
        private IPin            m_pinStill      = null;
        private Boolean         m_graphi_active = false;
        private CaptureListener m_cap_listener  = null;
        //
        private VideoInfoHeader m_video_info    =null;

        public CaptureDevice(DsDevice i_dev)
        {
            this.m_dev = i_dev;
        }
        public void Dispose()
        {
            //既に停止中なら諦める。
            if (this.m_graphi_active)
            {
                //キャプチャ停止
                StopCapture();
            }
            //フィルタオブジェクトをクリーンアップ
            CleanupGraphiObjects();
        }
        public int video_width
        {
            get { return m_video_info.BmiHeader.Width; } 
        }
        public int video_height
        {
            get { return m_video_info.BmiHeader.Height; }
        }
        public int video_bit_count
        {
            get { return m_video_info.BmiHeader.BitCount; } 
        }

        public bool video_vertical_flip
        {
            //VideoFormatから取るようにしないと…。
            get { return true;}
        }

        /**
         * キャプチャデバイスのパスを返す。
         */
        public String path
        {
            get{
                return this.m_dev.DevicePath;
            }
        }
        /**
         * キャプチャデバイスの名前を返す。
         *
         */
        public String name
        {
            get{
                return this.m_dev.Name;
            }
        }

        /**
         * キャプチャデバイスの概要を1行で表示する。
         */
        public void Dump()
        {
            Console.Out.WriteLine(this.name+"["+this.path+"]");
        }
        public void SetCaptureListener(CaptureListener i_listener)
        {
            //キャプチャ中は切り替えられない。
            if (this.m_graphi_active)
            {
                throw new Exception();
            }
            this.m_cap_listener=i_listener;
        }
        /**
         * 必要に応じてグラフオブジェクトを全て削除する。
         */
        private void CleanupGraphiObjects()
        {
            this.m_video_info = null;
            if (m_FilterGraph != null)
            {
                Marshal.ReleaseComObject(m_FilterGraph);
                m_FilterGraph = null;
            }

            if (m_VidControl != null)
            {
                Marshal.ReleaseComObject(m_VidControl);
                m_VidControl = null;
            }

            if (m_pinStill != null)
            {
                Marshal.ReleaseComObject(m_pinStill);
                m_pinStill = null;
            }
        }
        public void PrepareCapture(int i_width, int i_height,float i_frame_rate)
        {
            const int BBP = 32;
            //既にキャプチャ中なら諦める。
            if (this.m_graphi_active)
            {
                throw new Exception();
            }
            //現在確保中のグラフインスタンスを全て削除
            CleanupGraphiObjects();

            int hr;
            ISampleGrabber sampGrabber = null;
            IBaseFilter capFilter = null;
            IPin pSampleIn = null;

            //グラフビルダを作る。
            this.m_FilterGraph = new FilterGraph() as IFilterGraph2;

            try
            {
                //フィルタグラフにキャプチャを追加して、capFilterにピンを受け取る。
                hr = m_FilterGraph.AddSourceFilterForMoniker(this.m_dev.Mon, null, this.m_dev.Name, out capFilter);
                DsError.ThrowExceptionForHR(hr);

                //stillピンを探す
                m_pinStill = DsFindPin.ByCategory(capFilter, PinCategory.Still, 0);
                //stillピンが無ければPreviewを探す。
                if (m_pinStill == null)
                {
                    m_pinStill = DsFindPin.ByCategory(capFilter, PinCategory.Preview, 0);
                }
                // Still haven't found one.  Need to put a splitter in so we have
                // one stream to capture the bitmap from, and one to display.  Ok, we
                // don't *have* to do it that way, but we are going to anyway.
                if (m_pinStill == null)
                {
                    // There is no still pin
                    m_VidControl = null;
                    m_pinStill = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);

                }else{
                    // Get a control pointer (used in Click())
                    m_VidControl = capFilter as IAMVideoControl;

                    m_pinStill = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);
                }

                if (i_height + i_width + BBP > 0)
                {
                    SetConfigParms(m_pinStill, i_width, i_height,i_frame_rate, BBP);
                }

                // Get the SampleGrabber interface
                sampGrabber = new SampleGrabber() as ISampleGrabber;

                //sampGrabberの設定
                IBaseFilter baseGrabFlt = sampGrabber as IBaseFilter;
                ConfigureSampleGrabber(sampGrabber);

                pSampleIn = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Input, 0);

                // Add the sample grabber to the graph
                hr = m_FilterGraph.AddFilter(baseGrabFlt, "Ds.NET Grabber");
                DsError.ThrowExceptionForHR(hr);

                if (m_VidControl == null)
                {
                    // Connect the Still pin to the sample grabber
                    hr = m_FilterGraph.Connect(m_pinStill, pSampleIn);
                    DsError.ThrowExceptionForHR(hr);

                }else{
                    // Connect the Still pin to the sample grabber
                    hr = m_FilterGraph.Connect(m_pinStill, pSampleIn);
                    DsError.ThrowExceptionForHR(hr);
                }

                //CaptureHolderを作成
                VideoInfoHeader out_vih = GetOutVideoInfoHeader(sampGrabber);
                this.m_video_info = out_vih;
            }
            finally
            {
                if (sampGrabber != null)
                {
                    Marshal.ReleaseComObject(sampGrabber);
                    sampGrabber = null;
                }
                if (pSampleIn != null)
                {
                    Marshal.ReleaseComObject(pSampleIn);
                    pSampleIn = null;
                }
            }
        }
        public void StartCapture()
        {
            //ビデオ情報が無ければ準備がマダってことで。
            if (this.m_video_info == null){
                throw new Exception();
            }
            // Start the graph
            IMediaControl mediaCtrl = m_FilterGraph as IMediaControl;
            int hr = mediaCtrl.Run();
            DsError.ThrowExceptionForHR(hr);
            //インスタンスの状態を変更
            this.m_graphi_active = true;
        }
        public void StopCapture()
        {
            //既に停止中なら諦める。
            if (!this.m_graphi_active)
            {
                throw new Exception();
            }
            int hr;
            if (this.m_FilterGraph != null)
            {
                IMediaControl mediaCtrl = this.m_FilterGraph as IMediaControl;

                // Stop the graph
                hr = mediaCtrl.Stop();
            }
            //インスタンスの状態を変更
            this.m_graphi_active = false;
        }

        private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
        {
            int hr;
            AMMediaType media = new AMMediaType();

            // Set the media type to Video/RBG24
            media.majorType = MediaType.Video;
            media.subType   = MediaSubType.RGB32;
            media.formatType= FormatType.VideoInfo;
            hr = sampGrabber.SetMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            DsUtils.FreeAMMediaType(media);
            media = null;

            // Configure the samplegrabber
            hr = sampGrabber.SetCallback(this, 1);
            DsError.ThrowExceptionForHR(hr);
        }

        // Set the Framerate, and video size
        private void SetConfigParms(IPin pStill, int iWidth, int iHeight,float i_frame_rate, short iBPP)
        {
            int hr;
            AMMediaType media;
            VideoInfoHeader v;

            IAMStreamConfig videoStreamConfig = pStill as IAMStreamConfig;

            // Get the existing format block
            hr = videoStreamConfig.GetFormat(out media);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // copy out the videoinfoheader
                v = new VideoInfoHeader();
                Marshal.PtrToStructure(media.formatPtr, v);

                // if overriding the width, set the width
                if (iWidth > 0)
                {
                    v.BmiHeader.Width = iWidth;
                }

                // if overriding the Height, set the Height
                if (iHeight > 0)
                {
                    v.BmiHeader.Height = iHeight;
                }

                // if overriding the bits per pixel
                if (iBPP > 0)
                {
                    v.BmiHeader.BitCount = iBPP;
                }
                if (i_frame_rate > 0)
                {
                    //フレームレート☆
                    v.AvgTimePerFrame =(long)( 10000000 / i_frame_rate);
                }

                // Copy the media structure back
                Marshal.StructureToPtr(v, media.formatPtr, false);

                // Set the new format
                hr = videoStreamConfig.SetFormat(media);
                DsError.ThrowExceptionForHR(hr);
            }
            finally
            {
                DsUtils.FreeAMMediaType(media);
                media = null;
            }
        }
        private VideoInfoHeader GetOutVideoInfoHeader(ISampleGrabber sampGrabber)
        {
            // Get the media type from the SampleGrabber
            AMMediaType media = new AMMediaType();

            int hr;
            hr = sampGrabber.GetConnectedMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
            {
                throw new NotSupportedException("Unknown Grabber Media Format");
            }

            // Grab the size info
            VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
            DsUtils.FreeAMMediaType(media);
            media = null;
            return videoInfoHeader;
        }
        /// <summary> sample callback, NOT USED. </summary>
        int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample pSample)
        {
            Marshal.ReleaseComObject(pSample);
            return 0;
        }

        /// <summary> buffer callback, COULD BE FROM FOREIGN THREAD. </summary>
        int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            // Note that we depend on only being called once per call to Click.  Otherwise
            // a second call can overwrite the previous image.

            this.m_cap_listener.OnBuffer(this, SampleTime, pBuffer, BufferLen);
            return 0;
        }
    }


}
