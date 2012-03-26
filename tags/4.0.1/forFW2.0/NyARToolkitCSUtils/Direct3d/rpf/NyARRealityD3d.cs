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
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.rpf;
#if NyartoolkitCS_FRAMEWORK_CFW
using Microsoft.WindowsMobile.DirectX.Direct3D;
using Microsoft.WindowsMobile.DirectX;
#else
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
#endif

namespace NyARToolkitCSUtils.Direct3d
{

    /**
     * OpenGLに特化したNyARRealityクラスです。
     * @author nyatla
     */
    public class NyARRealityD3d : NyARReality
    {
        private NyARDoubleMatrix44 _gl_frustum_rh = new NyARDoubleMatrix44();

        public NyARRealityD3d(NyARParam i_param, double i_near, double i_far, int i_max_known_target, int i_max_unknown_target)
            : base(i_param, i_near, i_far, i_max_known_target, i_max_unknown_target)
        {
        }
        /**
         * 透視投影行列と視錐体パラメータを元に、インスタンスを作成します。
         * この関数は、樽型歪み矯正を外部で行うときに使います。
         * @param i_prjmat
         * ARToolKitスタイルのカメラパラメータです。通常は{@link NyARParam#getPerspectiveProjectionMatrix()}から得られた値を使います。
         * @param i_screen_size
         * スクリーン（入力画像）のサイズです。通常は{@link NyARParam#getScreenSize()}から得られた値を使います。
         * @param i_near
         * 視錐体のnear-pointをmm単位で指定します。
         * default値は{@link #FRASTRAM_ARTK_NEAR}です。
         * @param i_far
         * 視錐体のfar-pointをmm単位で指定します。
         * default値は{@link #FRASTRAM_ARTK_FAR}です。
         * @param i_max_known_target
         * @param i_max_unknown_target
         * @throws NyARException
         */
        public NyARRealityD3d(NyARPerspectiveProjectionMatrix i_prjmat, NyARIntSize i_screen_size, double i_near, double i_far, int i_max_known_target, int i_max_unknown_target)
            : base(i_screen_size, i_near, i_far, i_prjmat, null, i_max_known_target, i_max_unknown_target)
        {
        }

        /**
         * 
         * NyARToolKitの姿勢変換行列を返します。
         * @throws NyARException 
         */
        public void getD3dModelViewMatrix(NyARDoubleMatrix44 i_nyar_mat,ref Matrix o_mat)
        {
            NyARD3dUtil.toD3dCameraView(i_nyar_mat, 1f,ref o_mat);
            return;
        }

        /**
         * DirectXスタイルのProjectionMatrixを返却します。
         */
        public void getD3dCameraFrustum(ref Matrix o_mat)
        {
            NyARD3dUtil.mat44ToD3dMatrixT(this._frustum.getMatrix(), ref o_mat);
        }
        /**
         * 現在のViewPortに、i_rtsourceの内容を描画します。
         * @param i_gl
         * OpenGLインスタンスを指定します。
         * @param i_raster
         * @throws NyARException
         */
//        public void d3dDrawRealitySource(Microsoft.DirectX.Direct3D.Device i_dev, NyARRealitySource i_rtsource)
//        {
//            NyARGLDrawUtil.drawBackGround(i_dev, i_rtsource.refRgbSource(), 1.0);
//            return;
//        }
    }
}