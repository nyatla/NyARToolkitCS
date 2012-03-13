using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Microsoft.WindowsMobile.DirectX.Direct3D;
using Microsoft.WindowsMobile.DirectX;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.detector;
using jp.nyatla.nyartoolkit.cs.rpf.reality.nyartk;
using jp.nyatla.nyartoolkit.cs.rpf.realitysource.nyartk;
using jp.nyatla.nyartoolkit.cs.rpf.tracker.nyartk;
using jp.nyatla.nyartoolkit.cs.rpf.tracker.utils;
using jp.nyatla.nyartoolkit.cs.rpf.mklib;
using NyARToolkitCSUtils.WMCapture.rpf;
using NyARToolkitCSUtils.WMCapture;
using NyARToolkitCSUtils.Direct3d;
using NyARToolkitCSUtils.Direct3d.rpf;
using jp.nyatla.cs.NyWMCapture;


namespace NyARToolkitCS.WM5.RPF
{

    public partial class Test_NyARRealityD3d_ARMarker : IDisposable, IWmCaptureListener
    {
        private const int SCREEN_WIDTH = 240;
        private const int SCREEN_HEIGHT = 320;

        //背景テクスチャ
        private NyARRealityD3d _reality;
        private NyARRealitySource_WMCapture _reality_source;
        ARTKMarkerTable _mklib;
        /* 非同期イベントハンドラ
          * CaptureDeviceからのイベントをハンドリングして、バッファとテクスチャを更新する。
          */
        public void onSample(WmCapture i_sender, INySample i_sample)
        {
            int w = SCREEN_WIDTH;
            int h = SCREEN_HEIGHT;
            int s = w * h;

            Thread.Sleep(0); 
            lock (this)
            {
                //カメラ映像をARのバッファにコピー
                this._reality_source.setWMCaptureSample(i_sample.GetData(), i_sender.vertical_flip);
                this._reality.progress(this._reality_source);

                //テクスチャ内容を更新
                this._back_ground.CopyFromRaster((DsRGB565Raster)this._reality_source.refRgbSource());

                //UnknownTargetを1個取得して、遷移を試す。
                NyARRealityTarget t = this._reality.selectSingleUnknownTarget();
                if (t == null)
                {
                    return;
                }
                //ターゲットに一致するデータを検索
                ARTKMarkerTable.GetBestMatchTargetResult r = new ARTKMarkerTable.GetBestMatchTargetResult();
                if (this._mklib.getBestMatchTarget(t, this._reality_source, r))
                {
                    if (r.confidence < 0.5)
                    {	//一致率が低すぎる。
                        return;
                    }
                    //既に認識しているターゲットの内側のものでないか確認する？(この処理をすれば、二重認識は無くなる。)

                    //一致度を確認して、80%以上ならKnownターゲットへ遷移
                    if (!this._reality.changeTargetToKnown(t, r.artk_direction, r.marker_width))
                    {
                        //遷移の成功チェック
                        return;//失敗
                    }
                    //遷移に成功したので、tagにResult情報をコピーしておく。（後で表示に使う）
                    t.tag = r;
                }
                else
                {
                    //一致しないので、このターゲットは捨てる。
                    this._reality.changeTargetToDead(t, 10);
                }
            }

            return;
        }
        private D3dManager _d3dmgr;
        private ColorCube _d3dcube;
        private ID3dBackground _back_ground;
        private WmCapture _capture;
        //NyAR
        private DsRGB565Raster m_raster;

        public Test_NyARRealityD3d_ARMarker(Form1 topLevelForm, ResourceBuilder i_resource)
        {
            this._capture = i_resource.createWmCapture();
            this._capture.setOnSample(this);

            this._d3dmgr = i_resource.createD3dManager(topLevelForm);
            this._back_ground = i_resource.createBackGround(this._d3dmgr);
            this._d3dcube = new ColorCube(this._d3dmgr.d3d_device,40);


            //ARラスタを作る(DirectShowキャプチャ仕様)。
            this.m_raster = i_resource.createARRaster();
            //AR用のパターンコードを読み出


            //マーカライブラリ(ARTKId)の構築
            this._mklib = new ARTKMarkerTable(10, 16, 16, 25, 25, 4);
            //マーカテーブルの作成（1種類）
            this._mklib.addMarker(i_resource.createNyARCode(), 0, "HIRO", 80, 80);

            //Realityの準備
            this._reality = new NyARRealityD3d(i_resource.ar_param, 10, 10000, 1, 5);
            this._reality_source = new NyARRealitySource_WMCapture(SCREEN_WIDTH, SCREEN_HEIGHT, null, 2, 100);


        }
        //メインループ処理
        public void MainLoop()
        {
            //ARの計算
            lock (this)
            {
                Device dev = this._d3dmgr.d3d_device;
                // 背景サーフェイスを直接描画

                // 3Dオブジェクトの描画はここから
                dev.Clear(ClearFlags.ZBuffer, Color.DarkBlue, 1.0f, 0);
                dev.BeginScene();
                this._back_ground.drawBackGround(this._d3dmgr.d3d_device);


                //ターゲットリストを走査して、画面に内容を反映
                NyARRealityTargetList tl = this._reality.refTargetList();
                for (int i = tl.getLength() - 1; i >= 0; i--)
                {
                    NyARRealityTarget t = tl.getItem(i);
                    switch (t.getTargetType())
                    {
                        case NyARRealityTarget.RT_KNOWN:
                            //立方体を20mm上（マーカーの上）にずらしておく
                            Matrix mat = new Matrix();
                            this._reality.getD3dModelViewMatrix(t.refTransformMatrix(), ref mat);
                            Matrix transform_mat2 = Matrix.Translation(0, 0, 20.0f);
                            transform_mat2 = transform_mat2 * mat;
                            dev.SetTransform(TransformType.World, transform_mat2);
                            this._d3dcube.draw(dev);
                            break;
                        case NyARRealityTarget.RT_UNKNOWN:
                            //NyARDoublePoint2d[] p = t.refTargetVertex();
                            //NyARGLDrawUtil.beginScreenCoordinateSystem(this._gl, SCREEN_X, SCREEN_Y, true);
                            //NyARGLDrawUtil.endScreenCoordinateSystem(this._gl);
                            break;
                    }
                }
                // 描画はここまで
                dev.EndScene();

                // 実際のディスプレイに描画
                dev.Present();
            }
            return;
        }

        // リソースの破棄をするために呼ばれる
        public void Dispose()
        {
            lock (this)
            {

                //キャプチャ解除
                if (this._capture != null)
                {
                    this._capture.Dispose();
                }
                if (this._d3dcube != null)
                {
                    this._d3dcube.Dispose();
                }
                if (this._back_ground != null)
                {
                    this._back_ground.Dispose();
                }

                // Direct3D デバイスのリソース解放
                if (this._d3dmgr.d3d_device != null)
                {
                    this._d3dmgr.d3d_device.Dispose();
                }
            }
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
    }

}
