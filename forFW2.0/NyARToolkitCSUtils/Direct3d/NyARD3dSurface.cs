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
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Diagnostics;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.cs4;

namespace NyARToolkitCSUtils.Direct3d
{
    /* DsXRGB32Rasterのラスタデータを取り込むことが出来るSurfaceです。
     * このSurfaceはそのままARToolKitの背景描画に使います。
     */
    public class NyARD3dSurface:IDisposable
    {
        private bool _is_dispose=false;
        private int m_width;
        private int m_height;
        private Microsoft.DirectX.Direct3D.Device m_ref_dev;
        private Surface _surface;


        /* i_width x i_heightのテクスチャを格納するインスタンスを生成します。
         * 
         */
        public NyARD3dSurface(Microsoft.DirectX.Direct3D.Device i_dev, int i_width, int i_height)
        {
            this.m_ref_dev = i_dev;
            this.m_height = i_height;
            this.m_width = i_width;
            this._surface = i_dev.CreateOffscreenPlainSurface(i_width, i_height, Format.X8R8G8B8, Pool.Default);

            //OK、完成だ。
            return;
        }
        ~NyARD3dSurface()
        {
            this.Dispose();
        }
        public static explicit operator Surface(NyARD3dSurface s)
        {
            Debug.Assert(!s._is_dispose);
            return s._surface;
        }
        public bool isEqualSize(NyARIntSize i_s)
        {
            Debug.Assert(!this._is_dispose);
            return (i_s.w == this.m_width && i_s.h == this.m_height);
        }
        /* DsXRGB32Rasterの内容を保持しているサーフェイスにコピーします。
         */
        public void setRaster(INyARRgbRaster i_sample)
        {
            Debug.Assert(!this._is_dispose);
            int pitch;
            int s_stride = this.m_width * 4;
            using (GraphicsStream gs = this._surface.LockRectangle(LockFlags.None, out pitch))
            {
                try{                    
                    switch (i_sample.getBufferType())
                    {
                        case NyARBufferType.BYTE1D_B8G8R8X8_32:
                            if (pitch % s_stride == 0)
                            {
                                Marshal.Copy((byte[])i_sample.getBuffer(), 0, (IntPtr)((int)gs.InternalData), this.m_width * 4 * this.m_height);
                            }
                            else
                            {
                                int s_idx = 0;
                                int d_idx = (int)gs.InternalData;
                                for (int i = this.m_height - 1; i >= 0; i--)
                                {
                                    //どう考えてもポインタです。
                                    Marshal.Copy((byte[])i_sample.getBuffer(), s_idx, (IntPtr)(d_idx), s_stride);
                                    s_idx += s_stride;
                                    d_idx += pitch;
                                }
                            }
                            break;
                        case NyARBufferType.OBJECT_CS_Bitmap:
                            NyARBitmapRaster ra = (NyARBitmapRaster)(i_sample);
                            BitmapData bm = ra.lockBitmap();
                            try{
                                //コピー
                                int src = (int)bm.Scan0;
                                int dst = (int)gs.InternalData;
                                for (int r = this.m_height - 1; r >= 0; r--)
                                {
                                    NyARD3dUtil.RtlCopyMemory((IntPtr)dst, (IntPtr)src, s_stride);
                                    dst += pitch;
                                    src += bm.Stride;
                                }
                            }finally{
                                ra.unlockBitmap();
                            }
                            break;
                        default:
                            throw new NyARRuntimeException();
                    }
                }finally{
                    this._surface.UnlockRectangle();
                }
                return;
            }
        }
        public void Dispose()
        {
            if (this._surface != null)
            {
                this._surface.Dispose();
                this._surface = null;
            }
            this._is_dispose = true;

            //GCからデストラクタを除外
            GC.SuppressFinalize(this);
            return;
        }
    }
}
