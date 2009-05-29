using System;
using System.Collections.Generic;
using System.Text;
using jp.nyatla.nyartoolkit.cs.utils;

namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * 遠近法を用いたPerspectiveパラメータを計算するクラスです。
     *
     */
    public class NyARPerspectiveParamGenerator
    {
        protected int _local_x;
        protected int _local_y;
        protected int _width;
        protected int _height;
        public NyARPerspectiveParamGenerator(int i_local_x, int i_local_y, int i_width, int i_height)
        {
            this._height = i_height;
            this._width = i_width;
            this._local_x = i_local_x;
            this._local_y = i_local_y;
            return;
        }
        public virtual bool getParam(NyARIntPoint2d[] i_vertex, double[] o_param)
        {
            double[][] la1, la2;
            double[] ra1, ra2;
            double ltx = this._local_x;
            double lty = this._local_y;
            double rbx = ltx + this._width;
            double rby = lty + this._height;
            la1 = ArrayUtils.newDouble2dArray(4, 5);//new double[4][5];
            la2 = ArrayUtils.newDouble2dArray(4, 5);//new double[4][5];
            ra1 = new double[4];
            ra2 = new double[4];
            //A,B,C,(GH)の方程式
            la1[0][0] = ltx; la1[0][1] = lty; la1[0][2] = 1; la1[0][3] = -ltx * i_vertex[0].x; la1[0][4] = -lty * i_vertex[0].x;
            la1[1][0] = rbx; la1[1][1] = lty; la1[1][2] = 1; la1[1][3] = -rbx * i_vertex[1].x; la1[1][4] = -lty * i_vertex[1].x;
            la1[2][0] = rbx; la1[2][1] = rby; la1[2][2] = 1; la1[2][3] = -rbx * i_vertex[2].x; la1[2][4] = -rby * i_vertex[2].x;
            la1[3][0] = ltx; la1[3][1] = rby; la1[3][2] = 1; la1[3][3] = -ltx * i_vertex[3].x; la1[3][4] = -rby * i_vertex[3].x;
            ra1[0] = i_vertex[0].x; ra1[1] = i_vertex[1].x; ra1[2] = i_vertex[2].x; ra1[3] = i_vertex[3].x;
            NyARSystemOfLinearEquationsProcessor.doGaussianElimination(la1, ra1, 5, 4);

            //D,E,F,(GH)の方程式
            la2[0][0] = ltx; la2[0][1] = lty; la2[0][2] = 1; la2[0][3] = -ltx * i_vertex[0].y; la2[0][4] = -lty * i_vertex[0].y;
            la2[1][0] = rbx; la2[1][1] = lty; la2[1][2] = 1; la2[1][3] = -rbx * i_vertex[1].y; la2[1][4] = -lty * i_vertex[1].y;
            la2[2][0] = rbx; la2[2][1] = rby; la2[2][2] = 1; la2[2][3] = -rbx * i_vertex[2].y; la2[2][4] = -rby * i_vertex[2].y;
            la2[3][0] = ltx; la2[3][1] = rby; la2[3][2] = 1; la2[3][3] = -ltx * i_vertex[3].y; la2[3][4] = -rby * i_vertex[3].y;
            ra2[0] = i_vertex[0].y; ra2[1] = i_vertex[1].y; ra2[2] = i_vertex[2].y; ra2[3] = i_vertex[3].y;
            NyARSystemOfLinearEquationsProcessor.doGaussianElimination(la2, ra2, 5, 4);
            //GHを計算
            double A, B, C, D, E, F, G, H;
            H = (ra2[3] - ra1[3]) / (la2[3][4] - la1[3][4]);
            G = ra2[3] - la2[3][4] * H;
            //残りを計算
            F = ra2[2] - H * la2[2][4] - G * la2[2][3];
            E = ra2[1] - H * la2[1][4] - G * la2[1][3] - F * la2[1][2];
            D = ra2[0] - H * la2[0][4] - G * la2[0][3] - F * la2[0][2] - E * la2[0][1];
            C = ra1[2] - H * la1[2][4] - G * la1[2][3];
            B = ra1[1] - H * la1[1][4] - G * la1[1][3] - C * la1[1][2];
            A = ra1[0] - H * la1[0][4] - G * la1[0][3] - C * la1[0][2] - B * la1[0][1];
            o_param[0] = A;
            o_param[1] = B;
            o_param[2] = C;
            o_param[3] = D;
            o_param[4] = E;
            o_param[5] = F;
            o_param[6] = G;
            o_param[7] = H;
            return true;
        }
    }

}
