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
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using NyARToolkitCSUtils.Capture;
using NyARToolkitCSUtils.Direct3d;
using NyARToolkitCSUtils;
using jp.nyatla.nyartoolkit.cs.cs4;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.detector;

namespace SimpleLite_ImageSource
{

    public partial class SimpleLite_ImageSource : IDisposable
    {
        private const int SCREEN_WIDTH=320;
        private const int SCREEN_HEIGHT=240;
        private const String AR_CODE_FILE = "../../../../../data/patt.hiro";
        private const String AR_CAMERA_FILE = "../../../../../data/camera_para.dat";
        private const String TEST_IMAGE = "../../../../../data/320x240ABGR.png";
        //DirectShowからのキャプチャ
        //NyAR
        private NyARSingleDetectMarker _ar;
        private NyARBitmapRaster _raster;
        //背景テクスチャ
        private NyARD3dSurface _surface;
        /// Direct3D デバイス
        private Device _device = null;
        private ColorCube _cube;

        private NyARDoubleMatrix44 __OnBuffer_nyar_transmat = new NyARDoubleMatrix44();
        private bool _is_marker_enable;
        private Matrix _trans_mat;


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

        public bool InitializeApplication(Form1 topLevelForm)
        {
            topLevelForm.ClientSize=new Size(SCREEN_WIDTH,SCREEN_HEIGHT);
            this._raster = new NyARBitmapRaster(new Bitmap(TEST_IMAGE));
            

            //AR用カメラパラメタファイルをロードして設定
            NyARParam ap = NyARParam.loadFromARParamFile(File.OpenRead(AR_CAMERA_FILE),SCREEN_WIDTH, SCREEN_HEIGHT);

            //AR用のパターンコードを読み出し	
            NyARCode code = NyARCode.loadFromARPattFile(File.OpenRead(AR_CODE_FILE),16, 16);

            //１パターンのみを追跡するクラスを作成
            this._ar = NyARSingleDetectMarker.createInstance(ap, code, 80.0,NyARSingleDetectMarker.PF_NYARTOOLKIT);
            
            //計算モードの設定
            this._ar.setContinueMode(true);

            //3dデバイスを準備する
            this._device = PrepareD3dDevice(topLevelForm);
            this._device.RenderState.ZBufferEnable = true;
            this._device.RenderState.Lighting = false;


            //カメラProjectionの設定
            Matrix tmp = new Matrix();
            NyARD3dUtil.toCameraFrustumRH(ap, 10, 1000, ref tmp);
            this._device.Transform.Projection = tmp;

            // ビュー変換の設定(左手座標系ビュー行列で設定する)
            // 0,0,0から、Z+方向を向いて、上方向がY軸
            this._device.Transform.View = Matrix.LookAtLH(
                new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f));
            Viewport vp = new Viewport();
            vp.X = 0;
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

            NyARDoubleMatrix44 nyar_transmat = this.__OnBuffer_nyar_transmat;
            //マーカの認識
            bool is_marker_enable = this._ar.detectMarkerLite(this._raster, 110);
            if (is_marker_enable)
            {
                //あればMatrixを計算
                this._ar.getTransmationMatrix(nyar_transmat);
                NyARD3dUtil.toD3dCameraView(nyar_transmat, 1f, ref this._trans_mat);
            }
            this._is_marker_enable = is_marker_enable;
            //サーフェイスへ背景をコピー
            this._surface.setRaster(this._raster);
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


                //マーカーが見つかっていて、0.4より一致してたら描画する。
                if (this._is_marker_enable && this._ar.getConfidence()>0.4)
                {


                    //立方体を20mm上（マーカーの上）にずらしておく
                    Matrix transform_mat2 = Matrix.Translation(0f,0f,20.0f);

                    //変換行列を掛ける
                    transform_mat2 *= this._trans_mat;
                    // 計算したマトリックスで座標変換
                    this._device.SetTransform(TransformType.World, transform_mat2);

                    // レンダリング（描画）
                    this._cube.draw(this._device);
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
