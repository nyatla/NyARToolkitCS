/*
 * NyARToolkitCSUtils NyARToolkit for C#
 * SimpleLiteDirect3d for WindowsMobile
 * 
 * (c)2008 nyatla
 * airmail(at)ebony.plala.or.jp
 * http://nyatla.jp/
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
using NyARToolkitCSUtils.Raster;
using NyARToolkitCSUtils.NyAR;
using NyARToolkitCSUtils.Direct3d;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.raster;
using jp.nyatla.nyartoolkit.cs.detector;
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

        //NyAR
        private D3dSingleDetectMarker m_ar;
        private DsRGB565Raster m_raster;
        //背景テクスチャ
        private NyARSurface_RGB565 m_surface;
        //変数作りおき
        private Rectangle m_dest_rect;
        /// Direct3D デバイス
        private Device _device = null;
        // 頂点バッファ/インデックスバッファ/インデックスバッファの各頂点番号配列
        private VertexBuffer _vertexBuffer = null;
        private IndexBuffer _indexBuffer = null;
        private bool m_is_turn_vertical;
        private static Int16[] _vertexIndices = new Int16[] { 2, 0, 1, 1, 3, 2, 4, 0, 2, 2, 6, 4, 5, 1, 0, 0, 4, 5, 7, 3, 1, 1, 5, 7, 6, 2, 3, 3, 7, 6, 4, 6, 7, 7, 5, 4 };
        /* 非同期イベントハンドラ
         * CaptureDeviceからのイベントをハンドリングして、バッファとテクスチャを更新する。
         */
        public int OnSample(INySample i_sample)
        {
            lock (this)
            {
                IntPtr data=i_sample.GetData();
                this.m_raster.setBuffer(data);
                //テクスチャ内容を更新
                this.m_surface.CopyFromIntPtr(i_sample, this.m_is_turn_vertical);
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


        /* Direct3Dデバイスを準備する関数
         */
        private Device PrepareD3dDevice(Control i_window)
        {
            PresentParameters pp = new PresentParameters();

            // ウインドウモードなら true、フルスクリーンモードなら false を指定
            pp.Windowed = true;
            // スワップとりあえずDiscardを指定。
            pp.SwapEffect = SwapEffect.Flip;
            //pp.BackBufferCount = 1;

            return new Device(0, DeviceType.Default, i_window.Handle,CreateFlags.None, pp);
        }
        public bool InitializeApplication(NyARToolkitCS topLevelForm, DeviceAdapter i_adapter)
        {
            String current_path=Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            this.m_dev_adapter = i_adapter;
            Size cap_size = i_adapter.CaptureSize;
            this.m_is_turn_vertical = i_adapter.IsTurnCapVertical;
            
            //ARの設定

            //AR用カメラパラメタファイルをロードして設定
            D3dARParam ap = new D3dARParam();
            ap.loadFromARFile(current_path + "\\" + AR_CAMERA_FILE);
            ap.changeSize(cap_size.Width,cap_size.Height);

            //AR用のパターンコードを読み出し	
            NyARCode code = new NyARCode(16, 16);
            code.loadFromARFile(current_path+"\\"+AR_CODE_FILE);

            //１パターンのみを追跡するクラスを作成
            this.m_ar = new D3dSingleDetectMarker(ap, code, 80.0);

            //計算モードの設定
            this.m_ar.setContinueMode(false);

            
            //3dデバイスを準備する
            this._device = PrepareD3dDevice(topLevelForm);
            //ビューポート計算とスケールの計算
            Viewport vp=this.m_dev_adapter.D3dViewport;
            this._device.Viewport = vp;

            this.m_dest_rect = new Rectangle(vp.X, vp.Y, vp.Width, vp.Height);

            //カメラProjectionの設定
            this._device.Transform.Projection = ap.getCameraFrustumRH();

            // ビュー変換の設定(左手座標系ビュー行列で設定する)
            // 0,0,0から、Z+方向を向いて、上方向がY軸
            this._device.Transform.View = Matrix.LookAtLH(
                new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f));

            //立方体（頂点数8）の準備
            this._vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColored),
                8, this._device, Usage.None, CustomVertex.PositionColored.Format,Pool.SystemMemory);

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
            this._indexBuffer = new IndexBuffer(this._device, 12 * 3 * 2, Usage.WriteOnly,Pool.SystemMemory, true);

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

            //ARラスタを作る(DirectShowキャプチャ仕様)。
            this.m_raster = new DsRGB565Raster(cap_size.Width, cap_size.Height, this.m_dev_adapter.IsTurnCapVertical);
            //背景テクスチャを作成
            this.m_surface = new NyARSurface_RGB565(this._device, cap_size.Width, cap_size.Height);
            return true;
        }

        //メインループ処理
        public void MainLoop()
        {

            //ARの計算
            Matrix trans_matrix = new Matrix();
            bool is_marker_enable;
            lock (this)
            {
                //マーカーは見つかったかな？
                is_marker_enable = this.m_ar.detectMarkerLite(this.m_raster, 110);
                if (is_marker_enable)
                {
                    //あればMatrixを計算
                    this.m_ar.getD3dMatrix(out trans_matrix);
                }
                
                //背景描画
                Surface dest_surface=this._device.GetBackBuffer(0, BackBufferType.Mono);
                this._device.StretchRectangle(this.m_surface.d3d_surface, this.m_surface.d3d_surface_rect, dest_surface, this.m_dest_rect, TextureFilter.Point);

                // 3Dオブジェクトの描画はここから
                this._device.BeginScene();




                //マーカーが見つかっていて、0.4より一致してたら描画する。
                if (is_marker_enable && this.m_ar.getConfidence() > 0.4)
                {
                    // 頂点バッファをデバイスのデータストリームにバインド
                    this._device.SetStreamSource(0, this._vertexBuffer, 0);

                    //                // 描画する頂点のフォーマットをセット
                    //                this._device.VertexFormat = CustomVertex.PositionColored.Format;

                    // インデックスバッファをセット
                    this._device.Indices = this._indexBuffer;

                    //立方体を20mm上（マーカーの上）にずらしておく
                    Matrix transform_mat2 = Matrix.Translation(0, 0, 20.0f);

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
