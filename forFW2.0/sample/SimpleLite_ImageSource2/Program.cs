using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.cs4;
using jp.nyatla.nyartoolkit.cs.markersystem;
using NyARToolkitCSUtils;
using NyARToolkitCSUtils.Direct3d;
using NyARToolkitCSUtils.Capture;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;


/**
 * This sample program is rendering a cube into a static picture. 
 */
namespace SimpleLite
{
    class Sketch : D3dSketch
    {
        private const int SCREEN_WIDTH = 320;
        private const int SCREEN_HEIGHT = 240;
        private const String AR_CODE_FILE = "../../../../../data/patt.hiro";
        private const String TEST_IMAGE =   "../../../../../data/320x240ABGR.png";

        private NyARD3dMarkerSystem _ms;
        private NyARSensor _ss;
        private NyARD3dRender _rs;
        private int mid;
        public override void setup(CaptureDevice i_cap)
        {
            Device d3d = this.size(SCREEN_WIDTH, SCREEN_HEIGHT);
            INyARMarkerSystemConfig cf = new NyARMarkerSystemConfig(SCREEN_WIDTH, SCREEN_HEIGHT);
            d3d.RenderState.ZBufferEnable = true;
            d3d.RenderState.Lighting = false;
            d3d.RenderState.CullMode = Cull.CounterClockwise;
            this._ms = new NyARD3dMarkerSystem(cf);
            this._ss = new NyARSensor(cf.getScreenSize());
            this._rs = new NyARD3dRender(d3d, this._ms);
            this.mid = this._ms.addARMarker(AR_CODE_FILE, 16, 25, 80);
            //set View mmatrix
            this._rs.loadARViewMatrix(d3d);
            //set Viewport matrix
            this._rs.loadARViewPort(d3d);
            //setD3dProjectionMatrix
            this._rs.loadARProjectionMatrix(d3d);
            Bitmap src = new Bitmap(TEST_IMAGE);
            Bitmap input = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            using (Graphics g = Graphics.FromImage(input))
            {
                g.DrawImage(src,0,0);
            }
            this._ss.update(new NyARBitmapRaster(input));
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
                    //move cube to +20mm on Marker
                    Matrix transform_mat2 = Matrix.Translation(0, 0, 20.0f);
                    //transform
                    transform_mat2 *= this._ms.getD3dTransformMatrix(this.mid);
                    //convert matrix
                    i_d3d.SetTransform(TransformType.World, transform_mat2);
                    //rendering
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
            new Sketch().runWithoutCamera();
        }
    }
}
