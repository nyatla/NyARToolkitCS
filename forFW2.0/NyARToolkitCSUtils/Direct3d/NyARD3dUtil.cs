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
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using jp.nyatla.nyartoolkit.cs.core;
#if NyartoolkitCS_FRAMEWORK_CFW
using Microsoft.WindowsMobile.DirectX.Direct3D;
using Microsoft.WindowsMobile.DirectX;
#else
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
#endif

namespace NyARToolkitCSUtils.Direct3d
{
    public static class NyARD3dUtil
    {
        #region APIs
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void RtlCopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] int Length);
        #endregion

        /// <summary>
        /// この関数は、ControlにバインドしたDirectXデバイスを生成します。
        /// </summary>
        /// <param name="i_window"></param>
        /// <returns></returns>
        public static Device createD3dDevice(Control i_window)
        {
            PresentParameters pp = new PresentParameters();
            pp.Windowed = true;
            pp.SwapEffect = SwapEffect.Flip;
            pp.BackBufferFormat = Format.X8R8G8B8;
            pp.BackBufferCount = 1;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = DepthFormat.D16;
            CreateFlags fl_base = CreateFlags.FpuPreserve;
            try
            {
                return new Device(0, DeviceType.Hardware, i_window.Handle, fl_base | CreateFlags.HardwareVertexProcessing, pp);
            }
            catch (Exception ex1)
            {
                Debug.WriteLine(ex1.ToString());
                try
                {
                    return new Device(0, DeviceType.Hardware, i_window.Handle, fl_base | CreateFlags.SoftwareVertexProcessing, pp);
                }
                catch (Exception ex2)
                {
                    // 作成に失敗
                    Debug.WriteLine(ex2.ToString());
                    try
                    {
                        return new Device(0, DeviceType.Reference, i_window.Handle, fl_base | CreateFlags.SoftwareVertexProcessing, pp);
                    }
                    catch (Exception ex3)
                    {
                        throw ex3;
                    }
                }
            }
        }
        public static Matrix getARView()
        {
            return Matrix.LookAtLH(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f));
        }
        public static Viewport getARViewPort(int i_width,int i_height)
        {
            Viewport vp = new Viewport();
            vp.X = 0;
            vp.Y = 0;
            vp.Height = i_height;
            vp.Width = i_width;
            vp.MaxZ = 1.0f;
            return vp;
        }

        /* カメラのプロジェクションMatrix(RH)を返します。
         * このMatrixはMicrosoft.DirectX.Direct3d.Device.Transform.Projectionに設定できます。
         */
        public static void toCameraFrustumRH(NyARPerspectiveProjectionMatrix i_promat, NyARIntSize i_size, double i_scale, double i_near, double i_far, ref Matrix o_d3d_projection)
        {
            NyARDoubleMatrix44 m = new NyARDoubleMatrix44();
            i_promat.makeCameraFrustumRH(i_size.w, i_size.h, i_near * i_scale, i_far * i_scale, m);
            NyARD3dUtil.mat44ToD3dMatrixT(m, ref o_d3d_projection);
            return;
        }

        public static void toCameraFrustumRH(NyARParam i_arparam, double i_near, double i_far, ref Matrix o_d3d_projection)
        {
            toCameraFrustumRH(i_arparam.getPerspectiveProjectionMatrix(),i_arparam.getScreenSize(),1.0,i_near, i_far,ref o_d3d_projection);
        }
        /**
         * 
         */
        public static void mat44ToD3dMatrix(NyARDoubleMatrix44 i_src,ref Matrix o_dst)
        {
            o_dst.M11 = (float)i_src.m00;
            o_dst.M12 = (float)i_src.m01;
            o_dst.M13 = (float)i_src.m02;
            o_dst.M14 = (float)i_src.m03;

            o_dst.M21 = (float)i_src.m10;
            o_dst.M22 = (float)i_src.m11;
            o_dst.M23 = (float)i_src.m12;
            o_dst.M24 = (float)i_src.m13;

            o_dst.M31 = (float)i_src.m20;
            o_dst.M32 = (float)i_src.m21;
            o_dst.M33 = (float)i_src.m22;
            o_dst.M34 = (float)i_src.m23;

            o_dst.M41 = (float)i_src.m30;
            o_dst.M42 = (float)i_src.m31;
            o_dst.M43 = (float)i_src.m32;
            o_dst.M44 = (float)i_src.m33;
            return;
        }
        public static void mat44ToD3dMatrixT(NyARDoubleMatrix44 i_src, ref Matrix o_dst)
        {
            o_dst.M11 = (float)i_src.m00;
            o_dst.M21 = (float)i_src.m01;
            o_dst.M31 = (float)i_src.m02;
            o_dst.M41 = (float)i_src.m03;

            o_dst.M12 = (float)i_src.m10;
            o_dst.M22 = (float)i_src.m11;
            o_dst.M32 = (float)i_src.m12;
            o_dst.M42 = (float)i_src.m13;

            o_dst.M13 = (float)i_src.m20;
            o_dst.M23 = (float)i_src.m21;
            o_dst.M33 = (float)i_src.m22;
            o_dst.M43 = (float)i_src.m23;

            o_dst.M14 = (float)i_src.m30;
            o_dst.M24 = (float)i_src.m31;
            o_dst.M34 = (float)i_src.m32;
            o_dst.M44 = (float)i_src.m33;
            return;
        }

        /**
         * Direct3d形式のカメラビュー行列に変換します。
         */
        public static void toD3dCameraView(NyARDoubleMatrix44 i_ny_result, float i_scale, ref Matrix o_result)
        {
            Matrix m;
            m.M11 = (float)i_ny_result.m00;
            m.M12 = -(float)i_ny_result.m10;
            m.M13 = -(float)i_ny_result.m20;
            m.M14 = 0;
            m.M21 = (float)i_ny_result.m01;
            m.M22 = -(float)i_ny_result.m11;
            m.M23 = -(float)i_ny_result.m21;
            m.M24 = 0;
            m.M31 = (float)i_ny_result.m02;
            m.M32 = -(float)i_ny_result.m12;
            m.M33 = -(float)i_ny_result.m22;
            m.M34 = 0;
            float scale =(1 / i_scale);
            m.M41 = (float)i_ny_result.m03 * scale;
            m.M42 = -(float)i_ny_result.m13 * scale;
            m.M43 = -(float)i_ny_result.m23 * scale;
            m.M44 = 1;

            o_result = m;
            return;
        }
    }
}
