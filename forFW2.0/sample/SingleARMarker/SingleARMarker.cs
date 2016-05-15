using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.nyidmarker;
using jp.nyatla.nyartoolkit.cs.processor;
using NyARToolkitCSUtils.Capture;
using NyARToolkitCSUtils.Direct3d;
using NyARToolkitCSUtils;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace SingleARMarker
{
    /**
     * １個のRawBit-Idマーカを認識するロジッククラス。
     * detectMarker関数の呼び出しに同期して、transmatとcurrent_idパラメタを更新します。
     * 
     *
     */
    class MarkerProcessor : SingleARMarkerProcesser
    {
        public Matrix transmat = new Matrix();
        public int current_id = -1;

        public MarkerProcessor(NyARParam i_cparam, int i_raster_format)
        {
            //アプリケーションフレームワークの初期化
            initInstance(i_cparam);
            return;
        }
        /**
         * アプリケーションフレームワークのハンドラ（マーカ出現）
         */
        protected override void onEnterHandler(int i_code)
        {
            this.current_id = i_code;
        }
        /**
         * アプリケーションフレームワークのハンドラ（マーカ消滅）
         */
        protected override void onLeaveHandler()
        {
            this.current_id = -1;
            return;
        }
        /**
         * アプリケーションフレームワークのハンドラ（マーカ更新）
         */
        protected override void onUpdateHandler(NyARSquare i_square, NyARDoubleMatrix44 result)
        {
            NyARD3dUtil.toD3dCameraView(result,1f, ref this.transmat);
        }
    }

    public partial class SimpleLiteD3d : IDisposable, CaptureListener
    {
        private const int SCREEN_WIDTH = 640;
        private const int SCREEN_HEIGHT = 480;
        private const String AR_CODE_FILE1 = "../../../../../data/patt.hiro";
        private const String AR_CODE_FILE2 = "../../../../../data/patt.kanji";
        private const String AR_CAMERA_FILE = "../../../../../data/camera_para.dat";
        //DirectShowからのキャプチャ
        private CaptureDevice _cap;
        private MarkerProcessor _processor;
        //NyAR
        private DsRgbRaster _raster;
        //背景テクスチャ
        private NyARD3dSurface _surface;
        /// Direct3D デバイス
        private Device _device = null;
        //表示オブジェクト
        private TextPanel _text;

        private NyARDoubleMatrix44 __OnBuffer_nyar_transmat = new NyARDoubleMatrix44();
        /* 非同期イベントハンドラ
         * CaptureDeviceからのイベントをハンドリングして、バッファとテクスチャを更新する。
         */
        public void OnBuffer(CaptureDevice i_sender, double i_sample_time, IntPtr i_buffer, int i_buffer_len)
        {
            int w = i_sender.video_width;
            int h = i_sender.video_height;
            int s = w * (i_sender.video_bit_count / 8);
            //テクスチャにRGBを取り込み()
            lock (this)//このロックは、OnEnterとOnUpdateの間でMainLoopがtransmatを参照することを防ぎます。
            {
                //カメラ映像をARのバッファにコピー
                this._raster.setBuffer(i_buffer,i_buffer_len, i_sender.video_vertical_flip);
                //フレームワークに画像を転送
                this._processor.detectMarker(this._raster);
                //テクスチャ内容を更新
                this._surface.setRaster(this._raster);
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


            try
            {
                return new Device(0, DeviceType.Hardware, i_window.Handle, fl_base | CreateFlags.HardwareVertexProcessing, pp);
            }
            catch (Exception ex1)
            {
                Debug.WriteLine(ex1.ToString());
                try
                {
                    return new Device(0, DeviceType.Hardware, i_window.Handle, fl_base | CreateFlags.SoftwareVertexProcessing, pp);
                }
                catch (Exception ex2)
                {
                    // 作成に失敗
                    Debug.WriteLine(ex2.ToString());
                    try
                    {
                        return new Device(0, DeviceType.Reference, i_window.Handle, fl_base | CreateFlags.SoftwareVertexProcessing, pp);
                    }
                    catch (Exception ex3)
                    {
                        throw ex3;
                    }
                }
            }
        }

        public bool InitializeApplication(Form1 topLevelForm, CaptureDevice i_cap_device)
        {
            topLevelForm.ClientSize = new Size(SCREEN_WIDTH, SCREEN_HEIGHT);
            //キャプチャを作る(QVGAでフレームレートは30)
            i_cap_device.SetCaptureListener(this);
            i_cap_device.PrepareCapture(SCREEN_WIDTH, SCREEN_HEIGHT, 30);
            this._cap = i_cap_device;

            //ARラスタを作る(DirectShowキャプチャ仕様)。
            this._raster = new DsRgbRaster(i_cap_device.video_width, i_cap_device.video_height);

            //AR用カメラパラメタファイルをロードして設定
            NyARParam ap = NyARParam.loadFromARParamFile(File.OpenRead(AR_CAMERA_FILE),SCREEN_WIDTH, SCREEN_HEIGHT);

            //Direct3d用のユーティリティ準備

            //プロセッサの準備
            this._processor = new MarkerProcessor(ap, this._raster.getBufferType());
            NyARCode[] codes = new NyARCode[2];
            codes[0] = NyARCode.loadFromARPattFile(File.OpenRead(AR_CODE_FILE1),16, 16);
            codes[1] = NyARCode.loadFromARPattFile(File.OpenRead(AR_CODE_FILE2),16, 16);
            this._processor.setARCodeTable(codes,16,80.0);


            //3dデバイスを準備する
            this._device = PrepareD3dDevice(topLevelForm);
            this._device.RenderState.ZBufferEnable = true;
            this._device.RenderState.Lighting = false;
            this._device.RenderState.CullMode = Cull.CounterClockwise;

            Viewport vp = new Viewport();
            vp.X = 0;
            vp.Y = 0;
            vp.Height = ap.getScreenSize().h;
            vp.Width = ap.getScreenSize().w;
            vp.MaxZ = 1.0f;
            //ビューポート設定
            this._device.Viewport = vp;

            this._text = new TextPanel(this._device, 1);
            //カメラProjectionの設定
            Matrix tmp = new Matrix();
            NyARD3dUtil.toCameraFrustumRH(ap.getPerspectiveProjectionMatrix(), ap.getScreenSize(), 1, 10, 10000, ref tmp);
            this._device.Transform.Projection = tmp;

            // ビュー変換の設定(左手座標系ビュー行列で設定する)
            // 0,0,0から、Z+方向を向いて、上方向がY軸
            this._device.Transform.View = Matrix.LookAtLH(
                new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f));

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

                if (this._processor.current_id!=-1)
                {
                    long d = DateTime.Now.Ticks;
                    float r =  (d / 500000) % 360;
                    Matrix transform_mat2 = Matrix.Scaling(20.0f, 20.0f, 20.0f);
                    transform_mat2 *= Matrix.RotationX((float)(90.0 * 3.14 / 180.0));
                    transform_mat2 *= Matrix.Translation(-50f, 0, 50.0f);

                    transform_mat2 *= Matrix.RotationZ((float)(r * Math.PI / 180));

                    //変換行列を掛ける                     
                    transform_mat2 *= this._processor.transmat;
                    // 計算したマトリックスで座標変換
                    this._device.SetTransform(TransformType.World, transform_mat2);

                    this._text.draw("MarkerId:" + this._processor.current_id, 1);
                    // レンダリング（描画）
                    this._device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
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
