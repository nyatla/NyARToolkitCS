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


    public partial class SimpleLiteD3d : IDisposable,INySampleCB
    {
        DeviceAdapter m_dev_adapter;
        /*  exeファイルのあるディレクトリからの相対パス
         *  デバックの時は、exeファイルはprogram files\[project name]に配置されるみたい。
         */
        private const String AR_CODE_FILE = "data\\patt.hiro";
        private const String AR_CAMERA_FILE = "data\\camera_para.dat";
        private D3dManager _d3dmgr;
        private ID3dBackground _back_ground;
        //NyAR
        private NyARSingleDetectMarker_Quad m_ar;
        private DsRGB565Raster m_raster;
        private NyARD3dUtil _utils = new NyARD3dUtil();


        // 頂点バッファ/インデックスバッファ/インデックスバッファの各頂点番号配列
        private VertexBuffer _vertexBuffer = null;
        private IndexBuffer _indexBuffer = null;
        private bool m_is_turn_vertical;
        private static Int16[] _vertexIndices = new Int16[] { 2, 0, 1, 1, 3, 2, 4, 0, 2, 2, 6, 4, 5, 1, 0, 0, 4, 5, 7, 3, 1, 1, 5, 7, 6, 2, 3, 3, 7, 6, 4, 6, 7, 7, 5, 4 };
        private Brush _fps_brush = new SolidBrush(Color.Red);
        private System.Drawing.Font _fps_font = new System.Drawing.Font(FontFamily.GenericSerif, 9.0f, FontStyle.Bold);
        public int fps_x_100=0;
        private Matrix trans_matrix = new Matrix();
        private NyARTransMatResult trans_result = new NyARTransMatResult();

        /* 非同期イベントハンドラ
         * CaptureDeviceからのイベントをハンドリングして、バッファとテクスチャを更新する。
         */
        public int OnSample(INySample i_sample)
        {
            lock (this)
            {
                IntPtr data=i_sample.GetData();
                this.m_raster.setBuffer(data, this.m_is_turn_vertical);
                //テクスチャ内容を更新
                this._back_ground.setSample(i_sample);
//                Graphics gs=this._surface.d3d_surface.GetGraphics();
//                gs.DrawString(fps_x_100 / 100 + "." + fps_x_100%100+"fps", _fps_font, _fps_brush, 0, 0);
//                this.m_surface.d3d_surface.ReleaseGraphics();
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
            return 0;
        }
        /* キャプチャを開始する関数
         */
        public void StartCap()
        {
            this.m_dev_adapter.CaptureIf.Start();
        }
        /* キャプチャを停止する関数
         */
        public void StopCap()
        {
            this.m_dev_adapter.CaptureIf.Stop();
        }

        public bool InitializeApplication(NyARToolkitCS topLevelForm, DeviceAdapter i_adapter)
        {
            NyMath.initialize();
            String current_path=Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            this.m_dev_adapter = i_adapter;
            Size cap_size = i_adapter.CaptureSize;
            this.m_is_turn_vertical = i_adapter.IsTurnCapVertical;

            NyARParam ap = new NyARParam();
            ap.loadARParamFromFile(current_path + "\\" + AR_CAMERA_FILE);
            ap.changeScreenSize(cap_size.Width,cap_size.Height);


            this._d3dmgr = new D3dManager(topLevelForm,ap, -1);
            this._back_ground = new D3dTextureBackground(this._d3dmgr, -1);
            //ARの設定

            //カメラProjectionの設定
            Matrix tmp = new Matrix();
            this._utils.toCameraFrustumRH(ap, ref tmp);
            this._d3dmgr.d3d_device.Transform.Projection = tmp;

            //AR用カメラパラメタファイルをロードして設定

            //AR用のパターンコードを読み出し	
            NyARCode code = new NyARCode(16, 16);
            code.loadARPattFromFile(current_path+"\\"+AR_CODE_FILE);

            //１パターンのみを追跡するクラスを作成
            this.m_ar = new NyARSingleDetectMarker_Quad(ap, code, 80.0);
            this._utils = new NyARD3dUtil();
            //計算モードの設定
            this.m_ar.setContinueMode(false);

            //立方体（頂点数8）の準備
            this._vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColored),
                8, this._d3dmgr.d3d_device, Usage.None, CustomVertex.PositionColored.Format,Pool.SystemMemory);

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
            this._indexBuffer = new IndexBuffer(this._d3dmgr.d3d_device, 12 * 3 * 2, Usage.WriteOnly,Pool.SystemMemory, true);

            // インデックスバッファをロックする
            using (GraphicsStream data = this._indexBuffer.Lock(0, 0, LockFlags.None))
            {
                // インデックスデータをインデックスバッファにコピーします
                data.Write(_vertexIndices);

                // インデックスバッファのロックを解除します
                this._indexBuffer.Unlock();
            }

            //ARラスタを作る(DirectShowキャプチャ仕様)。
            this.m_raster = new DsRGB565Raster(cap_size.Width, cap_size.Height);
//            //背景テクスチャを作成
//            this.m_surface = new NyARSurface_RGB565(this._d3dmgr.d3d_device, cap_size.Width, cap_size.Height);
            return true;
        }
        private bool is_marker_enable=false;
        //メインループ処理
        public void MainLoop()
        {

            //ARの計算
            lock (this)
            {
                this._d3dmgr.d3d_device.Clear(ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
//                //背景描画
                // 3Dオブジェクトの描画はここから
                this._d3dmgr.d3d_device.BeginScene();
                this._back_ground.drawBackGround();


                //マーカーが見つかっていて、0.3より一致してたら描画する。
                if (is_marker_enable && this.m_ar.getConfidence() > 0.3)
                {
                    // 頂点バッファをデバイスのデータストリームにバインド
                    this._d3dmgr.d3d_device.SetStreamSource(0, this._vertexBuffer, 0);

                    // インデックスバッファをセット
                    this._d3dmgr.d3d_device.Indices = this._indexBuffer;

                    //立方体を20mm上（マーカーの上）にずらしておく
                    Matrix transform_mat2 = Matrix.Translation(0, 0, 20.0f);

                    //変換行列を掛ける
                    transform_mat2 *= trans_matrix;
                    // 計算したマトリックスで座標変換
                    this._d3dmgr.d3d_device.SetTransform(TransformType.World, transform_mat2);

                    // レンダリング（描画）
                    this._d3dmgr.d3d_device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
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
