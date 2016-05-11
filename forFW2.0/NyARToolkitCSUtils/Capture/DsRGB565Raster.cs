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
using System.Runtime.InteropServices;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using System.IO;

//参照元のプロジェクトによって、別の名前空間に配置。
#if NyartoolkitCS_FRAMEWORK_CFW
namespace NyARToolkitCSUtils.WMCapture
#else
namespace NyARToolkitCSUtils.Capture
#endif
{
    public class DsRGB565Raster : NyARRgbRaster
    {
        private short[] _buf;
        public DsRGB565Raster(int i_width, int i_height)
            : base(i_width, i_height,false)
        {
            if (i_width % 4 != 0)
            {
                throw new NyARRuntimeException();
            }
            this._buf=new short[i_width*i_height];
        }
        public void setBuffer(IntPtr i_buf, bool i_flip_vertical)
        {
            short[] buf=(short[])(this._buf);
            if (i_flip_vertical)
            {
                //上下反転させる
                int w = this._size.w;
                int st = w * (this._size.h - 1);
                int et = 0;
                for (int i = this._size.h - 1; i >= 0; i--)
                {
                    Marshal.Copy((IntPtr)((int)i_buf + et), buf, st, w);
                    st -= w;
                    et += w*2;
                }
            }
            else
            {
                //上下を反転させない。
                Marshal.Copy(i_buf, buf, 0, buf.Length);
            }
            return;
        }
        public override Object getBuffer()
        {
            return this._buf;
        }
        public override int getBufferType()
        {
            return NyARBufferType.WORD1D_R5G6B5_16LE;
        }
        public override void wrapBuffer(Object i_ref_buf)
        {
            throw new NyARRuntimeException();
        }
        public override int[] getPixel(int i_x, int i_y, int[] i_rgb)
        {
            throw new NyARRuntimeException();
        }
        public override int[] getPixelSet(int[] i_x, int[] i_y, int i_num, int[] i_intrgb)
        {
            throw new NyARRuntimeException();
        }
        public override void setPixel(int i_x, int i_y, int i_r, int i_g, int i_b)
        {
            throw new NyARRuntimeException();
        }
        public override void setPixel(int i_x, int i_y, int[] i_rgb)
        {
            throw new NyARRuntimeException();
        }
        public override void setPixels(int[] i_x, int[] i_y, int i_num, int[] i_intrgb)
        {
            throw new NyARRuntimeException();
        }
    }
}
