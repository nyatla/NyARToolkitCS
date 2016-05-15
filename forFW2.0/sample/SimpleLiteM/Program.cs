using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.markersystem;
using NyARToolkitCSUtils;
using NyARToolkitCSUtils.Direct3d;
using NyARToolkitCSUtils.Capture;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace SimpleLiteM
{
    /// <summary>
    /// このサンプルプログラムは、patt.kanjiとpatt.hiroマーカのそれぞれに、立方体を表示します。
    /// </summary>
    class Sketch : D3dSketch
    {
        private const int SCREEN_WIDTH = 640;
        private const int SCREEN_HEIGHT = 480;
        private const String AR_CODE_FILE1 = "../../../../../data/patt.hiro";
        private const String AR_CODE_FILE2 = "../../../../../data/patt.kanji";

        private NyARD3dMarkerSystem _ms;
        private NyARDirectShowCamera _ss;
        private NyARD3dRender _rs;
        private int mid1;
        private int mid2;
        public override void setup(CaptureDevice i_cap)
        {
            Device d3d = this.size(SCREEN_WIDTH, SCREEN_HEIGHT);
            i_cap.PrepareCapture(SCREEN_WIDTH, SCREEN_HEIGHT, 30.0f);
            INyARMarkerSystemConfig cf = new NyARMarkerSystemConfig(SCREEN_WIDTH, SCREEN_HEIGHT);
            d3d.RenderState.ZBufferEnable = true;
            d3d.RenderState.Lighting = false;
            d3d.RenderState.CullMode = Cull.CounterClockwise;
            this._ms = new NyARD3dMarkerSystem(cf);
            this._ss = new NyARDirectShowCamera(i_cap);
            this._rs = new NyARD3dRender(d3d, this._ms);
            this.mid1 = this._ms.addARMarker(AR_CODE_FILE1, 16, 25, 80);
            this.mid2 = this._ms.addARMarker(AR_CODE_FILE2, 16, 25, 80);

            //set View mmatrix
            this._rs.loadARViewMatrix(d3d);
            //set Viewport matrix
            this._rs.loadARViewPort(d3d);
            //setD3dProjectionMatrix
            this._rs.loadARProjectionMatrix(d3d);
            this._ss.start();
        }

        public override void loop(Device i_d3d)
        {
            lock (this._ss)
            {
                this._ms.update(this._ss);
                this._rs.drawBackground(i_d3d, this._ss.getSourceImage());
                i_d3d.BeginScene();
                i_d3d.Clear(ClearFlags.ZBuffer, Color.DarkBlue, 1.0f, 0);
                if (this._ms.isExist(this.mid1))
                {
                    //立方体を20mm上（マーカーの上）にずらしておく
                    Matrix transform_mat2 = Matrix.Translation(0, 0, 20.0f);
                    //変換行列を掛ける
                    transform_mat2 *= this._ms.getD3dTransformMatrix(this.mid1);
                    // 計算したマトリックスで座標変換
                    i_d3d.SetTransform(TransformType.World, transform_mat2);
                    // レンダリング（描画）
                    this._rs.colorCube(i_d3d, 40);
                }
                if (this._ms.isExist(this.mid2))
                {
                    //立方体を20mm上（マーカーの上）にずらしておく
                    Matrix transform_mat2 = Matrix.Translation(0, 0, 20.0f);
                    //変換行列を掛ける
                    transform_mat2 *= this._ms.getD3dTransformMatrix(this.mid2);
                    // 計算したマトリックスで座標変換
                    i_d3d.SetTransform(TransformType.World, transform_mat2);
                    // レンダリング（描画）
                    this._rs.colorCube(i_d3d, 40);
                }
                i_d3d.EndScene();
            }
            i_d3d.Present();
        }
        public override void cleanup()
        {
            this._rs.Dispose();
        }
        static void Main(string[] args)
        {
            new Sketch().run();

        }
    }
}
