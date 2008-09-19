/* 
 * PROJECT: NyARToolkitCSUtils NyARToolkit for C# 支援ライブラリ
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
//CFWでコンパイルするときはNyartoolkitCS_FRAMEWORK_CFWをアクティブにしてください。
//#define NyartoolkitCS_FRAMEWORK_CFW
using System;
using System.Collections.Generic;
using jp.nyatla.nyartoolkit.cs.core;
#if NyartoolkitCS_FRAMEWORK_CFW
using Microsoft.WindowsMobile.DirectX.Direct3D;
using Microsoft.WindowsMobile.DirectX;
#else
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
#endif

namespace NyARToolkitCSUtils.NyAR
{
    /*
     * Direct3d用のMatrixクラス出力機能を追加したNyARParamクラスです。
     */
    public class D3dARParam : NyARParam
    {
        private double view_distance_min = 10.0;  //1cm
        private double view_distance_max = 10000.0;//10m
        /* nearクリップを[mm]で指定します。
         */
        public void setViewDistanceMin(double i_new_value)
        {
            view_distance_min = i_new_value;
        }
        /* farクリップを[mm]で指定します。
         */
        public void setViewDistanceMax(double i_new_value)
        {
            view_distance_max = i_new_value;
        }
        /* カメラのプロジェクションMatrixを返します。
         * このMatrixはMicrosoft.DirectX.Direct3d.Device.Transform.Projectionに設定できます。
         */
        public Matrix getCameraFrustumRH()
        {
            NyARMat trans_mat = new NyARMat(3, 4);
            NyARMat icpara_mat = new NyARMat(3, 4);
            double[,] p = new double[3, 3], q = new double[4, 4];
            int width, height;
            int i, j;

            width = this._screen_size.w;
            height = this._screen_size.h;

            this.getPerspectiveProjectionMatrix().decompMat(icpara_mat, trans_mat);

            double[][] icpara = icpara_mat.getArray();
            double[][] trans = trans_mat.getArray();
            for (i = 0; i < 4; i++)
            {
                icpara[1][i] = (height - 1) * (icpara[2][i]) - icpara[1][i];
            }

            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    p[i, j] = icpara[i][j] / icpara[2][2];
                }
            }
            q[0, 0] = (2.0 * p[0, 0] / (width - 1));
            q[0, 1] = (2.0 * p[0, 1] / (width - 1));
            q[0, 2] = -((2.0 * p[0, 2] / (width - 1)) - 1.0);
            q[0, 3] = 0.0;

            q[1, 0] = 0.0;
            q[1, 1] = -(2.0 * p[1, 1] / (height - 1));
            q[1, 2] = -((2.0 * p[1, 2] / (height - 1)) - 1.0);
            q[1, 3] = 0.0;

            q[2, 0] = 0.0;
            q[2, 1] = 0.0;
            q[2, 2] = (view_distance_max + view_distance_min) / (view_distance_min - view_distance_max);
            q[2, 3] = 2.0 * view_distance_max * view_distance_min / (view_distance_min - view_distance_max);

            q[3, 0] = 0.0;
            q[3, 1] = 0.0;
            q[3, 2] = -1.0;
            q[3, 3] = 0.0;

            Matrix result = new Matrix();
            result.M11 = (float)(q[0, 0] * trans[0][0] + q[0, 1] * trans[1][0] + q[0, 2] * trans[2][0]);
            result.M12 = (float)(q[1, 0] * trans[0][0] + q[1, 1] * trans[1][0] + q[1, 2] * trans[2][0]);
            result.M13 = (float)(q[2, 0] * trans[0][0] + q[2, 1] * trans[1][0] + q[2, 2] * trans[2][0]);
            result.M14 = (float)(q[3, 0] * trans[0][0] + q[3, 1] * trans[1][0] + q[3, 2] * trans[2][0]);
            result.M21 = (float)(q[0, 1] * trans[0][1] + q[0, 1] * trans[1][1] + q[0, 2] * trans[2][1]);
            result.M22 = (float)(q[1, 1] * trans[0][1] + q[1, 1] * trans[1][1] + q[1, 2] * trans[2][1]);
            result.M23 = (float)(q[2, 1] * trans[0][1] + q[2, 1] * trans[1][1] + q[2, 2] * trans[2][1]);
            result.M24 = (float)(q[3, 1] * trans[0][1] + q[3, 1] * trans[1][1] + q[3, 2] * trans[2][1]);
            result.M31 = (float)(q[0, 2] * trans[0][2] + q[0, 1] * trans[1][2] + q[0, 2] * trans[2][2]);
            result.M32 = (float)(q[1, 2] * trans[0][2] + q[1, 1] * trans[1][2] + q[1, 2] * trans[2][2]);
            result.M33 = -(float)(q[2, 2] * trans[0][2] + q[2, 1] * trans[1][2] + q[2, 2] * trans[2][2]);
            result.M34 = -(float)(q[3, 2] * trans[0][2] + q[3, 1] * trans[1][2] + q[3, 2] * trans[2][2]);
            result.M41 = (float)(q[0, 3] * trans[0][3] + q[0, 1] * trans[1][3] + q[0, 2] * trans[2][3] + q[0, 3]);
            result.M42 = (float)(q[1, 3] * trans[0][3] + q[1, 1] * trans[1][3] + q[1, 2] * trans[2][3] + q[1, 3]);
            result.M43 = (float)(q[2, 3] * trans[0][3] + q[2, 1] * trans[1][3] + q[2, 2] * trans[2][3] + q[2, 3]);
            result.M44 = (float)(q[3, 3] * trans[0][3] + q[3, 1] * trans[1][3] + q[3, 2] * trans[2][3] + q[3, 3]);
            return result;
        }
    }
}
