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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs;
using System.IO;

namespace NyARToolkitCSUtils.NyAR
{
    public class DsBGRX32Raster : NyARRgbRaster_BasicClass
    {
        private class PixelReader : INyARRgbPixelReader
        {
            private DsBGRX32Raster _parent;

            public PixelReader(DsBGRX32Raster i_parent)
            {
                this._parent = i_parent;
            }

            public void getPixel(int i_x, int i_y, int[] o_rgb)
            {
                byte[] ref_buf = this._parent._ref_buf;
                int bp = (i_x + i_y * this._parent._size.w) * 4;
                o_rgb[0] = ref_buf[bp + 2];// R
                o_rgb[1] = ref_buf[bp + 1];// G
                o_rgb[2] = ref_buf[bp + 0];// B
                return;
            }

            public void getPixelSet(int[] i_x, int[] i_y, int i_num, int[] o_rgb)
            {
                int width = _parent._size.w;
                byte[] ref_buf = _parent._ref_buf;
                int bp;
                for (int i = i_num - 1; i >= 0; i--)
                {
                    bp = (i_x[i] + i_y[i] * width) * 4;
                    o_rgb[i * 3 + 0] = ref_buf[bp + 2];// R
                    o_rgb[i * 3 + 1] = ref_buf[bp + 1];// G
                    o_rgb[i * 3 + 2] = ref_buf[bp + 0];// B
                }
                return;
            }
            public void setPixel(int i_x, int i_y, int[] i_rgb)
            {
                NyARException.notImplement();
            }
            public void setPixels(int[] i_x, int[] i_y, int i_num, int[] i_intrgb)
            {
                NyARException.notImplement();
            }
        }

        private INyARRgbPixelReader _rgb_reader;
        private INyARBufferReader _buffer_reader;
        private byte[] _ref_buf;
        public DsBGRX32Raster(int i_width, int i_height, int i_stride)
            : base(new NyARIntSize(i_width, i_height))
        {
            if (i_stride != i_width*4)
            {
                throw new NyARException();
            }
            this._ref_buf= new byte[i_height * i_stride];
            this._rgb_reader = new PixelReader(this);
            this._buffer_reader = new NyARBufferReader(this._ref_buf, INyARBufferReader.BUFFERFORMAT_BYTE1D_B8G8R8X8_32);
            return;
        }
        public override INyARRgbPixelReader getRgbPixelReader()
        {
            return this._rgb_reader;
        }
        public override INyARBufferReader getBufferReader()
        {
            return this._buffer_reader;
        }
        public void setBuffer(IntPtr i_buf,bool i_flip_vertical)
        {
            if (i_flip_vertical)
            {
                //上下反転させる
                int w = this._size.w*4;
                int st = w * (this._size.h - 1);
                int et = 0;
                for (int i = this._size.h - 1; i >= 0; i--)
                {
                    Marshal.Copy((IntPtr)((int)i_buf + et), this._ref_buf, st, w);
                    st -= w;
                    et += w;
                }
            }
            else
            {
                //上下を反転させない。
                Marshal.Copy(i_buf, this._ref_buf, 0, this._ref_buf.Length);
            }
            return;
        }
    }
}
