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

namespace NyARToolkitCSUtils.Raster
{
    public class DsRGB565Raster : NyARRgbRaster_BasicClass
    {
        private class PixelReader : INyARRgbPixelReader
        {
            private DsRGB565Raster _parent;
            private int _stride;

            public PixelReader(DsRGB565Raster i_parent)
            {
                this._parent = i_parent;
                this._stride = i_parent._size.w * 2;
            }

            public void getPixel(int i_x, int i_y, int[] i_rgb)
            {
                byte[] buf = this._parent._ref_buf;
                int y = i_y;
                int idx = y * this._stride + i_x * 2;
                uint pixcel = (uint)(buf[idx + 1] << 8) | (uint)buf[idx];

                i_rgb[0] = (int)((pixcel & 0xf800) >> 8);//R
                i_rgb[1] = (int)((pixcel & 0x07e0) >> 3);//G
                i_rgb[2] = (int)((pixcel & 0x001f) << 3);//B
                return;
            }

            public void getPixelSet(int[] i_x, int[] i_y, int i_num, int[] i_rgb)
            {
                int stride = this._stride;
                byte[] buf = this._parent._ref_buf;
                int height = this._parent._size.h;

                    for (int i = i_num - 1; i >= 0; i--)
                    {
                        int idx = i_y[i] * stride + i_x[i] * 2;

                        uint pixcel = (uint)(buf[idx + 1] << 8) | (uint)buf[idx];
                        i_rgb[i * 3 + 0] = (int)((pixcel & 0xf800) >> 8);//R
                        i_rgb[i * 3 + 1] = (int)((pixcel & 0x07e0) >> 3);//G
                        i_rgb[i * 3 + 2] = (int)((pixcel & 0x001f) << 3);//B
                }
            }
        }
        private INyARRgbPixelReader _rgb_reader;
        private INyARBufferReader _buffer_reader;
        private byte[] _ref_buf;
        public DsRGB565Raster(int i_width, int i_height)
            : base(new NyARIntSize(i_width, i_height))
        {
            if (i_width % 4 != 0)
            {
                throw new NyARException();
            }
            this._ref_buf = new byte[i_height * i_width * 2];
            this._rgb_reader = new PixelReader(this);
            this._buffer_reader = new NyARBufferReader(this._ref_buf, INyARBufferReader.BUFFERFORMAT_BYTE1D_R5G6B5_16LE);
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
        public void setBuffer(IntPtr i_buf, bool i_is_top_to_botomm)
        {
            if (i_is_top_to_botomm)
            {
                //上下を反転させない。
                Marshal.Copy(i_buf, this._ref_buf, 0, this._ref_buf.Length);
            }else{
                //上下反転させる
                int w = this._size.w * 2;
                int st = w * (this._size.h - 1);
                int et = 0;
                for (int i = this._size.h - 1; i >= 0; i--)
                {
                    Marshal.Copy((IntPtr)((int)i_buf+et), this._ref_buf, st, w);
                    st -= w;
                    et+=w;
                }
            }
            return;
        }
        public void SaveToFile(String i_file_name)
        {
            FileStream fs = new FileStream(i_file_name, FileMode.Create);
            fs.Write(this._ref_buf, 0, this._ref_buf.Length);
            fs.Close();
            return;
        }

    }
}
