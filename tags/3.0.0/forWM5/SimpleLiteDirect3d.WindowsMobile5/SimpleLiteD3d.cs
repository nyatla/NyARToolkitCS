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
using System.Reflection;
using Microsoft.WindowsMobile.DirectX;
using Microsoft.WindowsMobile.DirectX.Direct3D;
using NyARToolkitCSUtils.WMCapture;
using NyARToolkitCSUtils.Direct3d;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.detector;
using jp.nyatla.nyartoolkit.cs.sandbox.quadx2;
using jp.nyatla.nyartoolkit.cs.sandbox.x2;
using jp.nyatla.cs.NyWMCapture;


namespace SimpleLiteDirect3d.WindowsMobile5
{


    public partial class SimpleLiteD3d : IDisposable, IWmCaptureListener
    {
        private D3dManager _d3dmgr;
        private ColorCube _d3dcube;
        private ID3dBackground _back_ground;
        private WmCapture _capture;
        //NyAR
        private NyARSingleDetectMarker m_ar;
        private DsRGB565Raster m_raster;

        public SimpleLiteD3d(NyARToolkitCS topLevelForm, ResourceBuilder i_resource)
        {
            NyMath.initialize();
            this._capture = i_resource.createWmCapture();
            this._capture.setOnSample(this);

            this._d3dmgr = i_resource.createD3dManager(topLevelForm);
            this._back_ground = i_resource.createBackGround(this._d3dmgr);
            this._d3dcube = new ColorCube(this._d3dmgr.d3d_device,40);


            //AR用のパターンコードを読み出
            NyARCode code = i_resource.createNyARCode();
            //ARラスタを作る(DirectShowキャプチャ仕様)。
            this.m_raster = i_resource.createARRaster();

            //１パターンのみを追跡するクラスを作成
            this.m_ar = new NyARSingleDetectMarker(i_resource.ar_param, code, 80.0, this.m_raster.getBufferType());
            //計算モードの設定
            this.m_ar.setContinueMode(false);

            ////立方体（頂点数8）の準備


            return;
        }


        /* 非同期イベントハンドラ
         * CaptureDeviceからのイベントをハンドリングして、バッファとテクスチャを更新する。
         */
        public void onSample(WmCapture i_sender, INySample i_sample)
        {
            lock (this)
            {
                IntPtr data=i_sample.GetData();
                this.m_raster.setBuffer(data, i_sender.vertical_flip);
                //テクスチャ内容を更新
                this._back_ground.CopyFromRaster(this.m_raster);
                //マーカーは見つかったかな？
                is_marker_enable = this.m_ar.detectMarkerLite(this.m_raster, 110);
                if (is_marker_enable)
                {
                    //あればMatrixを計算
                    this.m_ar.getTransmationMatrix(trans_result);
                    NyARD3dUtil.toD3dCameraView(trans_result,1, ref trans_matrix);
                }
            }
            Thread.Sleep(0);
            return;
        }
        /* キャプチャを開始する関数
         */
        public void start()
        {
            this._capture.start();
        }
        /* キャプチャを停止する関数
         */
        public void stop()
        {
            this._capture.stop();
        }

        private bool is_marker_enable=false;
        private Matrix trans_matrix = new Matrix();
        private NyARTransMatResult trans_result = new NyARTransMatResult();

        //メインループ処理
        public void MainLoop()
        {
            //ARの計算
            lock (this)
            {
                this._d3dmgr.d3d_device.Clear(ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                // 3Dオブジェクトの描画はここから
                this._d3dmgr.d3d_device.BeginScene();
                this._back_ground.drawBackGround(this._d3dmgr.d3d_device);


                //マーカーが見つかっていて、0.3より一致してたら描画する。
                if (is_marker_enable && this.m_ar.getConfidence() > 0.3)
                {

                    //立方体を20mm上（マーカーの上）にずらしておく
                   Matrix transform_mat2 = Matrix.Translation(0, 0, 20.0f);

                    //変換行列を掛ける
                    transform_mat2 *= trans_matrix;
                    // 計算したマトリックスで座標変換
                    this._d3dmgr.d3d_device.SetTransform(TransformType.World, transform_mat2);
                    //描画
                    this._d3dcube.draw(this._d3dmgr.d3d_device);
                }

                // 描画はここまで
                this._d3dmgr.d3d_device.EndScene();

                // 実際のディスプレイに描画*/
                this._d3dmgr.d3d_device.Present();
            }            
        }

        // リソースの破棄をするために呼ばれる
        public void Dispose()
        {
            //キャプチャ解除
            if (this._capture != null)
            {
                this._capture.Dispose();
            }
            if(this._d3dcube!=null){
                this._d3dcube.Dispose();
            }
            if (this._back_ground != null)
            {
                this._back_ground.Dispose();
            }
            // Direct3D デバイスのリソース解放
            if (this._d3dmgr != null)
            {
                this._d3dmgr.Dispose();
            }
        }
    }
}
