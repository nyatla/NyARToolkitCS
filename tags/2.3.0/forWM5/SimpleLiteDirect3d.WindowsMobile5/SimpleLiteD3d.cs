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
using NyARToolkitCSUtils.NyAR;
using NyARToolkitCSUtils.Direct3d;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.detector;
using jp.nyatla.nyartoolkit.cs.sandbox.quadx2;
using jp.nyatla.nyartoolkit.cs.sandbox.x2;
using jp.nyatla.cs.NyWMCapture;


namespace SimpleLiteDirect3d.WindowsMobile5
{
    public class D3dCube:IDisposable
    {
        private D3dManager _d3dmgr;

        // 頂点バッファ/インデックスバッファ/インデックスバッファの各頂点番号配列
        private VertexBuffer _vertexBuffer = null;
        private IndexBuffer _indexBuffer = null;
        private static Int16[] _vertexIndices = new Int16[] { 2, 0, 1, 1, 3, 2, 4, 0, 2, 2, 6, 4, 5, 1, 0, 0, 4, 5, 7, 3, 1, 1, 5, 7, 6, 2, 3, 3, 7, 6, 4, 6, 7, 7, 5, 4 };
        public D3dCube(D3dManager i_d3dmgr)
        {
            this._d3dmgr = i_d3dmgr;
            Device dev = i_d3dmgr.d3d_device;
            //立方体（頂点数8）の準備
            this._vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColored),
                8, dev, Usage.None, CustomVertex.PositionColored.Format, Pool.SystemMemory);

            //8点の情報を格納するためのメモリを確保
            CustomVertex.PositionColored[] vertices = new CustomVertex.PositionColored[8];
            const float CUBE_SIZE = 20.0f;//1辺40[mm]
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
            this._indexBuffer = new IndexBuffer(dev, 12 * 3 * 2, Usage.WriteOnly, Pool.SystemMemory, true);

            // インデックスバッファをロックする
            using (GraphicsStream data = this._indexBuffer.Lock(0, 0, LockFlags.None))
            {
                // インデックスデータをインデックスバッファにコピーします
                data.Write(_vertexIndices);

                // インデックスバッファのロックを解除します
                this._indexBuffer.Unlock();
            }
            return;
        }
        public void draw()
        {
            Device dev = this._d3dmgr.d3d_device;
            // 頂点バッファをデバイスのデータストリームにバインド
            dev.SetStreamSource(0, this._vertexBuffer, 0);
            // インデックスバッファをセット
            dev.Indices = this._indexBuffer;
            // レンダリング（描画）
            dev.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
            return;
        }
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
            return;
        }
    }


    public partial class SimpleLiteD3d : IDisposable, IWmCaptureListener
    {
        private D3dManager _d3dmgr;
        private D3dCube _d3dcube;
        private ID3dBackground _back_ground;
        private WmCapture _capture;
        //NyAR
        private NyARSingleDetectMarker_X2 m_ar;
        private DsRGB565Raster m_raster;
        private NyARD3dUtil _utils = new NyARD3dUtil();

        public SimpleLiteD3d(NyARToolkitCS topLevelForm, ResourceBuilder i_resource)
        {
            NyMath.initialize();
            this._capture = i_resource.createWmCapture();
            this._capture.setOnSample(this);

            this._d3dmgr = i_resource.createD3dManager(topLevelForm);
            this._back_ground = i_resource.createBackGround(this._d3dmgr);
            this._d3dcube = new D3dCube(this._d3dmgr);


            //AR用のパターンコードを読み出
            NyARCode code = i_resource.createNyARCode();

            //１パターンのみを追跡するクラスを作成
            this.m_ar = new NyARSingleDetectMarker_X2(i_resource.ar_param, code, 80.0);
            this._utils = new NyARD3dUtil();
            //計算モードの設定
            this.m_ar.setContinueMode(false);

            ////立方体（頂点数8）の準備

            //ARラスタを作る(DirectShowキャプチャ仕様)。
            this.m_raster = i_resource.createARRaster();
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
                    this._utils.toD3dMatrix(trans_result, ref trans_matrix);
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


        public bool InitializeApplication(NyARToolkitCS topLevelForm, ResourceBuilder i_resource)
        {
            NyMath.initialize();


            this._d3dmgr = i_resource.createD3dManager(topLevelForm);
            this._back_ground = i_resource.createBackGround(this._d3dmgr);
            this._d3dcube = new D3dCube(this._d3dmgr);
            //ARの設定

            //AR用のパターンコードを読み出
            NyARCode code = i_resource.createNyARCode();

            //１パターンのみを追跡するクラスを作成
            this.m_ar = new NyARSingleDetectMarker_X2(i_resource.ar_param, code, 80.0);
            this._utils = new NyARD3dUtil();
            //計算モードの設定
            this.m_ar.setContinueMode(false);

            //ARラスタを作る(DirectShowキャプチャ仕様)。
            this.m_raster = i_resource.createARRaster();
            return true;
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
                this._back_ground.drawBackGround();
                this._d3dmgr.d3d_device.RenderState.CullMode = Cull.Clockwise;


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
                    this._d3dcube.draw();
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
