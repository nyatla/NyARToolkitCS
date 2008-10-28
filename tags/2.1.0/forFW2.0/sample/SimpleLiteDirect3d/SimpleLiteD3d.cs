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
using NyARToolkitCSUtils.NyAR;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.detector;

namespace SimpleLiteDirect3d
{

    public partial class SimpleLiteD3d : IDisposable, CaptureListener
    {
        private const int SCREEN_WIDTH=320;
        private const int SCREEN_HEIGHT=240;
        private const String AR_CODE_FILE = "../../../../../data/patt.hiro";
        private const String AR_CAMERA_FILE = "../../../../../data/camera_para.dat";
        //DirectShowからのキャプチャ
        private CaptureDevice  _cap;
        //NyAR
        private NyARSingleDetectMarker _ar;
        private DsBGRX32Raster _raster;
        private NyARD3dUtil _utils;
        //背景テクスチャ
        private NyARSurface_XRGB32 _surface;
        /// Direct3D デバイス
        private Device _device = null;
        // 頂点バッファ/インデックスバッファ/インデックスバッファの各頂点番号配列
        private VertexBuffer _vertexBuffer = null;
        private IndexBuffer _indexBuffer = null;
        private static Int16[] _vertexIndices = new Int16[] { 2, 0, 1, 1, 3, 2, 4, 0, 2, 2, 6, 4, 5, 1, 0, 0, 4, 5, 7, 3, 1, 1, 5, 7, 6, 2, 3, 3, 7, 6, 4, 6, 7, 7, 5, 4 };
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
                this._raster.setBuffer(i_buffer);

                //テクスチャ内容を更新
                this._surface.CopyFromXRGB32(this._raster);
            }
            return;
        }
        /* キャプチャを開始する関数
         */
        public void StartCap()
        {
            this._cap.StartCapture();
        }
        /* キャプチャを停止する関数
         */
        public void StopCap()
        {
            this._cap.StopCapture();
        }


        /* Direct3Dデバイスを準備する関数
         */
        private Device PrepareD3dDevice(Control i_window)
        {
            PresentParameters pp = new PresentParameters();

            // ウインドウモードなら true、フルスクリーンモードなら false を指定
            pp.Windowed = true;
            // スワップとりあえずDiscardを指定。
            pp.SwapEffect = SwapEffect.Flip;
            pp.BackBufferFormat = Format.X8R8G8B8;
            pp.BackBufferCount = 1;
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
            //キャプチャを作る(QVGAでフレームレートは30)
            i_cap_device.SetCaptureListener(this);
            i_cap_device.PrepareCapture(SCREEN_WIDTH, SCREEN_HEIGHT, 30);
            this._cap = i_cap_device;
            
            //ARの設定

            //ARラスタを作る(DirectShowキャプチャ仕様)。
            this._raster = new DsBGRX32Raster(i_cap_device.video_width, i_cap_device.video_height, i_cap_device.video_width * i_cap_device.video_bit_count / 8);

            //AR用カメラパラメタファイルをロードして設定
            NyARParam ap = new NyARParam();
            ap.loadARParamFromFile(AR_CAMERA_FILE);
            ap.changeScreenSize(SCREEN_WIDTH, SCREEN_HEIGHT);

            //AR用のパターンコードを読み出し	
            NyARCode code = new NyARCode(16, 16);
            code.loadARPattFromFile(AR_CODE_FILE);

            //１パターンのみを追跡するクラスを作成
            this._ar = new NyARSingleDetectMarker(ap, code, 80.0);
            
            //Direct3d用のユーティリティ準備
            this._utils = new NyARD3dUtil();

            //計算モードの設定
            this._ar.setContinueMode(false);

            //3dデバイスを準備する
            this._device = PrepareD3dDevice(topLevelForm);

            //カメラProjectionの設定
            Matrix tmp = new Matrix();
            this._utils.toCameraFrustumRH(ap, ref tmp);
            this._device.Transform.Projection = tmp;

            // ビュー変換の設定(左手座標系ビュー行列で設定する)
            // 0,0,0から、Z+方向を向いて、上方向がY軸
            this._device.Transform.View = Matrix.LookAtLH(
                new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f));

            //立方体（頂点数8）の準備
            this._vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColored),
                8, this._device, Usage.None, CustomVertex.PositionColored.Format, Pool.Managed);

            //8点の情報を格納するためのメモリを確保
            CustomVertex.PositionColored[] vertices = new CustomVertex.PositionColored[8];
            const float CUBE_SIZE = 20.0f;//1辺40[mm]の
            //頂点を設定
            vertices[0] = new CustomVertex.PositionColored(-CUBE_SIZE, CUBE_SIZE, CUBE_SIZE, Color.Yellow.ToArgb());
            vertices[1] = new CustomVertex.PositionColored(CUBE_SIZE, CUBE_SIZE, CUBE_SIZE, Color.Gray.ToArgb());
            vertices[2] = new CustomVertex.PositionColored(-CUBE_SIZE, CUBE_SIZE, -CUBE_SIZE, Color.Purple.ToArgb());
            vertices[3] = new CustomVertex.PositionColored(CUBE_SIZE, CUBE_SIZE, -CUBE_SIZE, Color.Red.ToArgb());
            vertices[4] = new CustomVertex.PositionColored(-CUBE_SIZE, -CUBE_SIZE, CUBE_SIZE, Color.SkyBlue.ToArgb());
            vertices[5] = new CustomVertex.PositionColored(CUBE_SIZE, -CUBE_SIZE, CUBE_SIZE, Color.Orange.ToArgb());
            vertices[6] = new CustomVertex.PositionColored(-CUBE_SIZE, -CUBE_SIZE, -CUBE_SIZE, Color.Green.ToArgb());
            vertices[7] = new CustomVertex.PositionColored(CUBE_SIZE, -CUBE_SIZE, -CUBE_SIZE, Color.Blue.ToArgb());

            //頂点バッファをロックする
            using (GraphicsStream data = this._vertexBuffer.Lock(0, 0, LockFlags.None))
            {
                // 頂点データを頂点バッファにコピーします
                data.Write(vertices);

                // 頂点バッファのロックを解除します
                this._vertexBuffer.Unlock();
            }

            // インデックスバッファの作成
            // 第２引数の数値は(三角ポリゴンの数)*(ひとつの三角ポリゴンの頂点数)*
            // (16 ビットのインデックスサイズ(2byte))
            this._indexBuffer = new IndexBuffer(this._device, 12 * 3 * 2, Usage.WriteOnly,
                Pool.Managed, true);

            // インデックスバッファをロックする
            using (GraphicsStream data = this._indexBuffer.Lock(0, 0, LockFlags.None))
            {
                // インデックスデータをインデックスバッファにコピーします
                data.Write(_vertexIndices);

                // インデックスバッファのロックを解除します
                this._indexBuffer.Unlock();
            }
            // ライトを無効
            this._device.RenderState.Lighting = false;

            // カリングを無効にしてポリゴンの裏も描画する
            //this._device.RenderState.CullMode = Cull.None;

            //背景サーフェイスを作成
            this._surface = new NyARSurface_XRGB32(this._device, SCREEN_WIDTH, SCREEN_HEIGHT);

            return true;
        }
        private Matrix __MainLoop_trans_matrix = new Matrix();
        private NyARTransMatResult __MainLoop_nyar_transmat = new NyARTransMatResult();
        //メインループ処理
        public void MainLoop()
        {
            //ARの計算
            Matrix trans_matrix = this.__MainLoop_trans_matrix;
            NyARTransMatResult trans_result = this.__MainLoop_nyar_transmat;
            bool is_marker_enable;
            lock (this)
            {
                //マーカーは見つかったかな？
                is_marker_enable = this._ar.detectMarkerLite(this._raster, 110);
                if (is_marker_enable)
                {
                    //あればMatrixを計算
                    this._ar.getTransmationMatrix(trans_result);
                    this._utils.toD3dMatrix(trans_result,ref trans_matrix);
                }

                // 背景サーフェイスを直接描画
                Surface dest_surface = this._device.GetBackBuffer(0, 0, BackBufferType.Mono);
                Rectangle src_dest_rect = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
                this._device.StretchRectangle(this._surface.d3d_surface, src_dest_rect, dest_surface, src_dest_rect, TextureFilter.None);

                // 3Dオブジェクトの描画はここから
                this._device.BeginScene();


                //マーカーが見つかっていて、0.4より一致してたら描画する。
                if (is_marker_enable && this._ar.getConfidence()>0.4)
                {
                    // 頂点バッファをデバイスのデータストリームにバインド
                    this._device.SetStreamSource(0, this._vertexBuffer, 0);

                    // 描画する頂点のフォーマットをセット
                    this._device.VertexFormat = CustomVertex.PositionColored.Format;

                    // インデックスバッファをセット
                    this._device.Indices = this._indexBuffer;

                    //立方体を20mm上（マーカーの上）にずらしておく
                    Matrix transform_mat2 = Matrix.Translation(0,0,20.0f);

                    //変換行列を掛ける
                    transform_mat2 *= trans_matrix;
                    // 計算したマトリックスで座標変換
                    this._device.SetTransform(TransformType.World, transform_mat2);

                    // レンダリング（描画）
                    this._device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
                }

                // 描画はここまで
                this._device.EndScene();

                // 実際のディスプレイに描画
                this._device.Present();
            }
            
        }

        // リソースの破棄をするために呼ばれる
        public void Dispose()
        {
            // 頂点バッファを解放
            if (this._vertexBuffer != null)
            {
                this._vertexBuffer.Dispose();
            }

            // インデックスバッファを解放
            if (this._indexBuffer != null)
            {
                this._indexBuffer.Dispose();
            }              
            // Direct3D デバイスのリソース解放
            if (this._device != null)
            {
                this._device.Dispose();
            }
        }
    }
}
