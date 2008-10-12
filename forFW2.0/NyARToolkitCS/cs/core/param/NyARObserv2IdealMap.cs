using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.core
{
    public class NyARObserv2IdealMap
    {
        private int _stride;
        private double[] _mapx;
        private double[] _mapy;

        public NyARObserv2IdealMap(NyARCameraDistortionFactor i_distfactor, NyARIntSize i_screen_size)
        {
            NyARDoublePoint2d opoint = new NyARDoublePoint2d();
            this._mapx = new double[i_screen_size.w * i_screen_size.h];
            this._mapy = new double[i_screen_size.w * i_screen_size.h];
            this._stride = i_screen_size.w;
            int ptr = i_screen_size.h * i_screen_size.w - 1;
            //歪みマップを構築
            for (int i = i_screen_size.h - 1; i >= 0; i--)
            {
                for (int i2 = i_screen_size.w - 1; i2 >= 0; i2--)
                {
                    i_distfactor.observ2Ideal(i2, i, opoint);
                    this._mapx[ptr] = opoint.x;
                    this._mapy[ptr] = opoint.y;
                    ptr--;
                }
            }
            return;
        }
        public void observ2Ideal(double ix, double iy, NyARDoublePoint2d o_point)
        {
            int idx = (int)ix + (int)iy * this._stride;
            o_point.x = this._mapx[idx];
            o_point.y = this._mapy[idx];
            return;
        }
        public void observ2IdealBatch(int[] i_x_coord, int[] i_y_coord, int i_start, int i_num, double[] o_x_coord, double[] o_y_coord)
        {
            int idx;
            int ptr = 0;
            for (int j = 0; j < i_num; j++)
            {
                idx = i_x_coord[i_start + j] + i_y_coord[i_start + j] * this._stride;
                o_x_coord[ptr] = this._mapx[idx];
                o_y_coord[ptr] = this._mapy[idx];
                ptr++;
            }
            return;
        }
    }

}
