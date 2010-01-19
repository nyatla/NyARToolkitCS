/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
 * The NyARToolkitCS is C# edition ARToolKit class library.
 * Copyright (C)2008-2009 Ryo Iizuka
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp> or <nyatla(at)nyatla.jp>
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.utils;

namespace jp.nyatla.nyartoolkit.cs.sandbox.quadx2
{
    /**
     * 1/4のサイズの画像に変換しながら閾値判定する関数
     * 
     */
    public class NyARRasterFilter_ARTTh_Quad : INyARRasterFilter_Rgb2Bin
    {
        private int _threshold;

        public NyARRasterFilter_ARTTh_Quad(int i_threshold)
        {
            this._threshold = i_threshold;
        }
        public void setThreshold(int i_threshold)
        {
            this._threshold = i_threshold;
        }

        public void doFilter(INyARRgbRaster i_input, NyARBinRaster i_output)
        {
            //INyARBufferReader in_buffer_reader = i_input.getBufferReader();
            //INyARBufferReader out_buffer_reader = i_output.getBufferReader();
            int in_buf_type = i_input.getBufferType();

            NyARIntSize size = i_output.getSize();
            Debug.Assert(i_output.isEqualBufferType(NyARBufferType.INT2D_BIN_8));
            Debug.Assert(checkInputType(in_buf_type) == true);
            Debug.Assert(i_input.getSize().isEqualSize(size.w * 2, size.h * 2) == true);

            int[][] out_buf = (int[][])i_output.getBuffer();


            switch (i_input.getBufferType())
            {
                case NyARBufferType.BYTE1D_B8G8R8_24:
                case NyARBufferType.BYTE1D_R8G8B8_24:
                    convert24BitRgb((byte[])i_input.getBuffer(), out_buf, size);
                    break;
                case NyARBufferType.BYTE1D_B8G8R8X8_32:
                    convert32BitRgbx((byte[])i_input.getBuffer(), out_buf, size);
                    break;
                case NyARBufferType.WORD1D_R5G6B5_16LE:
                    convert16BitRgb565word((short[])i_input.getBuffer(), out_buf, size);
                    break;
                default:
                    throw new NyARException();
            }
            return;
        }
        private void convert16BitRgb565word(short[] i_in, int[][] i_out, NyARIntSize i_size)
        {
            int size_w = i_size.w*2;
            int x_mod_end = size_w - (size_w % 8);
            int th = this._threshold * 3;
            int bp = (size_w * i_size.h*2 - 1);
            uint w = 0;
            int x;
            uint px;
            for (int y = i_size.h - 1; y >= 0; y--)
            {
                int[] row_ptr = i_out[y];
                //端数分
                for (x = i_size.w - 1; x >= x_mod_end; x--)
                {
                    px = (uint)i_in[bp];
                    w = ((px & 0xf800) >> 8) + ((px & 0x07e0) >> 3) + ((px & 0x001f) << 3);
                    row_ptr[x] = w <= th ? 0 : 1;
                    bp-=2;
                }
                //タイリング		
                for (; x >= 0; x -= 8)
                {
                    px = (uint)i_in[bp];
                    w = ((px & 0xf800) >> 8) + ((px & 0x07e0) >> 3) + ((px & 0x001f) << 3);
                    row_ptr[x] = w <= th ? 0 : 1;
                    bp -= 2;
                    px = (uint)i_in[bp];
                    w = ((px & 0xf800) >> 8) + ((px & 0x07e0) >> 3) + ((px & 0x001f) << 3);
                    row_ptr[x - 1] = w <= th ? 0 : 1;
                    bp -= 2;
                    px = (uint)i_in[bp];
                    w = ((px & 0xf800) >> 8) + ((px & 0x07e0) >> 3) + ((px & 0x001f) << 3);
                    row_ptr[x - 2] = w <= th ? 0 : 1;
                    bp -= 2;
                    px = (uint)i_in[bp];
                    w = ((px & 0xf800) >> 8) + ((px & 0x07e0) >> 3) + ((px & 0x001f) << 3);
                    row_ptr[x - 3] = w <= th ? 0 : 1;
                    bp -= 2;
                    px = (uint)i_in[bp];
                    w = ((px & 0xf800) >> 8) + ((px & 0x07e0) >> 3) + ((px & 0x001f) << 3);
                    row_ptr[x - 4] = w <= th ? 0 : 1;
                    bp -= 2;
                    px = (uint)i_in[bp];
                    w = ((px & 0xf800) >> 8) + ((px & 0x07e0) >> 3) + ((px & 0x001f) << 3);
                    row_ptr[x - 5] = w <= th ? 0 : 1;
                    bp -= 2;
                    px = (uint)i_in[bp];
                    w = ((px & 0xf800) >> 8) + ((px & 0x07e0) >> 3) + ((px & 0x001f) << 3);
                    row_ptr[x - 6] = w <= th ? 0 : 1;
                    bp -= 2;
                    px = (uint)i_in[bp];
                    w = ((px & 0xf800) >> 8) + ((px & 0x07e0) >> 3) + ((px & 0x001f) << 3);
                    row_ptr[x - 7] = w <= th ? 0 : 1;
                    bp -= 2;
                }
                //1行飛ばし
                bp -= size_w;
            }
            return;
        }
        private void convert24BitRgb(byte[] i_in, int[][] i_out, NyARIntSize i_size)
        {
            int size_w = i_size.w * 2;
            int x_mod_end = size_w - (size_w % 8);
            int th = this._threshold * 3;
            int bp = (size_w * i_size.h * 2 - 1) * 3;
            int w;
            int x;
            for (int y = i_size.h - 1; y >= 0; y--)
            {
                //端数分
                int[] row_ptr = i_out[y];
                for (x = i_size.w - 1; x >= x_mod_end; x--)
                {
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x] = w <= th ? 0 : 1;
                    bp -= 6;
                }
                //タイリング		
                for (; x >= 0; x -= 8)
                {
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x] = w <= th ? 0 : 1;
                    bp -= 6;
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x - 1] = w <= th ? 0 : 1;
                    bp -= 6;
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x - 2] = w <= th ? 0 : 1;
                    bp -= 6;
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x - 3] = w <= th ? 0 : 1;
                    bp -= 6;
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x - 4] = w <= th ? 0 : 1;
                    bp -= 6;
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x - 5] = w <= th ? 0 : 1;
                    bp -= 6;
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x - 6] = w <= th ? 0 : 1;
                    bp -= 6;
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x - 7] = w <= th ? 0 : 1;
                    bp -= 6;
                }
                //1行飛ばし
                bp -= size_w * 3;
            }
            return;
        }
        private void convert32BitRgbx(byte[] i_in, int[][] i_out, NyARIntSize i_size)
        {
            int size_w = i_size.w*2;
            int x_mod_end = size_w - (size_w % 8);
            int th = this._threshold * 3;
            int bp = (size_w * i_size.h*2 - 1) * 4;
            int w;
            int x;
            for (int y = i_size.h - 1; y >= 0; y--)
            {
                int[] row_ptr = i_out[y];

                //端数分
                for (x = i_size.w - 1; x >= x_mod_end; x--)
                {
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x] = w <= th ? 0 : 1;
                    bp -= 8;
                }
                //タイリング
                for (; x >= 0; x -= 8)
                {
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x] = w <= th ? 0 : 1;
                    bp -= 8;
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x - 1] = w <= th ? 0 : 1;
                    bp -= 8;
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x - 2] = w <= th ? 0 : 1;
                    bp -= 8;
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x - 3] = w <= th ? 0 : 1;
                    bp -= 8;
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x - 4] = w <= th ? 0 : 1;
                    bp -= 8;
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x - 5] = w <= th ? 0 : 1;
                    bp -= 8;
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x - 6] = w <= th ? 0 : 1;
                    bp -= 8;
                    w = ((i_in[bp] & 0xff) + (i_in[bp + 1] & 0xff) + (i_in[bp + 2] & 0xff));
                    row_ptr[x - 7] = w <= th ? 0 : 1;
                    bp -= 8;
                }
                //1行飛ばし
                bp -= size_w * 4;
            }
            return;
        }

        private bool checkInputType(int i_input_type)
        {
            switch (i_input_type)
            {
                case NyARBufferType.BYTE1D_B8G8R8_24:
                case NyARBufferType.BYTE1D_R8G8B8_24:
                case NyARBufferType.BYTE1D_B8G8R8X8_32:
                case NyARBufferType.WORD1D_R5G6B5_16LE:
                    return true;
                default:
                    return false;
            }
        }
    }
}