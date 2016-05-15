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
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using NyARToolkitCSUtils.Capture;
using NyARToolkitCSUtils.Direct3d;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.detector;
using jp.nyatla.nyartoolkit.cs.rpf;


namespace Test_NyARRealityD3d_ARMarker
{

    public partial class Test_NyARRealityD3d_ARMarker : IDisposable, CaptureListener
    {
        private const int SCREEN_WIDTH=320;
        private const int SCREEN_HEIGHT=240;
        private const String AR_HIRO_FILE = "../../../../../data/patt.hiro";
        private const String AR_CAMERA_FILE = "../../../../../data/camera_para.dat";
        //DirectShowからのキャプチャ
        private CaptureDevice _cap;
        /// Direct3D デバイス
        private Device _device = null;
        private ColorCube _cube;
        //背景テクスチャ
        private NyARD3dSurface _surface;
        private NyARRealityD3d _reality;
        private NyARRealitySource_DShow _reality_source;
        ARTKMarkerTable _mklib;
        /* 非同期イベントハンドラ
          * CaptureDeviceからのイベントをハンドリングして、バッファとテクスチャを更新する。
          */
        public void OnBuffer(CaptureDevice i_sender, double i_sample_time, IntPtr i_buffer, int i_buffer_len)
        {
            int w = i_sender.video_width;
            int h = i_sender.video_height;
            int s = w * (i_sender.video_bit_count / 8);
            
            lock (this)
            {
                //カメラ映像をARのバッファにコピー
                this._reality_source.setDShowImage(i_buffer,i_buffer_len,i_sender.video_vertical_flip);
                this._reality.progress(this._reality_source);
                //テクスチャ内容を更新
                this._surface.setRaster(this._reality_source.refRgbSource());

                //UnknownTargetを1個取得して、遷移を試す。
				NyARRealityTarget t=this._reality.selectSingleUnknownTarget();
				if(t==null){
					return;
				}
				//ターゲットに一致するデータを検索
				ARTKMarkerTable.GetBestMatchTargetResult r=new ARTKMarkerTable.GetBestMatchTargetResult();
				if(this._mklib.getBestMatchTarget(t,this._reality_source,r)){
					if(r.confidence<0.6)
					{	//一致率が低すぎる。
						return;
					}
					//既に認識しているターゲットの内側のものでないか確認する？(この処理をすれば、二重認識は無くなる。)
					
					//一致度を確認して、80%以上ならKnownターゲットへ遷移
					if(!this._reality.changeTargetToKnown(t,r.artk_direction,r.marker_width)){
					//遷移の成功チェック
						return;//失敗
					}
					//遷移に成功したので、tagにResult情報をコピーしておく。（後で表示に使う）
					t.tag=r;
				}else{
					//一致しないので、このターゲットは捨てる。
					this._reality.changeTargetToDead(t,15);
				}
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

        /* Direct3Dデバイスを準備する関数
         */
        private Device PrepareD3dDevice(Control i_window)
        {
            PresentParameters pp = new PresentParameters();
            pp.Windowed = true;
            pp.SwapEffect = SwapEffect.Flip;
            pp.BackBufferFormat = Format.X8R8G8B8;
            pp.BackBufferCount = 1;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = DepthFormat.D16;
            CreateFlags fl_base = CreateFlags.FpuPreserve;

            try{
                return new Device(0, DeviceType.Hardware, i_window.Handle, fl_base|CreateFlags.HardwareVertexProcessing, pp);
            }catch (Exception ex1){
                Debug.WriteLine(ex1.ToString());
                try{
                    return new Device(0, DeviceType.Hardware, i_window.Handle, fl_base | CreateFlags.SoftwareVertexProcessing, pp);
                }catch (Exception ex2){
                    // 作成に失敗
                    Debug.WriteLine(ex2.ToString());
                    try{
                        return new Device(0, DeviceType.Reference, i_window.Handle, fl_base | CreateFlags.SoftwareVertexProcessing, pp);
                    }catch (Exception ex3){
                        throw ex3;
                    }
                }
            }
        }

        public bool InitializeApplication(Form1 topLevelForm,CaptureDevice i_cap_device)
        {
            topLevelForm.ClientSize=new Size(SCREEN_WIDTH,SCREEN_HEIGHT);
            //キャプチャを作る(QVGAでフレームレートは30)
            i_cap_device.SetCaptureListener(this);
            i_cap_device.PrepareCapture(SCREEN_WIDTH, SCREEN_HEIGHT, 30);
            this._cap = i_cap_device;

            //AR用カメラパラメタファイルをロードして設定
            NyARParam ap =NyARParam.createFromARParamFile(new StreamReader(AR_CAMERA_FILE));
            ap.changeScreenSize(SCREEN_WIDTH, SCREEN_HEIGHT);

            //マーカライブラリ(ARTKId)の構築
            this._mklib = new ARTKMarkerTable(10, 16, 16, 25, 25, 4);
            //マーカテーブルの作成（1種類）
            this._mklib.addMarkerFromARPatt(new StreamReader(AR_HIRO_FILE), 0, "HIRO", 80, 80);

            //Realityの準備
            this._reality = new NyARRealityD3d(ap, 10, 10000, 2, 10);
            this._reality_source = new NyARRealitySource_DShow(SCREEN_WIDTH, SCREEN_HEIGHT, null, 2, 100);

            //3dデバイスを準備する
            this._device = PrepareD3dDevice(topLevelForm);
            this._device.RenderState.ZBufferEnable = true;
            this._device.RenderState.Lighting = false;



            //カメラProjectionの設定
            Matrix tmp = new Matrix();
            this._reality.getD3dCameraFrustum(ref tmp);
            this._device.Transform.Projection = tmp;

            // ビュー変換の設定(左手座標系ビュー行列で設定する)
            // 0,0,0から、Z+方向を向いて、上方向がY軸
            this._device.Transform.View = Matrix.LookAtLH(
                new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f));
            Viewport vp = new Viewport();
            vp.Y = 0;
            vp.Height = ap.getScreenSize().h;
            vp.Width = ap.getScreenSize().w;
            vp.MaxZ = 1.0f;
            //ビューポート設定
            this._device.Viewport = vp;

            //カラーキューブの描画インスタンス
            this._cube = new ColorCube(this._device, 40);
            //背景サーフェイスを作成
            this._surface = new NyARD3dSurface(this._device, SCREEN_WIDTH, SCREEN_HEIGHT);
            return true;
        }
        //メインループ処理
        public void MainLoop()
        {
            //ARの計算
            lock (this)
            {
                // 背景サーフェイスを直接描画
                Surface dest_surface = this._device.GetBackBuffer(0, 0, BackBufferType.Mono);
                Rectangle src_dest_rect = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
                this._device.StretchRectangle((Surface)this._surface, src_dest_rect, dest_surface, src_dest_rect, TextureFilter.None);

                // 3Dオブジェクトの描画はここから
                this._device.BeginScene();
                this._device.Clear(ClearFlags.ZBuffer, Color.DarkBlue, 1.0f, 0);


                //ターゲットリストを走査して、画面に内容を反映
                NyARRealityTargetList tl = this._reality.refTargetList();
                for (int i = tl.getLength() - 1; i >= 0; i--)
                {
                    NyARRealityTarget t = tl.getItem(i);
                    switch (t.getTargetType())
                    {
                        case NyARRealityTarget.RT_KNOWN:
                            //立方体を20mm上（マーカーの上）にずらしておく
                            Matrix mat=new Matrix();
                            this._reality.getD3dModelViewMatrix(t.refTransformMatrix(),ref mat);
                            Matrix transform_mat2 = Matrix.Translation(0, 0, 20.0f);
                            transform_mat2 = transform_mat2 * mat;
                            this._device.SetTransform(TransformType.World, transform_mat2);
                            this._cube.draw(this._device);
                            break;
                        case NyARRealityTarget.RT_UNKNOWN:
                            //NyARDoublePoint2d[] p = t.refTargetVertex();
                            //NyARGLDrawUtil.beginScreenCoordinateSystem(this._gl, SCREEN_X, SCREEN_Y, true);
                            //NyARGLDrawUtil.endScreenCoordinateSystem(this._gl);
                            break;
                    }
                }
                // 描画はここまで
                this._device.EndScene();

                // 実際のディスプレイに描画
                this._device.Present();
            }
            return;
        }

        // リソースの破棄をするために呼ばれる
        public void Dispose()
        {
            lock (this)
            {

                // 頂点バッファを解放
                if (this._cube != null)
                {
                    this._cube.Dispose();
                }
                if (this._surface != null)
                {
                    this._surface.Dispose();
                }
                // Direct3D デバイスのリソース解放
                if (this._device != null)
                {
                    this._device.Dispose();
                }
            }
        }
    }
}
