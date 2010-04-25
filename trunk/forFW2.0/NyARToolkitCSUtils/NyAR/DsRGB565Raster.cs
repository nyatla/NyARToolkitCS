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
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs;
using System.IO;

namespace NyARToolkitCSUtils.NyAR
{
    public class DsRGB565Raster : NyARRgbRaster_BasicClass
    {
        private class PixelReader : INyARRgbPixelReader
        {
            private short[] _ref_buf;
            private int _stride;
            private int _height;

            public PixelReader(short[] i_ref_buf,int i_stride,int i_height)
            {
                this._ref_buf = i_ref_buf;
                this._stride = i_stride;
                this._height = i_height;
            }

            public void getPixel(int i_x, int i_y, int[] i_rgb)
            {
                short[] buf = this._ref_buf;
                int y = i_y;
                int idx = y * this._stride + i_x;
                uint pixcel =(uint)buf[idx];

                i_rgb[0] = (int)((pixcel & 0xf800) >> 8);//R
                i_rgb[1] = (int)((pixcel & 0x07e0) >> 3);//G
                i_rgb[2] = (int)((pixcel & 0x001f) << 3);//B
                return;
            }

            public void getPixelSet(int[] i_x, int[] i_y, int i_num, int[] i_rgb)
            {
                int stride = this._stride;
                short[] buf = this._ref_buf;
                int height = this._height;

                for (int i = i_num - 1; i >= 0; i--)
                {
                    int idx = i_y[i] * stride + i_x[i];

                    uint pixcel =(uint)buf[idx];
                    i_rgb[i * 3 + 0] = (int)((pixcel & 0xf800) >> 8);//R
                    i_rgb[i * 3 + 1] = (int)((pixcel & 0x07e0) >> 3);//G
                    i_rgb[i * 3 + 2] = (int)((pixcel & 0x001f) << 3);//B
                }
            }
            public void setPixel(int i_x, int i_y, int[] i_rgb)
            {
                NyARException.notImplement();
            }
            public void setPixels(int[] i_x, int[] i_y, int i_num, int[] i_intrgb)
            {
                NyARException.notImplement();
            }
            public void switchBuffer(object i_ref_buffer)
            {
                this._ref_buf = (short[])i_ref_buffer;
            }
        }
        private INyARRgbPixelReader _rgb_reader;
        private short[] _buf;
        public DsRGB565Raster(int i_width, int i_height)
            : base(i_width, i_height,NyARBufferType.WORD1D_R5G6B5_16LE)
        {
            if (i_width % 4 != 0)
            {
                throw new NyARException();
            }
            this._buf = new short[i_height * i_width];
            this._rgb_reader = new PixelReader(this._buf,i_width,i_height);
            return;
        }
        public override INyARRgbPixelReader getRgbPixelReader()
        {
            return this._rgb_reader;
        }
        public override object getBuffer()
        {
            return this._buf;
        }
        public override bool hasBuffer()
        {
            return this._buf != null;
        }
        public override void wrapBuffer(object i_ref_buf)
        {
            NyARException.notImplement();
        }
        public void setBuffer(IntPtr i_buf, bool i_flip_vertical)
        {
            if (i_flip_vertical)
            {
                //上下反転させる
                int w = this._size.w;
                int st = w * (this._size.h - 1);
                int et = 0;
                for (int i = this._size.h - 1; i >= 0; i--)
                {
                    Marshal.Copy((IntPtr)((int)i_buf + et), this._buf, st, w);
                    st -= w;
                    et += w*2;
                }
            }
            else
            {
                //上下を反転させない。
                Marshal.Copy(i_buf, this._buf, 0, this._buf.Length);
            }
            return;
        }

    }
}
