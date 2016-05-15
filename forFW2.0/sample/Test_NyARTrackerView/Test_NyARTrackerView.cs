/* 
 * Capture Test NyARToolkitCSサンプルプログラム
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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NyARToolkitCSUtils.Capture;
using NyARToolkitCSUtils;

using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.detector;
using jp.nyatla.nyartoolkit.cs.rpf;

namespace Test_NyARTrackerView
{

    public partial class Test_NyARTrackerView : IDisposable, CaptureListener
    {
        private const int SCREEN_WIDTH=320;
        private const int SCREEN_HEIGHT=240;
        //DirectShowからのキャプチャ
        private CaptureDevice  _cap;
        //NyAR
        private DsRgbRaster _raster;

        /* 非同期イベントハンドラ
          * CaptureDeviceからのイベントをハンドリングして、バッファとテクスチャを更新する。
          */
        public void OnBuffer(CaptureDevice i_sender, double i_sample_time, IntPtr i_buffer, int i_buffer_len)
        {
            int w = i_sender.video_width;
            int h = i_sender.video_height;
            int s = w * (i_sender.video_bit_count / 8);
            
            //テクスチャにRGBを取り込み()
            lock (this)
            {
                //カメラ映像をARのバッファにコピー
                this._raster.setBuffer(i_buffer,i_buffer_len,i_sender.video_vertical_flip);
                this.filter.convert(this.gs);
                this.tracksource.wrapBuffer(this.gs);
                this.tracker.progress(this.tracksource);

            }
            return;
        }
        /* キャプチャを開始する関数
         */
        public void StartCap()
        {
            this._cap.StartCapture();
            return;
        }
        /* キャプチャを停止する関数
         */
        public void StopCap()
        {
            this._cap.StopCapture();
            return;
        }
        private Form1 _top_form;
        NyARTrackerSource_Reference tracksource;
        NyARTracker tracker;
        INyARRgb2GsFilter filter;
        NyARGrayscaleRaster gs;
        public bool InitializeApplication(Form1 topLevelForm, CaptureDevice i_cap_device)
        {
            this._top_form = topLevelForm;
            topLevelForm.ClientSize=new Size(SCREEN_WIDTH,SCREEN_HEIGHT);
            //キャプチャを作る(QVGAでフレームレートは30)
            i_cap_device.SetCaptureListener(this);
            i_cap_device.PrepareCapture(SCREEN_WIDTH, SCREEN_HEIGHT, 30);
            this._cap = i_cap_device;
            
            //ARの設定

            //ARラスタを作る(DirectShowキャプチャ仕様)。
            this._raster = new DsRgbRaster(i_cap_device.video_width, i_cap_device.video_height,NyARBufferType.OBJECT_CS_Bitmap);

            this.gs = new NyARGrayscaleRaster(i_cap_device.video_width, i_cap_device.video_height);
            this.filter = NyARRgb2GsFilterFactory.createRgbAveDriver(this._raster);

            this.tracker = new NyARTracker(100, 1, 10);
            this.tracksource = new NyARTrackerSource_Reference(100, null, i_cap_device.video_width, i_cap_device.video_height,2, false);
            return true;
        }
        //メインループ処理
        public void MainLoop()
        {
            //状態の表示
            lock (this)
            {
                Graphics g = this._top_form.CreateGraphics();
                Graphics g2 = Graphics.FromImage(this._raster.getBitmap());


                for(int i=this.tracker._targets.getLength()-1;i>=0;i--){
    		        switch(this.tracker._targets.getItem(i)._st_type)
    		        {
    		        case NyARTargetStatus.ST_CONTURE:
                        drawContourTarget(this.tracker._targets.getItem(i), g2);
    			        break;
    		        case NyARTargetStatus.ST_IGNORE:
                        drawIgnoreTarget(this.tracker._targets.getItem(i), g2);
    			        break;
    		        case NyARTargetStatus.ST_NEW:
                        drawNewTarget(this.tracker._targets.getItem(i), g2);
    			        break;
    		        case NyARTargetStatus.ST_RECT:
                        drawRectTarget(this.tracker._targets.getItem(i), g2);
    			        break;
    		        }
    	        }
                g2.Dispose();
                g.DrawImage(this._raster.getBitmap(), 0, 0);
                g.Dispose();
            }
            return;
        }
        /**
         * RectTargetを表示します。
         */
        private void drawRectTarget(NyARTarget t,Graphics sink)
        {
            Font f = new Font("System", 14);
    	    //サンプリング結果の表示
		    NyARRectTargetStatus s=(NyARRectTargetStatus)t._ref_status;
            sink.DrawString("RT:" + t._serial + "(" + s.detect_type + ")" + "-" + t._delay_tick, f, Brushes.Cyan, new PointF(t._sample_area.x, t._sample_area.y));
            sink.DrawRectangle(Pens.Cyan, (int)s.vertex[0].x - 1, (int)s.vertex[0].y - 1, 2, 2);
		    for(int i2=0;i2<4;i2++){
//				g.fillRect((int)st.vecpos[i2].x-1, (int)st.vecpos[i2].y-1,2,2);
                sink.DrawLine(Pens.Cyan,
				    (int)s.vertex[i2].x,
				    (int)s.vertex[i2].y,
				    (int)s.vertex[(i2+1)%4].x,
				    (int)s.vertex[(i2+1)%4].y);
		    }
        }

        /**
         * ContourTargetを表示します。
         */
        private void drawContourTarget(NyARTarget t, Graphics sink)
        {
            Font f = new Font("System", 14);
            sink.DrawString("CT",f, Brushes.Blue, new PointF(t._sample_area.x, t._sample_area.y));
    //		g.drawRect(t._sample_area.x,t._sample_area.y,t._sample_area.w,t._sample_area.h);
		    NyARContourTargetStatus st=(NyARContourTargetStatus)t._ref_status;
		    VecLinearCoordinatesOperator vp=new VecLinearCoordinatesOperator();
		    vp.margeResembleCoords(st.vecpos);
		    for(int i2=0;i2<st.vecpos.length;i2++){
    //		for(int i2=43;i2<44;i2++){
    //			g.drawString(i2+":"+"-"+t._delay_tick,(int)st.vecpos.items[i2].x-1, (int)st.vecpos.items[i2].y-1);
                sink.FillRectangle(Brushes.Blue,(int)st.vecpos.items[i2].x, (int)st.vecpos.items[i2].y, 1, 1);
			    double co,si;
			    co=st.vecpos.items[i2].dx;
			    si=st.vecpos.items[i2].dy;
			    double p=Math.Sqrt(co*co+si*si);
			    co/=p;
			    si/=p;
			    double ss=st.vecpos.items[i2].scalar*3;
			    sink.DrawLine(
                    Pens.Blue,
				    (int)st.vecpos.items[i2].x,
				    (int)st.vecpos.items[i2].y,
				    (int)(co*ss)+(int)st.vecpos.items[i2].x,(int)(si*ss)+(int)st.vecpos.items[i2].y);
			    int xx=(int)st.vecpos.items[i2].x;
			    int yy=(int)st.vecpos.items[i2].y;
    //			g.drawRect(xx/8*8,yy/8*8,16,16);
    			
		    }
        }
        
        /**
         * IgnoreTargetを表示します。
         */
        private void drawIgnoreTarget(NyARTarget t, Graphics sink)
        {
            Font f = new Font("System", 14);
            //サンプリング結果の表示
            sink.DrawString("IG" + "-" + t._delay_tick, f, Brushes.Red, new PointF(t._sample_area.x, t._sample_area.y));
            sink.DrawRectangle(Pens.Red,t._sample_area.x, t._sample_area.y, t._sample_area.w, t._sample_area.h);
        }
            
        /**
         * Newtargetを表示します。
         */
        private void drawNewTarget(NyARTarget t, Graphics sink)
        {
            Font f = new Font("System", 14);
    	    //サンプリング結果の表示
            sink.DrawString("NW" + "-" + t._delay_tick, f, Brushes.Green, new PointF(t._sample_area.x, t._sample_area.y));
            sink.DrawRectangle(Pens.Green,t._sample_area.x, t._sample_area.y, t._sample_area.w, t._sample_area.h);
        }
        // リソースの破棄をするために呼ばれる
        public void Dispose()
        {
            lock (this)
            {
            }
        }
    }
}
