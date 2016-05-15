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
using NyARToolkitCSUtils;
using NyARToolkitCSUtils.Capture;
using NyARToolkitCSUtils.Direct3d;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.markersystem;

/**
 * このプログラムは、スケッチシステムを使わないNyARMarkerSystemのアプリケーションのサンプルです。
 */
namespace SimpleLiteForm
{

    public class SimpleLiteMain : IDisposable
    {
        private const int SCREEN_WIDTH=640;
        private const int SCREEN_HEIGHT=480;
        private const String AR_CODE_FILE = "../../../../../data/patt.hiro";
        //NyAR
        //背景テクスチャ
        private NyARD3dSurface _surface;
        private ColorCube _cube;

        private NyARD3dMarkerSystem _ms;
        private NyARDirectShowCamera _ss;
        private Device _d3d;
        private int mid;

        public SimpleLiteMain(Form i_form,CaptureDevice i_dev)
        {
            //setup camera
            i_dev.PrepareCapture(SCREEN_WIDTH, SCREEN_HEIGHT, 30.0f);

            //setup form
            i_form.ClientSize = new Size(SCREEN_WIDTH, SCREEN_HEIGHT);

            //setup AR
            INyARMarkerSystemConfig cf = new NyARMarkerSystemConfig(SCREEN_WIDTH, SCREEN_HEIGHT);
            this._ms = new NyARD3dMarkerSystem(cf);
            this._ss = new NyARDirectShowCamera(i_dev);
            this.mid = this._ms.addARMarker(AR_CODE_FILE, 16, 25, 80);

            //setup directx

            //3dデバイスを準備する
            this._d3d = NyARD3dUtil.createD3dDevice(i_form);
            this._d3d.RenderState.ZBufferEnable = true;
            this._d3d.RenderState.Lighting = false;
            //ビューポートとビューの位置
            this._d3d.Transform.View = NyARD3dUtil.getARView();
            this._d3d.Viewport = NyARD3dUtil.getARViewPort(SCREEN_WIDTH,SCREEN_HEIGHT);
            //Projectionの設定
            this._ms.setProjectionMatrixClipping(10, 10000);
            Matrix pm = new Matrix();
            NyARD3dUtil.toCameraFrustumRH(this._ms.getARParam(),10,10000, ref pm);
            this._d3d.Transform.Projection = pm;

            //カラーキューブの描画インスタンス
            this._cube = new ColorCube(this._d3d, 40);

            //背景サーフェイスを作成
            this._surface = new NyARD3dSurface(this._d3d,SCREEN_WIDTH,SCREEN_HEIGHT);
        }
        //メインループ処理
        public void MainLoop()
        {
            //ARの計算
            lock (this._ss)
            {
                this._ms.update(this._ss);
                // 背景サーフェイスを直接描画
                this._surface.setRaster(this._ss.getSourceImage());
                Surface dest_surface = this._d3d.GetBackBuffer(0, 0, BackBufferType.Mono);
                Rectangle rect = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
                this._d3d.StretchRectangle((Surface)this._surface, rect, dest_surface, rect, TextureFilter.None);

                // 3Dオブジェクトの描画はここから
                this._d3d.BeginScene();
                this._d3d.Clear(ClearFlags.ZBuffer, Color.DarkBlue, 1.0f, 0);
                if (this._ms.isExist(0))
                {
                    //立方体を20mm上（マーカーの上）にずらしておく
                    Matrix transform_mat2 = Matrix.Translation(0, 0, 20.0f);
                    //変換行列を掛ける
                    transform_mat2 *= this._ms.getD3dTransformMatrix(this.mid);
                    // 計算したマトリックスで座標変換
                    this._d3d.SetTransform(TransformType.World, transform_mat2);
                    // レンダリング（描画）
                    this._cube.draw(this._d3d);
                }
                // 描画はここまで
                this._d3d.EndScene();
                // 実際のディスプレイに描画
                this._d3d.Present();
            }
            return;
        }

        // リソースの破棄をするために呼ばれる
        public void Dispose()
        {
            lock (this._ss)
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
                if (this._d3d != null)
                {
                    this._d3d.Dispose();
                }
            }
        }
    }
}
