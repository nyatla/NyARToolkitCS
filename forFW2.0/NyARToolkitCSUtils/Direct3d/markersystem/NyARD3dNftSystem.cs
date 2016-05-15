using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.cs4;
using jp.nyatla.nyartoolkit.cs.markersystem;
using System.Diagnostics;
using System.Drawing;

namespace NyARToolkitCSUtils.Direct3d
{
    public class NyARD3dNftSystem: NyARNftSystem
    {
        public NyARD3dNftSystem(INyARNftSystemConfig i_config)
            : base(i_config)
        {
        }

        /**
         * この関数は、i_bufに指定idのOpenGL形式の姿勢変換行列を設定して返します。
         * @param i_id
         * @param i_buf
         * @return
         */
        public void getTransformMatrix(int i_id, ref Matrix i_buf)
        {
            NyARD3dUtil.toD3dCameraView(base.getTransformMatrix(i_id), 1, ref i_buf);
            return;
        }

        public Matrix getD3dTransformMatrix(int i_id)
        {
            Matrix p = new Matrix();
            this.getTransformMatrix(i_id, ref p);
            return p;
        }



        private NyARDoublePoint3d __wk_3dpos = new NyARDoublePoint3d();
        private NyARDoublePoint2d __wk_2dpos = new NyARDoublePoint2d();

        public void getMarkerPlanePos(int i_id, int i_x, int i_y, ref Vector3 i_buf)
        {
            NyARDoublePoint3d p = this.__wk_3dpos;
            base.getPlanePos(i_id, i_x, i_y, p);
            i_buf.X = (float)p.x;
            i_buf.Y = (float)p.y;
            i_buf.Z = (float)p.z;
            return;
        }
        public void getScreenPos(int i_id, double i_x, double i_y, double i_z,ref Vector2 i_out)
        {
            NyARDoublePoint2d wk_2dpos = this.__wk_2dpos;
            NyARDoublePoint3d wk_3dpos = this.__wk_3dpos;
            this.getTransformMatrix(i_id).transform3d(i_x, i_y, i_z, wk_3dpos);
            this._view.getFrustum().project(wk_3dpos, wk_2dpos);
            i_out.X = (float)wk_2dpos.x;
            i_out.Y = (float)wk_2dpos.y;
            return;
        }
        //
        // This reogion may be moved to NyARJ2seMarkerSystem.
        //

        /// <summary>
        /// この関数は、{@link #getMarkerPlaneImage(int, NyARSensor, int, int, int, int, int, int, int, int, INyARRgbRaster)}
        /// のラッパーです。取得画像を{@link #BufferedImage}形式で返します。
        /// </summary>
        /// <param name="i_id"></param>
        /// <param name="i_sensor"></param>
        /// <param name="i_x1"></param>
        /// <param name="i_y1"></param>
        /// <param name="i_x2"></param>
        /// <param name="i_y2"></param>
        /// <param name="i_x3"></param>
        /// <param name="i_y3"></param>
        /// <param name="i_x4"></param>
        /// <param name="i_y4"></param>
        /// <param name="i_img"></param>
        /// <returns></returns>

        public void getMarkerPlaneImage(
            int i_id,
            NyARSensor i_sensor,
            int i_x1, int i_y1,
            int i_x2, int i_y2,
            int i_x3, int i_y3,
            int i_x4, int i_y4,
            Bitmap i_img)
        {
            using (NyARBitmapRaster bmr = new NyARBitmapRaster(i_img))
            {
                base.getPlaneImage(i_id, i_sensor, i_x1, i_y1, i_x2, i_y2, i_x3, i_y3, i_x4, i_y4, bmr);
                return;
            }
        }
        /**
         * この関数は、{@link #getMarkerPlaneImage(int, NyARSensor, int, int, int, int, INyARRgbRaster)}
         * のラッパーです。取得画像を{@link #BufferedImage}形式で返します。
         * @param i_id
         * マーカid
         * @param i_sensor
         * 画像を取得するセンサオブジェクト。通常は{@link #update(NyARSensor)}関数に入力したものと同じものを指定します。
         * @param i_l
         * @param i_t
         * @param i_w
         * @param i_h
         * @param i_raster
         * 出力先のオブジェクト
         * @return
         * 結果を格納したi_rasterオブジェクト
         * @throws NyARException
         */
        public void getPlaneImage(
            int i_id,
            NyARSensor i_sensor,
            int i_l, int i_t,
            int i_w, int i_h,
            Bitmap i_img)
        {
            using (NyARBitmapRaster bmr = new NyARBitmapRaster(i_img))
            {
                base.getPlaneImage(i_id, i_sensor, i_l, i_t, i_w, i_h, bmr);
                return;
            }
        }
        [System.Obsolete("use getTransformMatrix")]
        public void getMarkerMatrix(int i_id, ref Matrix i_buf)
        {
            this.getTransformMatrix(i_id, ref i_buf);
            return;
        }
        [System.Obsolete("use getD3dTransformMatrix")]
        public Matrix getD3dMarkerMatrix(int i_id)
        {
            return this.getD3dTransformMatrix(i_id);
        }
    }
}
