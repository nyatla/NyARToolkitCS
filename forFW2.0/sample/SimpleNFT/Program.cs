using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Text;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.markersystem;
using NyARToolkitCSUtils;
using NyARToolkitCSUtils.Direct3d;
using NyARToolkitCSUtils.Capture;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace SimpleNFT
{
    class Program
    {
        class Sketch : D3dSketch
        {
            private const int SCREEN_WIDTH = 640;
            private const int SCREEN_HEIGHT = 480;
            String cparam_file = "../../../../../data/testcase/camera_para5.dat";
            String fset3file = "../../../../../data/nft/infinitycat";

            private NyARD3dNftSystem _ms;
            private NyARDirectShowCamera _ss;
            private NyARD3dRender _rs;
            private int mid;
            public override void setup(CaptureDevice i_cap)
            {
                Device d3d = this.size(SCREEN_WIDTH, SCREEN_HEIGHT);
                i_cap.PrepareCapture(SCREEN_WIDTH, SCREEN_HEIGHT, 30.0f);
                INyARNftSystemConfig cf = new NyARNftSystemConfig(File.OpenRead(cparam_file),SCREEN_WIDTH, SCREEN_HEIGHT);
                d3d.RenderState.ZBufferEnable = true;
                d3d.RenderState.Lighting = false;
                d3d.RenderState.CullMode = Cull.CounterClockwise;
                this._ms = new NyARD3dNftSystem(cf);
                this._ss = new NyARDirectShowCamera(i_cap);
                this._rs = new NyARD3dRender(d3d, this._ms);
                this.mid = this._ms.addNftTarget(fset3file, 160);

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
                    if (this._ms.isExist(this.mid))
                    {
                        //立方体を20mm上（マーカーの上）にずらしておく
                        Matrix transform_mat2 = Matrix.Translation(80,60,20);
                        //変換行列を掛ける
                        transform_mat2 *= this._ms.getD3dTransformMatrix(this.mid);
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
                this._ms.shutdown();
                this._rs.Dispose();
            }
        }
        static void Main(string[] args)
        {
            new Sketch().run();
        }
    }
}
