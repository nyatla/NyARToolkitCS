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

    public partial class ABGRAcapture : IDisposable, CaptureListener
    {
        private const int SCREEN_WIDTH = 320;
        private const int SCREEN_HEIGHT = 240;
        //DirectShowからのキャプチャ
        private CaptureDevice _cap;
        //NyAR
        private DsBGRX32Raster _raster;
        //背景テクスチャ
        private NyARSurface_XRGB32 _surface;
        /// Direct3D デバイス
        private Device _device = null;
        // 頂点バッファ/インデックスバッファ/インデックスバッファの各頂点番号配列
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
                this._raster.setBuffer(i_buffer,i_sender.video_vertical_flip);

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
        public void saveRawFile(String i_filename)
        {
            lock (this)
            {
                using (FileStream fs = new FileStream(i_filename, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write((byte[])this._raster.getBufferReader().getBuffer());
                    }
                }
            }
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
            //キャプチャを作る(QVGAでフレームレートは30)
            i_cap_device.SetCaptureListener(this);
            i_cap_device.PrepareCapture(SCREEN_WIDTH, SCREEN_HEIGHT, 30);
            this._cap = i_cap_device;

            //ARの設定

            //ARラスタを作る(DirectShowキャプチャ仕様)。
            this._raster = new DsBGRX32Raster(i_cap_device.video_width, i_cap_device.video_height, i_cap_device.video_width * i_cap_device.video_bit_count / 8);





            //3dデバイスを準備する
            this._device = PrepareD3dDevice(topLevelForm);

            // ライトを無効
            this._device.RenderState.Lighting = false;

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
            lock (this)
            {
                // 背景サーフェイスを直接描画
                Surface dest_surface = this._device.GetBackBuffer(0, 0, BackBufferType.Mono);
                Rectangle src_dest_rect = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
                this._device.StretchRectangle(this._surface.d3d_surface, src_dest_rect, dest_surface, src_dest_rect, TextureFilter.None);

                // 3Dオブジェクトの描画はここから
                this._device.BeginScene();


                // 描画はここまで
                this._device.EndScene();

                // 実際のディスプレイに描画
                this._device.Present();
            }

        }

        // リソースの破棄をするために呼ばれる
        public void Dispose()
        {
            // Direct3D デバイスのリソース解放
            if (this._device != null)
            {
                this._device.Dispose();
            }
        }
    }
}
